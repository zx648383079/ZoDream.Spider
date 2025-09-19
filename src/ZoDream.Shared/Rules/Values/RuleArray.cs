using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Rules.Values
{
    public class RuleArray : IRuleValue
    {
        public IList<IRuleValue> Items { get; set; } = new List<IRuleValue>();

        public RuleArray()
        {

        }

        public RuleArray(IList<IRuleValue> items)
        {
            Items = items;
        }

        public void Add(IRuleValue item)
        {
            Items.Add(item);
        }

        public void Add(string item)
        {
            Items.Add(new RuleString(item));
        }

        public IRuleValue Select(Func<IRuleValue, IRuleValue> selector)
        {
            return new RuleArray()
            {
                Items = Items.Select(selector).ToList()
            };
        }

        public object Clone()
        {
            return new RuleArray()
            {
                Items = Items.Select(i => (IRuleValue)i.Clone()).ToList()
            };
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var item in Items)
            {
                sb.AppendLine(item.ToString());
            }
            return sb.ToString();
        }

        public IEnumerator GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
