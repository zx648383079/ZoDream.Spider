using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Interfaces
{
    public interface IRequestProvider
    {
        public IRequest Getter();

        public IRequest Downloader();
    }
}
