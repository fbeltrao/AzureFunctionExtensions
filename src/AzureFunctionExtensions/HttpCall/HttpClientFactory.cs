using System.Net.Http;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Default implementation of <see cref="IHttpClientFactory"/>
    /// </summary>
    internal class HttpClientFactory : IHttpClientFactory
    {
        internal static HttpClientFactory Instance = new HttpClientFactory();

        HttpClient client;

        /// <summary>
        /// Constructor
        /// </summary>
        public HttpClientFactory()
        {
            this.client = new HttpClient();
        }

        /// <summary>
        /// Creates an instance of <see cref="HttpClient"/>
        /// </summary>
        /// <returns></returns>
        public HttpClient Create()
        {
            return this.client;
        }
    }
}
