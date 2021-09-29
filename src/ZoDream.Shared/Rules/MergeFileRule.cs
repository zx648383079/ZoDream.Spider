using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Local;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

namespace ZoDream.Shared.Rules
{
    public class MergeFileRule : IRule, IRuleSaver
    {
        public bool ShouldPrepare => false;

        public bool CanNext => false;

        private string ruleGroupName = string.Empty;

        private string fileName = string.Empty;

        public string GetFileName(string url)
        {
            return fileName;
        }

        public PluginInfo Info()
        {
            return new PluginInfo("合并文件");
        }

        public void Ready(RuleItem option)
        {
            ruleGroupName = option.Param1;
            fileName = option.Param2;
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var storage = container.Application.Storage;
            var ruleGroup = new RuleGroupItem() { Name = ruleGroupName };
            var writer = Open.Writer(await storage.CreateStreamAsync(GetFileName(string.Empty)), true);
            foreach (var item in container.Application.UrlProvider)
            {
                if (!ruleGroup.IsMatch(item.Source))
                {
                    continue;
                }
                var tempfile = await storage.OpenStreamAsync(item);
                if (tempfile != null)
                {
                    writer.WriteLine(Open.Read(tempfile));
                    writer.WriteLine();
                }
                tempfile?.Close();
            }
            writer.Close();
        }
    }
}
