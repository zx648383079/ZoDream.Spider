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
        public event ProgressEventHandler? ProgressChanged;
        public int Count {  get; }
        /// <summary>
        /// 同步项目添加的入口
        /// </summary>
        public void SyncEntry();

        public void Add(string url);

        public void Add(string url, UriType uriType);
        public void Add(int level, string url, UriType uriType);

        public void Add(IEnumerable<string> urls);

        public void Remove(string url);
        public void Remove(IEnumerable<UriItem> urls);
        public void Remove(UriCheckStatus status);
        public void Clear();

        public bool Contains(string url);

        public IList<UriItem> GetItems(int count);
        public bool HasMore { get; }

        public void EmitUpdate(int index, UriItem item);

        public void EmitUpdate(int index, UriCheckStatus status);

        public void EmitUpdate(UriItem item, UriCheckStatus status);

        public void EmitUpdate(UriItem url);
        public void EmitProgress(UriItem url, int groupIndex, int groupCount);
        public void EmitProgress(UriItem url, long index, long count, bool isStep);
        public UriItem? Get(string url);
        public void Reset();
        
    }
}
