using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.WebApi.Client.HMAC
{
    public class HMACHttpClient
    {
        private readonly string _applicationId;
        private readonly string _apiKey;
        private readonly HMACDelegatingHandler _hmacDelegateHandler;

        public HMACHttpClient(string applicationId, string apiKey)
        {
            _applicationId = applicationId;
            _apiKey = apiKey;
            _hmacDelegateHandler = new HMACDelegatingHandler(_applicationId, _apiKey);
        }

        public async Task<TResponse> PostJson<TResponse, TData>(string requestUri, TData data) 
                where TData : class 
                where TResponse : class, new()
        {
            var client = HttpClientFactory.Create(_hmacDelegateHandler);

            HttpResponseMessage response = await client.PostAsJsonAsync(requestUri, data).ConfigureAwait(continueOnCapturedContext: false);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext: false);
                return JsonConvert.DeserializeObject<TResponse>(responseString);
            }
            else
            {
                throw new Exception(string.Format("Failed to call the API. HTTP Status: {0}, Reason {1}", response.StatusCode, response.ReasonPhrase));
            }
        }
    }
}
