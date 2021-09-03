using System;
using System.Collections.Generic;
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

        public void Ready(RuleItem option)
        {
            fileName = option.Param1.Trim();
            template = option.Param2.Trim();
        }
        public string GetFileName(string url)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return Md5.Encode(url);
            }
            return url;
        }

        public void Render(ISpiderContainer container)
        {
            using (var fs = new FileStream(GetFileName(container.Url.Source), FileMode.Create))
            using (var writer = new StreamWriter(fs, Encoding.UTF8))
            {
                writer.BaseStream.Position = writer.BaseStream.Length;
                writer.WriteLine();
                writer.Write(container.RenderData(template));
            }
        }
    }
}
