using System;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Shared.Models
{
    public class UriItem
    {
        public string Title { get; set; } = string.Empty;

        public string Source { get; set; } = string.Empty;
        public UriCheckStatus Status { get; set; } = UriCheckStatus.None;

        public UriType Type { get; set; } = UriType.File;

        public UriProgress Progress { get; set; } = new();

        public UriItem()
        {
            
        }

        public UriItem(UriLoadItem uri)
        {
            Title = uri.Title;
            Source = uri.Source;
            Status = uri.Status;
            Type = uri.Type;
        }
    }
}
