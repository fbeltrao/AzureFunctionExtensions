using Moq;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Threading.Tasks;
using Xunit;

namespace Fbeltrao.AzureFunctionExtensions.Test
{
    /// <summary>
    /// Tests for the <see cref="RedisItemAsyncCollector"/> interactions with Redis
    /// </summary>
    public class RedisItemAsyncCollectorTest
    {
        [Fact]
        public async Task SetKeyValue_ConnectionInConfig_KeyInAttribute_OperationInAttribute_SetsValue()
        {
            var config = new RedisExtensionConfigProvider()
            {
                Connection = "localhost:3679",                
            };

            var attr = new RedisAttribute()
            {
                Key = "myKey",
                RedisItemOperation = RedisItemOperation.SetKeyValue,
            };

            var redisDatabase = new RedisDatabaseMock();

            var connectionManager = new Mock<IRedisDatabaseManager>();
            connectionManager.Setup(x => x.GetDatabase("localhost:3679", -1))
                .Returns(redisDatabase);

            var target = new RedisItemAsyncCollector(config, attr, connectionManager.Object);
            await target.AddAsync(new RedisItem()
            {
                TextValue = "test"
            });

            await target.FlushAsync();

            connectionManager.VerifyAll();

            var actual = redisDatabase.StringGet("myKey");
            Assert.Equal("test", (string)actual);
        }

        [Fact]
        public async Task IncrementValue_ConnectionInConfig_KeyInAttribute_OperationInAttribute_WithoutIncrementValue_IncrementsOne()
        {
            var config = new RedisExtensionConfigProvider()
            {
                Connection = "localhost:3679",
            };

            var attr = new RedisAttribute()
            {
                Key = "myKey",
                RedisItemOperation = RedisItemOperation.IncrementValue,                
            };

            var redisDatabase = new RedisDatabaseMock();

            var connectionManager = new Mock<IRedisDatabaseManager>();
            connectionManager.Setup(x => x.GetDatabase("localhost:3679", -1))
                .Returns(redisDatabase);

            var target = new RedisItemAsyncCollector(config, attr, connectionManager.Object);
            await target.AddAsync(new RedisItem()
            {                
            });

            await target.FlushAsync();

            connectionManager.VerifyAll();

            var actual = redisDatabase.StringGet("myKey");
            Assert.Equal(1, (long)actual);
        }

        [Fact]
        public async Task IncrementValue_ConnectionInConfig_KeyInAttribute_OperationInAttribute_By10_Increments10()
        {
            var config = new RedisExtensionConfigProvider()
            {
                Connection = "localhost:3679",
            };

            var attr = new RedisAttribute()
            {
                Key = "myKey",
                RedisItemOperation = RedisItemOperation.IncrementValue,
            };

            var redisDatabase = new RedisDatabaseMock();

            var connectionManager = new Mock<IRedisDatabaseManager>();
            connectionManager.Setup(x => x.GetDatabase("localhost:3679", -1))
                .Returns(redisDatabase);

            var target = new RedisItemAsyncCollector(config, attr, connectionManager.Object);
            await target.AddAsync(new RedisItem()
            {
                IncrementValue = 10
            });

            await target.FlushAsync();

            connectionManager.VerifyAll();

            var actual = redisDatabase.StringGet("myKey");
            Assert.Equal(10, (long)actual);
        }


        [Fact]
        public async Task ListRightPush_ConnectionInConfig_KeyInAttribute_OperationInAttribute_AddsItemToEndOfList()
        {
            var config = new RedisExtensionConfigProvider()
            {
                Connection = "localhost:3679",
            };

            var attr = new RedisAttribute()
            {
                Key = "myKey",
                RedisItemOperation = RedisItemOperation.ListRightPush,
            };

            var redisDatabase = new RedisDatabaseMock();

            var connectionManager = new Mock<IRedisDatabaseManager>();
            connectionManager.Setup(x => x.GetDatabase("localhost:3679", -1))
                .Returns(redisDatabase);

            var target = new RedisItemAsyncCollector(config, attr, connectionManager.Object);
            await target.AddAsync(new RedisItem()
            {
                TextValue = "second last value"
            });

            await target.AddAsync(new RedisItem()
            {
                TextValue = "last value"
            });

            await target.FlushAsync();

            connectionManager.VerifyAll();

            var actual = redisDatabase.ListRange("myKey", 0);
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Length);
            Assert.Equal("second last value", actual[0].ToString());
            Assert.Equal("last value", actual[1].ToString());            
        }

        [Fact]
        public async Task ListLeftPush_ConnectionInConfig_KeyInAttribute_OperationInAttribute_AddsItemToStartOfList()
        {
            var config = new RedisExtensionConfigProvider()
            {
                Connection = "localhost:3679",
            };

            var attr = new RedisAttribute()
            {
                Key = "myKey",
                RedisItemOperation = RedisItemOperation.ListLeftPush,
            };

            var redisDatabase = new RedisDatabaseMock();

            var connectionManager = new Mock<IRedisDatabaseManager>();
            connectionManager.Setup(x => x.GetDatabase("localhost:3679", -1))
                .Returns(redisDatabase);

            var target = new RedisItemAsyncCollector(config, attr, connectionManager.Object);
            await target.AddAsync(new RedisItem()
            {
                TextValue = "second value"
            });

            await target.AddAsync(new RedisItem()
            {
                TextValue = "first value"
            });

            await target.FlushAsync();

            connectionManager.VerifyAll();

            var actual = redisDatabase.ListRange("myKey", 0);
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Length);
            Assert.Equal("first value", actual[0].ToString());
            Assert.Equal("second value", actual[1].ToString());
        }
    }
}
