﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Http;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Storage;
using ZoDream.Shared.Utils;
using ZoDream.Spider.BookCrawlerRule.Models;

namespace ZoDream.Spider.BookCrawlerRule
{
    public class Crawler
    {
        public Crawler()
        {

        }

        public Crawler(ISpiderContainer container): this(container.Application)
        {
            TaskContainer = container;
        }
        public Crawler(ISpider container)
        {
            Container = container;
        }

        private readonly ISpiderContainer? TaskContainer;
        private readonly ISpider? Container;

        private ILogger? Logger => Container?.Logger;

        public int MaxRetries { get; set; } = 3;

        public bool Paused => Container != null && Container.Paused;

        public async Task RenderAsync(string url, string fileName)
        {
            await RenderAsync(new Uri(url), await GetAsync(url), fileName);
        }

        public async Task RenderAsync(Uri url, string? html, string fileName)
        {
            if (html == null || string.IsNullOrWhiteSpace(html))
            {
                return;
            }
            using var writer = LocationStorage.Writer(fileName);
            await RenderAsync(url, html, writer);
        }

        public async Task<string?> RenderAsync(string url)
        {
            return await RenderAsync(new Uri(url), await GetAsync(url));
        }

        public async Task<string?> RenderAsync(Uri url, string? html)
        {
            if (html == null || string.IsNullOrWhiteSpace(html))
            {
                return null;
            }
            using var writer = new StringWriter();
            await RenderAsync(url, html, writer);
            return writer.ToString();

        }

        public async Task RenderAsync(Uri url, string html, TextWriter writer)
        {
            var book = RenderTitle(html);
            Log($"开始下载 《{book}》");
            await writer.WriteLineAsync($"《{book}》");
            await writer.WriteLineAsync();
            await RenderCatalogAsync(html, url, writer);
        }

        public async Task<string?> GetAsync(string url)
        {
            if (Paused)
            {
                return null;
            }
            var waitTime = 2000;
            if (TaskContainer == null)
            {
                var client = new Client()
                {
                    MaxRetries = MaxRetries,
                    RetryTime = waitTime,
                };
                return await client.GetAsync(url);
            }
            else
            {
                return await TaskContainer.GetAsync(url);
            }
        }

        private void Log(string message)
        {
            Logger?.Debug(message);
        }

        private string RenderTitle(string html)
        {
            var match = Regex.Match(html, @"\<title\>(.+?)\</title\>", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return string.Empty;
            }
            return match.Groups[1].Value.Replace('_', ' ').Replace('-', ' ');
        }

        /// <summary>
        /// 解析页面内容，提取主要内容
        /// </summary>
        /// <param name="html"></param>
        /// <param name="baseUri"></param>
        /// <param name="IsCatalog">是否可能是章节页</param>
        /// <returns></returns>
        private async Task<int> RenderCatalogAsync(string html, Uri baseUri, TextWriter writer, bool IsCatalog = true)
        {
            var data = GetMainContent(html);
            if (data == null)
            {
                return -1;
            }
            if (data is not List<ChapterItem>)
            {
                var text = data.ToString()!;
                writer.WriteLine(text);
                return text.Length;
            }
            if (!IsCatalog)
            {
                return -1;
            }
            var items = (List<ChapterItem>)data;
            if (items == null)
            {
                return -1;
            }
            await LoadChapter(baseUri, items, writer);
            return items.Count;
        }


        /// <summary>
        /// 获取页面的主要内容，章节提取，文章提取
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public object GetMainContent(string html)
        {
            html = CleanHtml(html);
            var tags = GetTagMaps(html);
            return GetContent(tags, html);
        }

