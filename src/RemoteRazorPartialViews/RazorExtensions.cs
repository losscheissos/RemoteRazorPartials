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
        /// <param name="remotePartialUri">The target URI as string</param>
        /// <param name="retryPolicy">A Polly Policy to be used when gathering content</param>
        /// <returns>Raw HtmlContent representation of the given web resource</returns>
        public static Task<IHtmlContent> RenderRemotePartialAsync(this IHtmlHelper htmlHelper, string remotePartialUri, AsyncRetryPolicy<HttpResponseMessage> retryPolicy)
        {
            return RenderRemotePartialAsync(htmlHelper, new Uri(remotePartialUri), retryPolicy);
        }

        /// <summary>
        /// This method retrieves the HTML from the given URI and returns the HTML content, which can be placed
        /// at the desired position using razor default handling
        /// </summary>
        /// <param name="htmlHelper">The HTML Helper instance</param>
        /// <param name="remotePartialUri">The target URI</param>
        /// <returns>Raw HtmlContent representation of the given web resource</returns>
        public static Task<IHtmlContent> RenderRemotePartialAsync(this IHtmlHelper htmlHelper, Uri remotePartialUri)
        {
            return RenderRemotePartialAsync(htmlHelper, remotePartialUri, 
                RemoteRazorPartialViews.RemoteRazorPageViewsConfiguration.DefaultRetryPolicy);
        }

        /// <summary>
        /// This method retrieves the HTML from the given URI and returns the HTML content, which can be placed
        /// at the desired position using razor default handling
        /// </summary>
        /// <param name="htmlHelper">The HTML Helper instance</param>
        /// <param name="remotePartialUri">The target URI</param>
        /// <param name="retryPolicy">A Polly Policy to be used when gathering content</param>
        /// <returns>Raw HtmlContent representation of the given web resource</returns>
        public static async Task<IHtmlContent> RenderRemotePartialAsync(this IHtmlHelper htmlHelper, Uri remoteUri, AsyncRetryPolicy<HttpResponseMessage> retryPolicy)
        {
            HttpResponseMessage result = await retryPolicy
                .ExecuteAsync(() => httpClient.GetAsync(remoteUri.AbsoluteUri));

            result.EnsureSuccessStatusCode();

            var content = await result.Content.ReadAsStringAsync();

            return htmlHelper.Raw(content);
        }
    }
}
