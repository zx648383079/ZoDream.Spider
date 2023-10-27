using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
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
            var matches = Regex.Matches(html, @"url\([""']?([^""'\s\<\>]*)[""']?\)", RegexOptions.IgnoreCase);
            foreach (Match item in matches)
            {
                if (string.IsNullOrEmpty(item.Groups[1].Value) || 
                    item.Groups[1].Value.IndexOf("base64,") >= 0)
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
