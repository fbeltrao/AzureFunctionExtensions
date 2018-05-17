using Microsoft.Azure.WebJobs;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fbeltrao.AzureFunctionExtensions
{

    /// <summary>
    /// Collector for <see cref="SignalRMessage"/>
    /// </summary>
    public class SignalRAsyncCollector : IAsyncCollector<SignalRMessage>
    {
        private SignalRConfiguration config;
        private SignalRAttribute attr;
        private readonly IHttpClientFactory httpClientFactory;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="attr"></param>
        public SignalRAsyncCollector(SignalRConfiguration config, SignalRAttribute attr) : this(config, attr, HttpClientFactory.Instance)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="attr"></param>
        /// <param name="httpClientFactory"></param>
        public SignalRAsyncCollector(SignalRConfiguration config, SignalRAttribute attr, IHttpClientFactory httpClientFactory)
        {
            this.config = config;
            this.attr = attr;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task AddAsync(SignalRMessage item, CancellationToken cancellationToken = default(CancellationToken))
        {
            var mergedItem = MergeMessageProperties(item, config, attr);

            if (string.IsNullOrEmpty(mergedItem.Hub))
                throw new InvalidOperationException($"Missing hub");

            if (string.IsNullOrEmpty(mergedItem.Target))
                throw new InvalidOperationException($"Missing target");
           
            await SendSignalRMessage(mergedItem, attr, cancellationToken);
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
        private static SignalRMessage MergeMessageProperties(SignalRMessage message, SignalRConfiguration config, SignalRAttribute attr)
        {
            var result = new SignalRMessage
            {
                Arguments = message.Arguments,
                Groups = Utils.MergeValueForProperty(message.Groups, Utils.CreateListFrom(config.Groups), Utils.CreateListFrom(attr.Groups)),
                Users = Utils.MergeValueForProperty(message.Users, Utils.CreateListFrom(attr.Users)),
                Hub = Utils.MergeValueForProperty(message.Hub, config.Hub, attr.Hub),                
                Target = Utils.MergeValueForProperty(message.Target, config.Target, attr.Target),                
            };
            

            return result;
        }
 
        private async Task SendSignalRMessage(SignalRMessage message, SignalRAttribute attribute, CancellationToken cancellationToken = default(CancellationToken))
        {
            var signalRServiceName = Utils.MergeValueForProperty(attribute.ServiceName, this.config.ServiceName);
            if (string.IsNullOrEmpty(signalRServiceName))
                throw new InvalidOperationException("Missing Azure SignalR name");

            var accessKey = Utils.MergeValueForProperty(attribute.AccessKey, config.AccessKey);
            if (string.IsNullOrEmpty(accessKey))
                throw new InvalidOperationException("Missing Azure SignalR access key");

            try
            {
                var payload = new SignalRRestAPIMessage
                {
                    Target = message.Target,
                    Arguments = message.Arguments
                };


                // URL based on swagger: https://github.com/Azure/azure-signalr/blob/dev/docs/swagger.json
                var url = new StringBuilder()
                    .Append("https://")
                    .Append(signalRServiceName)
                    .Append(".service.signalr.net:5002/api/v1-preview/hub/")
                    .Append(message.Hub);

                if (message.Groups?.Count > 0)
                {
                    if (message.Groups.Count == 1)
                    {
                        url.Append("/group/").Append(message.Groups[0]);
                    }
                    else
                    {
                        url.Append("/groups/").Append(string.Join(",", message.Groups));                        
                    }                            
                }
                else if (message.Users?.Count > 0)
                {
                    if (message.Users.Count == 1)
                    {
                        url.Append("/user/").Append(message.Users[0]);
                    }
                    else
                    {
                        url.Append("/users/").Append(string.Join(",", message.Users));
                    }
                }

                var destinationUrl = url.ToString();


                var httpRequest = new HttpRequestMessage(HttpMethod.Post, destinationUrl);
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(destinationUrl, accessKey));
                httpRequest.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                var response = await this.httpClientFactory.Create().SendAsync(httpRequest, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"SignalR '{destinationUrl}' returned status {response.StatusCode}");
                }
            }
            catch (InvalidOperationException invalidOperationException)
            {
                Console.WriteLine(invalidOperationException.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Generates a token for the Azure SignalR REST API
        /// </summary>
        /// <param name="url"></param>
        /// <param name="accessKey"></param>
        /// <returns></returns>
        private string GetToken(string url, string accessKey)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtTokenHandler.CreateJwtSecurityToken(
                issuer: null,
                audience: url,
                subject: null,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}