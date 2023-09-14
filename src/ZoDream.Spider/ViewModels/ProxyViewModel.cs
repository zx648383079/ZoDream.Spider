using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ZoDream.Shared.Http;
using ZoDream.Shared.Models;
using ZoDream.Shared.Routes;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Spider.ViewModels
{
    public class ProxyViewModel : BindableBase, IExitAttributable
    {

        public ProxyViewModel()
        {
            BackCommand = new RelayCommand(TapBack);
            AddCommand = new RelayCommand(TapAdd);
            CheckCommand = new RelayCommand(TapCheck);
            DialogConfirmCommand = new RelayCommand(TapDialogConfirm);
            DeleteCommand = new RelayCommand(TapDelete);
            Load();
        }

        private bool IsUpdated = false;

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

        private async void TapCheck(object? _)
        {
            foreach (var item in UrlItems)
            {
                item.Status = UriCheckStatus.Doing;
                var proxy = new ProxyItem(item.Url);
                var res = await HttpProxy.TestAsync(proxy);
                item.Status = res ? UriCheckStatus.Done : UriCheckStatus.Error;
            }
        }

        private void TapDelete(object? arg)
        {
            if (arg is UrlCheckItem item)
            {
                UrlItems.Remove(item);
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
                var real = item.Trim();
                if (Contains(real))
                {
                    continue;
                }
                UrlItems.Add(new UrlCheckItem(real));
                IsUpdated = true;
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


        private void Load()
        {
            var project = App.ViewModel.Project;
            UrlItems.Clear();
            if (project == null)
            {
                return;
            }
            foreach (var item in project.ProxyItems)
            {
                UrlItems.Add(new UrlCheckItem(item));
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
            project.ProxyItems = UrlItems.Select(i => i.Url).ToList();
            project.SaveAsync();
        }
    }
}
