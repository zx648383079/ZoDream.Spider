using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Rules
{
    public class DownloadRule : IRule, IRuleSaver
    {
        public string GetFileName(string url)
        {
            throw new NotImplementedException();
        }

        public void Render(ISpiderContainer container)
        {
            throw new NotImplementedException();
        }
    }
}
