using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Local
{
    public static class Open
    {
        /// <summary>
        /// 读文本文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string Read(string file)
        {
            if (!File.Exists(file))
            {
                return string.Empty;
            }
            var fs = new FileStream(file, FileMode.Open);
            var reader = new StreamReader(fs, TxtEncoder.GetEncoding(fs));
            var content = reader.ReadToEnd();
            reader.Close();
            return content;
        }

        public static StreamReader Reader(string file)
        {
            var fs = new FileStream(file, FileMode.Open);
            return new StreamReader(fs, TxtEncoder.GetEncoding(fs));
        }

        /// <summary>
        /// 写文本文件 默认使用无bom 的UTF8编码
        /// </summary>
        /// <param name="file"></param>
        /// <param name="content"></param>
        public static void Write(string file, string content)
        {
            Write(file, content, new UTF8Encoding(false));
        }

        public static void Write(string file, string content, bool append)
        {
            Write(file, content, new UTF8Encoding(false), append);
        }

        public static void Write(string file, string content, string encoding)
        {
            Write(file, content, Encoding.GetEncoding(encoding));
        }

        public static void Write(string file, string content, Encoding encoding)
        {
            Write(file, content, encoding, false);
        }

        public static void Write(string file, string content, Encoding encoding, bool append)
        {
            using (var writer = new StreamWriter(file, append, encoding))
            {
                writer.Write(content);
            }
        }

        public static StreamWriter Writer(string file)
        {
            return Writer(file, false);
        }

        public static StreamWriter Writer(string file, bool append)
        {
            var fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            var encoding = TxtEncoder.GetEncoding(fs);
            if (append)
            {
                fs.Seek(0, SeekOrigin.End);
            }
            return new StreamWriter(fs, encoding);
        }
    }
}
