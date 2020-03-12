using System.Net;

namespace ZoDream.Helper.Http
{
    /// <summary>  
    /// Http返回参数类  
    /// </summary>  
    public class HttpResult
    {
        /// <summary>  
        /// Http请求返回的Cookie  
        /// </summary>  
        public string Cookie { get; set; }

        /// <summary>  
        /// Cookie对象集合  
        /// </summary>  
        public CookieCollection CookieCollection { get; set; }

        /// <summary>  
        /// 返回的String类型数据 只有ResultType.String时才返回数据，其它情况为空  
        /// </summary>  
        public string Html { get; set; } = string.Empty;

        /// <summary>  
        /// 返回的Byte数组 只有ResultType.Byte时才返回数据，其它情况为空  
        /// </summary>  
        public byte[] ResultByte { get; set; }

        /// <summary>  
        /// header对象  
        /// </summary>  
        public WebHeaderCollection Header { get; set; }

        /// <summary>  
        /// 返回状态说明  
        /// </summary>  
        public string StatusDescription { get; set; }

        /// <summary>  
        /// 返回状态码,默认为OK  
        /// </summary>  
        public HttpStatusCode StatusCode { get; set; }
    }
}
