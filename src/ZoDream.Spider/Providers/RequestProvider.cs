using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Http;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Spider.Providers
{
    public class RequestProvider : IRequestProvider
    {
        private ISpider Application;

        public RequestProvider(ISpider spider)
        {
            Application = spider;
        }

        public bool SupportTask => !Application.Option.UseBrowser;

        public IDownloadRequest Downloader()
        {
            return new HttpRequest();
        }

        public IRequest Getter()
        {
            return Application.Option.UseBrowser ? App.ViewModel.BroswerRequest : new HttpRequest();
        }
    }
}
