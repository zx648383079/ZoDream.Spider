using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;
using ZoDream.Shared.Utils;

namespace ZoDream.Spider.Rules
{
    public class MergeFileRule : IRule, IRuleSaver
    {
        public bool ShouldPrepare => false;

        public bool CanNext => false;

        private string RuleGroupName = string.Empty;

        private string FileNamePattern = string.Empty;

        public string GetFileName(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return FileNamePattern;
            }
            return RenderFileName(FileNamePattern, url); ;
        }

        public PluginInfo Info()
        {
            return new PluginInfo("合并文件");
        }

        public IFormInput[]? Form()
        {
            return new IFormInput[] {
                Input.Text(nameof(RuleGroupName), "规则组名称"),
                Input.Text(nameof(FileNamePattern), "文件匹配"),
            };
        }

        public void Ready(RuleItem option)
        {
            RuleGroupName = option.Get<string>(nameof(RuleGroupName)) ?? string.Empty;
            FileNamePattern = option.Get<string>(nameof(FileNamePattern)) ?? string.Empty;
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var storage = container.Application.Storage;
            var source = container.Application.UrlProvider;
            try
            {
                Dictionary<string, List<string>> files;
                if (RuleGroupName == "*")
                {
                    files = FindAll(storage, source);
                }
                if (Regex.IsMatch(RuleGroupName, @"^\w+\.\w+(\.\w+)?$"))
                {
                    files = FindHost(storage, source, RuleGroupName);
                } else
                {
                    files = FindRegex(storage, source, new Regex(RuleGroupName));
                }
                foreach (var item in files)
                {
                    container.Application.Logger?.Info($"Merge file: {item.Key}");
                    await SaveFileAsync(storage, item.Key, item.Value);
                }
            }
            catch (Exception ex)
            {
                container.Application.Logger?.Error($"Merge failure: {ex.Message}");
            }
        }

        private Dictionary<string, List<string>> FindRegex(IStorageProvider<string, string, System.IO.FileStream> storage, IEnumerable<UriItem> source, Regex regex)
        {
            var matches = Regex.Matches(FileNamePattern, @"\$\{([a-zA-Z0-9_]+)\}");
            var data = new Dictionary<string, List<string>>();
            foreach (var item in source)
            {
                var match = regex.Match(item.Source);
                if (match == null || !match.Success)
                {
                    continue;
                }
                var sourceFile = storage.GetFileName(item);
                if (string.IsNullOrEmpty(sourceFile))
                {
                    continue;
                }
                var saveFile = RenderFileName(FileNamePattern, matches, item.Source, match);
                if (!data.ContainsKey(saveFile))
                {
                    data.Add(saveFile, new List<string>());
                }
                data[saveFile].Add(sourceFile);
            }
            return data;
        }

        private Dictionary<string, List<string>> FindHost(IStorageProvider<string, string, System.IO.FileStream> storage, IEnumerable<UriItem> source, string host)
        {
            var matches = Regex.Matches(FileNamePattern, @"\$\{([a-zA-Z0-9_]+)\}");
            var data = new Dictionary<string, List<string>>();
            foreach (var item in source)
            {
                var uri = new Uri(item.Source);
                if (uri.Host != host)
                {
                    continue;
                }
                var sourceFile = storage.GetFileName(item);
                if (string.IsNullOrEmpty(sourceFile))
                {
                    continue;
                }
                var saveFile = RenderFileName(FileNamePattern, matches, uri);
                if (!data.ContainsKey(saveFile))
                {
                    data.Add(saveFile, new List<string>());
                }
                data[saveFile].Add(sourceFile);
            }
            return data;
        }

        private Dictionary<string, List<string>> FindAll(IStorageProvider<string, string, System.IO.FileStream> storage, IEnumerable<UriItem> source)
        {
            var matches = Regex.Matches(FileNamePattern, @"\$\{([a-zA-Z0-9_]+)\}");
            var data = new Dictionary<string, List<string>>();
            foreach (var item in source)
            {
                var sourceFile = storage.GetFileName(item);
                if (string.IsNullOrEmpty(sourceFile))
                {
                    continue;
                }
                var saveFile = RenderFileName(FileNamePattern, matches, item.Source);
                if (!data.ContainsKey(saveFile))
                {
                    data.Add(saveFile, new List<string>());
                }
                data[saveFile].Add(sourceFile);
            }
            return data;
        }

        private async Task SaveFileAsync(IStorageProvider<string, string, System.IO.FileStream> storage, string fileName, IList<string> files)
        {
            var writer = LocationStorage.Writer(await storage.CreateStreamAsync(fileName), true);
            foreach (var item in files)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                var tempfile = await storage.OpenStreamAsync(item);
                if (tempfile != null)
                {
                    writer.WriteLine(await LocationStorage.ReadAsync(tempfile));
                    writer.WriteLine();
                }
                tempfile?.Close();
            }
            writer.Close();
        }
        
        private string RenderFileName(string pattern, string uri)
        {
            return RenderFileName(pattern, Regex.Matches(pattern, @"\$\{([a-zA-Z0-9_]+)\}"), uri);
        }

        private string RenderFileName(string pattern, MatchCollection? matches, string url)
        {
            if (matches == null)
            {
                return pattern;
            }
            return RenderFileName(pattern, matches, new Uri(url));
        }

        private string RenderFileName(string pattern, MatchCollection? matches, Uri uri)
        {
            if (matches == null)
            {
                return pattern;
            }
            var sb = new StringBuilder();
            var start = 0;
            foreach (Match item in matches)
            {
                var len = item.Index - start;
                if (len > 0)
                {
                    sb.Append(pattern.Substring(start, len));
                }
                sb.Append(item.Groups[1].Value == "host" ? uri.Host : item.Groups[1].Value);
                start = item.Index + item.Value.Length;
            }
            if (start < pattern.Length - 1)
            {
                sb.Append(pattern.Substring(start, pattern.Length - 1 - start));
            }
            return sb.ToString();
        }

        private string RenderFileName(string pattern, MatchCollection? matches, string url, Match param)
        {
            if (matches == null)
            {
                return pattern;
            }
            var uri = new Uri(url);
            var sb = new StringBuilder();
            var start = 0;
            foreach (Match item in matches)
            {
                var len = item.Index - start;
                if (len > 0)
                {
                    sb.Append(pattern.Substring(start, len));
                }
                var tag = item.Groups[1].Value;
                int tagInt;
                if (tag == "host")
                {
                    sb.Append(uri.Host);
                } else if (int.TryParse(tag, out tagInt))
                {
                    sb.Append(param.Groups[tagInt].Value);
                } else
                {
                    sb.Append(tag);
                }
                start = item.Index + item.Value.Length;
            }
            if (start < pattern.Length - 1)
            {
                sb.Append(pattern.Substring(start, pattern.Length - 1 - start));
            }
            return sb.ToString();
        }
    }
}
