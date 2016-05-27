using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Spider.Model
{
    [Serializable]
    public class UrlItem
    {
        public string Url { get; set; }

        public List<RuleItem> Rults { get; set; }

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
