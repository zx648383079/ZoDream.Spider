﻿using System;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Spider.BookCrawlerRule
{
    public class Ruler : IRule
    {
        public PluginInfo Info()
        {
            return new PluginInfo("自动小说下载");
        }

        public void Ready(RuleItem option)
        {
            
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var crawler = new Crawler(container.Application);
            var content = await crawler.RenderAsync(
                new Uri(container.Url.Source),
                container.Data.ToString());
            if (content != null && !string.IsNullOrWhiteSpace(content))
            {
                container.Data = new RuleString(content);
                await container.NextAsync();
            }
        }
    }
}