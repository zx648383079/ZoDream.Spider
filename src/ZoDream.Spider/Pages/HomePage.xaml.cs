using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Spider.Loggers;
using ZoDream.Spider.ViewModels;

namespace ZoDream.Spider.Pages
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            Loaded += HomePage_Loaded;
        }

        private HomeViewModel ViewModel => (HomeViewModel)DataContext;

        private void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            ToggleDebug(App.ViewModel.Option.IsLogVisible);
            if (ViewModel.Logger is not EventLogger logger)
            {
                return;
            }
            var isLastProgress = false;
            logger.OnLog += (s, e) => {
                isLastProgress = false;
                App.ViewModel.DispatcherQueue.Invoke(() => {
                    InfoTb.AppendLine(s, App.ViewModel.Option.IsLogTime);
                });
            };
            logger.OnProgress += (s, e, msg) => {
                App.ViewModel.DispatcherQueue.Invoke(() => {
                    if (isLastProgress)
                    {
                        InfoTb.ReplaceLine($"{s}/{e} {msg}");
                    }
                    else
                    {
                        InfoTb.AppendLine($"{s}/{e} {msg}");
                    }
                });
                isLastProgress = true;
            };
        }

        public UriLoadItem[] SelectedItems {
            get {
                var items = new UriLoadItem[UrlListBox.SelectedItems.Count];
                UrlListBox.SelectedItems.CopyTo(items, 0);
                return items;
            }
        }

        public void ToggleDebug(bool val)
        {
            if (val)
            {
                RowDef.Height = new GridLength(Math.Max(60, ActualHeight / 5));
                Splitter.Visibility = InfoTb.Visibility = Visibility.Visible;
            } else
            {
                RowDef.Height = new GridLength(1);
                Splitter.Visibility = InfoTb.Visibility = Visibility.Collapsed;
            }
        }

        private void UrlListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedItem = e.AddedItems.Count > 0 ? e.AddedItems[0] as UriLoadItem : null;
            ViewModel.SelectedItems = SelectedItems;
        }
    }
}
