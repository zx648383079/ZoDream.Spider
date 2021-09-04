using ZoDream.Shared.Http;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Spiders;

namespace ZoDream.Shared.Providers
{
    public class RequestProvider : IRequestProvider
    {
        private ISpider Application;

        public RequestProvider(ISpider spider)
        {
            Application = spider;
        }

        public bool SupportTask { get; } = true;

        public IDownloadRequest Downloader()
        {
            return new HttpRequest();
        }

        public IRequest Getter()
        {
            return new HttpRequest();
        }
    }
}
