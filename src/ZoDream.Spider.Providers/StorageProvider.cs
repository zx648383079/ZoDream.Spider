using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

namespace ZoDream.Spider.Providers
{
    public class StorageProvider : IStorageProvider<string, string, FileStream>
    {
        public StorageProvider(ISpider spider)
        {
            Application = spider;
            BaseFolder = AppDomain.CurrentDomain.BaseDirectory;
        }

        public ISpider Application { get; set; }

        public string BaseFolder { get; set; }

        public string EntranceFile {
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                EntranceFolder = Path.GetDirectoryName(value);
            }
        }
        public string? EntranceFolder {
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                BaseFolder = value;
            }
        }

        public string FullWorkFolder
        {
            get
            {
                var option = Application.Option;
                if (string.IsNullOrWhiteSpace(option.WorkFolder) || option.WorkFolder == "\\")
                {
                    return BaseFolder + '\\';
                }
                var fileName = option.WorkFolder;
                if (option.WorkFolder.IndexOf(":\\", StringComparison.Ordinal) < 0)
                {
                    fileName = BaseFolder + '\\' + option.WorkFolder.TrimStart('\\');
                }
                if (fileName.EndsWith("\\"))
                {
                    return fileName;
                }
                return fileName + '\\';
            }
        }


        public Task CreateAsync(string fileName, byte[] data)
        {
            return Task.Factory.StartNew(() =>
            {
                var path = GetAbsolutePath(fileName);
                Disk.CreateDirectory(path);
                File.WriteAllBytes(path, data);
            });
        }

        public Task CreateAsync(UriItem uri, byte[] data)
        {
            return CreateAsync(Disk.RenderFile(uri.Source), data);
        }

        public Task<FileStream> CreateStreamAsync(string fileName)
        {
            return Task.Factory.StartNew(() =>
            {
                var path = GetAbsolutePath(fileName);
                Disk.CreateDirectory(path);
                return new FileStream(path, FileMode.Create);
            });
        }

        public Task<FileStream> CreateStreamAsync(UriItem uri)
        {
            return CreateStreamAsync(Disk.RenderFile(uri.Source));
        }

        public Task<FileStream?> OpenStreamAsync(string fileName)
        {
            return Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return null;
                }
                var path = GetAbsolutePath(fileName);
                if (!File.Exists(path))
                {
                    return null;
                }
                return new FileStream(path, FileMode.Open);
            });
        }

        public string GetFileName(UriItem uri)
        {
            return Application.RuleProvider.GetFileName(uri.Source);
        }

        public Task<FileStream?> OpenStreamAsync(UriItem uri)
        {
            return OpenStreamAsync(GetFileName(uri));
        }

        public string GetAbsolutePath(string fileName)
        {
            if (fileName.IndexOf(":\\", StringComparison.Ordinal) > 0)
            {
                return fileName;
            }
            return FullWorkFolder + fileName;
        }

        public string GetRelativePath(string fileName)
        {
            if (fileName.IndexOf(":\\", StringComparison.Ordinal) < 0)
            {
                return fileName.TrimStart('\\');
            }
            return new Uri(fileName).MakeRelativeUri(new Uri(FullWorkFolder)).ToString();
        }
    }
}
