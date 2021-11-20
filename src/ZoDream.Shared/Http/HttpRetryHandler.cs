using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZoDream.Shared.Http
{
    public class HttpRetryHandler : DelegatingHandler
    {
        private readonly int MaxRetries;
        private readonly int WaitTime;

        public HttpRetryHandler(HttpMessageHandler innerHandler, int maxRetries = 3, int waitSeconds = 0)
        : base(innerHandler)
        {
            MaxRetries = maxRetries;
            WaitTime = waitSeconds * 1000;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage? response = null;
            for (int i = 0; i < MaxRetries; i++)
            {
                response = await base.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    // Don't reattempt a bad request
                    break;
                }

                if (WaitTime > 0)
                {
                    await Task.Delay(WaitTime);
                }
            }

            return response;
        }
    }
}
