using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Interfaces
{
    public interface IRuleSaver
    {
        /// <summary>
        /// 是否需要预处理
        /// </summary>
        public bool ShouldPrepare{ get;}
        public string GetFileName(string url);
    }
}
