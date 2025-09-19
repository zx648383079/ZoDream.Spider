using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Spider.Rules
{
    public class HtmlUrlRule : IRule
    {

        public PluginInfo Info()
        {
            return new PluginInfo("提取默认链接");
        }

        public IFormInput[]? Form()
        {
            return null;
        }

        public void Ready(RuleItem option)
        {
            
        }
        public async Task RenderAsync(ISpiderContainer container)
        {
            container.Data = container.Data!.Select(i => RenderOne(container, i));
            await container.NextAsync();
        }

        private IRuleValue RenderOne(ISpiderContainer container, IRuleValue value)
        {
            var content = value.ToString();
            if (container.Url.Type == UriType.Css)
            {
                GetUrlFromCss(container, ref content);
            } else
            {
                GetUrlFromHtml(container, ref content);
            }
            return new RuleString(content);
        }

        public void GetUrlFromHtml(ISpiderContainer container, ref string html)
        {
            var matches = Regex.Matches(html, @"(\<(a|img|link|script|embed|audio|object|video|param|source|iframe)[^\<\>]+(src|href|value|data)\s?=)\s?[""']?([^""'\s\<\>]*)[""']?", RegexOptions.IgnoreCase);
            foreach (Match item in matches)
            {
                var originalUrl = item.Groups[4].Value;
                if (string.IsNullOrEmpty(originalUrl)
                    || originalUrl.IndexOf("javascript:", StringComparison.Ordinal) >= 0
                    || originalUrl.IndexOf("#", StringComparison.Ordinal) == 0
                    || originalUrl.IndexOf("data:", StringComparison.OrdinalIgnoreCase) >= 0
                    || originalUrl.IndexOf("ed2k://", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    continue;
                }
                var uriType = UriType.File;
                uriType = item.Groups[2].Value.ToLower() switch
                {
                    "iframe" or "a" => UriType.Html,
                    "link" => UriType.Css,
                    "img" => UriType.Image,
                    "script" => UriType.Js,
                    _ => UriType.File,
                };
                var uri = container.AddUri(originalUrl, uriType);
                if (originalUrl == uri)
                {
                    continue;
                }
                html = html.Replace(item.Value, item.Value.Replace(originalUrl, uri));  // 需要相对路径
            }
        }

        public void GetUrlFromCss(ISpiderContainer container, ref string html)
        {
            var matches = Regex.Matches(html, @"url\([""']?([^""'\s\<\>]*?)[""']?\)", RegexOptions.IgnoreCase);
            foreach (Match item in matches)
            {
                var url = item.Groups[1].Value;

                if (string.IsNullOrEmpty(url) ||
                    url.IndexOf("base64,") >= 0)
                {
                    continue;
                }
                var next = url.IndexOf('#');
                if (next >= 0)
                {
                    url = url.Substring(0, next);
                }
                next = url.LastIndexOf(": 0x");
                if (next >= 0)
                {
                    url = url.Substring(0, next);
                }
                if (url.EndsWith("?"))
                {
                    url = url.Substring(0, url.Length - 1);
                }
                var uri = container.AddUri(url, UriType.File);
                html = html.Replace(item.Value,
                    item.Value.Replace(url, uri));
            }
        }
    }
}
