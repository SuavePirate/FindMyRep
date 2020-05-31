using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using FindMyRep.Api.Providers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindMyRep.Api.Controllers
{
    [Route("api/[controller]")]
    public class AlexaController : ControllerBase
    {
        private readonly IGoogleCivicInfoProvider _civicInfoProvider;

        public AlexaController(IGoogleCivicInfoProvider civicInfoProvider)
        {
            _civicInfoProvider = civicInfoProvider;
        }

        [HttpPost]
        public async Task<SkillResponse> HandleAlexaRequest([FromBody]SkillRequest skillRequest)
        {
            if(skillRequest.GetRequestType() == typeof(LaunchRequest))
            {
                return ResponseBuilder.Ask("Welcome to Find My Rep! You can find contact information and details about your representatives from federal to local. Just say your zip code.", null);
            }
            else if(skillRequest.GetRequestType() == typeof(IntentRequest))
            {
                var intentRequest = skillRequest.Request as IntentRequest;
                if(intentRequest.Intent.Name == "RepZipCodeIntent")
                {
                    var zipCode = intentRequest.Intent.Slots["ZipCode"];
                    var civicResponse = await _civicInfoProvider.GetLocalCivicInfo(zipCode.Value);

                    var firstOfficial = civicResponse.Officials.FirstOrDefault();

                    return ResponseBuilder.Ask($"Here's what I found: {firstOfficial.Name}, and you can call them at {firstOfficial.Phones.FirstOrDefault()}. You can ask for another zip code's reps", null);
                }
                else if (intentRequest.Intent.Name == "AMAZON.HelpIntent")
                {
                    return ResponseBuilder.Ask("With the Find My Rep skill, you can ask for your local reps info by zip code", null);
                }
            }

            return ResponseBuilder.Tell("You said something I don't understand yet. Try something else later.");
        }
    }
}
