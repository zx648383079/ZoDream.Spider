using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZoDream.Shared.Storage
{
    /// <summary>   
    /// 用于取得一个文本文件的编码方式(Encoding)。   
    /// </summary>   
    public static class TxtEncoder
    {
        /// <summary>   
        /// 取得一个文本文件的编码方式。如果无法在文件头部找到有效的前导符，Encoding.Default将被返回。   
        /// </summary>   
        /// <param name="fileName">文件名。</param>   
        /// <returns></returns>   
        public static Encoding GetEncoding(string fileName)
        {
            return GetEncoding(fileName, Encoding.Default);
        }
        /// <summary>   
        /// 取得一个文本文件流的编码方式。   
        /// </summary>   
        /// <param name="stream">文本文件流。</param>   
        /// <returns></returns>   
        public static Encoding GetEncoding(Stream stream)
        {
            return GetEncoding(stream, Encoding.Default);
        }
        /// <summary>   
        /// 取得一个文本文件的编码方式。   
        /// </summary>   
        /// <param name="fileName">文件名。</param>   
        /// <param name="defaultEncoding">默认编码方式。当该方法无法从文件的头部取得有效的前导符时，将返回该编码方式。</param>   
        /// <returns></returns>   
        public static Encoding GetEncoding(string fileName, Encoding defaultEncoding)
        {
            var fs = new FileStream(fileName, FileMode.Open);
            var targetEncoding = GetEncoding(fs, defaultEncoding);
            fs.Close();
            return targetEncoding;
        }

        /// <summary>   
        /// 取得一个文本文件流的编码方式。   
        /// </summary>   
        /// <param name="stream">文本文件流。</param>   
        /// <param name="defaultEncoding">默认编码方式。当该方法无法从文件的头部取得有效的前导符时，将返回该编码方式。</param>   
        /// <returns></returns>   
        public static Encoding GetEncoding(Stream stream, Encoding defaultEncoding)
        {
            var targetEncoding = defaultEncoding;
            if (stream == null || stream.Length < 2) return targetEncoding;
            //保存文件流的前4个字节   
            byte byte3 = 0;
            //保存当前Seek位置   
            var origPos = stream.Seek(0, SeekOrigin.Begin);
            stream.Seek(0, SeekOrigin.Begin);

            var nByte = stream.ReadByte();
            var byte1 = Convert.ToByte(nByte);
            var byte2 = Convert.ToByte(stream.ReadByte());
            if (stream.Length >= 3)
            {
                byte3 = Convert.ToByte(stream.ReadByte());
            }
            //根据文件流的前4个字节判断Encoding   
            //Unicode {0xFF, 0xFE};   
            //BE-Unicode {0xFE, 0xFF};   
            //UTF8 = {0xEF, 0xBB, 0xBF};   
            if (byte1 == 0xFE && byte2 == 0xFF)//UnicodeBe   
            {
                targetEncoding = Encoding.BigEndianUnicode;
            }
            else if (byte1 == 0xFF && byte2 == 0xFE && byte3 != 0xFF)//Unicode   
            {
                targetEncoding = Encoding.Unicode;
            }
            else if (byte1 == 0xEF && byte2 == 0xBB && byte3 == 0xBF) //UTF8   
            {
                targetEncoding = Encoding.UTF8;
            }
            else
            {
                stream.Seek(0, SeekOrigin.Begin);
                int read;
                while ((read = stream.ReadByte()) != -1)
                {
                    if (read >= 0xF0)
                        break;
                    if (0x80 <= read && read <= 0xBF)
                        break;
                    if (0xC0 <= read && read <= 0xDF)
                    {
                        read = stream.ReadByte();
                        if (0x80 <= read && read <= 0xBF)
                            continue;
                        break;
                    }
                    if (0xE0 > read || read > 0xEF) continue;
                    read = stream.ReadByte();
                    if (0x80 <= read && read <= 0xBF)
                    {
                        read = stream.ReadByte();
                        if (0x80 <= read && read <= 0xBF)
                        {
                            targetEncoding = Encoding.UTF8;
                        }
                    }
                    break;
                }
            }
            //恢复Seek位置         
            stream.Seek(origPos, SeekOrigin.Begin);
            return targetEncoding;
        }
    }
}
