using System.Windows.Input;
using ZoDream.Shared.Models;
using ZoDream.Shared.Routes;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Spider.ViewModels
{
    public class SettingViewModel: BindableBase
    {

        public SettingViewModel()
        {
            BackCommand = new RelayCommand(TapBack);
            HomeCommand = new RelayCommand(TapHome);
            RuleCommand = new RelayCommand(TapRule);
            PluginCommand = new RelayCommand(TapPlugin);
            ProxyCommand = new RelayCommand(TapProxy);
            HeaderCommand = new RelayCommand(TapHeader);
            Load();
        }

        private int maxCount = 1;

        public int MaxCount {
            get => maxCount;
            set => Set(ref maxCount, value);
        }

        private int timeOut = 60;

        public int TimeOut {
            get => timeOut;
            set => Set(ref timeOut, value);
        }

        private bool useBrowser;

        public bool UseBrowser {
            get => useBrowser;
            set => Set(ref useBrowser, value);
        }

        private string workspace = string.Empty;

        public string Workspace {
            get => workspace;
            set => Set(ref workspace, value);
        }


        private bool isLogVisible;

        public bool IsLogVisible
        {
            get => isLogVisible;
            set => Set(ref isLogVisible, value);
        }

        private bool isLogTime;

        public bool IsLogTime
        {
            get => isLogTime;
            set => Set(ref isLogTime, value);
        }
        public ICommand BackCommand { get; private set; }
        public ICommand HomeCommand { get; private set; }

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
    }
}
