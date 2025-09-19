using System;
using System.Collections;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Rules.Values
{
    public class RuleSource(IHttpResponse response) : IRuleValue
    {

        public IHttpResponse Source => response;

        public object Clone()
        {
            return new RuleSource(response);
        }

        public IEnumerator GetEnumerator()
        {
            return new RuleSource[] { this }.GetEnumerator();
        }

        public IRuleValue Select(Func<IRuleValue, IRuleValue> selector)
        {
            return selector(this);
        }

        public override string ToString()
        {
            return response.ReadAsync().GetAwaiter().GetResult();
        }
    }
}
