using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Http;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.Rules
{
    public class RestRule : IRule
    {
        private string ApiUri = string.Empty;
        private string ApiToken = string.Empty;
        private string Template = string.Empty;
        public PluginInfo Info()
        {
            return new PluginInfo("POST保存");
        }

        public IFormInput[]? Form()
        {
            return new IFormInput[] {
                Input.Url(nameof(ApiUri), "接口地址", true),
                Input.Text(nameof(ApiToken), "授权令牌"),
                Input.File(nameof(Template), "模板文件"),
            };
        }

        public void Ready(RuleItem option)
        {
            ApiUri = option.Get<string>(nameof(ApiUri)) ?? string.Empty;
            ApiToken = option.Get<string>(nameof(ApiToken)) ?? string.Empty;
            Template = option.Get<string>(nameof(Template)) ?? string.Empty;
        }
        public string GetFileName(string url)
        {
            return string.Empty;
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var client = new Client();
            var data = container.RenderData(Template);
            await client.PostAsync(ApiUri, data, data.StartsWith("{") ? "application/json" : "application/x-www-form-urlencoded");
        }
    }
}
