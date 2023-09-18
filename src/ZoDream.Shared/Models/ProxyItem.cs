
using System;

namespace ZoDream.Shared.Models
{
    public class ProxyItem: Http.Models.ProxyItem
    {
        public ProxyItem()
        {
            
        }

        public ProxyItem(string url): base(url)
        {
        }

        public ProxyItem(Uri uri) : base(uri)
        {
        }
    }
}
