using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using ZoDream.Shared.Models;
using ZoDream.Shared.ViewModel;
using ZoDream.Spider.Models;
using ZoDream.Spider.Plugins;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace ZoDream.Spider.ViewModels
{
    public class SettingViewModel: BindableBase
    {

        public SettingViewModel()
        {
            //var plugins = PluginLoader.Load();
            //foreach (var item in plugins)
            //{
            //    if (PluginItems.Contains(item.FileName))
            //    {
            //        item.IsActive = true;
            //    }
            //    PluginFileItems.Add(item);
            //}
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

        private ObservableCollection<PluginInfoItem> pluginFileItems = new();

        public ObservableCollection<PluginInfoItem> PluginFileItems
        {
            get => pluginFileItems;
            set => Set(ref pluginFileItems, value);
        }

        private AppOption option = new();

        public AppOption Option
        {
            get
            {
                option.IsLogVisible = IsLogVisible;
                option.IsLogTime = IsLogTime;
                return option;
            }
            set
            {
                option = value;
                IsLogVisible = option.IsLogVisible;
                IsLogTime = option.IsLogTime;
            }
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
                    // PluginItems.Add(item.FileName);
                    // OnPropertyChanged(nameof(PluginItems));
                }
            }
        }
    }
}
