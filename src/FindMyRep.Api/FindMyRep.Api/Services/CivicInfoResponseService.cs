using FindMyRep.Api.Models.Voicify;
using FindMyRep.Api.Models.Voicify.Response;
using FindMyRep.Api.Providers;
using Google.Apis.CivicInfo.v2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindMyRep.Api.Services
{
    public class CivicInfoResponseService : ICivicInfoResponseService
    {
        private readonly IGoogleCivicInfoProvider _civicInfoProvider;

        public CivicInfoResponseService(IGoogleCivicInfoProvider civicInfoProvider)
        {
            _civicInfoProvider = civicInfoProvider;
        }

        [Obsolete]
        public string GetFallbackMessage()
        {
            return "You said something I don't understand yet. You can say your zip code to get all of the government information for your area, or ask for a specific office such as \"Governor of \" and your zip code. So, how can I help?";
        }

        [Obsolete]
        public string GetHelpMessage()
        {
            return "With the Find My Rep skill, you can ask for your local reps info by zip code";
        }

        [Obsolete]
        public async Task<(string OutputSpeech, string DisplayText)> GetResponseAsync(string intent, string zipCode)
        {
            if (zipCode.Length == 4)
            {
                zipCode = $"0{zipCode}";
            }

            if (intent == "AllZipCodeIntent")
            {
                var civicInfo = await _civicInfoProvider.GetLocalCivicInfo(zipCode);

                var response = "Here are all of your reps: ";
                var displayText = response;

                foreach (var office in civicInfo.Offices)
                {
                    if (office.Name.ToLower().Contains("president"))
                        continue;
                    var headerText = $" For - {office.Name}, you can contact: ";
                    response += headerText;
                    displayText += $"\r\n - {headerText}";
                    foreach (var officialIndex in office.OfficialIndices ?? Enumerable.Empty<long?>())
                    {
                        if (officialIndex is null)
                            continue;
                        var official = civicInfo.Officials[(int)officialIndex];
                        response += $"{official.Name} ";
                        displayText += official.Name;
                        if (official.Phones?.Any() == true)
                        {
                            response += $"by phone at {official.Phones.FirstOrDefault()} ";
                            displayText += $" phone - {official.Phones.FirstOrDefault()}";
                        }
                        if (official.Phones?.Any() == true && official.Emails?.Any() == true)
                        {
                            response += "or ";
                        }

                        if (official.Emails?.Any() == true)
                        {
                            response += $"by email at {official.Emails.FirstOrDefault()}";
                            displayText += $" email - {official.Emails.FirstOrDefault()}";
                        }
                        response += ".";
                    }
                }

                response += "You can ask for another zip code's reps.";

                return (response, displayText);
            }
            else
            {
                Official official = null;
                switch (intent)
                {
                    case "GovZipCodeIntent":
                        official = await _civicInfoProvider.GetLocalGovernor(zipCode);
                        break;
                    case "SenatorZipCodeIntent":
                        official = await _civicInfoProvider.GetLocalSenator(zipCode);
                        break;
                    case "MayorZipCodeIntent":
                        official = await _civicInfoProvider.GetLocalMayor(zipCode);
                        break;
                }
                if (official is null)
                {
                    var missingResponse = "I wasn't able to find anyone for that. Try asking something else.";
                    return (missingResponse, missingResponse);
                }

                var response = $"Here's what I found: {official.Name},";
                if (official.Phones?.Any() == true)
                    response += $"and you can call them at {official.Phones.FirstOrDefault()}. ";
                if (official.Emails?.Any() == true)
                    response += $"You can also email them at {official.Emails.FirstOrDefault()}. ";

                response += "You can ask for another zip code's reps.";

                return (response, response);
            }

            return (null, null);
        }

        [Obsolete]
        public string GetWelcomeMessage()
        {
            return "Welcome to Find My Rep! You can find contact information and details about your representatives from federal to local. Just say your zip code.";
        }




        public async Task<VoicifyResponse> GetMayorResponseAsync(string zipCode)
        {
            var official = await _civicInfoProvider.GetLocalMayor(zipCode);
            return GetOfficialResponse(official, zipCode);
        }
        public async Task<VoicifyResponse> GetGovernorResponseAsync(string zipCode)
        {
            var official = await _civicInfoProvider.GetLocalGovernor(zipCode);
            return GetOfficialResponse(official, zipCode);
        }
        public async Task<VoicifyResponse> GetSenatorResponseAsync(string zipCode)
        {
            var official = await _civicInfoProvider.GetLocalSenator(zipCode);
            return GetOfficialResponse(official, zipCode);
        }

        private VoicifyResponse GetOfficialResponse(Official official, string zipCode)
        {
            if (official is null)
            {
                var missingResponse = "I wasn't able to find anyone for that. Try asking something else.";
                return BuildResponse(missingResponse, null, null, zipCode);
            }

            var response = $"Here's what I found: {official.Name}, ";
            if (official.Phones?.Any() == true)
                response += $"and you can call them at {official.Phones.FirstOrDefault()}. ";
            if (official.Emails?.Any() == true)
                response += $"You can also email them at {official.Emails.FirstOrDefault()}. ";


            return BuildResponse(response, null, official.PhotoUrl, zipCode);
        }

        public async Task<VoicifyResponse> GetAllRepsResponseAsync(string zipCode)
        {
            if (zipCode.Length == 4)
            {
                zipCode = $"0{zipCode}";
            }


            var civicInfo = await _civicInfoProvider.GetLocalCivicInfo(zipCode);

            var response = "Here are all of your reps: ";
            var displayText = response;

            foreach (var office in civicInfo.Offices)
            {
                if (office.Name.ToLower().Contains("president"))
                    continue;
                var headerText = $" For - {office.Name}, you can contact: ";
                response += headerText;
                displayText += $"\r\n - {office.Name}: ";
                foreach (var officialIndex in office.OfficialIndices ?? Enumerable.Empty<long?>())
                {
                    if (officialIndex is null)
                        continue;
                    var official = civicInfo.Officials[(int)officialIndex];
                    response += $" {official.Name} ";
                    displayText += $"\r\n   {official.Name}";
                    if (official.Phones?.Any() == true)
                    {
                        response += $"by phone at {official.Phones.FirstOrDefault()} ";
                        displayText += $"\r\n   phone: {official.Phones.FirstOrDefault()}";
                    }
                    if (official.Phones?.Any() == true && official.Emails?.Any() == true)
                    {
                        response += "or ";
                    }

                    if (official.Emails?.Any() == true)
                    {
                        response += $"by email at {official.Emails.FirstOrDefault()}";
                        displayText += $"\r\n    email: {official.Emails.FirstOrDefault()}";
                    }
                    response += ".";
                }
            }

            return BuildResponse(response, displayText, null, zipCode);
        }


        private VoicifyResponse BuildResponse(string content, string displayText, string imageUrl, string zipCode)
        {
            var response = new VoicifyResponse
            {
                Data = new VoicifyResponseData
                {
                    Content = content,
                    AdditionalSessionAttributes = new Dictionary<string, object>
                    {
                        {"zipCode", zipCode }
                    }
                }
            };

            if (!string.IsNullOrEmpty(displayText))
                response.Data.DisplayTextOverride = displayText;
            if (!string.IsNullOrEmpty(imageUrl))
                response.Data.LargeImage = new MediaContent
                {
                    Url = imageUrl
                };


            return response;
        }

    }
}
