using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Http
{
    public class HttpRequest : IRequest
    {
        public Task<string> GetAsync(string url)
        {
            return Task.Factory.StartNew(() =>
            {
                var res = new Client().Get(url);
                return res == null ? string.Empty : res;
            });
        }

        public Task<string> GetAsync(string url, IList<HeaderItem> headers)
        {
            return Task.Factory.StartNew(() =>
            {
                var client = new Client();
                client.Headers = headers;
                var res = client.Get(url);
                return res == null ? string.Empty : res;
            });
        }
    }
}
