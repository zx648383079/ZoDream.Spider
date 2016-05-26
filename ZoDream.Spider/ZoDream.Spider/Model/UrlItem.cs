using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Spider.Model
{
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
    }
}
