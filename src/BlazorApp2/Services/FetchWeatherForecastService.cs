using BazorAuth.Shared;
using ClientSideAuth;
using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorApp2.Services
{
    public class FetchWeatherForecastService
    {
        private readonly HttpClient _httpClient;
        public FetchWeatherForecastService(IHostHttpClient hostHttpClient)
        {
            _httpClient = hostHttpClient.CreateHttpClient();
        }

        public async Task<WeatherForecast[]> GetPublicWeatherForeacast()
        {
            return await _httpClient.GetFromJsonAsync<WeatherForecast[]>("api/WeatherForecast");
        }

        public async Task<WeatherForecast[]> GetProtectedWeatherForeacast()
        {
            return await _httpClient.GetFromJsonAsync<WeatherForecast[]>("api/WeatherForecast/protected");
        }
    }
}
