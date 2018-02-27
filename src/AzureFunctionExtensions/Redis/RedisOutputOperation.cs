namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Defines the possible item operations
    /// </summary>
    public enum RedisOutputOperation
    {
        /// <summary>
        /// Not set
        /// </summary>
        NotSet = 0,

        /// <summary>
        /// Sets the value for a single key
        /// Default
        /// </summary>
        SetKeyValue = 1,

        /// <summary>
        /// Increments the value of a key
        /// </summary>
        IncrementValue,

        /// <summary>
        /// Adds item to the end of a list specified by the key
        /// </summary>
        ListRightPush,

        /// <summary>
        /// Adds items to the start of a list specific by the key
        /// </summary>
        ListLeftPush
    }
}