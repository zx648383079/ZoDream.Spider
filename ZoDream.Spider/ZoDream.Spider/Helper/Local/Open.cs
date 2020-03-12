using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZoDream.Helper.Local
{
    public class Open
    {
        /// <summary>
        /// 浏览文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void ExploreFile(string filePath)
        {
            var proc = new Process
            {
                StartInfo =
                {
                    FileName = "explorer",
                    Arguments = @"/select," + filePath
                }
            };
            //打开资源管理器
            //选中"notepad.exe"这个程序,即记事本
            proc.Start();
        }

        /// <summary>
        /// 浏览文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void ExplorePath(string path)
        {
            Process.Start("explorer.exe", path);
        }


        /// <summary>
        /// 遍历文件夹
        /// </summary>
        /// <param name="dir"></param>
        public static List<string> GetAllFile(string dir)
        {
            var files = new List<string>();
            if (string.IsNullOrWhiteSpace(dir))
            {
                return files;
            }
            var theFolder = new DirectoryInfo(dir);
            var dirInfo = theFolder.GetDirectories();
            //遍历文件夹
            foreach (var nextFolder in dirInfo)
            {
                files.AddRange(GetAllFile(nextFolder.FullName));
            }

            var fileInfo = theFolder.GetFiles();
            //遍历文件
            files.AddRange(fileInfo.Select(nextFile => nextFile.FullName));
            return files;
        }

        public static string ChooseSaveFile(string name = "", string filter = "文本文件|*.txt|所有文件|*.*")
        {
            var open = new Microsoft.Win32.SaveFileDialog
            {
                Title = "选择保存路径",
                Filter = filter,
                FileName = name
            };
            return open.ShowDialog() == true ? open.FileName : null;
        }

        /// <summary>
        /// 选择个文件
        /// </summary>
        /// <returns></returns>
        public static string ChooseFile(string filter = "文本文件|*.txt|所有文件|*.*")
        {
            var open = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = filter,
                Title = "选择文件"
            };
            return open.ShowDialog() == true ? open.FileName : string.Empty;
        }

        /// <summary>
        /// 选择多个文件
        /// </summary>
        /// <returns></returns>
        public static List<string> ChooseFiles(string filter = "文本文件|*.txt|所有文件|*.*")
        {
            var files = new List<string>();
            var open = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = filter,
                Title = "选择文件",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            if (open.ShowDialog() == true)
            {
                files.AddRange(open.FileNames);
            }
            return files;
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        /// <param name="dir">初始目录</param>
        /// <returns></returns>
        public static string ChooseFolder(string dir = "")
        {
            if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
            {
                dir = AppDomain.CurrentDomain.BaseDirectory;
            }
            var folder = new FolderBrowserDialog
            {
                SelectedPath = dir,
                ShowNewFolderButton = false
            };
            return folder.ShowDialog() == DialogResult.OK ? folder.SelectedPath : null;
        }

        /// <summary>
        /// 调用默认浏览器打开网址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool OpenBrowser(string url)
        {
            var key = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command\");
            if (key == null) return false;
            var s = key.GetValue("").ToString();
            var browserpath = s.StartsWith("\"") ? s.Substring(1, s.IndexOf('\"', 1) - 1) : s.Substring(0, s.IndexOf(" ", StringComparison.Ordinal));
            return Process.Start(browserpath, url) != null;
        }

        /// <summary>
        /// 读文本文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string Read(string file)
        {
            if (!File.Exists(file))
            {
                return null;
            }
            var fs = new FileStream(file, FileMode.Open);
            var reader = new StreamReader(fs, (new TxtEncoder()).GetEncoding(fs));
            var content = reader.ReadToEnd();
            reader.Close();
            return content;
        }

        public static StreamReader Reader(string file)
        {
            var fs = new FileStream(file, FileMode.Open);
            return new StreamReader(fs, new TxtEncoder().GetEncoding(fs));
        }

        /// <summary>
        /// 写文本文件 默认使用无bom 的UTF8编码
        /// </summary>
        /// <param name="file"></param>
        /// <param name="content"></param>
        public static void Writer(string file, string content)
        {
            Writer(file, content, new UTF8Encoding(false));
        }

        public static void Writer(string file, string content, bool append)
        {
            Writer(file, content, new UTF8Encoding(false), append);
        }

        public static void Writer(string file, string content, string encoding)
        {
            Writer(file, content, Encoding.GetEncoding(encoding));
        }

        public static void Writer(string file, string content, Encoding encoding)
        {
            Writer(file, content, encoding, false);
        }

        public static void Writer(string file, string content, Encoding encoding, bool append)
        {
            using (var writer = new StreamWriter(file, append, encoding))
            {
                writer.Write(content);
            }
        }
    }
}
