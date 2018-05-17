using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fbeltrao.AzureFunctionExtensions.Test
{
    /// <summary>
    /// Tests <see cref="SignalRAsyncCollector"/> interactions with <see cref="IHttpClientFactory"/>
    /// </summary>
    public class SignalRAsyncCollectorTest
    {
        private readonly Mock<IHttpClientFactory> httpClientFactory;
        private HttpRequestMessage httpRequestMessage;

        public SignalRAsyncCollectorTest()
        {            
            var httpMessageHandler = new HttpMessageHandlerMock()
                .SetupHandler(req =>
                {
                    httpRequestMessage = req;
                    return new HttpResponseMessage(HttpStatusCode.Accepted);
                });

            var httpClient = new HttpClient(httpMessageHandler);

            this.httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(x => x.Create()).Returns(httpClient);
        }

        [Fact]
        public async Task HubBroadcasts_SendsMessageToHub()
        {
            var config = new SignalRConfiguration()
            {
                AccessKey = "0123456789001234567890123456789012345678901234567890123456789",
                Hub = "testhub",
                ServiceName = "unittesthub"
            };

            var attr = new SignalRAttribute()
            {
            };
            

            var target = new SignalRAsyncCollector(config, attr, httpClientFactory.Object);

            await target.AddAsync(new SignalRMessage()
            {
                Target = "broadcastMessage",
                Arguments = new object[] { "say hello", 2 }
            });
            await target.FlushAsync();


            httpClientFactory.VerifyAll();                        

            Assert.NotNull(this.httpRequestMessage);
            Assert.NotNull(this.httpRequestMessage.Headers.Authorization);
            Assert.Equal("https://unittesthub.service.signalr.net:5002/api/v1-preview/hub/testhub", this.httpRequestMessage.RequestUri.ToString());
            Assert.Equal(HttpMethod.Post, this.httpRequestMessage.Method);
            Assert.NotNull(this.httpRequestMessage.Content);
            Assert.IsType<StringContent>(this.httpRequestMessage.Content);
            var stringContent = (StringContent)this.httpRequestMessage.Content;
            var payload = JsonConvert.DeserializeObject<SignalRRestAPIMessage>(await stringContent.ReadAsStringAsync());
            Assert.NotNull(payload);
            Assert.Equal("broadcastMessage", payload.Target);
            Assert.Equal(2, payload.Arguments.Length);
            Assert.Equal("say hello", payload.Arguments[0]);
            Assert.Equal((Int64)2, (Int64)payload.Arguments[1]);
        }

        [Fact]
        public async Task HubGroupBroadcasts_SendsMessageToGroup()
        {
            var config = new SignalRConfiguration()
            {
                AccessKey = "0123456789001234567890123456789012345678901234567890123456789",
                Hub = "testhub",
                ServiceName = "unittesthub"
            };

            var attr = new SignalRAttribute()
            {
            };


            var target = new SignalRAsyncCollector(config, attr, httpClientFactory.Object);

            await target.AddAsync(new SignalRMessage()
            {
                Target = "broadcastMessage",
                Arguments = new object[] { "say hello", 2 },
                Groups = new string[] { "testGroup" }
            });
            await target.FlushAsync();


            httpClientFactory.VerifyAll();

            Assert.NotNull(this.httpRequestMessage);
            Assert.NotNull(this.httpRequestMessage.Headers.Authorization);
            Assert.Equal("https://unittesthub.service.signalr.net:5002/api/v1-preview/hub/testhub/group/testGroup", this.httpRequestMessage.RequestUri.ToString());
            Assert.Equal(HttpMethod.Post, this.httpRequestMessage.Method);
            Assert.NotNull(this.httpRequestMessage.Content);
            Assert.IsType<StringContent>(this.httpRequestMessage.Content);
            var stringContent = (StringContent)this.httpRequestMessage.Content;
            var payload = JsonConvert.DeserializeObject<SignalRRestAPIMessage>(await stringContent.ReadAsStringAsync());
            Assert.NotNull(payload);
            Assert.Equal("broadcastMessage", payload.Target);
            Assert.Equal(2, payload.Arguments.Length);
            Assert.Equal("say hello", payload.Arguments[0]);
            Assert.Equal((Int64)2, (Int64)payload.Arguments[1]);
        }

        [Fact]
        public async Task HubUser_SendsMessageToUser()
        {
            var config = new SignalRConfiguration()
            {
                AccessKey = "0123456789001234567890123456789012345678901234567890123456789",
                Hub = "testhub",
                ServiceName = "unittesthub"
            };

            var attr = new SignalRAttribute()
            {
            };


            var target = new SignalRAsyncCollector(config, attr, httpClientFactory.Object);

            await target.AddAsync(new SignalRMessage()
            {
                Target = "broadcastMessage",
                Arguments = new object[] { "say hello", 2 },
                Users = new string[] { "1000" }
            });
            await target.FlushAsync();


            httpClientFactory.VerifyAll();

            Assert.NotNull(this.httpRequestMessage);
            Assert.NotNull(this.httpRequestMessage.Headers.Authorization);
            Assert.Equal("https://unittesthub.service.signalr.net:5002/api/v1-preview/hub/testhub/user/1000", this.httpRequestMessage.RequestUri.ToString());
            Assert.Equal(HttpMethod.Post, this.httpRequestMessage.Method);
            Assert.NotNull(this.httpRequestMessage.Content);
            Assert.IsType<StringContent>(this.httpRequestMessage.Content);
            var stringContent = (StringContent)this.httpRequestMessage.Content;
            var payload = JsonConvert.DeserializeObject<SignalRRestAPIMessage>(await stringContent.ReadAsStringAsync());
            Assert.NotNull(payload);
            Assert.Equal("broadcastMessage", payload.Target);
            Assert.Equal(2, payload.Arguments.Length);
            Assert.Equal("say hello", payload.Arguments[0]);
            Assert.Equal((Int64)2, (Int64)payload.Arguments[1]);
        }
    }
}
