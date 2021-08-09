using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Interfaces
{
    /// <summary>
    /// 定义规则
    /// </summary>
    public interface IRule
    {
        public ISpider Container { get; set; }
        public IRuleValue Render(IRuleValue value);
    }
}
