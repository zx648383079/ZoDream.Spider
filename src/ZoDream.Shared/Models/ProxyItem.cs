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

        public string Host { get; set; } = "localhost";

        public int Port { get; set; } = 80;

        public string? UserName { get; set; }

        public string? Password {  get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(string.IsNullOrWhiteSpace(Schema) ? "http" : Schema);
            sb.Append("://");
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                sb.Append(UserName);
                sb.Append(':');
                sb.Append(Password);
                sb.Append('@');
            }
            sb.Append(Host);
            if (Port > 0 && Port != 80)
            {
                sb.Append(':');
                sb.Append(Port);
            }
            return sb.ToString();
        }

        public ProxyItem()
        {

        }

        public ProxyItem(string url)
        {
            Parse(url);
        }

        public ProxyItem(Uri uri)
        {
            Parse(uri);
        }

        public void Parse(string url)
        {
            if (url.IndexOf("://") < 0)
            {
                url = $"http://{url}";
            }
            Parse(new Uri(url));
        }

        public void Parse(Uri uri)
        {
            if (!string.IsNullOrWhiteSpace(uri.Scheme))
            {
                Schema = uri.Scheme;
            }
            Host = uri.Host;
            Port = uri.Port;
            if (!string.IsNullOrWhiteSpace(uri.UserInfo))
            {
                var args = uri.UserInfo.Split(':');
                if (!string.IsNullOrWhiteSpace(args[0]))
                {
                    UserName = args[0];
                    Password = args[1];
                }
            }
        }
    }
}
