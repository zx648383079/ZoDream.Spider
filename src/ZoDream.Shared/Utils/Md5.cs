using System.Security.Cryptography;
using System.Text;

namespace ZoDream.Shared.Utils
{
    public static class Md5
    {
        /// <summary>
        /// MD5 编码
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Encode(string source)
        {
            var sor = Encoding.UTF8.GetBytes(source);
            var md5 = MD5.Create();
            var result = md5.ComputeHash(sor);
            md5.Dispose();
            var strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

            }
            return strbul.ToString();
        }
    }
}
