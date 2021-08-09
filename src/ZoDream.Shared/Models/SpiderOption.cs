using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Models
{
    public class SpiderOption: ILoader
    {
        public int MaxCount { get; set; } = 1;
        public int TimeOut { get; set; } = 60;
        public bool UseBrowser { get; set; } = false;
        /// <summary>
        /// 工作目录
        /// </summary>
        public string WorkFolder { get; set; }

        public IList<HeaderItem> HeaderItems { get; set; }

        public void Deserializer(StreamReader reader)
        {
            throw new NotImplementedException();
        }

        public void Serializer(StreamWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
