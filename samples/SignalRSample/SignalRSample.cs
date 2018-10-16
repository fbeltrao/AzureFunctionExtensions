using System;
using System.Threading.Tasks;
using Fbeltrao.AzureFunctionExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace SignalRSample
{
    public static class SampleFunctions
    {
        /// <summary>
        /// Http triggered function to broadcast to a SignalR hub
        /// </summary>
        /// <param name="req"></param>
        /// <param name="redisItem"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(HttpTriggerBroadcastToHub))]
        public static async Task<IActionResult> HttpTriggerBroadcastToHub(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [SignalR(ServiceName = "%service_name%", AccessKey = "%access_key%")] IAsyncCollector<SignalRMessage> message,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            await message.AddAsync(new SignalRMessage()
            {
                Target = "broadcastMessage",
                Hub = "chat",
                Arguments = new object[] { $"Hub broadcast from function {nameof(HttpTriggerBroadcastToHub)}", $"Now it is {DateTime.Now}" }
            });

            return new OkResult();
        }

        /// <summary>
        /// Http triggered function to broadcast to a SignalR hub group
        /// </summary>
        /// <param name="req"></param>
        /// <param name="redisItem"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(HttpTriggerBroadcastToGroupHub))]
        public static IActionResult HttpTriggerBroadcastToGroupHub(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [SignalR(ServiceName = "%service_name%", AccessKey = "%access_key%", Groups = "dashboard")] out SignalRMessage message,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");



            message = new SignalRMessage()
            {
                Target = "broadcastMessage",
                Hub = "chat",
                Arguments = new object[] { $"Group broadcast from function {nameof(HttpTriggerBroadcastToGroupHub)}", $"Now it is {DateTime.Now}" }
            };

            return new OkResult();
        }
    }
}
