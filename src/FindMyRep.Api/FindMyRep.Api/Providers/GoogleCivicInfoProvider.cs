using Google.Apis.CivicInfo.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindMyRep.Api.Providers
{
    public class GoogleCivicInfoProvider : IGoogleCivicInfoProvider
    {
        public async Task GetLocalCivicInfo(string zipCode)
        {
            var civicService = new CivicInfoService()
            {
            };
        }
    }
}
