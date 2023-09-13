using System.Collections;
using System.Collections.Generic;
using ZoDream.Shared.Events;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

namespace ZoDream.Spider.Providers
{
    public class UrlProvider : IUrlProvider
    {
        public UrlProvider(ISpider spider)
        {
            Application = spider;
            foreach (var item in spider.Project.EntryItems)
            {
                foreach (var url in Html.GenerateUrl(item))
                {
                    Add(url);
                }
            }
        }

        public ISpider Application { get; set; }

        public List<UriItem> Items { get; private set; } = new();

        public event UrlChangedEventHandler? UrlChanged;

        public int Count {  get {  return Items.Count; }  }

        public void Add(string url)
        {
            Add(url, UriType.Html);
        }

        public void Add(string url, UriType uriType)
        {
            Add(url, uriType, UriCheckStatus.None);
        }

        public void Add(string url, UriType uriType, UriCheckStatus status)
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
            return Items.GetEnumerator();
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
        public void Remove(UriCheckStatus status)
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
            return Items.GetEnumerator();
        }

        public IList<UriItem> GetItems(int count)
        {
            var items = new List<UriItem>();
            foreach (var item in Items)
            {
                if (item.Status == UriCheckStatus.None)
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
                    if (item.Status == UriCheckStatus.None)
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

        public void UpdateItem(int index, UriCheckStatus status)
        {
            UpdateItem(Items[index], status);
        }

        public void UpdateItem(UriItem item, UriCheckStatus status)
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
                UpdateItem(item, UriCheckStatus.None);
            }
        }

    }
}
