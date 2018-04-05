using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Azure Event Grid contract as described here https://docs.microsoft.com/en-us/azure/event-grid/event-schema
    /// </summary>
    public class AzureEventGridEvent
    {
        /// <summary>
        /// Unique identifier for the event
        /// </summary>
        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; private set; }

        /// <summary>
        /// One of the registered event types for this event source
        /// </summary>
        [JsonProperty("eventType", Required = Required.Always)]
        public string EventType { get; set; }

        /// <summary>
        /// Publisher-defined path to the event subject
        /// </summary>
        [JsonProperty("subject", Required = Required.Always)]
        public string Subject { get; set; }

        /// <summary>
        /// The time the event is generated based on the provider's UTC time
        /// </summary>
        [JsonProperty("eventTime", Required = Required.Always)]
        public string EventTime { get; private set; }

        /// <summary>
        /// Event data specific to the resource provider
        /// </summary>
        [JsonProperty("data", Required = Required.Always)]
        public object Data { get; set; }

        /// <summary>
        /// The schema version of the data object. The publisher defines the schema version
        /// </summary>
        [DefaultValue(null)]
        [JsonProperty("dataVersion", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string DataVersion { get; set; }
               

        /// <summary>
        /// Constructor
        /// </summary>

        public AzureEventGridEvent() : this(Guid.NewGuid())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public AzureEventGridEvent(Guid id, object data) : this(id)
        {
            this.Data = data;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public AzureEventGridEvent(object data) : this(Guid.NewGuid())
        {
            this.Data = data;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public AzureEventGridEvent(object data, string eventType, string subject)  : this(Guid.NewGuid())
        {
            this.Data = data;
            this.Subject = subject;
            this.EventType = eventType;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public AzureEventGridEvent(Guid id)
        {
            Id = id.ToString();

            DateTime localTime = DateTime.Now;
            DateTime utcTime = DateTime.UtcNow;
            DateTimeOffset localTimeAndOffset = new DateTimeOffset(localTime, TimeZoneInfo.Local.GetUtcOffset(localTime));

            EventTime = localTimeAndOffset.ToString("o");
        }
    }
}
