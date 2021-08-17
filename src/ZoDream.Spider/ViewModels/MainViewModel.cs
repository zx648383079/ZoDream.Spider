using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Spiders;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Spider.ViewModels
{
    public class MainViewModel: BindableBase
    {
        public string FileName { get; set; } = string.Empty;

        private ISpider? instance;

        public ISpider? Instance
        {
            get { return instance; }
            set {
                instance = value;
                IsNotEmpty = value != null;
            }
        }


        private bool isNotEmpty = false;

        public bool IsNotEmpty
        {
            get => isNotEmpty;
            set => Set(ref isNotEmpty, value);
        }

        private string message = string.Empty;

        public string Message
        {
            get => message;
            set => Set(ref message, value);
        }

        public void Load()
        {
            Load(string.Empty);
        }

        public void Load(string file)
        {
            FileName = file;
            Instance = new DefaultSpider();
            if (!string.IsNullOrEmpty(file))
            {
                Instance.Load(file);
            }

        }

        public void Close()
        {
            FileName = string.Empty;
            Instance = null;
        }

        public void Save()
        {
            if (Instance == null)
            {
                return;
            }
            Instance.Save(FileName);
        }

        public void Save(string fileName)
        {
            FileName = fileName;
            Save();
        }

        public void ShowMessage(string message)
        {
            Message = message;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(3000);
                Message = string.Empty;
            });
        }
    }
}
