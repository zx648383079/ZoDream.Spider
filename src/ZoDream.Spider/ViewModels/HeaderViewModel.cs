using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Shared.Models;
using ZoDream.Shared.Routes;
using ZoDream.Shared.Utils;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Spider.ViewModels
{
    public class HeaderViewModel: BindableBase
    {

        public HeaderViewModel()
        {
            BackCommand = new RelayCommand(TapBack);
            AddCommand = new RelayCommand(TapAdd);
            DialogConfirmCommand = new RelayCommand(TapDialogConfirm);
            DeleteCommand = new RelayCommand(TapDelete);
            Load();
        }

        private Array headerKeys = Enum.GetNames(typeof(HttpRequestHeader));
        public Array HeaderKeys {
            get {
                return headerKeys;
            }
            set {
                Set(ref headerKeys, value);
            }
        }

        private ObservableCollection<HeaderItem> headerItems = new();

        public ObservableCollection<HeaderItem> HeaderItems {
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
            if (arg is HeaderItem item)
            {
                HeaderItems.Remove(item);
            }
        }

        private void TapDialogConfirm(object? _)
        {
            if (string.IsNullOrWhiteSpace(InputName) || string.IsNullOrWhiteSpace(InputValue))
            {
                return;
            }
            AddHeader(new HeaderItem(InputName, InputValue));
            DialogVisible = false;
        }

        public void AddHeader(HeaderItem item)
        {
            var i = HeaderIndexOf(item.Name);
            if (i < 0)
            {
                HeaderItems.Add(item);
            }
            else
            {
                HeaderItems[i] = item;
            }
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
                HeaderItems.Add(item);
            }
        }
    }
}
