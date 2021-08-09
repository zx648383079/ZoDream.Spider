using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Interfaces
{
    public interface IRuleProvider: ILoader
    {
        public IList<IRuleGroup> Get(string uri);

        public IRule Render(string name);
    }
}
