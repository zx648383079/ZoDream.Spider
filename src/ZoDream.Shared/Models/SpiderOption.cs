using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Models
{
    public class SpiderOption : ILoader
    {
        public int MaxCount { get; set; } = 1;
        public int TimeOut { get; set; } = 60;
        public bool UseBrowser { get; set; } = false;
        /// <summary>
        /// 工作目录
        /// </summary>
        public string WorkFolder { get; set; } = string.Empty;

        public IList<HeaderItem> HeaderItems { get; set; } = new List<HeaderItem>();

        public string FullWorkFolder
        {
            get {
                if (string.IsNullOrWhiteSpace(WorkFolder) || WorkFolder == "\\")
                {
                    return AppDomain.CurrentDomain.BaseDirectory + '\\';
                }
                var fileName = WorkFolder;
                if (WorkFolder.IndexOf(":\\", StringComparison.Ordinal) < 0)
                {
                    fileName = AppDomain.CurrentDomain.BaseDirectory + '\\' + WorkFolder.TrimStart('\\');
                }
                if (fileName.EndsWith('\\'))
                {
                    return fileName;
                }
                return fileName + '\\';
            }
        }

        public string JoinPath(string fileName)
        {
            if (fileName.IndexOf(":\\", StringComparison.Ordinal) > 0)
            {
                return fileName;
            }
            return FullWorkFolder + fileName;
        }



        public void Deserializer(StreamReader reader)
        {
            string? line;
            while (null != (line = reader.ReadLine()))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    return;
                }
                var args = line.Split(':', 2);
                var name = args[0].Trim();
                switch (name.ToLower())
                {
                    case "count":
                        MaxCount = int.Parse(args[1].Trim());
                        break;
                    case "timeout":
                        TimeOut = int.Parse(args[1].Trim());
                        break;
                    case "browser":
                        UseBrowser = args[1].Trim().ToUpper() == "Y";
                        break;
                    case "folder":
                        WorkFolder = args[1].Trim();
                        break;
                    default:
                        if (!string.IsNullOrWhiteSpace(args[1]))
                        {
                            HeaderItems.Add(new HeaderItem(name, args[1].Trim()));
                        }
                        break;
                }
            }
        }

        public void Serializer(StreamWriter writer)
        {
            writer.WriteLine($"count: {MaxCount}");
            writer.WriteLine($"timeout: {TimeOut}");
            writer.WriteLine("browser: " + (UseBrowser ? 'Y' : 'N'));
            writer.WriteLine($"folder: {WorkFolder}");
            foreach (var item in HeaderItems)
            {
                writer.WriteLine($"{item.Name}: {item.Value}");
            }
        }
    }
}
