using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using System;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Binds a function parameter to write to Redis
    /// </summary>
    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter)]
    [Binding]
    public sealed class RedisOutputAttribute : Attribute, IConnectionProvider
    {
        /// <summary>
        /// Redis item key
        /// </summary>
        [AutoResolve(Default = "Key")]
        public string Key { get; set; }

        /// <summary>
        /// Defines the Redis server connection string
        /// </summary>
        [AutoResolve(Default = "ConnectionString")]
        public string Connection { get; set; }

        /// <summary>
        /// Send items in batch to Redis
        /// </summary>
        public bool SendInBatch { get; set; } = true;

        /// <summary>
        /// Send items in contiguous transaction to Redis
        /// </summary>
        public bool SendInTransaction { get; set; } = true;

        /// <summary>
        /// Sets the operation to performed in Redis
        /// Default is <see cref="RedisOutputOperation.SetKeyValue"/>
        /// </summary>
        public RedisOutputOperation Operation { get; set; } = RedisOutputOperation.SetKeyValue;
       
        /// <summary>
        /// Time to live in Redis
        /// </summary>
        public TimeSpan? TimeToLive { get; set; }
    }
}
