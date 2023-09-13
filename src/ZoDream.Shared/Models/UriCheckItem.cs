using System;
using System.Collections.Generic;
using System.Text;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Shared.Models
{
    public class UrlCheckItem: BindableBase
    {
        public string Url { get; set; }

        private UriCheckStatus status = UriCheckStatus.None;

        public UriCheckStatus Status {
            get => status;
            set => Set(ref status, value);
        }

        public UrlCheckItem(string url)
        {
            Url = url;
        }

    }
}
