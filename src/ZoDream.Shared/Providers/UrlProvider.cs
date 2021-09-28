using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Events;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Providers
{
    public class UrlProvider : IUrlProvider
    {
        public UrlProvider(ISpider spider)
        {
            Application = spider;
        }

        public ISpider Application { get; set; }

        public IList<UriItem> Items { get; private set; } = new List<UriItem>();

        public event UrlChangedEventHandler? UrlChanged;

        public int Count {  get {  return Items.Count; }  }

        public void Add(string url)
        {
            Add(url, UriType.Html);
        }

        public void Add(string url, UriType uriType)
        {
            Add(url, uriType, UriStatus.NONE);
        }

        public void Add(string url, UriType uriType, UriStatus status)
        {
            if (Contains(url))
            {
                return;
            }
            var item = new UriItem() { Source = url, Type = uriType, Status = status };
            Items.Add(item);
            UrlChanged?.Invoke(item, true);
        }

        public void Add(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
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


        public UriItem? Get(string url)
        {
            foreach (var item in Items)
            {
                if (item.Source == url)
                {
                    return item;
                }
            }
            return null;
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
            UrlChanged?.Invoke(null, true);
        }

        public void Remove(IEnumerable<UriItem> urls)
        {
            foreach (var item in urls)
            {
                Items.Remove(item);
            }
            UrlChanged?.Invoke(null, true);
        }
        public void Remove(UriStatus status)
        {
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                if (Items[i].Status == status)
                {
                    Items.RemoveAt(i);
                }
            }
            UrlChanged?.Invoke(null, true);
        }
        public void Clear()
        {
            Items.Clear();
            UrlChanged?.Invoke(null, true);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Serializer(StreamWriter writer)
        {
            foreach (var item in Items)
            {
                writer.WriteLine($"{item.Status};{item.Type};{item.Source}");
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
                var args = line.Split(';');
                if (args.Length == 1)
                {
                    Add(line);
                    continue;
                }
                if (args.Length == 2)
                {
                    Add(args[1], UriType.Html, (UriStatus)Enum.Parse(typeof(UriStatus), args[0]));
                    continue;
                }
                Add(args[2], (UriType)Enum.Parse(typeof(UriType), args[1]), (UriStatus)Enum.Parse(typeof(UriStatus), args[0]));
            }
        }

        public IList<UriItem> GetItems(int count)
        {
            var items = new List<UriItem>();
            foreach (var item in Items)
            {
                if (item.Status == UriStatus.NONE)
                {
                    items.Add(item);
                    count--;
                    if (count < 1)
                    {
                        break;
                    }
                }
            }
            return items;
        }

        public bool HasMore { 
            get
            {
                foreach (var item in Items)
                {
                    if (item.Status == UriStatus.NONE)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void UpdateItem(int index, UriItem item)
        {
            UpdateItem(Items[index] = item);
        }

        public void UpdateItem(int index, UriStatus status)
        {
            UpdateItem(Items[index], status);
        }

        public void UpdateItem(UriItem item, UriStatus status)
        {
            item.Status = status;
            UpdateItem(item);
        }

        public void UpdateItem(UriItem item)
        {
            UrlChanged?.Invoke(item, false);
        }

        public void Reset()
        {
            foreach (var item in Items)
            {
                UpdateItem(item, UriStatus.NONE);
            }
        }
    }
}
