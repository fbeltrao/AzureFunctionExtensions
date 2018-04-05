using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Configuration for EventGrid output
    /// </summary>
    public class EventGridOutputConfiguration : IExtensionConfigProvider
    {
        /// <summary>
        /// Sets the Sas Key to authenticate Event Grid requests
        /// </summary>
        public string SasKey { get; set; }

        /// <summary>
        /// Sets the Event Grid Endpoint (https://xxx.{region}.eventgrid.azure.net/api/events)
        /// </summary>
        public string TopicEndpoint { get; set; }

        /// <summary>
        /// Sets the Event Grid event type
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// Sets the Event Grid event subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// (Optional) Sets the Event Grid data version
        /// </summary>
        public string DataVersion { get; set; }
        
        public void Initialize(ExtensionConfigContext context)
        {
            // Allows user to bind to IAsyncCollector<JObject>, and the sdk will convert that to IAsyncCollector<HttpCallRequest>
            context.AddConverter<JObject, EventGridOutput>(input => input.ToObject<EventGridOutput>());

            context
                .AddBindingRule<EventGridOutputAttribute>()
                .BindToCollector<EventGridOutput>(attr => new EventGridOutputAsyncCollector(this, attr));
        }
    }
}
