using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    public interface IRequestHost
    {
        public bool Cannable(string url, UriType type);
        public Task InvokeAsync(string url, IHttpResponse response);
        
    }
}
