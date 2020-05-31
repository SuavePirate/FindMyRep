using FindMyRep.Api.Models.Configuration;
using Google.Apis.CivicInfo.v2;
using Google.Apis.CivicInfo.v2.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindMyRep.Api.Providers
{
    public class GoogleCivicInfoProvider : IGoogleCivicInfoProvider
    {
        private readonly GoogleApiSettings _apiSettings;
        public GoogleCivicInfoProvider(IOptions<GoogleApiSettings> settings)
        {
            _apiSettings = settings.Value;
        }
        public async Task<RepresentativeInfoResponse> GetLocalCivicInfo(string zipCode)
        {
            var civicService = new CivicInfoService(new BaseClientService.Initializer
            {
                ApiKey = _apiSettings.ApiKey,
                ApplicationName = _apiSettings.AppName
            });

            var request = civicService.Representatives.RepresentativeInfoByAddress();
            request.Address = zipCode;
            var response = await request.ExecuteAsync();

            return response;
        }

        public async Task<Official> GetLocalGovernor(string zipCode)
        {
            return await GetOfficialByLevel(zipCode, "administrativeArea1", "governor");
        }
        public async Task<Official> GetLocalMayor(string zipCode)
        {
            return await GetOfficialByLevel(zipCode, "locality", "mayor");
        }
        public async Task<Official> GetLocalSenator(string zipCode)
        {
            return await GetOfficialByLevel(zipCode, "country", "senator");
        }

        //public async Task<Official> GetLocalRep(string zipCode)
        //{
        //    return await GetOfficialByLevel(zipCode, "administrativeArea1");
        //}

        private async Task<Official> GetOfficialByLevel(string zipCode, string level, string requiredName)
        {
            var response = await GetLocalCivicInfo(zipCode);

            // find the mayor from the response;
            var office = response.Offices.FirstOrDefault(o => o.Levels.Contains(level) && o.Name.ToLower().Contains(requiredName.ToLower()));
            if (office is null)
                return null;

            var officialIndex = office.OfficialIndices[0];

            var official = response.Officials[(int)officialIndex];

            return official;
        }
    }
}
