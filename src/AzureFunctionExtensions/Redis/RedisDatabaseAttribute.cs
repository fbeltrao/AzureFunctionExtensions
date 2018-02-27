using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using StackExchange.Redis;
using System;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Binds a function parameter to a <see cref="IDatabase"/>.
    /// Allows you to call <see cref="IDatabase"/> methods to interact with a Redis instance
    /// </summary>
    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter)]
    [Binding]
    public sealed class RedisDatabaseAttribute : Attribute, IConnectionProvider
    {           
        /// <summary>
        /// Defines the Redis server connection string
        /// </summary>
        [AutoResolve(Default = "ConnectionString")]
        public string Connection { get; set; }            
    }
}
