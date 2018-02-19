using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Makes a HTTP call as an output of a function
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public class HttpCallAttribute : Attribute
    {
        /// <summary>
        /// Sets the Url for the outgoing message
        /// </summary>
        [AppSetting(Default = "HttpCallUrl")]
        public string Url { get; set; }

        /// <summary>
        /// Http request method (GET [default], POST, PUT, DELETE)
        /// </summary>
        [AutoResolve]
        public string HttpMethod { get; set; } = "GET";

        /// <summary>
        /// Media type
        /// </summary>
        public string MediaType { get; set; }


    }
}
