using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZoDream.Shared.Models
{
    public class RuleGroupItem
    {
        public string Name { get; set; } = string.Empty;

        public IList<RuleItem> Rules { get; set; } = new List<RuleItem>();

        public bool IsMatch(string uri)
        {
            if (Regex.IsMatch(Name, @"^\w+\.\w+(\.\w+)?$"))
            {
                return new Uri(uri).Host == Name;
            }
            return Regex.IsMatch(uri, Name);
        }
    }
}
