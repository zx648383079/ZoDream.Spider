using ZoDream.Shared.Http;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Spider.Providers
{
    public class BrowserProvider : IRequestProvider
    {
        private readonly ISpider Application;

        public BrowserProvider(ISpider spider)
        {
            Application = spider;
            UseBrowser = spider.Project.UseBrowser;
        }

        public bool UseBrowser { get; set; } = false;
        public bool SupportTask => UseBrowser;

        public IDownloadRequest Downloader()
        {
            return new HttpRequest();
        }

        public IRequest Getter()
        {
            return UseBrowser ? App.ViewModel.BrowserRequest : new HttpRequest();
        }
    }
}
