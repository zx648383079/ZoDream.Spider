using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

namespace ZoDream.Shared.Rules
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
            return Str.ReplaceCallback(fileName, @"\${([a-zA-Z0-9_])}", match => {
                switch (match.Groups[0].Value)
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


        public void Render(ISpiderContainer container)
        {
            var file = container.Application.GetAbsoluteFile(GetFileName(container.Url.Source));
            Disk.CreateDirectory(file);
            using (var fs = new FileStream(file, FileMode.Create))
            using (var writer = new StreamWriter(fs, Encoding.UTF8))
            {
                writer.BaseStream.Position = writer.BaseStream.Length;
                writer.WriteLine();
                writer.Write(container.RenderData(template));
            }
        }
    }
}
