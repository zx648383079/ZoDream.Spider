using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Spider.Model
{
    public class HeaderItem
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public HeaderItem()
        {

        }

        public HeaderItem(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
