using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExceptionApis
{
   
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddExceptionServices(this IServiceCollection services)
        {
            services.AddSingleton<IExceptionProducer, ExceptionProducer > ();
            return services;
        }

    }
    
}
