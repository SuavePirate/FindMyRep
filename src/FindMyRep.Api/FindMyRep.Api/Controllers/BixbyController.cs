using FindMyRep.Api.Models.Bixby;
using FindMyRep.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindMyRep.Api.Controllers
{
    [Route("api/[controller]")]
    public class BixbyController : ControllerBase
    {
        private readonly ICivicInfoResponseService _infoResponseService;

        public BixbyController(ICivicInfoResponseService infoResponseService)
        {
            _infoResponseService = infoResponseService;
        }

        [HttpPost("{intentName}")]
        public async Task<BixbyResponse> HandleRequest(string intentName, [FromBody]BixbyRequest bixbyRequest)
        {
            BixbyResponse response = null;
            if (intentName == "input.welcome")
            {
                response = Ask(_infoResponseService.GetWelcomeMessage());
            }
            else if (intentName.Contains("ZipCode"))
            {
                (string _, string displayText) = await _infoResponseService.GetResponseAsync(intentName, bixbyRequest.ZipCode);
                
                
                response = Ask($"Here's what I found for {bixbyRequest.ZipCode}", displayText);

            }


            if (response is null)
                response = Ask(_infoResponseService.GetFallbackMessage());

            return response;
        }

        private BixbyResponse Ask(string outputSpeech, string displayText = null)
        {
            return new BixbyResponse
            {
                OutputSpeech = outputSpeech,
                DisplayText = displayText?? outputSpeech
            };
        }
    }
}
