using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class AppOption
    {
        public bool IsLogVisible { get; set; } = false;

        public bool IsLogTime { get; set; } = false;

        public List<string> PluginItems { get; set; } = new();
    }
}
