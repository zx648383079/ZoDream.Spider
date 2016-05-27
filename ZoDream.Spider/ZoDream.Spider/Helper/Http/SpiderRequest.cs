
using System.Net;
using ZoDream.Helper.Http;

namespace ZoDream.Spider.Helper.Http
{
    public class SpiderRequest: Request
    {
        public SpiderRequest():base()
        {
            load();
        }

        public SpiderRequest(string url) : base(url)
        {
            load();
        }

        protected void load()
        {
            
        }


        /// <summary>
        /// 设置请求头
        /// </summary>
        /// <param name="request">HttpWebRequest对象</param>
        protected new void setHeader(HttpWebRequest request)
        {
            request.Accept = Accept;
            request.KeepAlive = true;
            request.UserAgent = UserAgent;
            request.Referer = Referer;
            request.AllowAutoRedirect = true;
            request.ProtocolVersion = HttpVersion.Version11;
            setCookie(request);
        }
    }
}
