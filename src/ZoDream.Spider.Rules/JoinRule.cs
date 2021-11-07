using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Spider.Rules
{
    public class JoinRule : IRule
    {
        private string joinLink = string.Empty;

        public PluginInfo Info()
        {
            return new PluginInfo("合并字符串");
        }

        public void Ready(RuleItem option)
        {
            joinLink = option.Param1;
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var data = container.Data;
            if (data is RuleArray)
            {
                var sb = new StringBuilder();
                var i = 0;
                foreach (var item in (data as RuleArray).Items)
                {
                    i++;
                    if (i < 2)
                    {
                        sb.Append(joinLink);
                    }
                    sb.Append(item.ToString());
                }
                container.Data = new RuleString(sb.ToString());
            }
            await container.NextAsync();
        }
    }
}
