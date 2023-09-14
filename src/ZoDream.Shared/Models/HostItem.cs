using System;
using System.Collections.Generic;
using System.Text;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Shared.Models
{
    public class HostItem
    {
        public string Host { get; set; }

        public string Ip { get; set; }

        public HostItem(string host, string ip)
        {
            Host = host;
            Ip = ip;
        }
    }

    public class HostBindingItem: BindableBase
    {
        public string Host { get; set; }

        private string ip = string.Empty;

        public string Ip {
            get => ip;
            set => Set(ref ip, value);
        }


        public HostBindingItem(string host, string ip)
        {
            Host = host;
            Ip = ip;
        }
    }
}
