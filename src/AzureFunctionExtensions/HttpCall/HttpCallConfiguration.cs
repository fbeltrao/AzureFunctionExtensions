using Microsoft.Azure.WebJobs.Host.Config;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Extension for binding <see cref="HttpCallMessage"/>
    /// </summary>
    public class HttpCallConfiguration : IExtensionConfigProvider
    {
        /// <summary>
        /// Sets the Url for the outgoing message
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Http request method (GET [default], POST, PUT, DELETE)
        /// </summary>
        public string HttpMethod { get; set; } = "GET";

        /// <summary>
        /// Media type
        /// </summary>
        public string MediaType { get; set; }        

        public void Initialize(ExtensionConfigContext context)
        {
            // Allows user to bind to IAsyncCollector<JObject>, and the sdk will convert that to IAsyncCollector<HttpCallRequest>
            context.AddConverter<JObject, HttpCallMessage>(input => input.ToObject<HttpCallMessage>());

            context.AddBindingRule<HttpCallAttribute>()
                .BindToCollector<HttpCallMessage>(attr => new HttpCallMessageAsyncCollector(this, attr));
                   
        }
    }
}
