using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// IAsyncCollector for <see cref="EventGridOutput"/>
    /// </summary>
    public class EventGridOutputAsyncCollector : IAsyncCollector<EventGridOutput>
    {
        private EventGridOutputConfiguration config;
        private EventGridOutputAttribute attr;
        private readonly IHttpClientFactory httpClientFactory;
        List<AzureEventGridEvent> eventGridEvents;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="attr"></param>
        public EventGridOutputAsyncCollector(EventGridOutputConfiguration config, EventGridOutputAttribute attr) : this(config, attr, HttpClientFactory.Instance)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="attr"></param>
        /// <param name="httpClientFactory"></param>
        public EventGridOutputAsyncCollector(EventGridOutputConfiguration config, EventGridOutputAttribute attr, IHttpClientFactory httpClientFactory)
        {
            this.config = config;
            this.attr = attr;
            this.httpClientFactory = httpClientFactory;
            this.eventGridEvents = new List<AzureEventGridEvent>();
        }

        

        public Task AddAsync(EventGridOutput item, CancellationToken cancellationToken = default(CancellationToken))
        {
            var eventGridEvent = CreateEventGridEvent(item, config, attr);
            this.eventGridEvents.Add(eventGridEvent);

            return Task.CompletedTask;
        }

        public async Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var sasKey = Utils.MergeValueForProperty(attr.SasKey, config.SasKey);
            if (string.IsNullOrEmpty(sasKey)) throw new ArgumentException("Sas Key is missing", nameof(sasKey));

            var topicEndpoint = Utils.MergeValueForProperty(attr.TopicEndpoint, config.TopicEndpoint);
            if (string.IsNullOrEmpty(topicEndpoint)) throw new ArgumentException("Topic endpoint is missing", nameof(topicEndpoint));

            string jsonContent = JsonConvert.SerializeObject(this.eventGridEvents);

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            content.Headers.Add("aeg-sas-key", sasKey);
            var response = await httpClientFactory.Create().PostAsync(topicEndpoint, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error sending event grid events (code: {response.StatusCode})");

        }

        /// <summary>
        /// Combine <see cref="HttpCallMessage"/> with <see cref="HttpCallConfiguration"/> and <see cref="HttpCallAttribute"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="config"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        private static AzureEventGridEvent CreateEventGridEvent(EventGridOutput @event, EventGridOutputConfiguration config, EventGridOutputAttribute attr)
        {
            var eventType = Utils.MergeValueForProperty(@event.EventType, config.EventType, attr.EventType);
            if (string.IsNullOrEmpty(eventType)) throw new ArgumentException("Event Grid event type is missing", nameof(eventType));
            
            var subject = Utils.MergeValueForProperty(@event.Subject, config.Subject, attr.Subject);
            if (string.IsNullOrEmpty(subject)) throw new ArgumentException("Event Grid event subject is missing", nameof(subject));
            

            var eventGridEvent = new AzureEventGridEvent(@event.Data, eventType, subject);

            // optional: data version
            var dataVersion = Utils.MergeValueForProperty(@event.DataVersion, config.DataVersion, attr.DataVersion);
            if (!string.IsNullOrEmpty(dataVersion))
                eventGridEvent.DataVersion = dataVersion;            

            return eventGridEvent;
        }
    }
}
