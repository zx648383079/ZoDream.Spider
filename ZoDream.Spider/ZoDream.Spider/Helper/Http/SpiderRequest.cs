
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
            if (contentType.IndexOf("text/html", StringComparison.Ordinal) >= 0 && Url.Kind == AssetKind.Html)
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
            FileHelper.CreateDirectory(Url.FullName);
            request.Download(response, Url.FullName);
            if (Url.Kind == AssetKind.Css)
            {
                var html = Open.Read(Url.FullName);
                GetUrlFromCss(ref html);
                Open.Writer(Url.FullName, html);
            }
        }

        public void GetUrlFromCss(ref string html)
        {
            var matches = Regex.Matches(html, @"url\([""']?([^""'\s\<\>]*)[""']?\)", RegexOptions.IgnoreCase);
            foreach (Match item in matches)
            {
                var uri = new UrlTask(GetAbsoluteUrl(item.Groups[1].Value, Url.Url))
                {
                    Kind = AssetKind.File
                };
                html = html.Replace(item.Value, 
                    item.Value.Replace(item.Groups[1].Value, 
                    GetRelativeUrl(Url.FullName, uri.FullName)));
                Results.Add(uri);
            }
        }

        public void GetUrlFromJs(ref string html)
        {
            
        }

        public void GetUrlFromHtml(ref string html)
        {
            var matches = Regex.Matches(html, @"(\<(a|img|link|script|embed|audio|object|video|param|source|iframe)[^\<\>]+(src|href|value|data)\s?=)\s?[""']?([^""'\s\<\>]*)[""']?", RegexOptions.IgnoreCase);
            foreach (Match item in matches)
            {
                var url = item.Groups[4].Value;
                if (string.IsNullOrEmpty(url) 
                    || url.IndexOf("javascript:", StringComparison.Ordinal) >= 0 
                    || url.IndexOf("#", StringComparison.Ordinal) == 0
                    || url.IndexOf("data:", StringComparison.OrdinalIgnoreCase) >= 0
                    || url.IndexOf("ed2k://", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    continue;
                }
                var uri = new UrlTask(GetAbsoluteUrl(url, Url.Url));
                html = html.Replace(item.Value, item.Value.Replace(item.Groups[4].Value, GetRelativeUrl(Url.FullName, uri.FullName)));  // 需要相对路径
                switch (item.Groups[2].Value.ToLower())
                {
                    case "iframe":
                    case "a":
                        uri.Kind = AssetKind.Html;
                        break;
                    case "link":
                        uri.Kind = AssetKind.Css;
                        break;
                    case "img":
                        uri.Kind = AssetKind.Image;
                        break;
                    case "script":
                        uri.Kind = AssetKind.Js;
                        break;
                    default:
                        uri.Kind = AssetKind.File;
                        break;
                }
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
            var uri = new Uri(Url.Url);
            foreach (var item in Rules)
            {
                var task = new HtmlTask(new HtmlObject(html), item.Rules) {
                    FullFile = Url.FullName, 
                    Url = uri,
                    Spider = this
                };
                task.Run();
            }
            
        }

    }
}
