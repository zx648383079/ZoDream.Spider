using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Interfaces
{
    public interface IServiceCollection
    {
        public void AddRule(Type rule);
        public void AddRule(IEnumerable<Type> rules);
    }
}
