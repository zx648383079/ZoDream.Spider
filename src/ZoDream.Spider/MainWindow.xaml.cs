using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Local;
using ZoDream.Shared.Loggers;
using ZoDream.Shared.Models;
using ZoDream.Spider.Loggers;
using ZoDream.Spider.Pages;
using ZoDream.Spider.ViewModels;

namespace ZoDream.Spider
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel = App.ViewModel;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void AddUrlBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new EditUrlView();
            if (page.ShowDialog() != true)
            {
                return;
            }
            if (ViewModel.Instance == null)
            {
                return;
            }
            ViewModel.Instance.UrlProvider.Add(page.UrlItems);
        }

        private void EditTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Instance == null)
            {
                return;
            }
            var page = new EditView();
            page.Option = ViewModel.Instance.Option;
            page.Rules = ViewModel.Instance.RuleProvider.All();
            if (page.ShowDialog() != true)
            {
                return;
            }
            ViewModel.Instance!.Option = page.Option;
            ViewModel.Instance.RuleProvider.Add(page.Rules);
        }

        private void ProxyBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new ProxyView();
            if (page.ShowDialog() != true)
            {
                return;
            }
        }

        private void NewProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new EditView();
            if (page.ShowDialog() != true)
            {
                return;
            }
            ViewModel.Load();
            Title = "Undefined Project - ZoDream Spider";
            ViewModel.Instance!.Option = page.Option;
            ViewModel.Instance.RuleProvider.Add(page.Rules);
        }

        private void OpenProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            var open = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "爬虫项目文件|*.sp|所有文件|*.*",
                Title = "选择文件"
            };
            if (open.ShowDialog() != true)
            {
                return;
            }
            ViewModel.Load(open.FileName);
            Title = System.IO.Path.GetFileNameWithoutExtension(open.FileName) + " - ZoDream Spider";
        }

        private void SaveProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ViewModel.FileName))
            {
                SaveProjectAsBtn_Click(sender, e);
                return;
            }
            ViewModel.Save();
        }

        private void SaveProjectAsBtn_Click(object sender, RoutedEventArgs e)
        {
            var open = new Microsoft.Win32.SaveFileDialog
            {
                Title = "选择保存路径",
                Filter = "爬虫项目文件|*.sp|所有文件|*.*",
                FileName = "new spider project",
            };
            if (open.ShowDialog() != true)
            {
                return;
            }
            ViewModel.Save(open.FileName);
            Title = System.IO.Path.GetFileNameWithoutExtension(open.FileName) + " - ZoDream Spider";
        }

        private void UrlListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.ShowMessage(((sender as ListBox).SelectedItem as UriItem).FormatTip);
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Instance?.Start();
            progressBar.Value = 0;
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Instance?.Pause();
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Instance?.Stop();
        }

        private void ResumeBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Instance?.Resume();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as MenuItem).Header as string)
            {
                case "选中":
                    var items = new UriItem[UrlListBox.SelectedItems.Count];
                    UrlListBox.SelectedItems.CopyTo(items, 0);
                    ViewModel.Instance?.UrlProvider.Remove(items);
                    break;
                case "已完成":
                    ViewModel.Instance?.UrlProvider.Remove(UriStatus.DONE);
                    break;
                case "全部":
                    ViewModel.Instance?.UrlProvider.Clear();
                    break;
                default:
                    break;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var height = this.ActualHeight;
            InfoTb.MinHeight = Math.Max(60, height / 5);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var logger = new EventLogger();
            logger.OnLog += (s, e) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    InfoTb.AppendLine(s);
                });
            };
            logger.OnProgress += (s, e) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    progressBar.Value = s * 100 / e;
                });
            };
            ViewModel.Logger = logger;
        }

        private void DebugBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UrlListBox.SelectedIndex < 0)
            {
                MessageBox.Show("请选择要调试的网址");
                return;
            }
            var item = ViewModel.UrlItems[UrlListBox.SelectedIndex];
            var brower = ViewModel.BroswerRequest;
            brower.BrowserFlag = BrowserFlags.DEBUG;
            brower.NavigateUrl(item.Source);
            brower.OnConfirm += Brower_OnConfirm;
        }

        private async void Brower_OnConfirm(object sender)
        {
            var spider = ViewModel.Instance;
            if (spider == null)
            {
                return;
            }
            var brower = sender as BrowserView;
            spider.UrlProvider.Add(brower.Source);
            await spider.InvokeAsync(brower.Source, await brower.GetHtmlAsync());
            brower.OnConfirm -= Brower_OnConfirm;
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.IsNotEmpty)
            {
                OpenProjectBtn_Click(sender, e);
                return;
            }
            StartBtn_Click(sender, e);
        }

        private void PauseBtn1_Click(object sender, RoutedEventArgs e)
        {
            PauseBtn_Click(sender, e);
        }

        private void StopBtn1_Click(object sender, RoutedEventArgs e)
        {
            StopBtn_Click(sender, e);
        }

        private void AboutBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new AboutView();
            page.Show();
        }
    }
}
