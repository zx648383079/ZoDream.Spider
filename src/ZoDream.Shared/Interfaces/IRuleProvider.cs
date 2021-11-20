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
        /// <summary>
        /// 是否有符合网址的规则
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public bool Canable(string uri);
        public IList<RuleGroupItem> Get(string uri);
        public IList<RuleGroupItem> GetEvent(string name);

        public void Add(RuleGroupItem rule);
        public void Add(IList<RuleGroupItem> rules);
        public IList<RuleGroupItem> All();
        public string GetFileName(string uri);
        
    }
}
