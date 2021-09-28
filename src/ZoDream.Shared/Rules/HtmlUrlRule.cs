using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Shared.Rules
{
    public class HtmlUrlRule : IRule
    {

        public PluginInfo Info()
        {
            return new PluginInfo("提取默认链接");
        }

        public void Ready(RuleItem option)
        {
            
        }
        public async Task RenderAsync(ISpiderContainer container)
        {
            container.Data = container.Data.Select(i => RenderOne(container, i));
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
                var url = item.Groups[4].Value;
                if (string.IsNullOrEmpty(url)
                    || url.IndexOf("javascript:", StringComparison.Ordinal) >= 0
                    || url.IndexOf("#", StringComparison.Ordinal) == 0
                    || url.IndexOf("data:", StringComparison.OrdinalIgnoreCase) >= 0
                    || url.IndexOf("ed2k://", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    continue;
                }
                var uriType = UriType.File;
                switch (item.Groups[2].Value.ToLower())
                {
                    case "iframe":
                    case "a":
                        uriType = UriType.Html;
                        break;
                    case "link":
                        uriType = UriType.Css;
                        break;
                    case "img":
                        uriType = UriType.Image;
                        break;
                    case "script":
                        uriType = UriType.Js;
                        break;
                    default:
                        uriType = UriType.File;
                        break;
                }
                var uri = container.AddUri(url, uriType);
                html = html.Replace(item.Value, item.Value.Replace(item.Groups[4].Value, uri));  // 需要相对路径
            }
        }

        public void GetUrlFromCss(ISpiderContainer container, ref string html)
        {
            var items = new List<string>();
            var matches = Regex.Matches(html, @"url\([""']?([^""'\s\<\>]*)[""']?\)", RegexOptions.IgnoreCase);
            foreach (Match item in matches)
            {
                if (item.Groups[1].Value.IndexOf("base64,") >= 0)
                {
                    continue;
                }
                var uri = container.AddUri(item.Groups[1].Value, UriType.File);
                html = html.Replace(item.Value,
                    item.Value.Replace(item.Groups[1].Value, uri));
            }
        }
    }
}
