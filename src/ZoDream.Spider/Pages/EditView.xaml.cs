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
using System.Windows.Shapes;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;
using ZoDream.Spider.ViewModels;

namespace ZoDream.Spider.Pages
{
    /// <summary>
    /// EditView.xaml 的交互逻辑
    /// </summary>
    public partial class EditView : Window
    {

        public EditViewModel ViewModel = new EditViewModel();
        public EditView()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public SpiderOption Option
        {
            get
            {
                return new SpiderOption()
                {
                    MaxCount = int.Parse(CountTb.Text),
                    UseBrowser = BrowserCb.IsChecked == true,
                    TimeOut = int.Parse(WaitTb.Text),
                    WorkFolder = FolderTb.Text.Trim(),
                    HeaderItems = ViewModel.HeaderItems.ToList()
                };
            }
            set
            {
                CountTb.Text = value.MaxCount.ToString();
                WaitTb.Text = value.TimeOut.ToString();
                BrowserCb.IsChecked = value.UseBrowser;
                FolderTb.Text = value.WorkFolder;
                ViewModel.HeaderItems.Clear();
                foreach (var item in value.HeaderItems)
                {
                    ViewModel.HeaderItems.Add(item);
                }
            }
        }

        public IList<RuleGroupItem> Rules
        {
            get
            {
                return ViewModel.RuleItems.ToList();
            }
            set
            {
                ViewModel.RuleItems.Clear();
                foreach (var item in value)
                {
                    ViewModel.RuleItems.Add(item);
                }
            }
        }

        private void AddRuleBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new RuleView();
            if (page.ShowDialog() != true)
            {
                return;
            }
            ViewModel.RuleItems.Add(page.RuleGroup);
        }

        private void EditRuleBtn_Click(object sender, RoutedEventArgs e)
        {
            var selected = RuleBox.SelectedIndex;
            if (selected < 0)
            {
                return;
            }
            var page = new RuleView();
            page.RuleGroup = ViewModel.RuleItems[selected];
            if (page.ShowDialog() != true)
            {
                return;
            }
            ViewModel.RuleItems[selected] = page.RuleGroup;
        }

        private void RuleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as MenuItem).Header as string)
            {
                case "选中":
                    var items = new RuleGroupItem[RuleBox.SelectedItems.Count];
                    RuleBox.SelectedItems.CopyTo(items, 0);
                    ListExtension.Remove(ViewModel.RuleItems, items);
                    break;
                case "全部":
                    ViewModel.RuleItems.Clear();
                    break;
                default:
                    break;
            }
        }

        private void BrowserBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new BrowserView(true);
            if (page.ShowDialog() != true)
            {
                return;
            }
            var items = page.HeaderItems;
            foreach (var item in items)
            {
                ViewModel.AddHeader(item);
            }
        }



        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            var folder = new System.Windows.Forms.FolderBrowserDialog
            {
                SelectedPath = AppDomain.CurrentDomain.BaseDirectory,
                ShowNewFolderButton = false
            };
            if (folder.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            FolderTb.Text = folder.SelectedPath;
        }



        private int headerSelected = -1;

        private void tapHeaderClear()
        {
            HeaderValueTb.Text = HeaderTb.Text = "";
            headerSelected = -1;
        }

        private void HeaderSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(HeaderTb.Text) 
                || string.IsNullOrWhiteSpace(HeaderValueTb.Text))
            {
                return;
            }
            var item = new HeaderItem(HeaderTb.Text, HeaderValueTb.Text);
            if (headerSelected < 0)
            {
                ViewModel.AddHeader(item);
            } else
            {
                ViewModel.HeaderItems[headerSelected] = item;
            }
            tapHeaderClear();
            
        }

        private void HeaderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as MenuItem).Header as string)
            {
                case "新增":
                    tapHeaderClear();
                    break;
                case "修改":
                    headerSelected = HeaderBox.SelectedIndex;
                    if (headerSelected < 0)
                    {
                        tapHeaderClear();
                        return;
                    }
                    var item = ViewModel.HeaderItems[headerSelected];
                    HeaderTb.Text = item.Name;
                    HeaderValueTb.Text = item.Value;
                    break;
                case "选中":
                    var items = new HeaderItem[HeaderBox.SelectedItems.Count];
                    HeaderBox.SelectedItems.CopyTo(items, 0);
                    ListExtension.Remove(ViewModel.HeaderItems, items);
                    break;
                case "全部":
                    ViewModel.HeaderItems.Clear();
                    break;
                default:
                    break;
            }
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.RuleItems.Count < 0 || string.IsNullOrWhiteSpace(FolderTb.Text))
            {
                return;
            }
            DialogResult = true;
        }
    }
}
