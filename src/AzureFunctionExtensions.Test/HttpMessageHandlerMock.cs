using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fbeltrao.AzureFunctionExtensions.Test
{
    class HttpMessageHandlerMock : HttpMessageHandler
    {
        private Func<HttpRequestMessage, HttpResponseMessage> handler;

        public HttpMessageHandlerMock SetupHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            this.handler = handler;
            return this;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = handler(request);
            return Task.FromResult(result);
        }
    }
}
