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

        private int _lastIndex = -1;
        public IList<ProxyItem> Items { get; private set; } = new List<ProxyItem>();

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

        public void Serializer(StreamWriter writer)
        {
            foreach (var item in Items)
            {
                writer.WriteLine(item.ToString());
            }
        }

        public void Deserializer(StreamReader reader)
        {
            string? line;
            while (null != (line = reader.ReadLine()))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    return;
                }
                Items.Add(new ProxyItem(line));
            }
        }
    }
}
