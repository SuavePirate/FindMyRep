using FindMyRep.Api.Models.Voicify.Request;
using FindMyRep.Api.Models.Voicify.Response;
using FindMyRep.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindMyRep.Api.Controllers
{
    [Route("api/[controller]")]
    public class VoicifyController : ControllerBase
    {
        private readonly ICivicInfoResponseService _civicInfoResponseService;

        public VoicifyController(ICivicInfoResponseService civicInfoResponseService)
        {
            _civicInfoResponseService = civicInfoResponseService;
        }

        [HttpPost("all-reps")]
        public async Task<IActionResult> HandleAllReps([FromBody]VoicifyRequest request)
        {
            var zipCode = GetZipCode(request);
            var allRepsResponse = await _civicInfoResponseService.GetAllRepsResponseAsync(zipCode);
            return Ok(BuildResponse(allRepsResponse.OutputSpeech, zipCode));
        }
        [HttpPost("mayor")]
        public async Task<IActionResult> HandleMayor([FromBody]VoicifyRequest request)
        {
            var zipCode = GetZipCode(request);
            var allRepsResponse = await _civicInfoResponseService.GetMayorResponseAsync(zipCode);
            return Ok(BuildResponse(allRepsResponse.OutputSpeech, zipCode));
        }
        [HttpPost("governor")]
        public async Task<IActionResult> HandleGovernor([FromBody]VoicifyRequest request)
        {
            var zipCode = GetZipCode(request);
            var allRepsResponse = await _civicInfoResponseService.GetGovernorResponseAsync(zipCode);
            return Ok(BuildResponse(allRepsResponse.OutputSpeech, zipCode));
        }
        [HttpPost("senator")]
        public async Task<IActionResult> HandleSenator([FromBody]VoicifyRequest request)
        {
            var zipCode = GetZipCode(request);
            var allRepsResponse = await _civicInfoResponseService.GetSenatorResponseAsync(zipCode);
            return Ok(BuildResponse(allRepsResponse.OutputSpeech, zipCode));
        }

        private string GetZipCode(VoicifyRequest request)
        {
            if (request?.OriginalRequest?.Slots?.ContainsKey("zipCode") == true)
                return request.OriginalRequest.Slots["zipCode"];
            if (request?.OriginalRequest?.SessionAttributes?.ContainsKey("zipCode") == true)
                return request.OriginalRequest.SessionAttributes["zipCode"].ToString();

            return null;
        }

        private VoicifyResponse BuildResponse(string content, string zipCode)
        {
            return new VoicifyResponse
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
        }
    }
}
