using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;
using ZoDream.Shared.Storage;
using ZoDream.Shared.Utils;

namespace ZoDream.Spider.Programs
{
    public class SpiderContainer : ISpiderContainer
    {
        public SpiderContainer(ISpider spider, UriItem url, IList<IRule> rules)
        {
            Application = spider;
            Logger = spider.Logger;
            Url = url;
            Rules = rules;
        }
        public ISpider Application { get; set; }

        public ILogger? Logger { get; private set; }

        public IDictionary<string, string> MapItems = new Dictionary<string, string>();
        public IList<IRule> Rules { get; set; } = new List<IRule>();
        public IRuleValue? Data { get; set; }
        public UriItem Url { get; set; }

        public int RuleIndex { get; private set; } = -1;

        public IEnumerable<string> AttributeKeys => MapItems.Keys;

        public string AddUri(string uri, UriType uriType)
        {
            var fromUri = new Uri(Url.Source);
            var toUri = new Uri(fromUri, uri);
            var fullUri = toUri.ToString();
            var relativeUri = fromUri.MakeRelativeUri(toUri);
            if (Application.RuleProvider.Cannable(fullUri))
            {
                return Uri.UnescapeDataString(relativeUri.ToString());
            }
            Application.UrlProvider.Add(fullUri, uriType);
            var saveFileName = Application.RuleProvider.GetFileName(uri);
            if (!string.IsNullOrEmpty(saveFileName))
            {
                return Application.Storage.GetRelativePath(saveFileName);
            }
            return Uri.UnescapeDataString(relativeUri.ToString());
        }

        public async Task NextAsync()
        {
            RuleIndex++;
            if (RuleIndex >= Rules.Count || Application.Paused)
            {
                return;
            }
            var rule = Rules[RuleIndex];
            await rule.RenderAsync(this);
            Application.UrlProvider.EmitProgress(Url, RuleIndex + 1, Rules.Count, false);
            return;
        }

        public void EmitProgress(int step, int count)
        {
            Application.UrlProvider.EmitProgress(Url, step, count, true);
        }

        public async Task<string?> GetAsync(string url)
        {
            return await Application.RequestProvider.Getter().GetAsync(
               Application.GetRequestData(url));
        }
        public async Task GetAsync(string fileName, string url)
        {
            await Application.RequestProvider.Downloader().GetAsync(
            fileName,
                Application.GetRequestData(url));
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
            if (string.IsNullOrWhiteSpace(content))
            {
                return Data == null ? string.Empty : Data.ToString();
            }
            return Regex.Replace(RenderTemplate(content), @"\$\{([a-zA-Z0-9_]+)\}", match => {
                return GetAttribute(match.Groups[1].Value);
            });
        }

        public string GetAttribute(string key)
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
                return LocationStorage.ReadAsync(content).GetAwaiter().GetResult();
            }
            return content;
        }
    }
}
