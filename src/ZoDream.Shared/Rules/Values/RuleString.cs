using System;
using System.Collections;
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

        public object? Clone()
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
