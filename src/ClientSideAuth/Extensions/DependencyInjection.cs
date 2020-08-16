using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSideAuth.Extensions
{
    public static class DependencyInjection
    {
        public static WebAssemblyHostBuilder AddClientSideAuth(this WebAssemblyHostBuilder builder)
        {
            builder.Services.AddTransient<AccountHelper>();
            builder.Services.AddTransient<AuthorizedHandler>();


            builder.Services.AddHttpClient(Constants.HostingHttpClientName,
              client => {
                  var baseAddress = builder.HostEnvironment.BaseAddress;
                  var uri = new Uri(baseAddress);
                  baseAddress = $"{uri.Scheme}://{uri.Authority}";
                  client.BaseAddress = new Uri(baseAddress);

              })
                .AddHttpMessageHandler<AuthorizedHandler>();
            builder.Services.AddSingleton<IHostHttpClient, HostHttpClient>();

            return builder;
        }
    }
}
