using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Http;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

namespace ZoDream.Spider.Rules
{
    public class DownloadRule : IRule, IRuleSaver
    {
        private string? FileName;

        public PluginInfo Info()
        {
            return new("直接下载");
        }

        public IFormInput[]? Form()
        {
            return [ Input.File(nameof(FileName), "保存路径") ];
        }

        public bool ShouldPrepare { get; } = false;
        public bool CanNext { get; } = false;

        public void Ready(RuleItem option)
        {
            FileName = option.Get<string>(nameof(FileName));
        }
        public string GetFileName(string url)
        {
            var path = Disk.RenderFile(url);
            if (string.IsNullOrEmpty(FileName))
            {
                return path;
            }
            var uri = new Uri(url);
            return Regex.Replace(FileName, @"\$\{([a-zA-Z0-9_]+)\}", match => {
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
