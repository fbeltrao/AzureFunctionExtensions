using Microsoft.Azure.WebJobs;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Collector for <see cref="HttpCallMessage"/>
    /// </summary>
    public class HttpCallMessageAsyncCollector : IAsyncCollector<HttpCallMessage>
    {
        private HttpCallConfiguration config;
        private HttpCallAttribute attr;
        private readonly IHttpClientFactory httpClientFactory;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="attr"></param>
        public HttpCallMessageAsyncCollector(HttpCallConfiguration config, HttpCallAttribute attr) : this(config, attr, HttpClientFactory.Instance)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="attr"></param>
        /// <param name="httpClientFactory"></param>
        public HttpCallMessageAsyncCollector(HttpCallConfiguration config, HttpCallAttribute attr, IHttpClientFactory httpClientFactory)
        {
            this.config = config;
            this.attr = attr;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task AddAsync(HttpCallMessage item, CancellationToken cancellationToken = default(CancellationToken))
        {
            var mergedItem = MergeMessageProperties(item, config, attr);
            await SendHttpCallRequest(mergedItem, attr, cancellationToken);
        }

        public Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Combine <see cref="HttpCallMessage"/> with <see cref="HttpCallConfiguration"/> and <see cref="HttpCallAttribute"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="config"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        private static HttpCallMessage MergeMessageProperties(HttpCallMessage message, HttpCallConfiguration config, HttpCallAttribute attr)
        {
            var result = new HttpCallMessage
            {
                HttpMethod = Utils.MergeValueForProperty(message.HttpMethod, config.HttpMethod, attr.HttpMethod),
                Url = Utils.MergeValueForProperty(message.Url, config.Url, attr.Url),
                MediaType = Utils.MergeValueForProperty(message.MediaType, config.MediaType, attr.MediaType),
                Body = message.Body
            };

            return result;
        }
 
        private async Task SendHttpCallRequest(HttpCallMessage message, HttpCallAttribute attribute, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                HttpResponseMessage response = null;
                switch (message.HttpMethod.ToUpper())
                {
                    case "GET":
                        {
                            response = await this.httpClientFactory.Create().GetAsync(message.Url, cancellationToken);
                            break;
                        }

                    case "POST":
                        {
                            var content = new StringContent(message.Body ?? string.Empty, Encoding.UTF8, message.MediaType);
                            response = await this.httpClientFactory.Create().PostAsync(message.Url, content, cancellationToken);
                            break;
                        }

                    case "PUT":
                        {
                            var content = new StringContent(message.Body ?? string.Empty, Encoding.UTF8, message.MediaType);
                            response = await this.httpClientFactory.Create().PutAsync(message.Url, content, cancellationToken);
                            break;
                        }
                    case "DELETE":
                        {
                            response = await this.httpClientFactory.Create().DeleteAsync(message.Url, cancellationToken);
                            break;
                        }
                    default:
                        {
                            throw new InvalidOperationException($"Http method {message.HttpMethod} not implemented");
                        }
                }
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Http call to '{message.Url}' method {message.HttpMethod} returned status {response.StatusCode}");
                }
            }
            catch (InvalidOperationException invalidOperationException)
            {
                Console.WriteLine(invalidOperationException.Message);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}