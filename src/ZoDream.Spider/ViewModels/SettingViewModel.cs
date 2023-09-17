using System.Windows.Input;
using ZoDream.Shared.Routes;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Spider.ViewModels
{
    public class SettingViewModel: BindableBase, IExitAttributable
    {

        public SettingViewModel()
        {
            BackCommand = new RelayCommand(TapBack);
            HomeCommand = new RelayCommand(TapHome);
            RuleCommand = new RelayCommand(TapRule);
            PluginCommand = new RelayCommand(TapPlugin);
            ProxyCommand = new RelayCommand(TapProxy);
            HeaderCommand = new RelayCommand(TapHeader);
            HostCommand = new RelayCommand(TapHost);
            Load();
        }
        private bool IsProjectUpdated = false;
        private bool IsSettingUpdated = false;

        private int parallelCount = 1;

        public int ParallelCount {
            get => parallelCount;
            set {
                Set(ref parallelCount, value);
                IsProjectUpdated = true;
            }
        }


        private int retryCount = 1;

        public int RetryCount {
            get => retryCount;
            set {
                Set(ref retryCount, value);
                IsProjectUpdated = true;
            }
        }

        private int retryTime = 0;

        public int RetryTime {
            get => retryCount;
            set {
                Set(ref retryTime, value);
                IsProjectUpdated = true;
            }
        }

        private int timeOut = 10;

        public int TimeOut {
            get => timeOut;
            set {
                Set(ref timeOut, value);
                IsProjectUpdated= true;
            }
        }

        private bool useBrowser;

        public bool UseBrowser {
            get => useBrowser;
            set {
                Set(ref useBrowser, value);
                IsProjectUpdated= true;
            }
        }

        private string workspace = string.Empty;

        public string Workspace {
            get => workspace;
            set {
                Set(ref workspace, value);
                IsProjectUpdated= true;
            }
        }


        private bool isLogVisible;

        public bool IsLogVisible
        {
            get => isLogVisible;
            set {
                Set(ref isLogVisible, value);
                IsSettingUpdated = true;
            }
        }

        private bool isLogTime;

        public bool IsLogTime
        {
            get => isLogTime;
            set {
                Set(ref isLogTime, value);
                IsSettingUpdated = true;
            }
        }

        private string version = App.ViewModel.Version;

        public string Version {
            get => version;
            set => Set(ref version, value);
        }

        public ICommand BackCommand { get; private set; }
        public ICommand HomeCommand { get; private set; }
        public ICommand HostCommand { get; private set; }

        public ICommand HeaderCommand { get; private set; }
        public ICommand RuleCommand { get; private set; }
        public ICommand PluginCommand { get; private set; }
        public ICommand ProxyCommand { get; private set; }

        private void TapBack(object? _)
        {
            ShellManager.BackAsync();
        }

        private void TapHome(object? _)
        {
            ShellManager.GoToAsync("home");
        }

        private void TapHost(object? _)
        {
            ShellManager.GoToAsync("host");
        }

        private void TapHeader(object? _)
        {
            ShellManager.GoToAsync("header");
        }

        private void TapRule(object? _)
        {
            ShellManager.GoToAsync("rule");
        }

        private void TapPlugin(object? _)
        {
            ShellManager.GoToAsync("plugin");
        }

        private void TapProxy(object? _)
        {
            ShellManager.GoToAsync("proxy");
        }


        private void Load()
        {
            var project = App.ViewModel.Project;
            if (project is not null)
            {
                ParallelCount = project.ParallelCount;
                RetryCount = project.RetryCount;
                RetryTime = project.RetryTime;
                TimeOut = project.TimeOut;
                UseBrowser = project.UseBrowser;
                Workspace = project.Workspace;
            }
            var option = App.ViewModel.Option;
            IsLogVisible = option.IsLogVisible;
            IsLogTime = option.IsLogTime;
        }

        public void ApplyExitAttributes()
        {
            if (IsProjectUpdated)
            {
                IsProjectUpdated = false;
                var project = App.ViewModel.Project;
                if (project is not null)
                {
                    project.ParallelCount = ParallelCount;
                    project.RetryCount = RetryCount;
                    project.RetryTime = RetryTime;
                    project.TimeOut = TimeOut;
                    project.UseBrowser = UseBrowser;
                    project.Workspace = Workspace;
                    project.SaveAsync();
                }
            }
            if (IsSettingUpdated)
            {
                var option = App.ViewModel.Option;
                option.IsLogVisible = IsLogVisible;
                option.IsLogTime = IsLogTime;
                _ = App.ViewModel.SaveOptionAsync();
            }
        }
    }
}
