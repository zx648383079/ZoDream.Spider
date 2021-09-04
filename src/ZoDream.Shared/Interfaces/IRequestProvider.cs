using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Interfaces
{
    public interface IRequestProvider
    {
        public bool SupportTask { get; }

        public IRequest Getter();

        public IDownloadRequest Downloader();
    }
}
