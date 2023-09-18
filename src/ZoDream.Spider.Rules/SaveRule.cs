using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

namespace ZoDream.Spider.Rules
{
    public class SaveRule : IRule, IRuleSaver
    {
        private string FileName = string.Empty;
        private string Template = string.Empty;
        public PluginInfo Info()
        {
            return new PluginInfo("本地保存");
        }

        public bool ShouldPrepare { get; } = true;
        public bool CanNext { get; } = true;

        public IFormInput[]? Form()
        {
            return new IFormInput[] {
                Input.File(nameof(FileName), "保存路径", true, true),
                Input.File(nameof(Template), "模板文件"),
            };
        }

        public void Ready(RuleItem option)
        {
            FileName = option.Get<string>(nameof(FileName)) ?? string.Empty;
            Template = option.Get<string>(nameof(Template)) ?? string.Empty;
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
                writer.Write(container.RenderData(Template));
            }
        }
    }
}
