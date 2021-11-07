using JiebaNet.Segmenter;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using ZoDream.Shared.Http;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.ImportSearchRule
{
    public class Ruler : IRule
    {
        private string apiUri = string.Empty;
        private string apiToken = string.Empty;
        public PluginInfo Info()
        {
            return new PluginInfo("µ¼ÈëËÑË÷ÒýÇæ");
        }

        public void Ready(RuleItem option)
        {
            apiUri = option.Param1.Trim();
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var segmenter = new JiebaNet.Analyser.TfidfExtractor();
            var tags = segmenter.ExtractTags(container.Data.ToString());
            var client = new Client();
            if (!string.IsNullOrEmpty(apiToken))
            {
                client.Headers.Add("Authorization", "Bearer " + apiToken);
            }
            var data = new PostForm();
            data.Title = container.GetAttribute("title");
            data.Description = container.GetAttribute("description");
            data.Content = container.GetAttribute("content");
            data.Link = container.GetAttribute("url");
            data.Keywords = tags;
            await client.PostAsync(apiUri, JsonConvert.SerializeObject(data), "application/json");
            await container.NextAsync();
        }
    }
}
