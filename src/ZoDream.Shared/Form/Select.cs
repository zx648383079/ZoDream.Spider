using System;
using System.Collections.Generic;
using System.Text;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Form
{
    public class Select : IFormInput
    {
        public string Name { get; private set; }

        public string Label { get; private set; }

        public DataItem[] Items { get; private set; }

        public string Tip { get; private set; } = string.Empty;

        public bool TryParse(ref object input)
        {
            foreach (var item in Items)
            {
                if (input == item)
                {
                    return true;
                }
            }
            input = Items[0].Value;
            return true;
        }

        public Select(string name, string label, DataItem[] items)
        {
            Name = name;
            Label = label;
            Items = items;
        }
    }
}
