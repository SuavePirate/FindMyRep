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

        /// <summary>
        /// Gets the governor's info for the given zipCode;
        /// </summary>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        Task<Official> GetLocalGovernor(string zipCode);
        Task<Official> GetLocalSenator(string zipCode);
        Task<Official> GetLocalMayor(string zipCode);
    }
}
