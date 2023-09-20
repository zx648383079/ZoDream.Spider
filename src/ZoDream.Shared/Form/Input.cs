using System;
using System.Collections.Generic;
using System.Text;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Form
{
    public static class Input
    {
        public static Text Text(string name, string label, bool required = false)
        {
            return new Text(name, label, required);
        }

        public static Text Url(string name, string label, bool required = false)
        {
            return new Text(name, label, required);
        }

        public static Switch Switch(string name, string label)
        {
            return new Switch(name, label);
        }

        public static Numeric Numeric(string name, string label)
        {
            return new Numeric(name, label);
        }

        public static File File(string name, string label, 
            bool required = false, 
            bool isSave = false, bool isFolder = false)
        {
            return new File(name, label, required, isSave, isFolder);
        }

        public static Select Select(string name, string label, DataItem[] items)
        {
            return new Select(name, label, items);
        }
    }
}
