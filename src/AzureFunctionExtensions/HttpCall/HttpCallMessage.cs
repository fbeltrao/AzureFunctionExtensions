using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Http request message to be send
    /// </summary>
    public class HttpCallMessage
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public HttpCallMessage()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url"></param>
        public HttpCallMessage(string url)
        {
            this.Url = url;
        }

        /// <summary>
        /// Http request method (GET [default], POST, PUT, DELETE)
        /// </summary>
        [JsonProperty("method")]
        public string HttpMethod { get; set; }

        /// <summary>
        /// Body to be sent (if <see cref="HttpMethod"/> == POST or PUT)
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; set; }

        /// <summary>
        /// Url to make Http request call
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Media type
        /// </summary>
        [JsonProperty("mediaType")]
        public string MediaType { get; set; }

        public HttpCallMessage AsJson()
        {
            this.MediaType = "application/json";
            return this;
        }

        /// <summary>
        /// Sets the <see cref="HttpCallMessage"/> to be a json post with the specified payload body
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public HttpCallMessage AsJsonPost(object payload)
        {
            this.MediaType = "application/json";
            this.HttpMethod = "POST";
            this.Body = JsonConvert.SerializeObject(payload);

            return this;
        }

    }
}
