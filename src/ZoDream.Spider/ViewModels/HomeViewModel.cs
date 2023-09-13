using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Routes;
using ZoDream.Shared.ViewModel;
using ZoDream.Spider.Programs;
using ZoDream.Spider.Providers;

namespace ZoDream.Spider.ViewModels
{
    public class HomeViewModel: BindableBase
    {

        public HomeViewModel()
        {
            SettingCommand = new RelayCommand(TapSetting);
            StartCommand = new RelayCommand(TapStart);
            PauseCommand = new RelayCommand(TapPause);
            StopCommand = new RelayCommand(TapStop);
            EntryCommand = new RelayCommand(TapEntry);
            ProxyCommand = new RelayCommand(TapProxy);
            OpenCommand = new RelayCommand(TapOpen);
            SaveCommand = new RelayCommand(TapSave);
            BrowserCommand = new RelayCommand(TapBrowser);
            DeleteDoneCommand = new RelayCommand(TapDeleteDone);
            DeleteSelectedCommand = new RelayCommand(TapDeleteSelected);
            ClearCommand = new RelayCommand(TapClear);
        }

        public ILogger? Logger { get; set; }

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


        private CancellationTokenSource messageToken = new();
        private string message = string.Empty;

        public string Message
        {
            get => message;
            set => Set(ref message, value);
        }

        private ObservableCollection<UriItem> urlItems = new();

        public ObservableCollection<UriItem> UrlItems
        {
            get => urlItems;
            set => Set(ref urlItems, value);
        }

        public ICommand SettingCommand { get; private set; }
        public ICommand StartCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand EntryCommand { get; private set; }
        public ICommand ProxyCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand BrowserCommand { get; private set; }
        public ICommand DeleteSelectedCommand { get; private set; }
        public ICommand DeleteDoneCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }

        private void TapProxy(object? _)
        {
            ShellManager.GoToAsync("proxy");
        }
        private void TapEntry(object? _)
        {
            ShellManager.GoToAsync("entry");
        }

        private void TapSetting(object? _)
        {
            ShellManager.GoToAsync("setting");
        }

        private void TapStart(object? _)
        {
            if (Instance == null)
            {
                return;
            }
            Instance.Start();
        }

        private void TapPause(object? _)
        {
            if (Instance == null)
            {
                return;
            }
            Instance.Pause();
        }

        private void TapStop(object? _)
        {
            if (Instance == null)
            {
                return;
            }
            Instance.Stop();
        }

        private void TapOpen(object? _)
        {
        }

        private void TapSave(object? _)
        {

        }

        private void TapBrowser(object? _)
        {

        }

        private void TapDeleteSelected(object? _)
        {

        }

        private void TapDeleteDone(object? _)
        {

        }

        private void TapClear(object? _)
        {

        }

        public void Load()
        {
            Instance = new DefaultSpider(App.ViewModel.Project!, Logger, App.ViewModel.Plugin);
            Instance.RequestProvider = new BrowserProvider(Instance);
            Instance.UrlProvider.UrlChanged += UrlProvider_UrlChanged;
            Instance.PausedChanged += v =>
            {
                Paused = v;
            };
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
            Instance = null;
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
