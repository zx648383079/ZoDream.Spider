
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using ZoDream.Spider.Model;

namespace ZoDream.Spider.Helper.Http
{
    public class SpiderRequest
    {
        public UrlTask Url { get; set; }

        public List<UrlItem> Rules { get; set; }

        public IList<HeaderItem> Headers { get; set; }

        public List<UrlTask> Results { get; set; } = new List<UrlTask>();

        public void Start()
        {
            var request = WebRequest.Create(Url.Url);
            request.Method = "GET";
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Headers = GetHeader();
            if (Regex.IsMatch(Url.Url, "^https://"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
            }
            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return;
            }
            var contentType = response.Headers[HttpRequestHeader.ContentType];
            if (contentType.IndexOf("text/html") >= 0)
            {
                var html = GetHtml(response);
                GetUrlFromHtml(ref html);
                foreach (var item in Rules)
                {
                    item.DealHtml(html);
                }
            }
            if (contentType.IndexOf("text/css") >= 0)
            {

            }

            if (contentType.IndexOf("application/javascript") >= 0)
            {

            }
            // 文件下载，断点续传
        }

        /// <summary>
        /// ssl/https请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }


        public WebHeaderCollection GetHeader()
        {
            var headers = new WebHeaderCollection();
            foreach (var item in Headers)
            {
                headers.Add(item.Name, item.Value);
            }
            return headers;
        }

        public void GetUrlFromCss(ref string html)
        {

        }

        public void GetUrlFromJs(ref string html)
        {

        }

        public void GetUrlFromHtml(ref string html)
        {
            var matches = Regex.Matches(html, @"(\<(a|img|link|script|embed|audio|object|video|param|source)[^\<\>]+(src|href|value|data)\s?=)\s?""?([^""\s\<\>]*)""?");
            foreach (Match item in matches)
            {
                var url = item.Groups[2].Value;
                if (string.IsNullOrEmpty(url) 
                    || url.IndexOf("javascript:") >= 0 || url.IndexOf("#") == 0)
                {
                    continue;
                }
                var uri = new UrlTask(url);
                html = html.Replace(item.Value, item.Value.Replace(item.Groups[2].Value, uri.RelativeUrl));  // 需要相对路径
                Results.Add(uri);
            }
        }

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public string GetHtml(WebResponse response)
        {
            var html = string.Empty;
            #region 判断解压

            if (((HttpWebResponse)response).StatusCode != HttpStatusCode.OK) return html;
            Stream stream = null;
            stream = ((HttpWebResponse)response).ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase) ? new GZipStream(response.GetResponseStream(), mode: CompressionMode.Decompress) : response.GetResponseStream();
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

            #endregion
            return html;
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
