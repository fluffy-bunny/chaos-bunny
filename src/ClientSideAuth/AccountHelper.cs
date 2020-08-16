using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;

namespace ClientSideAuth
{
    public class AccountHelper
    {
        private const string LogInPath = "Identity/Account/Login";
        private const string LogOutPath = "Identity/Account/Logout";

        private readonly NavigationManager _navigation;
        private readonly IHostHttpClient _hostHttpClient;
        private readonly ILogger<AccountHelper> _logger;


        public AccountHelper(
            NavigationManager navigation,
            IHostHttpClient hostHttpClient,
            ILogger<AccountHelper> logger)
        {
            _navigation = navigation;
            _hostHttpClient = hostHttpClient;
            _logger = logger;
        }

        public void SignIn(string customReturnUrl = null)
        {
            var httpClient = _hostHttpClient.CreateHttpClient();
            var returnUrl = customReturnUrl != null ? _navigation.ToAbsoluteUri(customReturnUrl).ToString() : null;
            var encodedReturnUrl = Uri.EscapeDataString(returnUrl ?? new Uri(_navigation.Uri).PathAndQuery);
            var logInUrl = new Uri(httpClient.BaseAddress, $"{LogInPath}?returnUrl={encodedReturnUrl}");
            //            var logInUrl = _navigation.ToAbsoluteUri($"{LogInPath}?returnUrl={encodedReturnUrl}");
            var sLoginUrl = logInUrl.ToString();
            _navigation.NavigateTo(logInUrl.ToString(), true);
        }

        public void SignOut()
        {
            var httpClient = _hostHttpClient.CreateHttpClient();
            var logOutPath = new Uri(httpClient.BaseAddress, $"{LogOutPath}?returnUrl=/");
            _navigation.NavigateTo(logOutPath.ToString(), true); ;
        }
    }
}
