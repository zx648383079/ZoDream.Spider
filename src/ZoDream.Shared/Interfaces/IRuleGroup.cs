using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Interfaces
{
    public interface IRuleGroup
    {
        public string Pattern { get; set; }

        public IList<IRule> Items { get; set; }

        public bool IsMatch(string uri);
    }
}
