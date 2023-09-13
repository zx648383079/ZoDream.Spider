using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Events;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    /// <summary>
    /// 这是转载所有网址的容器，用于验证网址是否重复
    /// </summary>
    public interface IUrlProvider: IEnumerable<UriItem>
    {

        public event UrlChangedEventHandler? UrlChanged;
        public int Count {  get; }

        public void Add(string url);

        public void Add(string url, UriType uriType);

        public void Add(IEnumerable<string> urls);

        public void Remove(string url);
        public void Remove(IEnumerable<UriItem> urls);
        public void Remove(UriCheckStatus status);
        public void Clear();

        public bool Contains(string url);

        public IList<UriItem> GetItems(int count);
        public bool HasMore { get; }

        public void UpdateItem(int index, UriItem item);

        public void UpdateItem(int index, UriCheckStatus status);

        public void UpdateItem(UriItem item, UriCheckStatus status);

        public void UpdateItem(UriItem url);
        public UriItem? Get(string url);
        public void Reset();
        
    }
}
