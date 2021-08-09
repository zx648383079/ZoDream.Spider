using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Http
{
    public class HttpAccept
    {
        /// <summary>
        /// html
        /// </summary>
        public const string Html = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";

        /// <summary>
        /// 图片
        /// </summary>
        public const string Image = "image/png,image/*;q=0.8,*/*;q=0.5";

        /// <summary>
        /// css
        /// </summary>
        public const string Css = "text/css,*/*;q=0.1";

        /// <summary>
        /// js
        /// </summary>
        public const string Js = "*/*";
    }
}
