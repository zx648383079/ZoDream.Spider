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

        private int maxCount = 1;

        public int MaxCount {
            get => maxCount;
            set {
                Set(ref maxCount, value);
                IsProjectUpdated = true;
            }
        }

        private int timeOut = 60;

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
                MaxCount = project.MaxCount;
                TimeOut = project.TimeOut;
                UseBrowser = project.UseBrowser;
                Workspace = project.WorkFolder;
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
                    project.MaxCount = MaxCount;
                    project.TimeOut = TimeOut;
                    project.UseBrowser = UseBrowser;
                    project.WorkFolder = Workspace;
                    project.SaveAsync();
                }
            }
            if (IsSettingUpdated)
            {
                var option = App.ViewModel.Option;
                option.IsLogVisible = IsLogVisible;
                option.IsLogTime = IsLogTime;
                App.ViewModel.SaveOptionAsync();
            }
        }
    }
}
