using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ZoDream.Shared.Models;
using ZoDream.Shared.Routes;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Spider.ViewModels
{
    public class HostViewModel: BindableBase, IExitAttributable
    {

        public HostViewModel()
        {
            BackCommand = new RelayCommand(TapBack);
            AddCommand = new RelayCommand(TapAdd);
            DialogConfirmCommand = new RelayCommand(TapDialogConfirm);
            DeleteCommand = new RelayCommand(TapDelete);
            Load();
        }

        private bool IsUpdated = false;

        private ObservableCollection<HostBindingItem> hostItems = new();

        public ObservableCollection<HostBindingItem> HostItems {
            get => hostItems;
            set => Set(ref hostItems, value);
        }

        private bool dialogVisible;

        public bool DialogVisible {
            get => dialogVisible;
            set => Set(ref dialogVisible, value);
        }

        private string inputContent = string.Empty;

        public string InputContent {
            get => inputContent;
            set => Set(ref inputContent, value);
        }

        public ICommand BackCommand { get; private set; }
        public ICommand AddCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand DialogConfirmCommand { get; private set; }

        private void TapBack(object? _)
        {
            ShellManager.BackAsync();
        }

        private void TapAdd(object? _)
        {
            DialogVisible = true;
        }


        private void TapDelete(object? arg)
        {
            if (arg is HostBindingItem item)
            {
                HostItems.Remove(item);
                IsUpdated = true;
            }
        }

        private void TapDialogConfirm(object? _)
        {
            foreach (var item in InputContent.Split(new char[] { '\r', '\n' }))
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                var line = item.Trim();
                if (line.StartsWith("#"))
                {
                    continue;
                }
                var args = line.Split(new char[] { ' ' }, 2);
                var ip = args[0].Trim();
                var host = args[2].Trim();
                if (string.IsNullOrWhiteSpace(ip) || 
                    string.IsNullOrWhiteSpace(host))
                {
                    continue;
                }
                Add(host, ip);
            }
            InputContent = string.Empty;
            DialogVisible = false;
        }

        public void Add(string host, string ip)
        {
            IsUpdated = true;
            foreach (var item in HostItems)
            {
                if (item.Host == host)
                {
                    item.Ip = ip;
                    return;
                }
            }
            HostItems.Add(new HostBindingItem(host, ip));
        }

        public bool Contains(string host)
        {
            foreach (var item in HostItems)
            {
                if (item.Host == host)
                {
                    return true;
                }
            }
            return false;
        }

        private void Load()
        {
            var project = App.ViewModel.Project;
            HostItems.Clear();
            if (project == null)
            {
                return;
            }
            foreach (var item in project.HostItems)
            {
                HostItems.Add(new HostBindingItem(item.Host, item.Ip));
            }
        }

        public void ApplyExitAttributes()
        {
            if (!IsUpdated)
            {
                return;
            }
            IsUpdated = false;
            var project = App.ViewModel.Project;
            if (project == null)
            {
                return;
            }
            project.HostItems = HostItems.Select(i => new HostItem(i.Host, i.Ip)).ToList();
            project.SaveAsync();
        }
    }
}
