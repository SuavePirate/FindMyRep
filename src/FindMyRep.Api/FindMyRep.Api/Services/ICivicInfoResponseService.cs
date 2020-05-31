using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindMyRep.Api.Services
{
    public interface ICivicInfoResponseService
    {
        Task<string> GetResponseAsync(string intent, string zipCode);
        string GetWelcomeMessage();
        string GetHelpMessage();
        string GetFallbackMessage();
    }
}
