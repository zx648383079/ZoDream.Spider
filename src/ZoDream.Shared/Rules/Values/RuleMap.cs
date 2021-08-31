using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Rules.Values
{
    public class RuleMap: IRuleValue
    {
        public string Content { get; set; } = string.Empty;
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();


        public RuleMap()
        {

        }

        public RuleMap(string[] tags, Match match)
        {
            Content = match.Value;
            foreach (var item in tags)
            {
                Data.Add(item, match.Groups[item].Value);
            }
        }

        public IRuleValue Clone()
        {
            throw new NotImplementedException();
        }

        public IRuleValue Select(Func<IRuleValue, IRuleValue> selector)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
