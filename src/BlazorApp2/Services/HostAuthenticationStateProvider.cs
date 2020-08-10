using BazorAuth.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorApp2.Services
{

    public class HostAuthenticationStateProvider : AuthenticationStateProvider
    {
        private static readonly TimeSpan _userCacheRefreshInterval = TimeSpan.FromSeconds(60);

        private const string LogInPath = "Identity/Account/Login";
        private const string LogOutPath = "Identity/Account/Logout";

        private readonly NavigationManager _navigation;
        private readonly HttpClient _client;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<HostAuthenticationStateProvider> _logger;

        private DateTimeOffset _userLastCheck = DateTimeOffset.FromUnixTimeSeconds(0);
        private ClaimsPrincipal _cachedUser = new ClaimsPrincipal(new ClaimsIdentity());

        public HostAuthenticationStateProvider(
            NavigationManager navigation,
            IHttpClientFactory clientFactory,
            ILogger<HostAuthenticationStateProvider> logger)
        {
            _navigation = navigation;
            _client = clientFactory.CreateClient();
            _clientFactory = clientFactory;
            _logger = logger;
        }

      //  public override async Task<AuthenticationState> GetAuthenticationStateAsync() => new AuthenticationState(await GetUser(useCache: true));

        public void SignIn(string customReturnUrl = null)
        {
            var httpClient = _clientFactory.CreateClient("authorizedClient");
            var returnUrl = customReturnUrl != null ? _navigation.ToAbsoluteUri(customReturnUrl).ToString() : null;
            var encodedReturnUrl = Uri.EscapeDataString(returnUrl ?? new Uri(_navigation.Uri).PathAndQuery);
            var logInUrl = new Uri(httpClient.BaseAddress, $"{LogInPath}?returnUrl={encodedReturnUrl}");
//            var logInUrl = _navigation.ToAbsoluteUri($"{LogInPath}?returnUrl={encodedReturnUrl}");
            var sLoginUrl = logInUrl.ToString();
            _navigation.NavigateTo(logInUrl.ToString(), true);
        }

        public void SignOut()
        {
            _navigation.NavigateTo(_navigation.ToAbsoluteUri(LogOutPath).ToString(), true);
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
