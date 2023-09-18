using System;
using System.Collections.Generic;
using System.Text;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Form
{
    public class Numeric : IFormInput
    {
        public string Name { get; private set; }

        public string Label { get; private set; }

        public string Tip { get; private set; } = string.Empty;

        public bool TryParse(ref object input)
        {
            if (input is null)
            {
                input = 0;
            } else if (int.TryParse(input.ToString(), out var res))
            {
                input = res;
            } else
            {
                input = 0;
            }
            return true;
        }

        public Numeric(string name, string label)
        {
            Name = name;
            Label = label;
        }
    }
}
