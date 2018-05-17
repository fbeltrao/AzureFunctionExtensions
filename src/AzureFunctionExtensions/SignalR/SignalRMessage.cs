using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// SignalR message to be sent
    /// </summary>
    public class SignalRMessage
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SignalRMessage()
        {

        }


        /// <summary>
        /// SignalR hub name
        /// Your Azure SignalR service will be something like 'example.service.signalr.net'. In that case the value of this property should be 'example'
        /// </summary>
        [JsonProperty("hub")]
        public string Hub { get; set; }

        /// <summary>
        /// SignalR target
        /// This is the 'target-goes-here' part in your client script
        /// connection.on('target-goes-here', callback);
        /// </summary>
        [JsonProperty("target")]
        public string Target { get; set; }

        /// <summary>
        /// SignalR arguments
        /// Those arguments will be mapped to your callback function (param1, param2 and param3)
        /// connection.on('target-goes-here', function(param1, param2, param3) {});
        /// </summary>
        [JsonProperty("arguments")]
        public object[] Arguments { get; set; }
        
        /// <summary>
        /// (Optional) Identifier of users that should receive the message
        /// </summary>
        [JsonProperty("users", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IReadOnlyList<string> Users { get; set; }

        /// <summary>
        /// (Optional) Identifier of groups that should receive the message
        /// </summary>
        [JsonProperty("groups", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IReadOnlyList<string> Groups { get; set; }


    }
}
