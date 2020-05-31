using FindMyRep.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindMyRep.Api.Controllers
{
    [Route("api/[controller]")]
    public class DialogflowController : ControllerBase
    {
        private readonly ICivicInfoResponseService _infoResponseService;

        public DialogflowController(ICivicInfoResponseService infoResponseService)
        {
            _infoResponseService = infoResponseService;
        }

        [HttpPost]
        public async Task<IActionResult> HandleDialogflowRequest([FromBody]dynamic dialogflowRequest)
        {
            var intentName = dialogflowRequest?.queryResult?.action?.ToString();
            dynamic response = null;

            if(intentName == "input.welcome")
            {
                response = Ask(_infoResponseService.GetWelcomeMessage());
            }
            else if(intentName.Contains("ZipCode"))
            {
                // get the zip slot
                var zipCode = dialogflowRequest?.queryResult?.parameters["zip-code"]?.ToString();
                var responseString = await _infoResponseService.GetResponseAsync(intentName, zipCode);

                response = Ask(responseString);               
                
            }


            if (response is null)
                response = Ask(_infoResponseService.GetFallbackMessage());

            return new JsonResult(response);

        }

        private dynamic Ask(string outputText)
        {

            dynamic response = new
            {
                FulfillmentText = outputText,
                Payload = new
                {
                    Google = new
                    {
                        ExpectUserResponse = true,
                        RichResponse = new
                        {
                            Items = new dynamic[]
                            {
                                new
                                {
                                    SimpleResponse = new
                                    {
                                        TextToSpeech = outputText
                                    }
                                }
                            }
                        }
                    }
                }
            };
            return response;

        }
    }
}
