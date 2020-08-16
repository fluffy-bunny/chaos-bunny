using BazorAuth.Shared;
using ClientSideAuth;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorApp2.Services
{
    public class FetchExceptionsService
    {
        private readonly HttpClient _httpClient;
        public FetchExceptionsService(IHostHttpClient hostHttpClient)
        {
            _httpClient = hostHttpClient.CreateHttpClient();
        }

        public async Task ProduceException(ExceptionType et)
        {
            await _httpClient.GetAsync($"api/Exceptions/{et}");
        }
    }
}
 