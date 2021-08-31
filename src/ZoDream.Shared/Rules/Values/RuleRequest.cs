using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Rules.Values
{
    public class RuleRequest : IRuleValue
    {
        public string Url { get; set; }

        public ISpider Container { get; set; }

        public IRuleValue Clone()
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IRuleValue Render(IRuleValue value)
        {
            throw new NotImplementedException();
        }

        public IRuleValue Select(Func<IRuleValue, IRuleValue> selector)
        {
            throw new NotImplementedException();
        }
    }
}
