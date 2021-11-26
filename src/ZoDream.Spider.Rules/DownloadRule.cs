using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Http;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

namespace ZoDream.Spider.Rules
{
    public class DownloadRule : IRule, IRuleSaver
    {
        private string fileName = string.Empty;

        public PluginInfo Info()
        {
            return new PluginInfo("直接下载");
        }

        public bool ShouldPrepare { get; } = false;
        public bool CanNext { get; } = false;

        public void Ready(RuleItem option)
        {
            fileName = option.Param1.Trim();
        }
        public string GetFileName(string url)
        {
            var path = Disk.RenderFile(url);
            if (string.IsNullOrEmpty(fileName))
            {
                return path;
            }
            var uri = new Uri(url);
            return Regex.Replace(fileName, @"\$\{([a-zA-Z0-9_]+)\}", match => {
                switch (match.Groups[1].Value)
                {
                    case "host":
                        return uri.Host;
                    case "path":
                        return path;
                    case "md5":
                        return Md5.Encode(url);
                    default:
                        return match.Groups[0].Value;
                }
            });
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var file = container.Application.Storage.GetAbsolutePath(GetFileName(container.Url.Source));
            await container.Application.RequestProvider.Downloader().GetAsync(
                file, 
                container.Url.Source,
                container.Application.Option.HeaderItems,
                container.Application.ProxyProvider.Get());
        }
    }
}
