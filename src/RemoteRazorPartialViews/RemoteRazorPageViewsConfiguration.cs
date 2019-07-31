using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace RemoteRazorPartialViews
{
    public static class RemoteRazorPageViewsConfiguration
    {
        public static int NumberOfRetries = 2;
        public static int WaitTimeInMilliseconds = 200;
        public static readonly AsyncRetryPolicy<HttpResponseMessage> DefaultRetryPolicy =
            Policy
              .Handle<HttpRequestException>()
              .OrResult<HttpResponseMessage>(r => new[]
                {
                    HttpStatusCode.RequestTimeout, // 408
                    HttpStatusCode.InternalServerError, // 500
                    HttpStatusCode.BadGateway, // 502
                    HttpStatusCode.ServiceUnavailable, // 503
                    HttpStatusCode.GatewayTimeout // 504
                }.Contains(r.StatusCode))
              .WaitAndRetryAsync(
                        retryCount: NumberOfRetries, // Retry 2 times
                        sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(WaitTimeInMilliseconds));
    }
}
