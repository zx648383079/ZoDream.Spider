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
            var file = container.Application.GetAbsoluteFile(GetFileName(string.Empty));
            Disk.CreateDirectory(file);
            var ruleGroup = new RuleGroupItem() { Name = ruleGroupName };
            var writer = Open.Writer(file, true);
            foreach (var item in container.Application.UrlProvider)
            {
                if (!ruleGroup.IsMatch(item.Source))
                {
                    continue;
                }
                var tempfile = container.Application.RuleProvider.GetFileName(item.Source);
                if (!string.IsNullOrEmpty(tempfile))
                {
                    writer.WriteLine(Open.Read(tempfile));
                    writer.WriteLine();
                }
            }
            writer.Close();
        }
    }
}
