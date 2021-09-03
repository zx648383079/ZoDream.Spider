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
using System.Reflection.Metadata;
using static System.Net.WebRequestMethods;

namespace ZoDream.Shared.Http
{
    public class Client
    {
        public string Url { get; set; } = string.Empty;

        public string Accept { get; set; } = HttpAccept.Html;

        public string UserAgent { get; set; } = HttpUserAgent.Firefox;

        public string Referer { get; set; } = "";

        public bool KeepAlive { get; set; } = true;

        public string Method { get; set; } = "GET";

        public bool AllowAutoRedirect { get; set; } = true;

        public ProxyItem? Proxy { get; set; }

        public IList<HeaderItem> Headers { get; set; } = new List<HeaderItem>() {
            new HeaderItem("Accept-Encoding", "gzip, deflate"),
            new HeaderItem("Accept-Language", "zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3"),
            new HeaderItem("Cache-Control", "max-age=0"),
        };

        /// <summary>
        /// 毫秒为单位
        /// </summary>
        public int TimeOut { get; set; } = 5 * 1000;

        public int ReadWriteTimeOut = 2 * 1000;

        public CookieCollection? Cookies { get; set; }

        public Client()
        {

        }

        public Client(string url)
        {
            Url = url;
        }

        public string? Get(string url)
        {
            Url = url;
            return Get();
        }

        public string? Get()
        {
            Method = "GET";
            return ReadAsString(PrepareRequest());
        }

        public string? Post(string url, IDictionary<string, string> param)
        {
            Url = url;
            return Post(param);
        }

        public string? Post(IDictionary<string, string> param)
        {
            var request = PrepareRequest();
            ReadyPost(request, param);
            return ReadAsString(request);
        }


        public string? Post(string param)
        {
            return Post(param, "application/x-www-form-urlencoded");
        }

        public string? Post(string param, string contentType)
        {
            var request = PrepareRequest();
            ReadyPost(request, param, contentType);
            return ReadAsString(request);
        }


        private void ReadyPost(HttpRequestMessage request, IDictionary<string, string> param)
        {
            Method = "POST";
            request.Content = new FormUrlEncodedContent(param);
        }

        private void ReadyPost(HttpRequestMessage request, string args)
        {
            ReadyPost(request, Encoding.UTF8.GetBytes(args));
        }

        private void ReadyPost(HttpRequestMessage request, string args, string contentType)
        {
            Method = "POST";
            if (contentType.IndexOf("json") > 0)
            {
                request.Content = new StringContent(args);
                return;
            }
            request.Content = new StringContent(args, Encoding.UTF8, contentType);
        }

        private void ReadyPost(HttpRequestMessage request, byte[] args)
        {
            ReadyPost(request, args, "application/x-www-form-urlencoded");
        }

        private void ReadyPost(HttpRequestMessage request, byte[] args, string contentType)
        {
            ReadyPost(request, Encoding.UTF8.GetString(args), contentType);
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
            if (Cookies != null && Cookies.Count > 0)
            {   
                handler.CookieContainer.Add(Cookies);
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
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMilliseconds(TimeOut)
            };
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
                request.Headers.TryAddWithoutValidation(item.Name, item.Value);
            }
            request.Headers.TryAddWithoutValidation("Accept", Accept);
            request.Headers.TryAddWithoutValidation("User-Agent", UserAgent);
            request.Headers.TryAddWithoutValidation("Referer", Referer);
            request.Headers.TryAddWithoutValidation("Referer", Referer);
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
                case "PATCH":
                    return HttpMethod.Patch;
                case "TRACE":
                    return HttpMethod.Trace;
                default:
                    return HttpMethod.Get;
            }
        }

        /// <summary>
        /// 返回响应报文
        /// </summary>
        /// <param name="request">WebRequest对象</param>
        /// <returns>响应对象</returns>
        public string? ReadAsString(HttpRequestMessage request)
        {
            try
            {
                string html;
                using (var client = PrepareClient())
                using (var response = client.Send(request))
                {
                    html = ReadAsString(response);
                }
                return html;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public HttpResponseMessage? Read()
        {
            try
            {
                using (var request = PrepareRequest())
                using (var client = PrepareClient())
                using (var response = client.Send(request))
                {
                    return response;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public HttpResponseMessage? Read(string url)
        {
            Url = url;
            return Read();
        }


        public MemoryStream? ReadAsMemoryStream(HttpResponseMessage response)
        {

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            #region 判断解压
            var stream = ReadAsStream(response);
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

        public void ReadAsFile(string url, string file)
        {
            var response = Read(url);
            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                return;
            }
            ReadAsFile(response, file);
        }

        public void ReadAsFile(HttpResponseMessage response, string file)
        {
            var responseStream = ReadAsStream(response);
            //创建本地文件写入流
            var stream = new FileStream(file, FileMode.Create);
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
        /// <summary>
        /// 获取一段文件，断点续传用，请先使用 ReadContentLength() 获取内容大小，进行判断
        /// </summary>
        /// <param name="file"></param>
        /// <param name="current"></param>
        /// <param name="maxSize"></param>
        public void ReadAsFile(string file, long current, long maxSize = 512000)
        {
            try
            {
                using (var request = PrepareRequest())
                {
                    request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(current, current + maxSize - 1);
                    using (var client = PrepareClient())
                    using (var response = client.Send(request))
                    {
                        var responseStream = ReadAsStream(response);
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
                return;
            }
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <returns></returns>
        public long ReadContentLength()
        {
            try
            {
                using (var request = PrepareRequest())
                using (var client = PrepareClient())
                using (var response = client.Send(request))
                {
                    if (response.StatusCode != HttpStatusCode.OK)
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

        public string ReadAsString(HttpResponseMessage response)
        {
            var html = string.Empty;
            #region 判断解压
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return html;
            }
            var stream = ReadAsStream(response);
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
            html = GetEncoding(bytes, ReadCharset(response)).GetString(bytes);
            stream.Close();
            response.Dispose();
            #endregion
            return html;
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
        /// 获取响应流
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public Stream ReadAsStream(HttpResponseMessage response)
        {
            if (response.Content.Headers.ContentEncoding.Contains("gzip"))
            {
                return new GZipStream(response.Content.ReadAsStream(), mode: CompressionMode.Decompress);
            }
            return response.Content.ReadAsStream();
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
    }
}
