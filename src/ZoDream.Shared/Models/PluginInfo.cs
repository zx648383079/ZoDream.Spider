using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Models
{
    public class PluginInfo
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Version { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public PluginInfo()
        {

        }

        public PluginInfo(string name)
        {
            Name = name;
        }
    }
}
