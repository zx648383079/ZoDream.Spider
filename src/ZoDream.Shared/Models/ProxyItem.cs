using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Models
{
    public class ProxyItem
    {
        public string Schema { get; set; } = "http";

        public string Host { get; set; }

        public int Port { get; set; } = 80;

        public string UserName { get; set; }

        public string Password {  get; set; }
    }
}
