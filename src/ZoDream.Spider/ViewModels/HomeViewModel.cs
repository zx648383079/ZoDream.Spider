using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ZoDream.Shared.Events;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Loaders;
using ZoDream.Shared.Models;
using ZoDream.Shared.Routes;
using ZoDream.Shared.Utils;
using ZoDream.Shared.ViewModel;
using ZoDream.Spider.Loggers;
using ZoDream.Spider.Pages;
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
            SyncUrlCommand = new RelayCommand(TapSyncUrl);
            BrowserOpenCommand = new RelayCommand(TapBrowserOpen);
            BrowserTestCommand = new RelayCommand(TapBrowserTest);
            HttpTestCommand = new RelayCommand(TapHttpTest);
            DeleteDoneCommand = new RelayCommand(TapDeleteDone);
            DeleteSelectedCommand = new RelayCommand(TapDeleteSelected);
            ClearCommand = new RelayCommand(TapClear);
            Load();
        }

        public ILogger? Logger { get; set; }

        private ISpider? instance;

        public ISpider? Instance
        {
            get { return instance; }
            set {
                instance = value;
                OnPropertyChanged(nameof(IsNotEmpty));
            }
        }

        public bool IsNotEmpty => UrlItems.Count > 0 && !Paused;

        private bool paused = true;

        public bool Paused
        {
            get => paused;
            set {
                Set(ref paused, value);
                OnPropertyChanged(nameof(IsNotEmpty));
            }
        }

        private double progress;

        public double Progress {
            get => progress;
            set => Set(ref progress, value);
        }



        private CancellationTokenSource messageToken = new();
        private string message = string.Empty;

        public string Message
        {
            get => message;
            set => Set(ref message, value);
        }

        private ObservableCollection<UriLoadItem> urlItems = [];

        public ObservableCollection<UriLoadItem> UrlItems
        {
            get => urlItems;
            set => Set(ref urlItems, value);
        }

        public UriLoadItem? SelectedItem { get; set; }
        public UriLoadItem[]? SelectedItems { get; set; }

        public ICommand SettingCommand { get; private set; }
        public ICommand StartCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand EntryCommand { get; private set; }
        public ICommand ProxyCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        public ICommand SyncUrlCommand {  get; private set; }
        public ICommand BrowserOpenCommand { get; private set; }
        public ICommand BrowserTestCommand { get; private set; }
        public ICommand HttpTestCommand { get; private set; }
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
            if (Instance.RequestProvider is BrowserProvider o)
            {
                o.UseBrowser = Instance.Project.UseBrowser;
            }
            Instance.IsDebug = false;
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

        private async void TapOpen(object? _)
        {
            if (!Paused)
            {
                return;
            }
            var open = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "爬虫项目文件|*.sp|所有文件|*.*",
                Title = "选择文件"
            };
            if (open.ShowDialog() != true)
            {
                return;
            }
            await App.ViewModel.OpenProjectAsync(open.FileName);
            Load();
        }

        private async void TapSave(object? _)
        {
            await App.ViewModel.Project!.SaveAsync();
            MessageBox.Show("保存成功");
        }

        private void TapSyncUrl(object? _)
        {
            if (Instance is null)
            {
                foreach (var item in App.ViewModel.Project!.EntryItems)
                {
                    foreach (var url in Html.GenerateUrl(item))
                    {
                        Add(url);
                    }
                }
                return;
            }
            foreach (var item in Instance.UrlProvider)
            {
                Add(item);
            }
        }

        private void TapBrowserOpen(object? _)
        {
            if (!Paused || SelectedItem is null)
            {
                return;
            }
            var browser = new BrowserView();
            browser.Show();
            browser.Navigate(SelectedItem.Source, 
                Instance?.GetRequestData(SelectedItem.Source));
        }

        private void TapBrowserTest(object? _)
        {
            TapTestSelected(true);
        }

        private void TapHttpTest(object? _)
        {
            TapTestSelected(false);
        }

        private void TapTestSelected(bool useBrowser)
        {
            if (!Paused || SelectedItem is null)
            {
                return;
            }
            if (Instance is null)
            {
                return;
            }
            if (Instance.RequestProvider is BrowserProvider o)
            {
                o.UseBrowser = useBrowser;
            }
            Instance.IsDebug = true;
            _ = Instance.ExecuteAsync(new UriItem(SelectedItem));
        }

        private void TapDeleteSelected(object? _)
        {
            if (!Paused)
            {
                return;
            }
            if (SelectedItems is null) {
                return;
            }
            foreach (var item in SelectedItems)
            {
                UrlItems.Remove(item);
                Instance?.UrlProvider?.Remove(item.Source);
            }
        }

        private void TapDeleteDone(object? _)
        {
            if (!Paused)
            {
                return;
            }
            for (int i = UrlItems.Count - 1; i >= 0; i--)
            {
                var item = UrlItems[i];
                if (item.Status == UriCheckStatus.Done)
                {
                    UrlItems.RemoveAt(i);
                    Instance?.UrlProvider?.Remove(item.Source);
                }
            }
        }

        private void TapClear(object? _)
        {
            if (!Paused)
            {
                return;
            }
            UrlItems.Clear();
            Instance?.UrlProvider?.Clear();
        }

        public void Add(string url)
        {
            foreach (var item in UrlItems)
            {
                if (item.Source == url)
                {
                    return;
                }
            }
            App.ViewModel.DispatcherQueue.Invoke(() => {
                UrlItems.Add(new UriLoadItem(url));
            });
        }

        public void Add(UriItem data)
        {
            foreach (var item in UrlItems)
            {
                if (item.Source == data.Source)
                {
                    App.ViewModel.DispatcherQueue.Invoke(() => {
                        item.Title = data.Title;
                        item.Status = data.Status;
                    });
                    return;
                }
            }
            App.ViewModel.DispatcherQueue.Invoke(() => {
                UrlItems.Add(new UriLoadItem(data));
            });
        }

        public void Load()
        {
            Logger = new EventLogger();
            var project = App.ViewModel.Project!;
            var plugin = App.ViewModel.Plugin;
            Instance = IsUseLazyProject(project, plugin) ? 
                new LazySpider(project, Logger, plugin) 
                : new DefaultSpider(project!, Logger, plugin);
            Instance.RequestProvider = new BrowserProvider(Instance);
            Instance.UrlProvider.UrlChanged += UrlProvider_UrlChanged;
            Instance.UrlProvider.ProgressChanged += UrlProvider_UrlChanged;
            Instance.PausedChanged += v =>
            {
                App.ViewModel.DispatcherQueue.Invoke(() => {
                    Paused = v;
                });
            };
        }

        private bool IsUseLazyProject(ProjectLoader project, IPluginLoader plugin)
        {
            foreach (var item in project.RuleItems)
            {
                foreach (var rule in item.Rules)
                {
                    if (plugin.IsUseLazy(rule))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void UrlProvider_UrlChanged(UriItem uri)
        {
            foreach (var item in UrlItems)
            {
                if (item.Source == uri.Source)
                {
                    App.ViewModel.DispatcherQueue.Invoke(() => {
                        item.Status = uri.Status;
                        item.Progress = uri.Progress.Value;
                    });
                    return;
                }
            }
        }

        private void UrlProvider_UrlChanged(UrlChangedEventArgs args)
        {
            if (Instance == null)
            {
                App.ViewModel.DispatcherQueue.Invoke(() => {
                    UrlItems.Clear();
                });
                Progress = 0;
                return;
            }
            if (args.Uri == null)
            {
                App.ViewModel.DispatcherQueue.Invoke(() => {
                    UrlItems.Clear();
                    var done = 0;
                    foreach (var item in Instance.UrlProvider)
                    {
                        if (item.Status == UriCheckStatus.Done 
                        || item.Status == UriCheckStatus.Error || item.Status == UriCheckStatus.Jump)
                        {
                            done++;
                        }
                        UrlItems.Add(new UriLoadItem(item));
                    }
                    Progress = done <= 0 ?  0 : done * 100 / UrlItems.Count;
                });
                return;
            }
            //if (isNew)
            //{
            //    UrlItems.Add(new UriLoadItem(url));
            //    return;
            //}
            foreach (var item in UrlItems)
            {
                if (item.Source == args.Uri.Source)
                {
                    App.ViewModel.DispatcherQueue.Invoke(() => {
                        item.Title = args.Uri.Title;
                        item.Status = args.Uri.Status;
                        if (!string.IsNullOrEmpty(args.Message))
                        {
                            item.Message = args.Message;
                        }
                    });
                    UpdateProgress();
                    return;
                }
            }
            App.ViewModel.DispatcherQueue.Invoke(() => {
                UrlItems.Add(new UriLoadItem(args.Uri));
            });
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            var done = 0;
            var error = 0;
            var jump = 0;
            foreach (var item in UrlItems)
            {
                if (item.Status == UriCheckStatus.Done)
                {
                    done++;
                } else if (item.Status == UriCheckStatus.Error)
                {
                    error++;
                }
                else if (item.Status == UriCheckStatus.Jump)
                {
                    jump++;
                }
            }
            Message = $"完成 {done} |错误 {error} |跳过 {jump} |总 {UrlItems.Count}";
            Progress = UrlItems.Count <= 0 ? 0 : ((done + error + jump) * 100 / UrlItems.Count);
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
