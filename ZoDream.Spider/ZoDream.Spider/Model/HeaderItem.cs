using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Spider.Model
{
    public class HeaderItem
    {
        public HttpRequestHeader Name { get; set; }

        public string Value { get; set; }

        public HeaderItem()
        {

        }

        public HeaderItem(string name, string value)
        {
            Name = (HttpRequestHeader)Enum.Parse(typeof(HttpRequestHeader), name);
            Value = value;
        }

        public HeaderItem(HttpRequestHeader name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
