using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Http;
using ZoDream.Shared.Models;
using ZoDream.Shared.Interfaces;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZoDream.Shared.Http
{
    public class Client: IHttpClient
    {

        public const string ContentTypeKey = "Content-Type";
        public string Url { get; set; } = string.Empty;

        public string Method { get; set; } = "GET";

        public bool AllowAutoRedirect { get; set; } = true;

        public ProxyItem? Proxy { get; set; }

        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// 毫秒为单位
        /// </summary>
        public int TimeOut { get; set; } = 5 * 1000;

        public int MaxRetries { get; set; } = 1;
        /// <summary>
        /// 重试间隔时间(/s)
        /// </summary>
        public int RetryTime { get; set; } = 0;

        public Client()
        {

        }

        public Client(string url)
        {
            Url = url;
        }

        public HttpClient PrepareClient()
        {
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = AllowAutoRedirect,
                UseCookies = true,
                CookieContainer = new CookieContainer(),
                ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
            };
            //if (Cookies != null && Cookies.Count > 0)
            //{   
            //    handler.CookieContainer.Add(Cookies);
            //}
            var uri = new Uri(Url);
            if (Headers.ContainsKey("Cookie"))
            {
                foreach (var item in Headers["Cookie"].Split(';'))
                {
                    handler.CookieContainer.SetCookies(uri, item);
                }
            }
            if (Proxy != null)
            {
                // handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls;
                handler.Proxy = new WebProxy()
                {
                    Address = new Uri($"{Proxy.Schema}://{Proxy.Host}:{Proxy.Port}"),
                    UseDefaultCredentials = string.IsNullOrWhiteSpace(Proxy.UserName),
                    Credentials = string.IsNullOrWhiteSpace(Proxy.UserName) ? null : new NetworkCredential(
                    userName: Proxy.UserName,
                    password: Proxy.Password)
                };
            }
            HttpClient client;
            if (MaxRetries > 1)
            {
                client = new HttpClient(new HttpRetryHandler(handler, MaxRetries, RetryTime));
            } else
            {
                client = new HttpClient(handler);
            }
            client.Timeout = TimeSpan.FromMilliseconds(TimeOut);
            return client;
        }

        public HttpRequestMessage PrepareRequest()
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(Url),
                Method = FormatMethod()
            };
            foreach (var item in Headers)
            {
                request.Headers.TryAddWithoutValidation(item.Key, item.Value);
            }
            if (Method == "POST")
            {
                if (Headers.ContainsKey(ContentTypeKey))
                {
                    request.Content = new StringContent(Body, Encoding.UTF8, Headers[ContentTypeKey]);
                } else
                {
                    request.Content = new StringContent(Body);
                }
            }
            return request;
        }

        private HttpMethod FormatMethod()
        {
            switch (Method.ToUpper())
            {
                case "POST":
                    return HttpMethod.Post;
                case "PUT":
                    return HttpMethod.Put;
                case "DELETE":
                    return HttpMethod.Delete;
                case "HEAD":
                    return HttpMethod.Head;
                case "OPTIONS":
                    return HttpMethod.Options;
                case "TRACE":
                    return HttpMethod.Trace;
                default:
                    return HttpMethod.Get;
            }
        }


        private string ReadCharset(HttpResponseMessage response)
        {
            var items = response.Content.Headers.GetValues("Content-Type");
            foreach (var item in items)
            {
                var i = item.IndexOf("charset=");
                if (i < 0)
                {
                    continue;
                }
                return item.Substring(i + 7);
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取HTML网页的编码
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="charSet"></param>
        /// <returns></returns>
        public Encoding GetEncoding(byte[] bytes, string charSet)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var html = Encoding.Default.GetString(bytes);
            var regCharset = new Regex(@"charset\b\s*=\s*""*(?<charset>[^""]*)");
            if (regCharset.IsMatch(html))
            {
                return Encoding.GetEncoding(regCharset.Match(html).Groups["charset"].Value);
            }
            return charSet != string.Empty ? Encoding.GetEncoding(charSet) : Encoding.Default;
        }

        public Task<string?> GetAsync(string url)
        {
            Url = url;
            return ReadAsync();
        }

        public Task<string?> PostAsync(string url, string data)
        {
            Url = url;
            Body = data;
            Method = "POST";
            return ReadAsync();
        }

        public Task<string?> PostAsync(string url, string data, string contentType)
        {
            Headers.Add(ContentTypeKey, contentType);
            return PostAsync(url, data);
        }

        public async Task<string?> ReadAsync()
        {
            return await ReadAsync(ReadAsync);
        }

        public async Task<T?> ReadAsync<T>(Func<HttpResponseMessage, Task<T?>> func)
        {
            using (var request = PrepareRequest())
            using (var client = PrepareClient())
            using (var response = await client.SendAsync(request))
            {
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    return default;
                }
                return await func.Invoke(response);
            }
        } 

        public async Task<string?> ReadAsync(HttpResponseMessage response)
        {
            #region 判断解压
            using var stream = await ReadStreamAsync(response);
            #region 把网络流转成内存流
            var ms = new MemoryStream();
            var buffer = new byte[1024];
            while (true)
            {
                if (stream == null) continue;
                var sz = stream.Read(buffer, 0, 1024);
                if (sz == 0) break;
                ms.Write(buffer, 0, sz);
            }
            #endregion
            var bytes = ms.ToArray();
            var html = GetEncoding(bytes, ReadCharset(response)).GetString(bytes);
            stream.Close();
            #endregion
            return html;
        }

        public async Task<T?> ReadJsonAsync<T>()
        {
            var content = await ReadAsync();
            if (content == null)
            {
                return default(T);
            }
            if (typeof(T) == typeof(string))
            {
                return (T)(object)content;
            }
            try
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception)
            {
            }
            return default(T);
        }


        public async Task<HttpResponseMessage?> ReadResponseAsync()
        {
            using var request = PrepareRequest();
            using var client = PrepareClient();
            return await client.SendAsync(request);
        }

        public async Task<Stream?> ReadStreamAsync()
        {
            using var response = await ReadResponseAsync();
            return await ReadStreamAsync(response);
        }

        public async Task<Stream?> ReadStreamAsync(HttpResponseMessage? response)
        {
            if (response == null)
            {
                return null;
            }
            if (response.Content.Headers.ContentEncoding.Contains("gzip"))
            {
                return new GZipStream(await response.Content.ReadAsStreamAsync(), mode: CompressionMode.Decompress);
            }
            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<long> GetLengthAsync()
        {
            try
            {
                using (var response = await ReadResponseAsync())
                {
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                    {
                        return 0;
                    }
                    var length = response.Content.Headers.ContentLength;
                    if (length == null)
                    {
                        return 0;
                    }
                    return (long)length;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<bool> SaveAsync(string file)
        {
            using (var responseStream = await ReadStreamAsync())
            {
                if (responseStream == null)
                {
                    return false;
                }
                using var stream = new FileStream(file, FileMode.Create);
                var bArr = new byte[1024];
                if (responseStream != null)
                {
                    var size = responseStream.Read(bArr, 0, bArr.Length);
                    while (size > 0)
                    {
                        stream.Write(bArr, 0, size);
                        size = responseStream.Read(bArr, 0, bArr.Length);
                    }
                }
            }
            return true;
        }


        public async Task<bool> SaveAsync(string file, long current, long maxSize = 512000)
        {
            try
            {
                using (var request = PrepareRequest())
                {
                    request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(current, current + maxSize - 1);
                    using (var client = PrepareClient())
                    using (var response = await client.SendAsync(request))
                    {
                        var responseStream = await ReadStreamAsync(response);
                        if (responseStream is null)
                        {
                            return false;
                        }
                        //创建本地文件写入流
                        var stream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        var bArr = new byte[1024];
                        if (responseStream != null)
                        {
                            var size = responseStream.Read(bArr, 0, bArr.Length);
                            while (size > 0)
                            {
                                stream.Write(bArr, 0, size);
                                size = responseStream.Read(bArr, 0, bArr.Length);
                            }
                        }
                        stream.Close();
                        responseStream?.Close();
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<MemoryStream?> ReadMemoryStreamAsync(HttpResponseMessage? response)
        {

            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            #region 判断解压
            var stream = await ReadStreamAsync(response);
            #endregion
            #region 把网络流转成内存流
            var ms = new MemoryStream();
            var buffer = new byte[1024];
            while (true)
            {
                if (stream == null) continue;
                var sz = stream.Read(buffer, 0, 1024);
                if (sz == 0) break;
                ms.Write(buffer, 0, sz);
            }
            stream.Close();
            #endregion
            return ms;
        }

        public async Task<MemoryStream?> ReadMemoryStreamAsync()
        {
            using (var response = await ReadResponseAsync())
            {
                return await ReadMemoryStreamAsync(response);
            }
        }
    }
}
