using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Fbeltrao.AzureFunctionExtensions;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CSharpSample
{
    /// <summary>
    /// Samples for Redis usage in Azure functions
    /// </summary>
    public static class RedisSample
    {
        /// <summary>
        /// Retrieve the current value of a value in Redis
        /// </summary>
        /// <param name="req"></param>
        /// <param name="redisItem"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(RetrieveIntegerValue))]
        public static async Task<IActionResult> RetrieveIntegerValue(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [RedisDatabase(Connection = "%redis_connectionstring%")] IDatabase db,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var key = req.Query["key"].ToString();
            if (string.IsNullOrEmpty(key))
                return new BadRequestObjectResult("Please pass a key on the query string or in the request body");

            var result = await db.StringGetAsync(key);
            if (result.IsNullOrEmpty)
                return new NotFoundResult();

            return new OkObjectResult((int)result);
        }


        /// <summary>
        /// Retrieve the current value of a list in Redis
        /// </summary>
        /// <param name="req"></param>
        /// <param name="redisItem"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(RetrieveList))]
        public static async Task<IActionResult> RetrieveList(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [RedisDatabase(Connection = "%redis_connectionstring%")] IDatabase db,
            TraceWriter log)
        {
            var key = req.Query["key"].ToString();
            if (string.IsNullOrEmpty(key))
                return new BadRequestObjectResult("Please pass a key on the query string or in the request body");

            var list = await db.ListRangeAsync(key);
            var resultList = new List<string>();

            if (list != null)
            {
                foreach (var srcItem in list)
                {
                    if (!srcItem.IsNullOrEmpty)
                        resultList.Add((string)srcItem);
                }
            }
            
            return new OkObjectResult(resultList);
        }

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
            [RedisOutput(Connection = "%redis_connectionstring%")] out RedisOutput redisItem,
            TraceWriter log)
        {
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

            redisItem = new RedisOutput()
            {
                Key = key,
                Operation = RedisOutputOperation.IncrementValue,
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
            [RedisOutput(Connection = "%redis_connectionstring%", Key = "%redis_setvalueinredis_key%")]  out RedisOutput redisItem,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();

            redisItem = new RedisOutput()
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
            [RedisOutput(Connection = "%redis_connectionstring%", Key = "myList", Operation = RedisOutputOperation.ListRightPush)]  out RedisOutput redisItem,
            TraceWriter log)
        {
            string itemValue = new StreamReader(req.Body).ReadToEnd();
            if (string.IsNullOrEmpty(itemValue))
                itemValue = req.Query["value"].ToString();

            redisItem = new RedisOutput()
            {
                TextValue = itemValue
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
            [RedisOutput(Connection = "%redis_connectionstring%", Key = "myList", Operation = RedisOutputOperation.ListLeftPush)] out RedisOutput redisItem,
            TraceWriter log)
        {
            string itemValue = new StreamReader(req.Body).ReadToEnd();
            if (string.IsNullOrEmpty(itemValue))
                itemValue = req.Query["value"].ToString();

            redisItem = new RedisOutput()
            {
                TextValue = itemValue
            };

            return new OkResult();
        }
    }
}
