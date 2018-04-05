using Fbeltrao.AzureFunctionExtensions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fbeltrao.AzureFunctionExtensions.Test
{
    /// <summary>
    /// Tests <see cref="EventGridOutputAsyncCollector"/> interactions with <see cref="IHttpClientFactory"/>
    /// </summary>
    public class EventGridOutputAsyncCollectorTest
    {
        [Fact]
        public async Task ThrowsError_If_SasKey_IsMissing()
        {
            var config = new EventGridOutputConfiguration()
            {
                EventType = "My.Test",
                Subject = "Test/1"
            };

            var attr = new EventGridOutputAttribute()
            {
            };
            
            var httpClientFactory = new Mock<IHttpClientFactory>();
           
            var target = new EventGridOutputAsyncCollector(config, attr, httpClientFactory.Object);

            await target.AddAsync(new EventGridOutput(new { test = true }));
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => target.FlushAsync());
            Assert.Equal("sasKey", exception.ParamName);


        }


        [Fact]        
        public async Task ThrowsError_If_Endpoint_IsMissing()
        {
            var config = new EventGridOutputConfiguration()
            {
                SasKey = "1234594949=",
                EventType = "My.Test",
                Subject = "Test/1"
            };

            var attr = new EventGridOutputAttribute()
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

            var target = new EventGridOutputAsyncCollector(config, attr, httpClientFactory.Object);

            await target.AddAsync(new EventGridOutput(new { test = true }));

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => target.FlushAsync());
            Assert.Equal("topicEndpoint", exception.ParamName);
            
        }


        [Fact]
        public async Task ThrowsError_If_EventType_IsMissing()
        {
            var config = new EventGridOutputConfiguration()
            {
                SasKey = "1234594949=",
                TopicEndpoint = "https://xxxx.westeurope-1.eventgrid.azure.net/api/events",                
                Subject = "Test/1"
            };

            var attr = new EventGridOutputAttribute()
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

            var target = new EventGridOutputAsyncCollector(config, attr, httpClientFactory.Object);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => target.AddAsync(new EventGridOutput(new { test = true })));
            Assert.Equal("eventType", exception.ParamName);
        }



        [Fact]
        public async Task ThrowsError_If_Subject_IsMissing()
        {
            var config = new EventGridOutputConfiguration()
            {
                SasKey = "1234594949=",
                TopicEndpoint = "https://xxxx.westeurope-1.eventgrid.azure.net/api/events",
                EventType = "My.Test",
            };

            var attr = new EventGridOutputAttribute()
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

            var target = new EventGridOutputAsyncCollector(config, attr, httpClientFactory.Object);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => target.AddAsync(new EventGridOutput(new { test = true })));
            Assert.Equal("subject", exception.ParamName);
        }



        [Fact]
        public async Task SendsMessage_If_All_IsCorrect()
        {
            var config = new EventGridOutputConfiguration()
            {
                SasKey = "1234594949=",
                TopicEndpoint = "https://xxxx.westeurope-1.eventgrid.azure.net/api/events",
                EventType = "My.Test",
                Subject = "Test/1"
            };

            var attr = new EventGridOutputAttribute()
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

            var target = new EventGridOutputAsyncCollector(config, attr, httpClientFactory.Object);

            await target.AddAsync(new EventGridOutput(new { test = true }));
            await target.FlushAsync();


            httpClientFactory.VerifyAll();

            Assert.True(message.Content.Headers.Contains("aeg-sas-key"), "aeg-sas-key header is missing");
            Assert.Equal(message.Content.Headers.GetValues("aeg-sas-key").First(), config.SasKey);
            Assert.NotNull(message);
            Assert.Equal("https://xxxx.westeurope-1.eventgrid.azure.net/api/events", message.RequestUri.ToString());
            Assert.Equal(HttpMethod.Post, message.Method);
            Assert.NotNull(message.Content);
            var requestBody = await ((StringContent)message.Content).ReadAsStringAsync();
            var requestJson = (JArray)JsonConvert.DeserializeObject(requestBody);            
            Assert.Single(requestJson);
            Assert.True(!string.IsNullOrEmpty(requestJson[0]["data"].ToString()), "Required EventGrid property data was not sent");
            Assert.True(!string.IsNullOrEmpty(requestJson[0]["id"].ToString()), "Required EventGrid property id was not sent");
            Assert.True(!string.IsNullOrEmpty(requestJson[0]["eventTime"].ToString()), "Required EventGrid property eventTime was not sent");
            Assert.Equal(config.EventType, requestJson[0]["eventType"].ToString());
            Assert.Equal(config.Subject, requestJson[0]["subject"].ToString());
            Assert.Equal("true", requestJson[0]["data"]["test"].ToString().ToLowerInvariant());
            
        }
    }
}
