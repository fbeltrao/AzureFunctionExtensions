using System;
using System.Collections.Generic;
using System.Text;

namespace Fbeltrao.AzureFunctionExtensions
{
    public class EventGridEvent
    {

        public string Id { get; private set; }

        public string EventType { get; set; }

        public string Subject { get; set; }

        public string EventTime { get; private set; }

        public object Data { get; set; }


        public EventGridEvent() : this(Guid.NewGuid())
        {
        }

        public EventGridEvent(Guid id, object data) : this(id)
        {
            this.Data = data;
        }

        public EventGridEvent(object data) : this(Guid.NewGuid())
        {
            this.Data = data;
        }

        public EventGridEvent(object data, string eventType, string subject)  : this(Guid.NewGuid())
        {
            this.Data = data;
            this.Subject = subject;
            this.EventType = eventType;
        }


        public EventGridEvent(Guid id)
        {
            Id = id.ToString();

            DateTime localTime = DateTime.Now;
            DateTime utcTime = DateTime.UtcNow;
            DateTimeOffset localTimeAndOffset = new DateTimeOffset(localTime, TimeZoneInfo.Local.GetUtcOffset(localTime));

            EventTime = localTimeAndOffset.ToString("o");

        }
    }
}
