using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Spider.BookCrawlerRule.Models
{
    public class NodeCountItem
    {
        public string Tag { get; set; }

        public int Count { get; set; }

        public int Index { get; set; }

        public bool Center { get; set; } = false;
    }
}
