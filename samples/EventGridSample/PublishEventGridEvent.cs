using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Fbeltrao.AzureFunctionExtensions;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace EventGridSample
{
    /// <summary>
    /// Publish Event Grid Azure function sample
    /// </summary>
    public static class PublishEventGridEvent
    {
        /// <summary>
        /// Publishes a Event Grid event with fix type and subject
        /// </summary>
        /// <param name="req"></param>
        /// <param name="outputEvent"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(WithFixTypeAndSubject))]
        public static IActionResult WithFixTypeAndSubject(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [EventGridOutput(SasKey = "%eventgrid_sas%", TopicEndpoint = "%eventgrid_endpoint%",  EventType = "EventGridSample.PublishEventGridEvent.Event", Subject = "message/fromAzureFunction")] out EventGridOutput outputEvent,
            ILogger log)
        {
            outputEvent = null;

            // POST? used the body as the content of the event
            string requestBody = new StreamReader(req.Body).ReadToEnd();


            if (!string.IsNullOrEmpty(requestBody))
            {
                outputEvent = new EventGridOutput(JsonConvert.DeserializeObject(requestBody));
            }
            else
            {
                outputEvent = new EventGridOutput(new
                {
                    city = "Zurich",
                    country = "CHE",
                    customerID = 123120,
                    firstName = "Mark",
                    lastName = "Muller",
                    userID = "123",
                })
                .WithEventType("MyCompany.Customer.Created")
                .WithSubject($"customer/created/CHE/Zurich");
            }

            return new OkResult();
        }


        /// <summary>
        /// Publishes multiple event grid events (Sync)
        /// </summary>
        /// <param name="req"></param>
        /// <param name="outputEvents"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(MultipleSync))]
        public static IActionResult MultipleSync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [EventGridOutput(SasKey = "%eventgrid_sas%", TopicEndpoint = "%eventgrid_endpoint%", EventType = "EventGridSample.PublishEventGridEvent.Event", Subject = "message/fromAzureFunction")] out EventGridOutput[] outputEvents,
            ILogger log)
        {
            outputEvents = new EventGridOutput[]
            {
                new EventGridOutput(new { source = "MultipleSync", index = 0, ticks = DateTime.UtcNow.Ticks }),
                new EventGridOutput(new { source = "MultipleSync", index = 1, ticks = DateTime.UtcNow.Ticks }),
                new EventGridOutput(new { source = "MultipleSync", index = 2, ticks = DateTime.UtcNow.Ticks }),
                new EventGridOutput(new { source = "MultipleSync", index = 3, ticks = DateTime.UtcNow.Ticks }),
                new EventGridOutput(new { source = "MultipleSync", index = 4, ticks = DateTime.UtcNow.Ticks }),
                new EventGridOutput(new { source = "MultipleSync", index = 5, ticks = DateTime.UtcNow.Ticks }).WithDataVersion("1.0"),
            };

            return new OkResult();
        }


        /// <summary>
        /// Publishes multiple event grid events (Async)
        /// </summary>
        /// <param name="req"></param>
        /// <param name="outputEvents"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(MultipleAsync))]
        public static async Task<IActionResult> MultipleAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [EventGridOutput(SasKey = "%eventgrid_sas%", TopicEndpoint = "%eventgrid_endpoint%", EventType = "EventGridSample.PublishEventGridEvent.Event", Subject = "message/fromAzureFunction")] IAsyncCollector<EventGridOutput> outputEvents,
            ILogger log)
        {
            await outputEvents.AddAsync(new EventGridOutput(new { source = "MultipleAsync", index = 0, ticks = DateTime.UtcNow.Ticks }));
            await outputEvents.AddAsync(new EventGridOutput(new { source = "MultipleAsync", index = 1, ticks = DateTime.UtcNow.Ticks }));
            await outputEvents.AddAsync(new EventGridOutput(new { source = "MultipleAsync", index = 2, ticks = DateTime.UtcNow.Ticks }));
            await outputEvents.AddAsync(new EventGridOutput(new { source = "MultipleAsync", index = 3, ticks = DateTime.UtcNow.Ticks }));
            await outputEvents.AddAsync(new EventGridOutput(new { source = "MultipleAsync", index = 4, ticks = DateTime.UtcNow.Ticks }));
            await outputEvents.AddAsync(new EventGridOutput(new { source = "MultipleAsync", index = 5, ticks = DateTime.UtcNow.Ticks }).WithDataVersion("1.0"));
            

            return new OkResult();
        }
    }
}
