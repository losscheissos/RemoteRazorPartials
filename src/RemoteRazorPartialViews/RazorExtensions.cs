using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class RemotePartialExtensions
    {
        static HttpClient httpClient = new HttpClient();

        public static Task<IHtmlContent> RenderRemotePartialAsync(this IHtmlHelper htmlHelper, string remotePartialUri)
        {
            return RenderRemotePartialAsync(htmlHelper, new Uri(remotePartialUri));
        }
        public static async Task<IHtmlContent> RenderRemotePartialAsync(this IHtmlHelper htmlHelper, Uri remotePartialUri)
        {
            var result = await httpClient.GetAsync(remotePartialUri.AbsoluteUri);

            result.EnsureSuccessStatusCode();

            var content = await result.Content.ReadAsStringAsync();

            return htmlHelper.Raw(content);
        }
    }
}
