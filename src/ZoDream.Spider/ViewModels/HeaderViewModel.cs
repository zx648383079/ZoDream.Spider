using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows.Input;
using ZoDream.Shared.Models;
using ZoDream.Shared.Routes;
using ZoDream.Shared.ViewModel;
using ZoDream.Spider.Pages;

namespace ZoDream.Spider.ViewModels
{
    public class HeaderViewModel: BindableBase, IExitAttributable
    {

        public HeaderViewModel()
        {
            BackCommand = new RelayCommand(TapBack);
            AddCommand = new RelayCommand(TapAdd);
            GetCommand = new RelayCommand(TapGet);
            DialogConfirmCommand = new RelayCommand(TapDialogConfirm);
            DeleteCommand = new RelayCommand(TapDelete);
            Load();
        }

        private bool IsUpdated = false;

        private Array headerKeys = Enum.GetNames(typeof(HttpRequestHeader));
        public Array HeaderKeys {
            get {
                return headerKeys;
            }
            set {
                Set(ref headerKeys, value);
            }
        }

        private ObservableCollection<HeaderBindingItem> headerItems = new();

        public ObservableCollection<HeaderBindingItem> HeaderItems {
            get => headerItems;
            set => Set(ref headerItems, value);
        }


        private bool dialogVisible;

        public bool DialogVisible {
            get => dialogVisible;
            set => Set(ref dialogVisible, value);
        }

        private string inputName = string.Empty;

        public string InputName {
            get => inputName;
            set => Set(ref inputName, value);
        }

        private string inputValue = string.Empty;

        public string InputValue {
            get => inputValue;
            set => Set(ref inputValue, value);
        }

        public ICommand BackCommand { get; private set; }
        public ICommand AddCommand { get; private set; }
        public ICommand GetCommand { get; private set; }
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

        private void TapGet(object? _)
        {
            var page = new BrowserDebugView(BrowserFlags.CONFIRM);
            if (page.ShowDialog() != true)
            {
                return;
            }
            var items = page.HeaderItems;
            if (items is null)
            {
                return;
            }
            foreach (var item in items)
            {
                AddHeader(item.Name, item.Value);
            }
        }

        private void TapDelete(object? arg)
        {
            if (arg is HeaderBindingItem item)
            {
                HeaderItems.Remove(item);
                IsUpdated = true;
            }
        }

        private void TapDialogConfirm(object? _)
        {
            if (string.IsNullOrWhiteSpace(InputName) || string.IsNullOrWhiteSpace(InputValue))
            {
                return;
            }
            AddHeader(InputName, InputValue);
            DialogVisible = false;
        }

        public void AddHeader(string name, string val)
        {
            var i = HeaderIndexOf(name);
            if (i < 0)
            {
                HeaderItems.Add(new HeaderBindingItem(name, val));
            }
            else
            {
                HeaderItems[i].Value = val;
            }
            IsUpdated = true;
        }
        public int HeaderIndexOf(string name)
        {
            for (int i = 0; i < HeaderItems.Count; i++)
            {
                if (name == HeaderItems[i].Name)
                {
                    return i;
                }
            }
            return -1;
        }

        private void Load()
        {
            var project = App.ViewModel.Project;
            HeaderItems.Clear();
            if (project == null)
            {
                return;
            }
            foreach (var item in project.HeaderItems)
            {
                HeaderItems.Add(new HeaderBindingItem(item));
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
            project.HeaderItems = HeaderItems.Select(i => new HeaderItem(i.Name, i.Value)).ToList();
            project.SaveAsync();
        }
    }
}
