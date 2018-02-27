using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Collector for <see cref="RedisOutput"/>    
    /// </summary>
    public class RedisOutputAsyncCollector : IAsyncCollector<RedisOutput>
    {
        readonly RedisConfiguration config;
        readonly RedisOutputAttribute attr;
        readonly IRedisDatabaseManager redisDatabaseManager;
        readonly List<RedisOutput> redisOutputCollection;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="attr"></param>
        public RedisOutputAsyncCollector(RedisConfiguration config, RedisOutputAttribute attr) : this(config, attr, RedisDatabaseManager.GetInstance())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="attr"></param>
        public RedisOutputAsyncCollector(RedisConfiguration config, RedisOutputAttribute attr, IRedisDatabaseManager redisDatabaseManager)
        {
            this.config = config;
            this.attr = attr;
            this.redisDatabaseManager = redisDatabaseManager;
            this.redisOutputCollection = new List<RedisOutput>();
        }

        /// <summary>
        /// Adds item to collection to be sent to redis
        /// </summary>
        /// <param name="item"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task AddAsync(RedisOutput item, CancellationToken cancellationToken = default(CancellationToken))
        {
            // create item based on the item + attribute + configuration
            var finalItem = new RedisOutput()
            {
                BinaryValue = item.BinaryValue,
                ObjectValue = item.ObjectValue,
                TextValue = item.TextValue,                
                Key = Utils.MergeValueForProperty(item.Key, attr.Key, config.Key),
                TimeToLive = Utils.MergeValueForNullableProperty<TimeSpan>(item.TimeToLive, attr.TimeToLive, config.TimeToLive),
                IncrementValue = item.IncrementValue,
                Operation = item.Operation
            };

            if (finalItem.Operation == RedisOutputOperation.NotSet)
            {
                if (attr.Operation != RedisOutputOperation.NotSet)
                    finalItem.Operation = attr.Operation;
                else
                    finalItem.Operation = config.RedisItemOperation;
            }



            if (this.config.SendInBatch)
            {
                this.redisOutputCollection.Add(finalItem);
            }
            else
            {
                await SendToRedis(finalItem);
            }            
        }

        /// <summary>
        /// Flushs all items to redis
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var item in this.redisOutputCollection)
            {
                await SendToRedis(item);

                if (cancellationToken != null && cancellationToken.IsCancellationRequested)
                    break;
            }
        }

        /// <summary>
        /// Sends <see cref="RedisOutput"/> to Redis
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        async Task SendToRedis(RedisOutput item)
        {
            var connectionString = Utils.MergeValueForProperty(attr.Connection, config.Connection);            
            var db = redisDatabaseManager.GetDatabase(connectionString); // TODO: add support for multiple databases

            RedisValue value = CreateRedisValue(item);

            switch (item.Operation)
            {
                case RedisOutputOperation.SetKeyValue:
                    {
                        await db.StringSetAsync(item.Key, value, item.TimeToLive, When.Always, CommandFlags.FireAndForget);
                        break;
                    }

                case RedisOutputOperation.IncrementValue:
                    {
                        await db.StringIncrementAsync(item.Key, item.IncrementValue);
                        break;
                    }

                case RedisOutputOperation.ListRightPush:
                    {
                        await db.ListRightPushAsync(item.Key, value, When.Always, CommandFlags.FireAndForget);
                        break;
                    }

                case RedisOutputOperation.ListLeftPush:
                    {
                        await db.ListLeftPushAsync(item.Key, value, When.Always, CommandFlags.FireAndForget);
                        break;
                    }
            }
        }

        /// <summary>
        /// Helper method that creates a <see cref="RedisValue"/> based on <see cref="RedisOutput"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private RedisValue CreateRedisValue(RedisOutput item)
        {
            RedisValue returnValue = RedisValue.Null;
            if (item.BinaryValue != null)
            {
                returnValue = item.BinaryValue;
            }
            else if (item.ObjectValue != null)
            {
                returnValue = JsonConvert.SerializeObject(item.ObjectValue);
            }
            else
            {
                returnValue = item.TextValue;
            }

            return returnValue;
        }
    }
}
