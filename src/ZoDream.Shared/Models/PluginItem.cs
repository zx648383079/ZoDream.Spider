using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Models
{
    public class PluginItem: PluginInfo
    {
        public string FileName {  get; set; } = string.Empty;

        public Type? Callback {  get; set; }

        public PluginItem()
        {

        }

        public PluginItem(string name): base(name)
        {
        }

        public PluginItem(PluginInfo info)
        {
            Name = info.Name;
            Description = info.Description;
            Author = info.Author;
            Version = info.Version;
        }
    }
}
