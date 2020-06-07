using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindMyRep.Api.Services
{
    public interface ICivicInfoResponseService
    {
        Task<(string OutputSpeech, string DisplayText)> GetResponseAsync(string intent, string zipCode);
        Task<(string OutputSpeech, string DisplayText)> GetAllRepsResponseAsync(string zipCode);
        Task<(string OutputSpeech, string DisplayText)> GetMayorResponseAsync(string zipCode);
        Task<(string OutputSpeech, string DisplayText)> GetGovernorResponseAsync(string zipCode);
        Task<(string OutputSpeech, string DisplayText)> GetSenatorResponseAsync(string zipCode);
        string GetWelcomeMessage();
        string GetHelpMessage();
        string GetFallbackMessage();
    }
}
