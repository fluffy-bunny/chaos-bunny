using BazorAuth.Shared;
using ClientSideAuth;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorApp2.Services
{
    public class FetchAuthStatusService
    {
        private readonly HttpClient _httpClient;
        public FetchAuthStatusService(IHostHttpClient hostHttpClient)
        {
            _httpClient = hostHttpClient.CreateHttpClient();
        }

        public async Task<string> GetUserDisplayNameStatus()
        {
            var displayName =  await _httpClient.GetFromJsonAsync<string>("api/AuthStatus/display-name");
            return displayName;
        }
        public async Task<ClaimHandle[]> GetClaimsAsync()
        {
            var claims = await _httpClient.GetFromJsonAsync<ClaimHandle[]>("api/AuthStatus/claims");
            return claims;
        }
        public async Task<OpenIdConnectSessionDetails> GetOpenIdConnectSessionDetailsAsync()
        {
            var openIdConnectSessionDetails = await _httpClient.GetFromJsonAsync<OpenIdConnectSessionDetails>("api/AuthStatus/oidc-session-details");
            return openIdConnectSessionDetails;
        }
    }
}
 