using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ZoDream.Helper.Local
{
    public class FileHelper
    {
        public string FileName { get; set; }

        public FileHelper()
        {

        }

        public FileHelper(string file)
        {
            FileName = file;
        }

        public string GetMd5()
        {
            if (!File.Exists(FileName))
            {
                return string.Empty;
            }
            try
            {
                byte[] buffers;
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    using (var fs = new FileStream(FileName, FileMode.Open))
                    {
                        buffers = md5.ComputeHash(fs);
                    }
                }
                var sb = new StringBuilder();
                foreach (var item in buffers)
                {
                    sb.Append(item.ToString("X2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string GetSha1()
        {
            if (!File.Exists(FileName))
            {
                return string.Empty;
            }
            byte[] buffers;
            using (var hash = new SHA1Managed()) // 创建Hash算法对象
            {
                using (var fs = new FileStream(FileName, FileMode.Open)) // 创建文件流对象
                {
                    buffers = hash.ComputeHash(fs); // 计算   
                }
            }
            var sb = new StringBuilder();
            foreach (var item in buffers)
            {
                sb.Append(item.ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string MakeRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath))
            {
                return toPath;
            };
            if (string.IsNullOrEmpty(toPath))
            {
                return "";
            };

            var fromUri = new Uri(fromPath);
            var toUri = new Uri(toPath);

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            return relativePath.Replace('/', Path.DirectorySeparatorChar);
        }

        public static string GetRelativePaths(string path, string current)
        {
            var a = current.ToLower();
            var b = path.ToLower();
            var i = 0;
            for (; i < Math.Min(a.Length, b.Length); i++)
            {
                if (a[i] != b[i]) break;
            }
            var cur = Regex.Replace(a.Substring(i - 1), @"\\?[a-zA-Z]+:?", @"..\");
            return (cur + path.Substring(i)).Replace(@"\\", @"\");
        }

        public static void CreateDirectory(string filefullpath)
        {
            if (File.Exists(filefullpath))
            {
                return;
            }
             //判断路径中的文件夹是否存在
            var dirpath = filefullpath.Substring(0, filefullpath.LastIndexOf('\\'));
            var pathes = dirpath.Split('\\');
            if (pathes.Length <= 1) return;
            var path = pathes[0];
            for (var i = 1; i < pathes.Length; i++)
            {
                path += "\\" + pathes[i];
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }
    }
}
