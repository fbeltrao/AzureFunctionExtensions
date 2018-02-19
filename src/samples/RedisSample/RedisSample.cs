
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Fbeltrao.AzureFunctionExtensions;

namespace CSharpSample
{
    /// <summary>
    /// Samples for Redis usage in Azure functions
    /// </summary>
    public static class RedisSample
    {
        /// <summary>
        /// Increments a value in Redis
        /// </summary>
        /// <param name="req"></param>
        /// <param name="redisItem"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(IncrementRedisValue))]
        public static IActionResult IncrementRedisValue(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, 
            [Redis(Connection = "%redis_connectionstring%")] out RedisItem redisItem,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            redisItem = null;

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            var key = data?.key ?? req.Query["key"];
            if (string.IsNullOrEmpty(key))
                return new BadRequestObjectResult("Please pass a key on the query string or in the request body");

            var valueInput = data?.value ?? req.Query["value"];
            if (!long.TryParse(valueInput.ToString(), out long value))
                value = 0;

            if (value == 0)
            {
                return new BadRequestObjectResult("Please pass a value on the query string or in the request body");
            }

            redisItem = new RedisItem()
            {
                Key = key,
                RedisItemOperation = RedisItemOperation.IncrementValue,
                IncrementValue = value
            };

            return new OkResult();
        }


        /// <summary>
        /// Sets a value in Redis
        /// </summary>
        /// <param name="req"></param>
        /// <param name="redisItem"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(SetValueInRedis))]
        public static IActionResult SetValueInRedis(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Redis(Connection = "%redis_connectionstring%", Key = "%redis_setvalueinredis_key%")]  out RedisItem redisItem,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();

            redisItem = new RedisItem()
            {
                TextValue = requestBody
            };

            return new OkResult();
        }


        /// <summary>
        /// Appends a value to a redis list
        /// </summary>
        /// <param name="req"></param>
        /// <param name="redisItem"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(AppendToListInRedis))]
        public static IActionResult AppendToListInRedis(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Redis(Connection = "%redis_connectionstring%", Key = "myList", RedisItemOperation = RedisItemOperation.ListRightPush)]  out RedisItem redisItem,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();

            redisItem = new RedisItem()
            {
                TextValue = requestBody
            };

            return new OkResult();
        }


        /// <summary>
        /// Inserts a value to a redis list
        /// </summary>
        /// <param name="req"></param>
        /// <param name="redisItem"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(InsertToListInRedis))]
        public static IActionResult InsertToListInRedis(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Redis(Connection = "%redis_connectionstring%", Key = "myList", RedisItemOperation = RedisItemOperation.ListLeftPush)]  out RedisItem redisItem,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();

            redisItem = new RedisItem()
            {
                TextValue = requestBody
            };

            return new OkResult();
        }
    }
}
