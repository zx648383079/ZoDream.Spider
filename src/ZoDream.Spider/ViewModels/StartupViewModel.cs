using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Shared.Routes;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Spider.ViewModels
{
    public class StartupViewModel: BindableBase
    {

        public StartupViewModel()
        {
            OpenCommand = new RelayCommand(TapOpen);
            CreateCommand = new RelayCommand(TapCreate);
            SiteCommand = new RelayCommand(TapSite);
            PageCommand = new RelayCommand(TapPage);
            CustomCommand = new RelayCommand(TapCustom);
            Version = App.ViewModel.Version;
        }

        private string version = string.Empty;

        public string Version {
            get => version;
            set => Set(ref version, value);
        }

        private string tip = string.Empty;

        public string Tip {
            get => tip;
            set => Set(ref tip, value);
        }

        private bool dialogVisible = false;

        public bool DialogVisible {
            get => dialogVisible;
            set => Set(ref dialogVisible, value);
        }


        public ICommand OpenCommand { get; private set; }

        public ICommand CreateCommand { get; private set; }

        public ICommand SiteCommand { get; private set; }
        public ICommand PageCommand { get; private set; }
        public ICommand CustomCommand { get; private set; }

        private void TapSite(object? _)
        {
            ShellManager.GoToAsync("quickly/site");
        }

        private void TapPage(object? _)
        {
            ShellManager.GoToAsync("quickly/single");
        }

        private void TapCustom(object? _)
        {
            ShellManager.GoToAsync("quickly/custom");
        }

        private async void TapOpen(object? _)
        {
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
            ShellManager.GoToAsync("home");
        }

        private void TapCreate(object? _)
        {
            DialogVisible = true;
            // ShellManager.GoToAsync("home");
        }
    }
}
