using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Providers
{
    public class RuleProvider : IRuleProvider
    {
        public IList<IRuleGroup> Items { get; private set; } = new List<IRuleGroup>();

        public IList<IRuleGroup> Get(string uri)
        {
            return Items.Where(item => item.IsMatch(uri)).ToList();
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
