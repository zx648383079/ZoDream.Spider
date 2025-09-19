using System;
using System.Collections;

namespace ZoDream.Shared.Interfaces
{
    public interface IRuleValue: IEnumerable, ICloneable
    {

        public string ToString();

        public IRuleValue Select(Func<IRuleValue, IRuleValue> selector);
    }
}
