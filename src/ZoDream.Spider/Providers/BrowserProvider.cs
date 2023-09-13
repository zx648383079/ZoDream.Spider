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
        }

        public bool SupportTask => !Application.Project.UseBrowser;

        public IDownloadRequest Downloader()
        {
            return new HttpRequest();
        }

        public IRequest Getter()
        {
            return Application.Project.UseBrowser ? App.ViewModel.BrowserRequest : new HttpRequest();
        }
    }
}
