﻿using System.Text;
using System.Text.RegularExpressions;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Shared.Spiders.Containers
{
    public class SpiderContainer : ISpiderContainer
    {
        public SpiderContainer(ISpider spider)
        {
            Application = spider;
        }
        public ISpider Application { get; set; }

        public IDictionary<string, string> MapItems = new Dictionary<string, string>();
        public IList<IRule> Rules { get; set; } = new List<IRule>();
        public IRuleValue Data { get; set; }
        public UriItem Url { get; set; }

        private int ruleIndex = -1;

        public string AddUri(string uri, UriType uriType)
        {
            var fromUri = new Uri(Url.Source);
            var toUri = new Uri(fromUri, uri);
            var fullUri = toUri.ToString();
            Application.UrlProvider.Add(fullUri, uriType);
            var saveFileName = Application.RuleProvider.GetFileName(uri);
            if (!string.IsNullOrEmpty(saveFileName))
            {
                return Path.GetRelativePath(Application.Option.WorkFolder, saveFileName);
            }
            var relativeUri = fromUri.MakeRelativeUri(toUri);
            return Uri.UnescapeDataString(relativeUri.ToString());
        }

        public void Next()
        {
            ruleIndex++;
            if (ruleIndex >= Rules.Count)
            {
                return;
            }
            var rule = Rules[ruleIndex];
            rule.Render(this);
        }

        public void SetAttribute(string name, string value)
        {
            MapItems.Add(name, value);
        }

        public void UnsetAttribute(string name)
        {
            MapItems.Remove(name);
        }

        public string AddUri(string uri)
        {
            return AddUri(uri, UriType.Html);
        }

        public string RenderData(string content)
        {
            content = RenderTemplate(content);
            var matches = Regex.Matches(content, @"\${([a-zA-Z0-9_])}");
            if (matches.Count == 0)
            {
                return content;
            }
            var sb = new StringBuilder();
            var lastIndex = 0;
            foreach (Match item in matches)
            {
                sb.Append(content.Substring(lastIndex, item.Index - lastIndex));
                sb.Append(GetValue(item.Groups[0].Value));
                lastIndex = item.Index + 1;
            }
            sb.Append(content.AsSpan(lastIndex));
            return sb.ToString();
        }

        private string GetValue(string key)
        {
            switch (key)
            {
                case "url":
                    return Url.Source;
                case "title":
                    return Url.Title;
                case "content":
                    return Data.ToString();
                default:
                    break;
            }
            if (MapItems.ContainsKey(key))
            {
                return MapItems[key];
            }
            return string.Empty;
        }

        private string RenderTemplate(string content)
        {
            if (File.Exists(content))
            {
                return File.ReadAllText(content);
            }
            return content;
        }
    }
}
