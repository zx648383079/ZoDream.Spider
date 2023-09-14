using ZoDream.Shared.Http;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Spider.Providers
{
    public class RequestProvider : IRequestProvider
    {
        private readonly ISpider Application;

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
