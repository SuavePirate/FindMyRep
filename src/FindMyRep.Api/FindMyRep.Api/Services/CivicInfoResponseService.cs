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

        public string GetFallbackMessage()
        {
            return "You said something I don't understand yet. Try something else later.";
        }

        public string GetHelpMessage()
        {
            return "With the Find My Rep skill, you can ask for your local reps info by zip code";
        }

        public async Task<string> GetResponseAsync(string intent, string zipCode)
        {
            if (zipCode.Length == 4)
            {
                zipCode = $"0{zipCode}";
            }

            if (intent == "AllZipCodeIntent")
            {
                var civicInfo = await _civicInfoProvider.GetLocalCivicInfo(zipCode);

                var response = "Here are all of your representatives and their offices: ";
                foreach(var office in civicInfo.Offices)
                {
                    if (office.Name.ToLower().Contains("president"))
                        continue;

                    response += $"For the office - {office.Name}, you can contact: ";

                    foreach(var officialIndex in office.OfficialIndices ?? Enumerable.Empty<long?>())
                    {
                        if (officialIndex is null)
                            continue;
                        var official = civicInfo.Officials[(int)officialIndex];
                        response += $"{official.Name}. ";
                        response += "You can contact them ";
                        if (official.Phones?.Any() == true)
                            response += $"by phone at {official.Phones.FirstOrDefault()} ";
                        if (official.Emails?.Any() == true)
                            response += $"by email at {official.Emails.FirstOrDefault()} ";
                        response += ".";
                    }
                }

                response += "You can ask for another zip code's reps.";

                return response;
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
                    return "I wasn't able to find anyone for that. Try asking something else.";

                var response = $"Here's what I found: {official.Name},";
                if (official.Phones?.Any() == true)
                    response += $"and you can call them at {official.Phones.FirstOrDefault()}. ";
                if (official.Emails?.Any() == true)
                    response += $"You can also email them at {official.Emails.FirstOrDefault()}. ";

                response += "You can ask for another zip code's reps.";

                return response;
            }

            return null;
        }

        public string GetWelcomeMessage()
        {
            return "Welcome to Find My Rep! You can find contact information and details about your representatives from federal to local. Just say your zip code.";
        }
    }
}
