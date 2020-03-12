using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZoDream.Helper.Local
{
    public class Csv
    {
        public StreamWriter Writer { get; set; }

        public List<string> Columns { get; set; } = new List<string>();

        public Csv()
        {
            
        }

        public Csv(string file)
        {
            var fi = new FileInfo(file);
            if (fi.Directory != null && !fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            Writer = new StreamWriter(file, true, Encoding.UTF8);
        }

        public Csv(StreamWriter writer)
        {
            Writer = writer;
        }

        public void WriteRow(string row)
        {
            Writer.WriteLine(row);
        }

        public void WriteRow(IList<string> columns)
        {
            WriteRow(GetRow(columns));
        }

        public void WriteRow()
        {
            if (Columns.Count < 0)
            {
                return;
            }
            WriteRow(Columns);
            Columns.Clear();
        }

        public void AppendColumn(string column)
        {
            Columns.Add(column);
        }

        public void Close()
        {
            WriteRow();
            Writer.Close();
        }

        /// <summary>
        /// 写入到CSV文件中
        /// </summary>
        public static void Export(IList<string> lists, string fullPath)
        {
            var fi = new FileInfo(fullPath);
            if (fi.Directory != null && !fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            var sw = new StreamWriter(fullPath, true, Encoding.UTF8);
            foreach (var item in lists)
            {
                sw.WriteLine(GetString(item));
            }
            sw.Close();
        }

        public static string GetRow(IList<string> args)
        {
            for (var i = 0; i < args.Count; i++)
            {
                args[i] = GetString(args[i]);
            }
            return string.Join(",", args);
        }

        /// <summary>
        /// 转义字符串
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetString(string content)
        {
            content = content.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
            if (content.Contains(',') || content.Contains('"')
                || content.Contains('\r') || content.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
            {
                content = $"\"{content}\"";
            }
            return content;
        }
    }
}
