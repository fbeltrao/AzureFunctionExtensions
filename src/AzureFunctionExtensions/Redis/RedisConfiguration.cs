using Microsoft.Azure.WebJobs.Host.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Initializes the Redis binding
    /// </summary>
    public class RedisConfiguration : IExtensionConfigProvider
    {
        private readonly IRedisDatabaseManager redisDatabaseManager;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="redisDatabaseManager"></param>
        public RedisConfiguration(IRedisDatabaseManager redisDatabaseManager)
        {
            this.redisDatabaseManager = redisDatabaseManager;
        }

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
        /// Default is <see cref="RedisOutputOperation.SetKeyValue"/>
        /// </summary>
        public RedisOutputOperation RedisItemOperation { get; set; }

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
            context.AddConverter<JObject, RedisOutput>(input => input.ToObject<RedisOutput>());

            // Redis output binding
            context
                .AddBindingRule<RedisOutputAttribute>()
                .BindToCollector<RedisOutput>(attr => new RedisOutputAsyncCollector(this, attr, this.redisDatabaseManager));

            // Redis database (input) binding
            context
                .AddBindingRule<RedisDatabaseAttribute>()
                .BindToInput<IDatabase>(ResolveRedisDatabase);
        }

        /// <summary>
        /// Resolves the Redis database as input parameter
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        public IDatabase ResolveRedisDatabase(RedisDatabaseAttribute attr)
        {
            var connectionString = Utils.MergeValueForProperty(attr.Connection, this.Connection);
            return this.redisDatabaseManager.GetDatabase(connectionString);
        }        
    }
}