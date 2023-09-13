using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.Providers
{
    public class ProxyProvider : IProxyProvider
    {
        public ProxyProvider(ISpider app)
        {
            foreach (var item in app.Project.ProxyItems)
            {
                Items.Add(new ProxyItem(item));
            }
        }

        private int _lastIndex = -1;
        public List<ProxyItem> Items { get; private set; } = new();

        public ProxyItem? Get()
        {
            if (Items.Count < 1)
            {
                return null;
            }
            _lastIndex++;
            if (_lastIndex >= Items.Count)
            {
                _lastIndex = 0;
            }
            return Items[_lastIndex];
        }
    }
}
