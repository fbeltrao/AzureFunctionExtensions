using Microsoft.Azure.WebJobs.Host.Config;
using Newtonsoft.Json.Linq;
using System;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Initializes the Redis binding
    /// </summary>
    public class RedisExtensionConfigProvider : IExtensionConfigProvider
    {
        /// <summary>
        /// Redis item key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Defines the Redis server connection string
        /// </summary>
        public string Connection { get; set; }

        /// <summary>
        /// Send items in batch to Redis
        /// </summary>
        public bool SendInBatch { get; set; } = true;


        /// <summary>
        /// Sets the operation to performed in Redis
        /// Default is <see cref="RedisItemOperation.SetKeyValue"/>
        /// </summary>
        public RedisItemOperation RedisItemOperation { get; set; }

        /// <summary>
        /// Time to live in Redis
        /// </summary>
        public TimeSpan? TimeToLive { get; set; }

        /// <summary>
        /// Initializes attributes, configuration and async collector
        /// </summary>
        /// <param name="context"></param>
        public void Initialize(ExtensionConfigContext context)
        {
            // Converts json to RedisItem
            context.AddConverter<JObject, RedisItem>(input => input.ToObject<RedisItem>());

            // Use RedisItemAsyncCollector to send items to Redis
            context.AddBindingRule<RedisAttribute>()
                .BindToCollector<RedisItem>(attr => new RedisItemAsyncCollector(this, attr));
        }
    }
}