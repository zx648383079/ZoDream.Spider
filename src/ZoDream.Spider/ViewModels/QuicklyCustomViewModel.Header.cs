using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Input;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.ViewModels
{
    public partial class QuicklyCustomViewModel
    {

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



        private bool headerDialogVisible;

        public bool HeaderDialogVisible {
            get => headerDialogVisible;
            set => Set(ref headerDialogVisible, value);
        }

        private string headerName = string.Empty;

        public string HeaderName {
            get => headerName;
            set => Set(ref headerName, value);
        }

        private string headerValue = string.Empty;

        public string HeaderValue {
            get => headerValue;
            set => Set(ref headerValue, value);
        }

        public ICommand HeaderDialogConfirmCommand { get; private set; }
        public ICommand HeaderDeleteCommand { get; private set; }

        private void TapHeaderDialogConfirm(object? _)
        {
            if (string.IsNullOrWhiteSpace(HeaderName) || string.IsNullOrWhiteSpace(HeaderValue))
            {
                return;
            }
            AddHeader(new HeaderItem(HeaderName, HeaderValue));
            HeaderDialogVisible = false;
        }
        private void TapHeaderDelete(object? arg)
        {
            if (arg is HeaderItem item)
            {
                HeaderItems.Remove(item);
            }
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
    }
}
