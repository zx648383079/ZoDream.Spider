using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Spider.Helper;

namespace ZoDream.Spider.Model
{
    [Serializable]
    public class UrlItem
    {
        public string Url { get; set; }

        public List<RuleItem> Rults { get; set; }

        public bool IsMatch(string url)
        {
            return Regex.IsMatch(url, Url);
        }

        public UrlItem()
        {

        }

        public UrlItem(string url, List<RuleItem> rules)
        {
            Url = url;
            Rults = rules;
        }

        public override string ToString() {
            return $"有 {Rults.Count} 条规则";
        }
    }
}
