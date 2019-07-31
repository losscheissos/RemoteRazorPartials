using Microsoft.AspNetCore.Html;
using Polly;
using Polly.Retry;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class RemotePartialExtensions
    {
        static HttpClient httpClient = new HttpClient();
        public static int NumberOfRetries = 2;
        public static int WaitTimeInMilliseconds = 200;
        public static AsyncRetryPolicy<HttpResponseMessage> RetryPolicy =
            Policy
              .Handle<HttpRequestException>()
              .OrResult<HttpResponseMessage>(r => new [] 
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

        /// <summary>
        /// This method retrieves the HTML from the given URI and returns the HTML content, which can be placed
        /// at the desired position using razor default handling
        /// </summary>
        /// <param name="htmlHelper">The HTML Helper instance</param>
        /// <param name="remotePartialUri">The target URI as string</param>
        /// <returns>Raw HtmlContent representation of the given web resource</returns>
        public static Task<IHtmlContent> RenderRemotePartialAsync(this IHtmlHelper htmlHelper, string remotePartialUri)
        {
            return RenderRemotePartialAsync(htmlHelper, new Uri(remotePartialUri));
        }

        /// <summary>
        /// This method retrieves the HTML from the given URI and returns the HTML content, which can be placed
        /// at the desired position using razor default handling
        /// </summary>
        /// <param name="htmlHelper">The HTML Helper instance</param>
        /// <param name="remotePartialUri">The target URI</param>
        /// <returns>Raw HtmlContent representation of the given web resource</returns>
        public static async Task<IHtmlContent> RenderRemotePartialAsync(this IHtmlHelper htmlHelper, Uri remotePartialUri)
        {
            HttpResponseMessage result = await RetryPolicy
              .ExecuteAsync(() => httpClient.GetAsync(remotePartialUri.AbsoluteUri));

            result.EnsureSuccessStatusCode();

            var content = await result.Content.ReadAsStringAsync();

            return htmlHelper.Raw(content);
        }
    }
}
