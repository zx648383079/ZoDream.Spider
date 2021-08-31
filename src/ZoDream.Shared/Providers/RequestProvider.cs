using ZoDream.Shared.Http;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Providers
{
    public class RequestProvider : IRequestProvider
    {
        public IRequest Downloader()
        {
            return new HttpRequest();
        }

        public IRequest Getter()
        {
            return new HttpRequest();
        }
    }
}
