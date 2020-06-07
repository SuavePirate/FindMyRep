using FindMyRep.Api.Models.Voicify.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindMyRep.Api.Services
{
    public interface ICivicInfoResponseService
    {
        Task<(string OutputSpeech, string DisplayText)> GetResponseAsync(string intent, string zipCode);
        Task<VoicifyResponse> GetAllRepsResponseAsync(string zipCode);
        Task<VoicifyResponse> GetMayorResponseAsync(string zipCode);
        Task<VoicifyResponse> GetGovernorResponseAsync(string zipCode);
        Task<VoicifyResponse> GetSenatorResponseAsync(string zipCode);
        string GetWelcomeMessage();
        string GetHelpMessage();
        string GetFallbackMessage();
    }
}
