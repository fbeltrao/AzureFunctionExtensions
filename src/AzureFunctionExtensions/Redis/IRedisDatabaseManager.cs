using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Defines a redis <see cref="IDatabase"/> connection manager
    /// </summary>
    public interface IRedisDatabaseManager
    {
        /// <summary>
        /// Gets the database
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseId"></param>
        /// <returns></returns>
        IDatabase GetDatabase(string connectionString, int databaseId = -1);
    }
}
