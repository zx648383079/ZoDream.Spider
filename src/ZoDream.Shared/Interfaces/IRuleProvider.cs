using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    public interface IRuleProvider: ILoader
    {
        public IList<RuleGroupItem> Get(string uri);

        public IRule Render(string name);
        public void Add(RuleGroupItem rule);
        public void Add(IList<RuleGroupItem> rules);
        public IList<RuleGroupItem> All();
    }
}
