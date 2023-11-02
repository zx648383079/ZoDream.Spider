using System;
using System.Collections.Generic;
using ZoDream.Shared.Utils;

namespace ZoDream.Shared.Models
{
    public class RequestData
    {
        public string Url { get; set; }

        public string Method { get; set; } = "GET";
        /// <summary>
        /// 是否允许重定向
        /// </summary>
        public bool AllowAutoRedirect { get; set; } = true;

        public IList<HeaderItem>? Headers { get; set; }

        public ProxyItem? Proxy { get; set; }

        public HostItem? HostMap { get; set; }
        /// <summary>
        /// 请求超时/s
        /// </summary>
        public int Timeout { get; set; } = 10;
        /// <summary>
        /// 重试间隔时间(/s)
        /// </summary>
        public int RetryTime { get; set; } = 0;
        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; set; } = 1;
        /// <summary>
        /// 获取真实的网址
        /// </summary>
        public string RealUrl => GetRequestUrl(Url);
        /// <summary>
        /// 获取实际请求的地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetRequestUrl(string url)
        {
            return HostMap is null ? url : Html.ReplaceHost(url, HostMap.Host, HostMap.Ip);
        }
        /// <summary>
        /// 获取实际请求的地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetRequestUrl(Uri url)
        {
            return HostMap is null || url.Host != HostMap.Host ? url.ToString() : Html.ReplaceHost(url.ToString(), HostMap.Host, HostMap.Ip);
        }

        /// <summary>
        /// 显示虚假的网址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetSourceUrl(string url)
        {
            return HostMap is null ? url : Html.ReplaceHost(url, HostMap.Ip, HostMap.Host);
        }
        /// <summary>
        /// 显示虚假的网址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetSourceUrl(Uri url)
        {
            return HostMap is null || url.Host != HostMap.Ip ? url.ToString() : Html.ReplaceHost(url.ToString(), HostMap.Ip, HostMap.Host);
        }

        public RequestData(string url)
        {
            Url = url;
        }

        public RequestData(string url, IList<HeaderItem> headers, 
            ProxyItem? proxy, HostItem? host = null): this(url)
        {
            Headers = headers;
            Proxy = proxy;
            HostMap = host;
        }
    }
}
