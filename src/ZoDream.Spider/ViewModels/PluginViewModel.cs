using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Shared.Models;
using ZoDream.Shared.Routes;
using ZoDream.Shared.ViewModel;
using ZoDream.Spider.Models;
using ZoDream.Spider.Plugins;

namespace ZoDream.Spider.ViewModels
{
    public class PluginViewModel : BindableBase, IExitAttributable
    {
        public PluginViewModel()
        {
            BackCommand = new RelayCommand(TapBack);
            ImportCommand = new RelayCommand(TapImport);
            InstallCommand = new RelayCommand(TapInstall);
            UninstallCommand = new RelayCommand(TapUninstall);
            Load();
        }

        private bool IsUpdated = false;

        private ObservableCollection<PluginInfoItem> pluginFileItems = new();

        public ObservableCollection<PluginInfoItem> PluginFileItems {
            get => pluginFileItems;
            set => Set(ref pluginFileItems, value);
        }


        public ICommand BackCommand { get; private set; }
        public ICommand ImportCommand { get; private set; }
        public ICommand InstallCommand { get; private set; }
        public ICommand UninstallCommand { get; private set; }

        private void TapBack(object? _)
        {
            ShellManager.BackAsync();
        }

        private void TapInstall(object? arg)
        {
            if (arg is PluginInfoItem o)
            {
                PluginInstall(o);
            }
        }

        private void TapUninstall(object? arg)
        {
            if (arg is PluginInfoItem o)
            {
                PluginUnInstall(o);
            }
        }


        private void TapImport(object? _)
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                RestoreDirectory = true,
                Filter = "DLL|*.dll|All|*.*",
                Multiselect = true,
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            PluginImport(picker.FileNames);
        }


        public void PluginImport(string[] fileNames)
        {
            //var items = PluginLoader.Save(fileNames);
            //foreach (var item in items)
            //{
            //    PluginFileItems.Add(item);
            //}
        }

        public void PluginUnInstall(PluginInfoItem? pluginItem)
        {
            foreach (var item in PluginFileItems)
            {
                if (item == pluginItem)
                {
                    item.IsActive = false;
                    //PluginItems.Remove(item.FileName);
                    // OnPropertyChanged(nameof(PluginItems));
                    IsUpdated = true;
                }
            }
        }

        public void PluginInstall(PluginInfoItem? pluginItem)
        {
            foreach (var item in PluginFileItems)
            {
                if (item == pluginItem)
                {
                    item.IsActive = true;
                    IsUpdated = true;
                }
            }
        }

        private void Load()
        {
            var plugins = App.ViewModel.Plugin.All();
            var items = App.ViewModel.Option.PluginItems;
            foreach (var item in plugins)
            {
                PluginFileItems.Add(new PluginInfoItem(item)
                {
                    IsActive = items.Count == 0 || items.Contains(item.FileName),
                });
            }
        }

        public void ApplyExitAttributes()
        {
            if (!IsUpdated)
            {
                return;
            }
            IsUpdated = false;
            var option = App.ViewModel.Option;
            option.PluginItems = PluginFileItems.Where(item => item.IsActive)
                .Select(item => item.FileName).ToList();
            App.ViewModel.SaveOptionAsync();
        }
    }
}
