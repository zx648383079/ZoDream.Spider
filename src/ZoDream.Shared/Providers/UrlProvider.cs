using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Providers
{
    public class UrlProvider : IUrlProvider
    {
        public IList<UriItem> Items { get; private set; } = new List<UriItem>();
        public void Add(string url)
        {
            if (Contains(url))
            {
                return;
            }
            Items.Add(new UriItem() { Source = url });
        }

        public bool Contains(string url)
        {
            foreach (var item in Items)
            {
                if (item.Source == url)
                {
                    return true;
                }
            }
            return false;
        }


        public IEnumerator<UriItem> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Remove(string url)
        {
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                if (Items[i].Source == url)
                {
                    Items.RemoveAt(i);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Serializer(StreamWriter writer)
        {
            foreach (var item in Items)
            {
                writer.WriteLine(item.Source);
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
                Add(line);
            }
        }
    }
}
