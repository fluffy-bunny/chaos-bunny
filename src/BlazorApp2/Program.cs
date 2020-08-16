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
using BlazorApp2.Services;
using Blazored.SessionStorage;
using ClientSideAuth.Extensions;

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
            builder.AddClientSideAuth();
            builder.Services.AddTransient<FetchWeatherForecastService>();
            builder.Services.AddTransient<FetchAuthStatusService>();
            builder.Services.AddTransient<FetchExceptionsService>();
            
            builder.RootComponents.Add<App>("app");


            await builder.Build().RunAsync();
        }
    }
}
