using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorApp2.Services;
using Blazored.SessionStorage;

namespace BlazorApp2
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddBlazoredSessionStorage(config =>
                    config.JsonSerializerOptions.WriteIndented = true);
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddTransient<AccountHelper>();
            builder.Services.AddTransient<AuthorizedHandler>();
            

            builder.Services.AddHttpClient("authorizedClient",
              client => {
                  var baseAddress = builder.HostEnvironment.BaseAddress;
                  var uri = new Uri(baseAddress);
                  baseAddress = $"{uri.Scheme}://{uri.Authority}";
                  client.BaseAddress = new Uri(baseAddress);

                  })
                .AddHttpMessageHandler<AuthorizedHandler>();
            builder.Services.AddTransient<FetchWeatherForecastService>();
            builder.Services.AddTransient<FetchAuthStatusService>();
            builder.RootComponents.Add<App>("app");


            await builder.Build().RunAsync();
        }
    }
}
