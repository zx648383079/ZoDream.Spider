using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.Marshal;

namespace ZoDream.Spider.Helper.Cookie
{
    /// <summary></summary>  
    /// 取得WebBrowser的完整Cookie。  
    /// 因为默认的webBrowser1.Document.Cookie取不到HttpOnly的Cookie  
    ///   
    public class FullWebBrowserCookie
    {

        [SecurityCritical]
        public static string GetCookieInternal(Uri uri, bool throwIfNoCookie)
        {
            uint pchCookieData = 0;
            string url = UriToString(uri);
            uint flag = (uint)NativeMethods.InternetFlags.INTERNET_COOKIE_HTTPONLY;

            //Gets the size of the string builder  
            if (NativeMethods.InternetGetCookieEx(url, null, null, ref pchCookieData, flag, IntPtr.Zero))
            {
                pchCookieData++;
                StringBuilder cookieData = new StringBuilder((int)pchCookieData);

                //Read the cookie  
                if (NativeMethods.InternetGetCookieEx(url, null, cookieData, ref pchCookieData, flag, IntPtr.Zero))
                {
                    DemandWebPermission(uri);
                    return cookieData.ToString();
                }
            }

            var lastErrorCode = GetLastWin32Error();

            if (throwIfNoCookie || (lastErrorCode != (int)NativeMethods.ErrorFlags.ERROR_NO_MORE_ITEMS))
            {
                throw new Win32Exception(lastErrorCode);
            }

            return null;
        }

        private static void DemandWebPermission(Uri uri)
        {
            string uriString = UriToString(uri);

            if (uri.IsFile)
            {
                string localPath = uri.LocalPath;
                new FileIOPermission(FileIOPermissionAccess.Read, localPath).Demand();
            }
            else
            {
                new WebPermission(NetworkAccess.Connect, uriString).Demand();
            }
        }

        private static string UriToString(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            UriComponents components = (uri.IsAbsoluteUri ? UriComponents.AbsoluteUri : UriComponents.SerializationInfoString);
            return new StringBuilder(uri.GetComponents(components, UriFormat.SafeUnescaped), 2083).ToString();
        }
    }
}
