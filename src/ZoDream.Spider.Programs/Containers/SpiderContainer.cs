﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;
using ZoDream.Shared.Utils;

namespace ZoDream.Spider.Programs
{
    public class SpiderContainer : ISpiderContainer
    {
        public SpiderContainer(ISpider spider, 
            UriItem url, 
            IList<IRule> rules, CancellationToken token = default)
        {
            Token = token;
            Application = spider;
            Logger = spider.Logger;
            Url = url;
            Rules = rules;
        }
        public ISpider Application { get; set; }

        public bool IsDebug => Application.IsDebug;

        public CancellationToken Token { get; }

        public ILogger? Logger { get; private set; }

        public IDictionary<string, string> MapItems = new Dictionary<string, string>();
        public IList<IRule> Rules { get; set; } = [];

        public string? OriginData { get; set; }
        public IRuleValue? Data { get; set; }
        public UriItem Url { get; set; }

        public int RuleIndex { get; private set; } = -1;

        public IEnumerable<string> AttributeKeys => MapItems.Keys;

        private string? saveFileName;

        public string SaveFileName {
            get {
                saveFileName ??= Application.RuleProvider.GetFileName(Url.Source);
                return saveFileName;
            }
        }


        public string AddUri(string uri, UriType uriType)
        {
            var fromUri = new Uri(Url.Source);
            var toUri = new Uri(fromUri, uri);
            var fullUri = toUri.ToString();
            var relativeUri = fromUri.MakeRelativeUri(toUri);
            if (!Application.RuleProvider.Cannable(fullUri, uriType))
            {
                return Uri.UnescapeDataString(relativeUri.ToString());
            }
            Application.UrlProvider.Add(Url.Level + 1, fullUri, uriType);
            var saveFileName = Application.RuleProvider.GetFileName(fullUri);
            if (!string.IsNullOrEmpty(saveFileName))
            {
                return Application.Storage.GetRelativePath(SaveFileName, saveFileName);
            }
            return Uri.UnescapeDataString(relativeUri.ToString());
        }

        public async Task NextAsync()
        {
            RuleIndex++;
            if (RuleIndex >= Rules.Count || Application.Paused || Token.IsCancellationRequested)
            {
                return;
            }
            var rule = Rules[RuleIndex];
            await rule.RenderAsync(this);
            Application.UrlProvider.EmitProgress(Url, RuleIndex + 1, Rules.Count, false);
        }

        public void EmitProgress(long step, long count)
        {
            Application.UrlProvider.EmitProgress(Url, step, count, true);
        }

        public async Task<string?> GetAsync(string url)
        {
            if (OriginData is not null)
            {
                return OriginData;
            }
            return await Application.RequestProvider.Getter().GetAsync(
               Application.GetRequestData(url));
        }
        public async Task GetAsync(string fileName, string url)
        {
            Disk.CreateDirectory(fileName);
            await Application.RequestProvider.Downloader().GetAsync(
            fileName,
                Application.GetRequestData(url), EmitProgress, Token);
        }

        public async Task SaveAsync(string fileName, string content)
        {
            Disk.CreateDirectory(fileName);
            await LocationStorage.WriteAsync(fileName, content);
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
                    return Data?.ToString() ?? string.Empty;
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
