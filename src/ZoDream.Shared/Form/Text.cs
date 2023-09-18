using System;
using System.Collections.Generic;
using System.Text;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Form
{
    public class Text : IFormInput
    {
        public string Name { get; private set; }

        public string Label { get; private set; }

        public bool Required { get; private set; }

        public string Tip { get; private set; } = string.Empty;

        public bool TryParse(ref object input)
        {
            return true;
        }

        public Text(string name, string label, bool required = false)
        {
            Name = name;
            Label = label;
            Required = required;
        }
    }
}
