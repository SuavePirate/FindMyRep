using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindMyRep.Api.Models.Voicify.Response
{
    public class VoicifyResponseData
    {
        public string Content { get; set; }
        public string DisplayTextOverride { get; set; }
        public MediaContent LargeImage { get; set; }


        public Dictionary<string, object> AdditionalSessionAttributes { get; set; }
    }
}
