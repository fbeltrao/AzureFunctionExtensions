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
    public class SignalRConfiguration : IExtensionConfigProvider
    {
        /// <summary>
        /// Your Azure SignalR service will be something like 'example.service.signalr.net'. In that case the value of this property should be 'example'
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// SignalR hub name        
        /// In case of you client code like:
        ///     var connection = new signalR.HubConnectionBuilder()
        ///                      .withUrl('/chat')
        ///                      .build();
        /// The value will be 'chat'
        /// </summary>
        public string Hub { get; set; }

        /// <summary>
        /// SignalR Access Key
        /// Retrieve it from the portal, in the SignalR Azure service under properties
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// (Optional) Identifier of groups that should receive the message
        /// </summary>       
        public string Groups { get; set; }

        /// <summary>
        /// SignalR target
        /// This is the 'target-goes-here' part in your client script
        /// connection.on('target-goes-here', callback);
        /// </summary>
        public string Target { get; set; }



        SignalRMessage CreateMessageFromAttribute(SignalRAttribute attr) => new SignalRMessage()
        {
            Users = Utils.CreateListFrom(attr.Users),
            Groups = Utils.CreateListFrom(attr.Groups),
            Hub = attr.Hub,
            Target = attr.Target
        };

        public void Initialize(ExtensionConfigContext context)
        {
            // Allows user to bind to IAsyncCollector<JObject>, and the sdk will convert that to IAsyncCollector<SignalRMessage>
            context.AddConverter<JObject, SignalRMessage>(input => input.ToObject<SignalRMessage>());

            context.AddConverter<SignalRAttribute, SignalRMessage>(attr => CreateMessageFromAttribute(attr));

            context.AddBindingRule<SignalRAttribute>()                
                .BindToCollector<SignalRMessage>(attr => new SignalRAsyncCollector(this, attr));

            //context.AddBindingRule<SignalRAttribute>()
            //    .BindToInput<SignalRMessage>(attr => CreateMessageFromAttribute(attr));
        }
    }
}
