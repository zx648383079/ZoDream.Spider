using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    public interface IHttpClient
    {

        public string Url { get; set; }

        public string Method { get; set; }

        public ProxyItem? Proxy { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public string Body { get; set; }

        /// <summary>
        /// 毫秒为单位
        /// </summary>
        public int TimeOut { get; set; }


        public Task<string?> GetAsync(string url);

        public Task<string?> PostAsync(string url, string data);


        public Task<string?> ReadAsync();

        public Task<T?> ReadJsonAsync<T>();

        public Task<Stream?> ReadStreamAsync();

        public Task<HttpResponseMessage?> ReadResponseAsync();

        public Task<long> GetLengthAsync();

        public Task<bool> SaveAsync(string file);
        /// <summary>
        /// 获取一段文件，断点续传用，请先使用 GetLengthAsync() 获取内容大小，进行判断
        /// </summary>
        /// <param name="file"></param>
        /// <param name="current"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public Task<bool> SaveAsync(string file, long current, long maxSize = 512000);



    }
}
