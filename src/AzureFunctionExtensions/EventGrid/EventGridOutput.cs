using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Event grid output
    /// </summary>
    public class EventGridOutput
    {
        public EventGridOutput()
        {
        }

        public EventGridOutput(object data)
        {
            this.Data = data;
        }

        public EventGridOutput(object data, string eventType, string subject)
        {
            this.Data = data;
            this.EventType = eventType;
            this.Subject = subject;
        }

        /// <summary>
        /// Sets event type (Fluent API)
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public EventGridOutput WithEventType(string eventType)
        {
            this.EventType = eventType;
            return this;
        }



        /// <summary>
        /// Sets the Event Grid event type
        /// </summary>
        [JsonProperty("eventType")]
        public string EventType { get; set; }

        /// <summary>
        /// Sets the Event Grid event subject
        /// </summary>
        [JsonProperty("subject")]
        public string Subject { get; set; }

        /// <summary>
        /// Sets subject (Fluent API)
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public EventGridOutput WithSubject(string subject)
        {
            this.Subject = subject;
            return this;
        }

        /// <summary>
        /// The event grid data
        /// </summary>
        [JsonProperty("data")]
        public object Data { get; set; }
    }
}