        private async Task LoadChapter(Uri baseUri, List<ChapterItem> items, TextWriter writer)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (Paused)
                {
                    break;
                }
                var item = items[i];
                var uri = new Uri(baseUri, item.Url);
                var content = await GetAsync(uri.ToString());
                if (string.IsNullOrWhiteSpace(content))
                {
                    Logger?.Info($"[{i + 1}/{items.Count}]下载章节 《{item.Title}》 失败");
                    continue;
                }
                await writer.WriteLineAsync(item.Title);
                await writer.WriteLineAsync();
                var res = await RenderCatalogAsync(content!, uri, writer, false);
                if (res < 0)
                {
                    continue;
                }
                await writer.WriteLineAsync();
                await writer.WriteLineAsync();
                TaskContainer?.EmitProgress(i + 1, items.Count);
                Logger?.Progress(i + 1, items.Count, $"下载章节 《{item.Title}》 完成-{res}");
            }
        }

        private object GetContent(IList<NodeCountItem> tags, string html)
        {
            var i = tags.Count - 1;
            for (; i >= 0; i--)
            {
                if (tags[i].Center)
                {
                    break;
                }
            }
            if (i < 0)
            {
                i = 0;
            }
            if (tags.Count == 0)
            {
                return null;
            }
            var tag = tags[i];
            if (tag.Tag.Contains('a'))
            {
                return GetCatalog(GetChaptersStart(tags, i),
                    GetChaptersEnd(tags, i), html);
            }
            var start = GetContentStart(tags, i);
            var content = html.Substring(start, tags[i + 1].Index - start + 1);
            // content = Regex.Replace(content, @"^\<.+?</[^<>]+?\>", "");
            return Html.ToText(content);
        }

        private int GetContentStart(IList<NodeCountItem> tags, int i)
        {
            return i >= 1 ? tags[i - 1].Index + 1 : 0;
        }

        private List<ChapterItem> GetCatalog(int start, int end, string html)
        {
            var content = end > 0 ? html.Substring(start, end - start) : html.Substring(start);
            var matches = Regex.Matches(content, @"\<a href=""(.+?)""\>(.+?)\</a\>");
            var items = new List<ChapterItem>();
            foreach (Match match in matches)
            {
                items.Add(new ChapterItem()
                {
                    Url = match.Groups[1].Value,
                    Title = match.Groups[2].Value,
                });
            }
            return items;
        }

        private int GetChaptersEnd(IList<NodeCountItem> tags, int i)
        {
            while (i + 2 < tags.Count)
            {
                if (tags[i + 1].Count < 10 && tags[i + 2].Tag == tags[i].Tag)
                {
                    i += 2;
                    continue;
                }
                break;
            }
            return i + 1 > tags.Count ? -1 : (tags[i + 1].Index + 1);
        }

        private int GetChaptersStart(IList<NodeCountItem> tags, int i)
        {
            while (i >= 2)
            {
                if (tags[i - 1].Count < 10 &&
                    !tags[i - 1].Tag.Contains("dt") &&
                    tags[i - 2].Tag == tags[i].Tag)
                {
                    i -= 2;
                    continue;
                }
                break;
            }
            return tags[i].Index;
        }

        private IList<NodeCountItem> GetTagMaps(string html)
        {
            var maps = new List<NodeCountItem>();
            string? tag = null;
            var len = html.Length;
            var center = Math.Floor((double)len / 2);
            for (var i = 0; i < len; i++)
            {
                if (i == center && maps.Count > 0)
                {
                    maps[^1].Center = true;
                }
                var code = html[i];
                if (code == '/')
                {
                    continue;
                }
                if (code == '<')
                {
                    if (html[i + 1] == '/')
                    {
                        continue;
                    }
                    tag = "";
                    continue;
                }
                if (tag != null && (code == '>' || code == ' '))
                {
                    maps.Add(new NodeCountItem()
                    {
                        Tag = tag,
                        Count = 1,
                        Index = i - tag.Length - 2
                    });
                    tag = null;
                    continue;
                }
                if (tag != null)
                {
                    tag += code;
                }
            }
            return CompactTags(MergeTags(maps));
        }

        /// <summary>
        /// 压缩合并单个标签
        /// </summary>
        /// <param name="maps"></param>
        /// <returns></returns>
        private IList<NodeCountItem> CompactTags(List<NodeCountItem> maps)
        {
            for (int i = maps.Count - 1; i >= 1; i--)
            {
                if (maps[i].Tag != maps[i - 1].Tag)
                {
                    continue;
                }
                maps[i - 1].Count += maps[i].Count;
                if (maps[i].Center)
                {
                    maps[i - 1].Center = true;
                }
                maps.RemoveAt(i);
            }
            return maps;
        }

        /// <summary>
        /// 倒序合并多个空标签
        /// </summary>
        /// <param name="maps"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private List<NodeCountItem> MergeTags(List<NodeCountItem> maps)
        {
            var data = new List<NodeCountItem>();
            NodeCountItem? prev = null;
            for (int i = maps.Count - 1; i >= 0; i--)
            {
                var item = maps[i];
                if (item.Count > 1)
                {
                    if (prev != null)
                    {
                        data.Add(prev);
                        prev = null;
                    }
                    data.Add(item);
                    continue;
                }
                if (prev == null)
                {
                    prev = item;
                    continue;
                }
                if (item.Index + item.Tag.Length + 2 != prev.Index)
                {
                    data.Add(prev);
                    prev = item;
                    continue;
                }
                item.Tag = $"{item.Tag}.{prev.Tag}";
                if (!item.Center)
                {
                    item.Center = prev.Center;
                }
                prev = item;
                var count = data.Count;
                if (count < 1)
                {
                    continue;
                }
                if (prev.Tag != data[count - 1].Tag)
                {
                    continue;
                }
                data[count - 1].Index = prev.Index;
                data[count - 1].Count++;
                if (prev.Center)
                {
                    data[count - 1].Center = true;
                }
                prev = null;
            }
            if (prev != null)
            {
                data.Add(prev);
            }
            data.Reverse();
            return data;
        }

        private string CleanHtml(string html)
        {
            html = Regex.Replace(html, @"\<!DOCTYPE\s+html[^\>]*\>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"\<head\>([\s\S]+?)\</head\>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"\<!--[\s\S]*?--\>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"\s+", " ", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"(\>)[\s\n\r]+(\</?\w+)", "$1$2", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"\<style .*?\</style\>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"\<script.*?\</script\>", "", RegexOptions.IgnoreCase);
            return Regex.Replace(html, @"\</?(\w+)([^\<\>]*)/?\>", match =>
            {
                if (match.Groups[2].Value.Length < 1)
                {
                    return match.Value;
                }
                var replace = "";
                if (match.Groups[1].Value == "img")
                {
                    replace = CleanAttribute(match, "src");
                }
                else if (match.Groups[1].Value == "a")
                {
                    replace = CleanAttribute(match, "href");
                }
                else if (match.Groups[1].Value == "br")
                {
                    return "<br>";
                }
                return match.Value.Replace(match.Groups[2].Value, replace);
            });
        }

        private string CleanAttribute(Match match, string tag)
        {
            var m = Regex.Match(match.Groups[2].Value, tag + @"\s?=[\s""']?(\S+)[""']");
            if (match.Success)
            {
                return $" {tag}=\"{m.Groups[1].Value}\"";
            }
            return string.Empty;
        }
    }
}
