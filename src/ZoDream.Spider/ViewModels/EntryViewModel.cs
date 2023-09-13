using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Shared.Models;
using ZoDream.Shared.Routes;
using ZoDream.Shared.Utils;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Spider.ViewModels
{
    public class EntryViewModel : BindableBase
    {

        public EntryViewModel()
        {
            BackCommand = new RelayCommand(TapBack);
            AddCommand = new RelayCommand(TapAdd);
            CheckCommand = new RelayCommand(TapCheck);
            DialogConfirmCommand = new RelayCommand(TapDialogConfirm);
            DeleteCommand = new RelayCommand(TapDelete);
            Load();
        }

        private ObservableCollection<UrlCheckItem> urlItems = new();

        public ObservableCollection<UrlCheckItem> UrlItems {
            get => urlItems;
            set => Set(ref urlItems, value);
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
        public ICommand CheckCommand { get; private set; }
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

        private void TapCheck(object? _)
        {
            //var items = ProxyItems;
            //foreach (var item in items)
            //{
            //    Debug.WriteLine(item.Host);
            //    Debug.WriteLine(await HttpProxy.TestAsync(item));
            //}
        }

        private void TapDelete(object? arg)
        {
            if (arg is UrlCheckItem item)
            {
                UrlItems.Remove(item);
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
                Add(item.Trim());
            }
            InputContent = string.Empty;
            DialogVisible = false;
        }

        public bool Contains(string url)
        {
            foreach (var item in UrlItems)
            {
                if (item.Url == url)
                {
                    return true;
                }
            }
            return false;
        }

        public void Add(string url)
        {
            if (Contains(url))
            {
                return;
            }
            UrlItems.Add(new UrlCheckItem(url));
            //foreach (var item in Html.GenerateUrl(url))
            //{
            //    if (Contains(item))
            //    {
            //        continue;
            //    }
            //    UrlItems.Add(new UrlCheckItem(item));
            //}
        }

        private void Load()
        {
            var project = App.ViewModel.Project;
            UrlItems.Clear();
            if (project == null)
            {
                return;
            }
            foreach (var item in project.EntryItems)
            {
                UrlItems.Add(new UrlCheckItem(item));
            }
        }
    }
}
