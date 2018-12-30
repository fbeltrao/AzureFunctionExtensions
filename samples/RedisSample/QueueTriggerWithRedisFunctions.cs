using System;
using System.Threading.Tasks;
using Fbeltrao.AzureFunctionExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Fbeltrao.RedisSample
{
    public static class QueueTriggerWithRedisFunctions
    {            
        [FunctionName(nameof(MoveQueueItemToRedis))]
        public static async Task MoveQueueItemToRedis(
            [QueueTrigger("myqueue-items", Connection = "AzureWebJobsStorage")] string myQueueItem,
            [RedisDatabase(Connection = "%redis_connectionstring%")] IDatabase db,
            ILogger log)
        {
            await db.ListRightPushAsync("my-queue", myQueueItem);
            log.LogInformation($"Queue trigger pushed to redis list: {myQueueItem}");
        }


        [FunctionName(nameof(GetRedisList))]
        public static async Task<IActionResult> GetRedisList(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [RedisDatabase(Connection = "%redis_connectionstring%")] IDatabase db,
            ILogger log)
        {
            var values = await db.ListRangeAsync("my-queue");
            return new OkObjectResult(new { values });
        }
    }
}
