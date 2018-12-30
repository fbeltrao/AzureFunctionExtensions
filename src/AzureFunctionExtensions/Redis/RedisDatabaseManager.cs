using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Redis connection manager
    /// </summary>
    public class RedisDatabaseManager : IRedisDatabaseManager
    {
        System.Collections.Concurrent.ConcurrentDictionary<string, Lazy<ConnectionMultiplexer>> connections;

        /// <summary>
        /// Constructor
        /// </summary>
        public RedisDatabaseManager()
        {
            connections = new System.Collections.Concurrent.ConcurrentDictionary<string, Lazy<ConnectionMultiplexer>>();
        }

        /// <summary>
        /// Gets the redis database
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseId"></param>
        /// <returns></returns>
        public IDatabase GetDatabase(string connectionString, int databaseId = -1)
        {
            return GetConnection(connectionString).GetDatabase(databaseId);
        }

        /// <summary>
        /// Retrieves the connection for the connectionString
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        IConnectionMultiplexer GetConnection(string connectionString)
        {
            return this.connections.GetOrAdd(connectionString, (localConnectionString) =>
            {
                return new Lazy<ConnectionMultiplexer>(() =>
                {
                    return ConnectionMultiplexer.Connect(localConnectionString);
                });
            })
            .Value;
            
        }

    }
}
