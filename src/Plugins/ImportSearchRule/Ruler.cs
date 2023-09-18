using JiebaNet.Segmenter;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Http;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.ImportSearchRule
{
    public class Ruler : IRule
    {
        private string ApiUri = string.Empty;
        private string ApiToken = string.Empty;
        public PluginInfo Info()
        {
            return new PluginInfo("导入搜索引擎");
        }

        public IFormInput[]? Form()
        {
            return new IFormInput[] { 
                Input.Url(nameof(ApiUri), "接口地址", true),
                Input.Text(nameof(ApiToken), "授权令牌"),
            };
        }

        public void Ready(RuleItem option)
        {
            ApiUri = option.Get<string>(nameof(ApiUri)) ?? string.Empty;
            ApiToken = option.Get<string>(nameof(ApiToken)) ?? string.Empty;
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var logger = container.Logger;
            var segmenter = new JiebaNet.Analyser.TfidfExtractor();
            var tags = segmenter.ExtractTags(container.Data.ToString());
            logger?.Debug(string.Join(" ", tags));
            var client = new Client();
            if (!string.IsNullOrEmpty(ApiToken))
            {
                client.Headers.Add("Authorization", "Bearer " + ApiToken);
            }
            var data = new PostForm
            {
                Title = container.GetAttribute("title"),
                Description = container.GetAttribute("description"),
                Content = container.GetAttribute("content"),
                Link = container.GetAttribute("url"),
                Keywords = tags
            };
            await client.PostAsync(ApiUri, JsonConvert.SerializeObject(data), "application/json");
            await container.NextAsync();
        }
    }
}
