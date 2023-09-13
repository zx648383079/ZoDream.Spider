using System.Collections.ObjectModel;
using System.Windows.Input;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

namespace ZoDream.Spider.ViewModels
{
    public partial class QuicklyCustomViewModel
    {

        private ObservableCollection<UrlCheckItem> proxyItems = new();

        public ObservableCollection<UrlCheckItem> ProxyItems {
            get => proxyItems;
            set => Set(ref proxyItems, value);
        }


        private bool proxyDialogVisible;

        public bool ProxyDialogVisible {
            get => proxyDialogVisible;
            set => Set(ref proxyDialogVisible, value);
        }

        private string proxyContent = string.Empty;

        public string ProxyContent {
            get => proxyContent;
            set => Set(ref proxyContent, value);
        }

        public ICommand ProxyDialogConfirmCommand { get; private set; }
        public ICommand ProxyDeleteCommand { get; private set; }

        private void TapProxyDialogConfirm(object? _)
        {
            foreach (var item in ProxyContent.Split(new char[] { '\r', '\n' }))
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                var real = item.Trim();
                if (ListExtension.IndexOf(ProxyItems, real) >= 0)
                {
                    continue;
                }
                ProxyItems.Add(new UrlCheckItem(real));
            }
            ProxyContent = string.Empty;
            ProxyDialogVisible = false;
        }
        private void TapProxyDelete(object? arg)
        {
            if (arg is UrlCheckItem item)
            {
                ProxyItems.Remove(item);
            }
        }
    }
}
