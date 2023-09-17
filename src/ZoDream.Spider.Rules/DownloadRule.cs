using System;
using System.Collections.Generic;
using System.IO;
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
                return match.Groups[1].Value switch
                {
                    "host" => uri.Host,
                    "path" => path,
                    "md5" => Md5.Encode(url),
                    _ => match.Groups[0].Value,
                };
            });
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var file = container.Application.Storage.GetAbsolutePath(GetFileName(container.Url.Source));
            await container.GetAsync(
                file,
                container.Url.Source);
        }
    }
}
