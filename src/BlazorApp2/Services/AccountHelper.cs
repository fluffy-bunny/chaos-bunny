using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Security.Claims;

namespace BlazorApp2.Services
{
    public class AccountHelper 
    {
        private const string LogInPath = "Identity/Account/Login";
        private const string LogOutPath = "Identity/Account/Logout";

        private readonly NavigationManager _navigation;
        private readonly HttpClient _client;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<AccountHelper> _logger;

        private DateTimeOffset _userLastCheck = DateTimeOffset.FromUnixTimeSeconds(0);
        private ClaimsPrincipal _cachedUser = new ClaimsPrincipal(new ClaimsIdentity());

        public AccountHelper(
            NavigationManager navigation,
            IHttpClientFactory clientFactory,
            ILogger<AccountHelper> logger)
        {
            _navigation = navigation;
            _client = clientFactory.CreateClient();
            _clientFactory = clientFactory;
            _logger = logger;
        }

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
            var httpClient = _clientFactory.CreateClient("authorizedClient");
            var logOutPath = new Uri(httpClient.BaseAddress, $"{LogOutPath}?returnUrl=/");
            _navigation.NavigateTo(logOutPath.ToString(), true); ;
        }
    }
}
