using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Utils;

namespace ZoDream.Shared.Storage
{
    public static class LocationStorage
    {
        /// <summary>
        /// 读文本文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<string> ReadAsync(string file)
        {
            if (!File.Exists(file))
            {
                return string.Empty;
            }
            var fs = new FileStream(file, FileMode.Open);
            using var reader = new StreamReader(fs, TxtEncoder.GetEncoding(fs));
            var content = await reader.ReadToEndAsync();
            return content;
        }

        public static StreamReader Reader(string file)
        {
            var fs = new FileStream(file, FileMode.Open);
            return new StreamReader(fs, TxtEncoder.GetEncoding(fs));
        }

        public static async Task<string> ReadAsync(Stream fs)
        {
            using var reader = new StreamReader(fs, TxtEncoder.GetEncoding(fs));
            var content = await reader.ReadToEndAsync();
            return content;
        }

        /// <summary>
        /// 写文本文件 默认使用无 bom 的UTF8编码
        /// </summary>
        /// <param name="file"></param>
        /// <param name="content"></param>
        public static async Task WriteAsync(string file, string content)
        {
            await WriteAsync(file, content, new UTF8Encoding(false));
        }

        public static async Task WriteAsync(string file, string content, bool append)
        {
            await WriteAsync(file, content, new UTF8Encoding(false), append);
        }

        public static async Task WriteAsync(string file, string content, string encoding)
        {
            await WriteAsync(file, content, Encoding.GetEncoding(encoding));
        }

        public static async Task WriteAsync(string file, string content, Encoding encoding)
        {
            await WriteAsync(file, content, encoding, false);
        }

        public static async Task WriteAsync(string file, string content, Encoding encoding, bool append)
        {
            using var writer = new StreamWriter(file, append, encoding);
            await writer.WriteAsync(content);
        }

        public static StreamWriter Writer(string file)
        {
            return Writer(file, false);
        }

        public static StreamWriter Writer(string file, bool append)
        {
            FileStream fs;
            if (!append)
            {
                fs = new FileStream(file, FileMode.Create, FileAccess.ReadWrite);
                return new StreamWriter(fs, Encoding.UTF8);
            }
            fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            var encoding = TxtEncoder.GetEncoding(fs);
            fs.Seek(0, SeekOrigin.End);
            return new StreamWriter(fs, encoding);
        }

        public static StreamWriter Writer(Stream stream, bool append)
        {
            if (!append) { 
                return new StreamWriter(stream, Encoding.UTF8);
            }
            var encoding = TxtEncoder.GetEncoding(stream);
            stream.Seek(0, SeekOrigin.End);
            return new StreamWriter(stream, encoding);
        }


        public static string GetMD5(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
            {
                return string.Empty;
            }
            using var fs = new FileStream(fileName, FileMode.Open);
            return GetMD5(fs);
        }

        public static string GetMD5(Stream fs)
        {
            var md5 = MD5.Create();
            var res = md5.ComputeHash(fs);
            var sb = new StringBuilder();
            foreach (var b in res)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
