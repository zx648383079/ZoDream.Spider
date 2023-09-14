using System;
using System.Net;
using ZoDream.Shared.ViewModel;

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

    public class HeaderBindingItem: BindableBase
    {
        public string Name { get; set; }

        private string _value = string.Empty;

        public string Value {
            get => _value;
            set => Set(ref _value, value);
        }


        public HeaderBindingItem(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public HeaderBindingItem(HeaderItem item): this(item.Name, item.Value)
        {
            
        }
    }
}
