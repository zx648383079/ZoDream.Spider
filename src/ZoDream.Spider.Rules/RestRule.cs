using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Http;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.Rules
{
    public class RestRule : IRule
    {
        private string postUri = string.Empty;
        private string template = string.Empty;
        public PluginInfo Info()
        {
            return new PluginInfo("POST保存");
        }

        public void Ready(RuleItem option)
        {
            postUri = option.Param1.Trim();
            template = option.Param2.Trim();
        }
        public string GetFileName(string url)
        {
            throw new NotImplementedException();
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var client = new Client();
            var data = container.RenderData(template);
            await client.PostAsync(postUri, data, data.StartsWith("{") ? "application/json" : "application/x-www-form-urlencoded");
        }
    }
}
