
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using ZoDream.Helper.Http;
using ZoDream.Helper.Local;
using ZoDream.Spider.Model;

namespace ZoDream.Spider.Helper.Http
{
    public class SpiderRequest
    {
        public UrlTask Url { get; set; }

        public List<UrlItem> Rules { get; set; }

        public IList<HeaderItem> Headers { get; set; }

        public List<UrlTask> Results { get; set; } = new List<UrlTask>();

        public int TimeOut { get; set; }

        public void Start()
        {
            var request = new Request(Url.Url)
            {
                TimeOut = TimeOut
            };
            foreach (var item in Headers)
            {
                request.HeaderCollection.Add(item.Name, item.Value);
            }
            var response = request.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return;
            }
            var contentType = response.Headers[HttpResponseHeader.ContentType];
            if (contentType.IndexOf("text/html", StringComparison.Ordinal) >= 0)
            {
                var html = request.GetHtml(response);
                DealHtml(html);
                return;
            }
            FileHelper.CreateDirectory(Url.FullName);
            if (contentType.IndexOf("text/css", StringComparison.Ordinal) >= 0)
            {
                
            }

            if (contentType.IndexOf("application/javascript", StringComparison.Ordinal) >= 0)
            {

            }
            // 文件下载，断点续传
            request.Download(response, Url.FullName);
        }

        public void GetUrlFromCss(ref string html)
        {

        }

        public void GetUrlFromJs(ref string html)
        {

        }

        public void GetUrlFromHtml(ref string html)
        {
            var matches = Regex.Matches(html, @"(\<(a|img|link|script|embed|audio|object|video|param|source)[^\<\>]+(src|href|value|data)\s?=)\s?""?([^""\s\<\>]*)""?");
            foreach (Match item in matches)
            {
                var url = item.Groups[4].Value;
                if (string.IsNullOrEmpty(url) 
                    || url.IndexOf("javascript:", StringComparison.Ordinal) >= 0 
                    || url.IndexOf("#", StringComparison.Ordinal) == 0 
                    || url.IndexOf("data:", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    continue;
                }
                var uri = new UrlTask(GetAbsoluteUrl(url, Url.Url));
                html = html.Replace(item.Value, item.Value.Replace(item.Groups[4].Value, GetRelativeUrl(Url.FullName, uri.FullName)));  // 需要相对路径
                Results.Add(uri);
            }
        }

        public string GetAbsoluteUrl(string reltiveUrl, string baseUrl)
        {
            return new Uri(new Uri(baseUrl), reltiveUrl).ToString();
        }

        public string GetRelativeUrl(string fromUrl, string toUrl)
        {
            if (string.IsNullOrEmpty(fromUrl))
            {
                return toUrl;
            };
            if (string.IsNullOrEmpty(toUrl))
            {
                return "";
            };

            var fromUri = new Uri(fromUrl);
            var toUri = new Uri(toUrl);

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            return Uri.UnescapeDataString(relativeUri.ToString());
        }

        public void DealHtml(string html)
        {
            GetUrlFromHtml(ref html);
            foreach (var item in Rules)
            {
                var task = new HtmlTask(new Html(html), item.Rults) {FullFile = Url.FullName};
                task.Run();
            }
            
        }

    }
}
