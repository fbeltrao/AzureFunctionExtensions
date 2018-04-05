using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Fbeltrao.AzureFunctionExtensions
{
    public interface IHttpClientFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="HttpClient"/>
        /// </summary>
        /// <returns></returns>
        HttpClient Create();
    }
}
