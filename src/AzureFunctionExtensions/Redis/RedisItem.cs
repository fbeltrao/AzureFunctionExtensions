using Newtonsoft.Json;
using System;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Defines a Redis item to be saved
    /// </summary>
    public class RedisItem 
    {
        /// <summary>
        /// Redis item key
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// Defines the value as text to be set
        /// </summary>
        [JsonProperty("textValue")]
        public string TextValue { get; set; }

        /// <summary>
        /// Defines the value as object to be set. The content will be converted to json using JsonConvert.
        /// </summary>
        [JsonProperty("objectValue")]
        public object ObjectValue { get; set; }

        /// <summary>
        /// Defines the value as a byte array to be set
        /// </summary>
        [JsonProperty("binaryValue")]
        public byte[] BinaryValue { get; set; }

        /// <summary>
        /// Sets the operation to performed in Redis
        /// </summary>
        [JsonProperty("operation")]
        public RedisItemOperation RedisItemOperation { get; set; }

        /// <summary>
        /// Time to live in Redis
        /// </summary>
        [JsonProperty("ttl")]
        public TimeSpan? TimeToLive { get; set; }

        /// <summary>
        /// Value to increment by when used in combination with <see cref="RedisItemOperation.IncrementValue"/>
        /// Default: 1
        /// </summary>
        [JsonProperty("incrementValue")]
        public long IncrementValue { get; set; } = 1;
    }
}