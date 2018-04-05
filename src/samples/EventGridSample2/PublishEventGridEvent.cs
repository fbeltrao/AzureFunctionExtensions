using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fbeltrao.AzureFunctionExtensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace EventGridSample
{
    /// <summary>
    /// Publish Event Grid Azure function sample
    /// </summary>
    public static class PublishEventGridEvent
    {
        [FunctionName(nameof(PublishEventWithFixTypeAndSubject))]
        public static async Task<HttpResponseMessage> PublishEventWithFixTypeAndSubject(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req, 
            [EventGridOutput(SasKey = "%eventgrid_sas%", EventType = "EventGridSample.PublishEventGridEvent.Event", Subject = "message/fromAzureFunction")] IAsyncCollector<EventGridOutput> events,
            TraceWriter log)
        {           
            // POST? used the body as the content of the event
            dynamic data = await req.Content.ReadAsAsync<object>();
            if (data != null)
            {
                await events.AddAsync(new EventGridOutput(data));
            }
            else
            {
                await events.AddAsync(new EventGridOutput(new
                {
                    createdFrom = nameof(PublishEventGridEvent),
                }));
            }
            
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
