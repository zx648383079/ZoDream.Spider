using System;
using System.Net;

namespace ZoDream.Shared.Models
{
    public class HeaderItem
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public HeaderItem(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
