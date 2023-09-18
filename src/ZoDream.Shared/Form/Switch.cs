using System;
using System.Collections.Generic;
using System.Text;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Form
{
    public class Switch : IFormInput
    {
        public string Name { get; private set; }

        public string Label { get; private set; }

        public string Tip { get; private set; } = string.Empty;

        public bool TryParse(ref object input)
        {
            if (input is bool)
            {
            } else if (input is null)
            {
                input = false;
            } else
            {
                var val = input.ToString().ToUpper();
                input = val == "1" || val == "Y" || val == "TRUE";
            }
            return true;
        }

        public Switch(string name, string label)
        {
            Name = name;
            Label = label;
        }
    }
}
