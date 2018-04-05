using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Attribute to define an Event Grid output
    /// </summary>
    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter)]
    [Binding]
    public class EventGridOutputAttribute : Attribute
    {
        /// <summary>
        /// Sets the Sas Key to authenticate Event Grid requests
        /// </summary>
        [AutoResolve(Default = "SasKey")]
        public string SasKey { get; set; }

        /// <summary>
        /// Sets the Event Grid Endpoint (https://xxx.{region}.eventgrid.azure.net/api/events)
        /// </summary>
        [AutoResolve(Default = "TopicEndpoint")]
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
    }
}
