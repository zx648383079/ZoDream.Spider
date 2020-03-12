using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace ZoDream.Helper.Http
{
    public class Request
    {
        public string Url { get; set; }

        public string Accept { get; set; } = Accepts.Html;

        public string UserAgent { get; set; } = UserAgents.Firefox;

        public string Referer { get; set; } = "";

        public bool KeepAlive { get; set; } = true;

        public string Method { get; set; } = "GET";

        public bool AllowAutoRedirect { get; set; } = true;

        public Version ProtocolVersion { get; set; } = HttpVersion.Version11;

        public WebHeaderCollection HeaderCollection { get; set; } = new WebHeaderCollection
            {
                {HttpRequestHeader.AcceptEncoding, "gzip, deflate"},
                {HttpRequestHeader.AcceptLanguage, "zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3"},
                {HttpRequestHeader.CacheControl, "max-age=0"}
            };

        /// <summary>
        /// 毫秒为单位
        /// </summary>
        public int TimeOut { get; set; } = 5 * 1000;

        public int ReadWriteTimeOut = 2 * 1000;

        public CookieCollection Cookies { get; set; }

        public Request()
        {
            
        }

        public Request(string url)
        {
            Url = url;
        }

        public string Get(string url)
        {
            Url = url;
            return Get();
        }

        public string Get()
        {
            var request = GetRequest();
            Method = "GET";
            _setProperty(request);
            return GetHtml(request);
        }

        public string Post(string url, IDictionary<string, string> param)
        {
            Url = url;
            return Post(param);
        }

        public string Post(IDictionary<string, string> param)
        {
            var request = GetRequest();
            _post(request, param);
            _setProperty(request);
            return GetHtml(request);
        }

        public string Post(string param)
        {
            var request = GetRequest();
            _post(request, param);
            _setProperty(request);
            return GetHtml(request);
        }

        public WebRequest GetRequest()
        {
            var request = WebRequest.Create(Url);
            request.Credentials = CredentialCache.DefaultCredentials;
            return request;
        }

        private void _setProperty(WebRequest request)
        {
            request.Method = Method;
            SetHeader((HttpWebRequest) request);
            if (!Regex.IsMatch(Url, "^https://")) return;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
        }

        public WebRequest GetRequest(string url)
        {
            Url = url;
            return GetRequest();
        }

        /// <summary>
        /// ssl/https请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        /// <summary>
        /// 设置请求头
        /// </summary>
        /// <param name="request">HttpWebRequest对象</param>
        protected void SetHeader(HttpWebRequest request)
        {
            request.Timeout = TimeOut;
            request.ReadWriteTimeout = ReadWriteTimeOut;
            request.Accept = Accept;
            request.KeepAlive = KeepAlive;
            request.UserAgent = UserAgent;
            request.Referer = Referer;
            request.AllowAutoRedirect = AllowAutoRedirect;
            request.ProtocolVersion = ProtocolVersion;
            request.Headers = HeaderCollection;
            SetCookie(request);
        }

        /// <summary>
        /// 设置请求Cookie
        /// </summary>
        /// <param name="request">HttpWebRequest对象</param>
        protected void SetCookie(HttpWebRequest request)
        {
            // 必须实例化，否则响应中获取不到Cookie
            request.CookieContainer = new CookieContainer();

            if (Cookies != null && Cookies.Count > 0)
            {
                request.CookieContainer.Add(Cookies);
            }
        }

        private void _post(WebRequest request, IDictionary<string, string> param)
        {
            var data = new StringBuilder(string.Empty);
            foreach (var keyValuePair in param)
            {
                data.AppendFormat("{0}={1}&", keyValuePair.Key, keyValuePair.Value);
            }
            _post(request, data.Remove(data.Length - 1, 1).ToString());
        }


        private void _post(WebRequest request, string args)
        {
            _post(request, Encoding.UTF8.GetBytes(args));
        }

        private void _post(WebRequest request, byte[] args)
        {
            Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = args.Length;
            var stream = request.GetRequestStream();
            stream.Write(args, 0, args.Length);
            stream.Close();
        }

        /// <summary>
        /// 返回响应报文
        /// </summary>
        /// <param name="request">WebRequest对象</param>
        /// <returns>响应对象</returns>
        public string GetHtml(WebRequest request)
        {
            string html;
            using (var response = request.GetResponse())
            {
                html = GetHtml(response);
            }
            return html;
        }

        public HttpWebResponse GetResponse()
        {
            var request = GetRequest();
            _setProperty(request);
            return (HttpWebResponse)request.GetResponse();
        }

        public HttpWebResponse GetResponse(string url)
        {
            var request = GetRequest(url);
            _setProperty(request);
            return (HttpWebResponse)request.GetResponse();
        }


        public MemoryStream GetMemoryStream(WebResponse response)
        {

            if (((HttpWebResponse)response).StatusCode != HttpStatusCode.OK) return null;
            #region 判断解压

            var stream = GetStream(response);
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

        public void Download(string url, string file)
        {
            var response = GetResponse(url);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return;
            }
            Download(response, file);
        }

        public void Download(WebResponse response, string file)
        {
            var responseStream = GetStream(response);
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

        public string GetHtml(WebResponse response)
        {
            var html = string.Empty;
            #region 判断解压

            if (((HttpWebResponse) response).StatusCode != HttpStatusCode.OK) return html;
            var stream = GetStream(response);
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
            html = GetEncoding(bytes, ((HttpWebResponse)response).CharacterSet).GetString(bytes);
            stream.Close();
            response.Close();
            #endregion
            return html;
        }

        /// <summary>
        /// 获取响应流
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public Stream GetStream(WebResponse response)
        {
            return ((HttpWebResponse)response).ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase) ? new GZipStream(response.GetResponseStream(), mode: CompressionMode.Decompress) : response.GetResponseStream();
        }



        /// <summary>
        /// 获取HTML网页的编码
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="charSet"></param>
        /// <returns></returns>
        public Encoding GetEncoding(byte[] bytes, string charSet)
        {
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
