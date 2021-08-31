using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Rules.Values
{
    public class RuleString : IRuleValue
    {
        public string Content { get; set; } = string.Empty;

        public IRuleValue Select(Func<IRuleValue, IRuleValue> selector)
        {
            return selector(this);
        }

        public IRuleValue Clone()
        {
            return new RuleString() { Content = Content };
        }

        public override string ToString()
        {
            return Content;
        }

        public IEnumerator GetEnumerator()
        {
            return new RuleString[] { this }.GetEnumerator();
        }

        public RuleString()
        {

        }

        public RuleString(string content)
        {
            Content = content;
        }
    }
}
