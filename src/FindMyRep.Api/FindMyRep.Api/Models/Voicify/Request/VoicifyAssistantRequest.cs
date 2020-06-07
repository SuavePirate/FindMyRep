using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindMyRep.Api.Models.Voicify.Request
{
    public class VoicifyAssistantRequest
    {
        public Dictionary<string, string> Slots { get; set; }
        public Dictionary<string, object> SessionAttributes { get; set; }
    }
}
