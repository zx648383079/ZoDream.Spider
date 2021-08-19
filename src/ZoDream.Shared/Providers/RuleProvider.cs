using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Providers
{
    public class RuleProvider : IRuleProvider
    {
        public IList<RuleGroupItem> Items { get; private set; } = new List<RuleGroupItem>();

        public IList<RuleGroupItem> Get(string uri)
        {
            return Items.Where(item => item.IsMatch(uri)).ToList();
        }

        public void Add(RuleGroupItem rule)
        {
            Items.Add(rule);
        }

        public void Add(IList<RuleGroupItem> rules)
        {
            Items = rules.ToList();
        }
        public IList<RuleGroupItem> All()
        {
            return Items.ToList();
        }

        public IRule Render(string name)
        {
            throw new NotImplementedException();
        }

        public void Deserializer(StreamReader reader)
        {
            
        }

        public void Serializer(StreamWriter writer)
        {
            
        }
    }
}
