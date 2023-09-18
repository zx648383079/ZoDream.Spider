using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class DataItem
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public DataItem(string name, object val)
        {
            Name = name;
            Value = val;
        }
    }
}
