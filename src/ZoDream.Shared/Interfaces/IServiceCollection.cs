using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ZoDream.Shared.Interfaces
{
    public interface IServiceCollection
    {
        public void AddRule(Assembly assemby);
        public void AddRule(Type rule);
        public void AddRule(IEnumerable<Type> rules);
    }
}
