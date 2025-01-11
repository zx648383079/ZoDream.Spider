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
            SyncEntry();
        }

        private readonly ISpider Application;

        public List<UriItem> Items { get; private set; } = new();

        public event UrlChangedEventHandler? UrlChanged;
        public event ProgressEventHandler? ProgressChanged;

        public int Count => Items.Count;

        public void SyncEntry()
        {
            foreach (var item in Application.Project.EntryItems)
            {
                foreach (var url in Html.GenerateUrl(item))
                {
                    Add(url, UriType.Html);
                }
            }
        }

        public void Add(string url)
        {
            Add(url, UriType.Html);
        }

        public void Add(string url, UriType uriType)
        {
            Add(url, uriType, UriCheckStatus.None);
        }

        public void Add(int level, string url, UriType uriType)
        {
            Add(level, url, uriType, UriCheckStatus.None);
        }

        public void Add(string url, UriType uriType, UriCheckStatus status)
        {
            Add(0, url, uriType, status);
        }

        public void Add(int level, string url, UriType uriType, UriCheckStatus status)
        {
            if (Contains(url))
            {
                return;
            }
            AddUrl(level, url, uriType, status);
        }

        private UriItem AddUrl(int level, string url, UriType uriType, UriCheckStatus status)
        {
            var item = new UriItem() { Source = url, Type = uriType, Status = status, Level = level };
            Items.Add(item);
            UrlChanged?.Invoke(new UrlChangedEventArgs(item, true));
            return item;
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

        public UriItem? TryAdd(string url, UriType uriType)
        {
            var item = Get(url);
            if (item is not null)
            {
                return item;
            }
            return AddUrl(0, url, uriType, UriCheckStatus.Doing);
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
            UrlChanged?.Invoke(new UrlChangedEventArgs(true));
        }

        public void Remove(IEnumerable<UriItem> urls)
        {
            foreach (var item in urls)
            {
                Items.Remove(item);
            }
            UrlChanged?.Invoke(new UrlChangedEventArgs(true));
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
            UrlChanged?.Invoke(new UrlChangedEventArgs(true));
        }
        public void Clear()
        {
            Items.Clear();
            UrlChanged?.Invoke(new UrlChangedEventArgs(true));
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

        public void EmitUpdate(int index, UriItem item)
        {
            EmitUpdate(Items[index] = item);
        }

        public void EmitUpdate(int index, UriCheckStatus status)
        {
            EmitUpdate(Items[index], status);
        }

        public void EmitUpdate(UriItem item, UriCheckStatus status)
        {
            item.Status = status;
            EmitUpdate(item);
        }

        public void EmitUpdate(UriItem item, UriCheckStatus status, string message)
        {
            item.Status = status;
            UrlChanged?.Invoke(new UrlChangedEventArgs(item, message));
        }

        public void EmitUpdate(UriItem item)
        {
            UrlChanged?.Invoke(new UrlChangedEventArgs(item));
        }

        public void EmitProgress(UriItem url, int groupIndex, int groupCount)
        {
            url.Progress.UpdateGroup(groupIndex, groupCount);
            ProgressChanged?.Invoke(url);
        }
        public void EmitProgress(UriItem url, long index, long count, bool isStep)
        {
            if (isStep)
            {
                url.Progress.UpdateStep(index, count);
            }
            else
            {
                url.Progress.UpdateRule((int)index, (int)count);
            }
            ProgressChanged?.Invoke(url);
        }

        public void Reset()
        {
            foreach (var item in Items)
            {
                EmitUpdate(item, UriCheckStatus.None);
            }
        }

    }
}
