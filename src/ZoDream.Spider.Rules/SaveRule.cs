using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

namespace ZoDream.Spider.Rules
{
    public class SaveRule : IRule, IRuleSaver
    {
        private string fileName = string.Empty;
        private string template = string.Empty;
        public PluginInfo Info()
        {
            return new PluginInfo("本地保存");
        }

        public bool ShouldPrepare { get; } = true;
        public bool CanNext { get; } = true;
        public void Ready(RuleItem option)
        {
            fileName = option.Param1.Trim();
            template = option.Param2.Trim();
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
                    _ => match.Value,
                };
            });
        }


        public async Task RenderAsync(ISpiderContainer container)
        {
            var storage = container.Application.Storage;
            using (var fs = await storage.CreateStreamAsync(GetFileName(container.Url.Source)))
            using (var writer = new StreamWriter(fs, new UTF8Encoding(false)))
            {
                writer.BaseStream.Position = writer.BaseStream.Length;
                writer.WriteLine();
                writer.Write(container.RenderData(template));
            }
        }
    }
}
