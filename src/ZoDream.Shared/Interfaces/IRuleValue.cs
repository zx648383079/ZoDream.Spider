using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Interfaces
{
    public interface IRuleValue: IEnumerable
    {

        public string ToString();

        public IRuleValue Clone();

        public IRuleValue Select(Func<IRuleValue, IRuleValue> selector);
    }
}
