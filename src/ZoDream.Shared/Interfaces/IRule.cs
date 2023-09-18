using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    /// <summary>
    /// 定义规则
    /// </summary>
    public interface IRule
    {

        public PluginInfo Info();

        public IFormInput[]? Form();

        public void Ready(RuleItem option);

        public Task RenderAsync(ISpiderContainer container);
    }
}
