using BazorAuth.Shared;
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
        public FetchWeatherForecastService(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("authorizedClient");
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
