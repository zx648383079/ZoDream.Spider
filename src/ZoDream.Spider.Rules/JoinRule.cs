using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Spider.Rules
{
    public class JoinRule : IRule
    {
        private string? Separator;

        public PluginInfo Info()
        {
            return new PluginInfo("合并字符串");
        }

        public IFormInput[]? Form()
        {
            return new IFormInput[] { Input.Text(nameof(Separator), "连接符") };
        }
        public void Ready(RuleItem option)
        {
            Separator = option.Get<string>(nameof(Separator));
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
                        sb.Append(Separator);
                    }
                    sb.Append(item.ToString());
                }
                container.Data = new RuleString(sb.ToString());
            }
            await container.NextAsync();
        }
    }
}
