using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using FindMyRep.Api.Providers;
using FindMyRep.Api.Services;
using Google.Apis.CivicInfo.v2.Data;
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
        private readonly ICivicInfoResponseService _infoResponseService;

        public AlexaController(ICivicInfoResponseService infoResponseService)
        {
            _infoResponseService = infoResponseService;
        }

        [HttpPost]
        public async Task<SkillResponse> HandleAlexaRequest([FromBody]SkillRequest skillRequest)
        {
            if (skillRequest.GetRequestType() == typeof(LaunchRequest))
            {
                return ResponseBuilder.Ask(_infoResponseService.GetWelcomeMessage(), null);
            }
            else if (skillRequest.GetRequestType() == typeof(IntentRequest))
            {
                var intentRequest = skillRequest.Request as IntentRequest;
                if (intentRequest.Intent.Name == "AMAZON.HelpIntent")
                {
                    return ResponseBuilder.Ask(_infoResponseService.GetHelpMessage(), null);
                }


                if (intentRequest.Intent.Name.Contains("ZipCode"))
                {
                    var zipCode = intentRequest.Intent.Slots["ZipCode"];
                    var response = await _infoResponseService.GetResponseAsync(intentRequest.Intent.Name, zipCode.Value);

                    return ResponseBuilder.Ask(response, null);
                }
            }

            return ResponseBuilder.Ask(_infoResponseService.GetFallbackMessage(), null);
        }
    }
}
