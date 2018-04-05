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
        /// <summary>
        /// Constructor
        /// </summary>
        public EventGridOutput()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public EventGridOutput(object data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public EventGridOutput(object data, string eventType, string subject)
        {
            this.Data = data;
            this.EventType = eventType;
            this.Subject = subject;
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
        /// The event grid data
        /// </summary>
        [JsonProperty("data")]
        public object Data { get; set; }

        /// <summary>
        /// (Optional) Sets the Event Grid data version
        /// </summary>
        [JsonProperty("dataVersion")]
        public string DataVersion { get; set; }

     
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
        /// Sets data version (Fluent API)
        /// </summary>
        /// <param name="dataVersion"></param>
        /// <returns></returns>
        public EventGridOutput WithDataVersion(string dataVersion)
        {
            this.DataVersion = dataVersion;
            return this;
        }
    }
}
