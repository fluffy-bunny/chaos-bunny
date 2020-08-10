using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorApp2.Services
{
    public class AuthorizedHandler : DelegatingHandler
    {
        private readonly AccountHelper _accountHelper;

        public AuthorizedHandler(AccountHelper accountHelper)
        {
            _accountHelper = accountHelper;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage responseMessage = await base.SendAsync(request, cancellationToken);

            if (responseMessage.StatusCode == HttpStatusCode.Unauthorized ||
                responseMessage.StatusCode == HttpStatusCode.Forbidden)
            {
                // if server returned 401 Unauthorized, redirect to login page
                _accountHelper.SignIn();
            }

            return responseMessage;
        }
    }
}
