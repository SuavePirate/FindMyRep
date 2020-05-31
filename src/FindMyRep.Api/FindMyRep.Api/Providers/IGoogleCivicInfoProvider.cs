using Google.Apis.CivicInfo.v2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindMyRep.Api.Providers
{
    public interface IGoogleCivicInfoProvider
    {
        /// <summary>
        /// Get the information about your local civic providers by zip code
        /// </summary>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        Task<RepresentativeInfoResponse> GetLocalCivicInfo(string zipCode);
    }
}
