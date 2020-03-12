using System;
using System.IO;
using System.Net;

namespace ZoDream.Helper.Http
{
    /// <summary>
    /// 断点续传
    /// </summary>
    public class Downloader: Request
    {
        public long Total { get; set; }

        public long Rang { get; set; } = 512000;

        public long Current { get; set; }

        public string File { get; set; }

        public int BufferSize { get; set; } = 1024;

        public FileStream FStream;

        /// <summary>
        /// 下载
        /// </summary>
        public void Download()
        {
            if (FStream == null)
            {
                if (string.IsNullOrWhiteSpace(File))
                {
                    return;
                } 
                FStream = new FileStream(File, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            if (Total == 0)
            {
                Total = GetFileContentLength(Url);
            }
            if (Total > 0 && Total <= Current)
            {
                return;
            }
            var response = GetResponse(Url);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return;
            }
            var responseStream = GetStream(response);
            var bArr = new byte[BufferSize];
            if (responseStream != null)
            {
                var readSize = 0;
                var size = responseStream.Read(bArr, 0, bArr.Length);
                while (size > 0)
                {
                    FStream.Write(bArr, 0, size);
                    readSize += size;
                    size = responseStream.Read(bArr, 0, bArr.Length);
                }
                Current += readSize;
            }
            if (response.Headers["Content-Range"] == null)
            {
                Current = Total;
            }
            responseStream?.Close();
        }

        protected new void SetHeader(HttpWebRequest request)
        {
            base.SetHeader(request);
            if (Current < 0)
            {
                Current = 0;
            }

            if (Total > 0)
            {
                Rang = Total - Total;
            }
            request.AddRange("bytes", Current, Current + Rang - 1);
        }

        /// <summary>
        /// 获取下载文件长度
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public long GetFileContentLength(string url)
        {
            HttpWebRequest request = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = TimeOut;
                
                //向服务器请求，获得服务器回应数据流
                var respone = request.GetResponse();
                request.Abort();
                return respone.ContentLength;
            }
            catch (Exception)
            {
                request?.Abort();
                return 0;
            }
        }
    }
}
