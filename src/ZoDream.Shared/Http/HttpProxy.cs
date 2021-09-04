using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Http
{
    public static class HttpProxy
    {

        public static bool Test(ProxyItem proxy)
        {
            return Test(proxy, "https://www.baidu.com/");
        }

        public static bool Test(ProxyItem proxy, string testUrl)
        {
            if (proxy == null || string.IsNullOrWhiteSpace(testUrl))
            {
                return false;
            }
            var client = new Client();
            client.Proxy = proxy;
            client.TimeOut = 20 * 1000;
            var html = client.Get(testUrl);
            return html != null;
        }
    }
}
