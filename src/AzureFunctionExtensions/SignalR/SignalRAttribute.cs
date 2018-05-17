using System;
using System.Collections.Generic;
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
    public class SignalRAttribute : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SignalRAttribute()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceName">Your Azure SignalR service. 
        /// Will be something like 'example.service.signalr.net'. In that case the value of this property should be 'example'</param>
        /// <param name="hub">
        /// SignalR hub name        
        /// In case of you client code like:
        ///     var connection = new signalR.HubConnectionBuilder()
        ///                      .withUrl('/chat')
        ///                      .build();
        /// The value will be 'chat'
        /// </param>
        public SignalRAttribute(string serviceName, string hub)
        {
            this.ServiceName = serviceName;
            this.Hub = hub;
        }


        /// <summary>
        /// Your Azure SignalR service. 
        /// Will be something like 'example.service.signalr.net'. In that case the value of this property should be 'example'
        /// </summary>
        //[AppSetting(Default = "SignalR")]
        [AutoResolve]
        public string ServiceName { get; set; }

        /// <summary>
        /// SignalR hub name        
        /// In case of you client code like:
        ///     var connection = new signalR.HubConnectionBuilder()
        ///                      .withUrl('/chat')
        ///                      .build();
        /// The value will be 'chat'
        /// </summary>
        // [AppSetting(Default = "SignalRHub")]
        [AutoResolve]
        public string Hub { get; set; }

        /// <summary>
        /// SignalR Access Key
        /// Retrieve it from the portal, in the SignalR Azure service under properties
        /// </summary>
       // [AppSetting(Default = "SignalRAccessKey")]
        [AutoResolve]
        public string AccessKey { get; set; }

        /// <summary>
        /// (Optional) Identifier of users that should receive the message
        /// </summary>
        [AutoResolve]
        public string Users { get; set; }

        /// <summary>
        /// (Optional) Identifier of groups that should receive the message
        /// </summary>       
        [AutoResolve]
        public string Groups { get; set; }


        /// <summary>
        /// SignalR target
        /// This is the 'target-goes-here' part in your client script
        /// connection.on('target-goes-here', callback);
        /// </summary>
       // [AppSetting(Default = "SignalRTarget")]
        [AutoResolve]
        public string Target { get; set; }
    }
}
