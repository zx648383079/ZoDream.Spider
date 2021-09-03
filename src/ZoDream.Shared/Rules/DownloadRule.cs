using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Http;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Rules
{
    public class DownloadRule : IRule, IRuleSaver
    {
        private string fileName;

        public PluginInfo Info()
        {
            return new PluginInfo("直接下载");
        }

        public void Ready(RuleItem option)
        {
            fileName = option.Param1.Trim();
        }
        public string GetFileName(string url)
        {
            throw new NotImplementedException();
        }

        public void Render(ISpiderContainer container)
        {
            var client = new Client();
            client.ReadAsFile(container.Url.Source, GetFileName(container.Url.Source));
        }
    }
}
