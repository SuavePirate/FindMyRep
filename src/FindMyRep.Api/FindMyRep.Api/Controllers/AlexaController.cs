using Alexa.NET;
using Alexa.NET.Request;
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
            return ResponseBuilder.Tell("Hello world");
        }
    }
}
