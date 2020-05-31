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
    }
}
