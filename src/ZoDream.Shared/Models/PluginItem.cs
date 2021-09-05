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

        /// <summary>
        /// 是否需要预处理
        /// </summary>
        public bool ShouldPrepare { get; set; } = true;
        /// <summary>
        /// 判断是否会执行下一个规则
        /// </summary>
        public bool CanNext { get; set; } = true;

        public bool IsSaver { get; set; } = false;

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
