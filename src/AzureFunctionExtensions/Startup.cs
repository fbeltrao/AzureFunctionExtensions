using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: WebJobsStartup(typeof(Fbeltrao.AzureFunctionExtensions.Startup))]

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Extension initializer
    /// </summary>
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExtension<EventGridOutputConfiguration>();
            builder.AddExtension<HttpCallConfiguration>();
            builder.AddExtension<RedisConfiguration>();
            builder.AddExtension<SignalRConfiguration>();
            builder.Services.AddSingleton(typeof(IRedisDatabaseManager), typeof(RedisDatabaseManager));
        }
    }
}
