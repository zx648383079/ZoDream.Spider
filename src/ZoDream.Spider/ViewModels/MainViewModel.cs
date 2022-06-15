using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Loggers;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;
using ZoDream.Shared.ViewModel;
using ZoDream.Spider.Pages;
using ZoDream.Spider.Plugins;
using ZoDream.Spider.Programs;
using ZoDream.Spider.Providers;

namespace ZoDream.Spider.ViewModels
{
    public class MainViewModel: BindableBase
    {

        public MainViewModel()
        {
            Plugin = new PluginLoader();
            _ = LoadOptionAsync();
            Task.Factory.StartNew(() =>
            {
                Plugin.Load(AppDomain.CurrentDomain.BaseDirectory);
            });
        }

        public AppOption Option = new();
        public string FileName { get; set; } = string.Empty;

        public ILogger? Logger { get; set; }

        public IPluginLoader Plugin;

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

        private bool paused = true;

        public bool Paused
        {
            get => paused;
            set => Set(ref paused, value);
        }


        private CancellationTokenSource messageToken = new CancellationTokenSource();
        private string message = string.Empty;

        public string Message
        {
            get => message;
            set => Set(ref message, value);
        }

        private ObservableCollection<UriItem> urlItems = new ObservableCollection<UriItem>();

        public ObservableCollection<UriItem> UrlItems
        {
            get => urlItems;
            set => Set(ref urlItems, value);
        }

        private BrowserView? broswerRequest;

        public BrowserView BroswerRequest
        {
            get {
                if (broswerRequest == null)
                {
                    broswerRequest = new BrowserView();
                }
                if (!broswerRequest.IsVisible)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        broswerRequest.Show();
                    });
                }
                return broswerRequest;
            }
        }


        public void Load()
        {
            Load(string.Empty);
        }

        public void Load(string file)
        {
            FileName = file;
            Instance = new DefaultSpider(Logger, Plugin);
            Instance.Storage.EntranceFile = file;
            Instance.RequestProvider = new BrowserProvider(Instance);
            Instance.UrlProvider.UrlChanged += UrlProvider_UrlChanged;
            Instance.PausedChanged += v =>
            {
                Paused = v;
            };
            if (!string.IsNullOrEmpty(file))
            {
                Instance.Load(file);
                ShowMessage("载入项目成功！");
            } else
            {
                ShowMessage("新建项目成功！");
            }
        }

        private void UrlProvider_UrlChanged(UriItem? url, bool isNew)
        {
            if (Instance == null)
            {
                UrlItems.Clear();
                return;
            }
            if (url == null)
            {
                UrlItems.Clear();
                foreach (var item in Instance.UrlProvider)
                {
                    UrlItems.Add(item);
                }
                return;
            }
            if (isNew)
            {
                UrlItems.Add(url);
            }
        }

        public void Close()
        {
            FileName = string.Empty;
            Instance = null;
        }

        public async Task<AppOption> LoadOptionAsync()
        {
            var option = await AppData.LoadAsync<AppOption>();
            if (option != null)
            {
                Option = option;
            }
            return Option;
        }

        public async Task SaveOptionAsync()
        {
            await AppData.SaveAsync(Option);
        }

        public void Save()
        {
            if (Instance == null)
            {
                return;
            }
            Instance.Save(FileName);
            Instance.Storage.EntranceFile = FileName;
        }

        public void Save(string fileName)
        {
            FileName = fileName;
            Save();
        }

        public void ShowMessage(string message)
        {
            messageToken.Cancel();
            messageToken = new CancellationTokenSource();
            var token = messageToken.Token;
            Message = message;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(3000);
                if (token.IsCancellationRequested)
                {
                    return;
                }
                Message = string.Empty;
            }, token);
            
        }
    }
}
