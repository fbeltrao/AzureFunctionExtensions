using Moq;
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
    /// Tests <see cref="HttpCallMessageAsyncCollector"/> interactions with <see cref="IHttpClientFactory"/>
    /// </summary>
    public class HttpCallMessageAsyncCollectorTest
    {

        [Fact]
        public async Task Get_Request_Calls_Correct_Url_WithoutBody()
        {
            HttpCallConfiguration config = new HttpCallConfiguration()
            {
                HttpMethod = "GET",
                MediaType = "application/json",
                Url = "http://www.example.com/webhook"
            };

            HttpCallAttribute attr = new HttpCallAttribute()
            {
            };

            HttpRequestMessage message = null;
            var httpMessageHandler = new HttpMessageHandlerMock()
                .SetupHandler(req =>
                {
                    message = req;
                    return new HttpResponseMessage(HttpStatusCode.OK);
                });

            var httpClient = new HttpClient(httpMessageHandler);
                     
            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(x => x.Create()).Returns(httpClient);

            var target = new HttpCallMessageAsyncCollector(config, attr, httpClientFactory.Object);

            await target.AddAsync(new HttpCallMessage()
            {
                Body = "{ type: 'test' }"
            });
            await target.FlushAsync();


            httpClientFactory.VerifyAll();                        

            Assert.NotNull(message);
            Assert.Equal("http://www.example.com/webhook", message.RequestUri.ToString());
            Assert.Equal(HttpMethod.Get, message.Method);
            Assert.Null(message.Content);
        }

        [Fact]
        public async Task Delete_Request_Calls_Correct_Url_WithoutBody()
        {
            HttpCallConfiguration config = new HttpCallConfiguration()
            {
                HttpMethod = "DELETE",
                MediaType = "application/json",
                Url = "http://www.example.com/webhook"
            };

            HttpCallAttribute attr = new HttpCallAttribute()
            {
            };

            HttpRequestMessage message = null;
            var httpMessageHandler = new HttpMessageHandlerMock()
                .SetupHandler(req =>
                {
                    message = req;
                    return new HttpResponseMessage(HttpStatusCode.OK);
                });

            var httpClient = new HttpClient(httpMessageHandler);

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(x => x.Create()).Returns(httpClient);

            var target = new HttpCallMessageAsyncCollector(config, attr, httpClientFactory.Object);

            await target.AddAsync(new HttpCallMessage()
            {
                Body = "{ type: 'test' }"
            });
            await target.FlushAsync();


            httpClientFactory.VerifyAll();

            Assert.NotNull(message);
            Assert.Equal("http://www.example.com/webhook", message.RequestUri.ToString());
            Assert.Equal(HttpMethod.Delete, message.Method);
            Assert.Null(message.Content);
        }

        [Fact]
        public async Task Post_Request_Calls_Correct_Url_WithBody()
        {
            HttpCallConfiguration config = new HttpCallConfiguration()
            {
                HttpMethod = "POST",
                MediaType = "application/json",
                Url = "http://www.example.com/webhook",                
            };

            HttpCallAttribute attr = new HttpCallAttribute()
            {
            };

            HttpRequestMessage message = null;
            var httpMessageHandler = new HttpMessageHandlerMock()
                .SetupHandler(req =>
                {
                    message = req;
                    return new HttpResponseMessage(HttpStatusCode.OK);
                });

            var httpClient = new HttpClient(httpMessageHandler);

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(x => x.Create()).Returns(httpClient);

            var target = new HttpCallMessageAsyncCollector(config, attr, httpClientFactory.Object);

            await target.AddAsync(new HttpCallMessage()
            {
                Body = "{ type: 'test' }"
            });
            await target.FlushAsync();


            httpClientFactory.VerifyAll();

            Assert.NotNull(message);
            Assert.Equal("http://www.example.com/webhook", message.RequestUri.ToString());
            Assert.Equal(HttpMethod.Post, message.Method);
            Assert.Equal("{ type: 'test' }", await message.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Put_Request_Calls_Correct_Url_WithBody()
        {
            HttpCallConfiguration config = new HttpCallConfiguration()
            {
                HttpMethod = "PUT",
                MediaType = "application/json",
                Url = "http://www.example.com/webhook",
            };

            HttpCallAttribute attr = new HttpCallAttribute()
            {
            };

            HttpRequestMessage message = null;
            var httpMessageHandler = new HttpMessageHandlerMock()
                .SetupHandler(req =>
                {
                    message = req;
                    return new HttpResponseMessage(HttpStatusCode.OK);
                });

            var httpClient = new HttpClient(httpMessageHandler);

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(x => x.Create()).Returns(httpClient);

            var target = new HttpCallMessageAsyncCollector(config, attr, httpClientFactory.Object);

            await target.AddAsync(new HttpCallMessage()
            {
                Body = "{ type: 'test' }"
            });
            await target.FlushAsync();


            httpClientFactory.VerifyAll();

            Assert.NotNull(message);
            Assert.Equal("http://www.example.com/webhook", message.RequestUri.ToString());
            Assert.Equal(HttpMethod.Put, message.Method);
            Assert.Equal("{ type: 'test' }", await message.Content.ReadAsStringAsync());
        }
    }
}
