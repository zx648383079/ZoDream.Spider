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
using ZoDream.Spider.ViewModels;

namespace ZoDream.Spider.Pages
{
    /// <summary>
    /// RuleView.xaml 的交互逻辑
    /// </summary>
    public partial class RuleView : Window
    {

        public RuleViewModel ViewModel = new RuleViewModel();
        public RuleView()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public RuleGroupItem RuleGroup
        {
            get
            {
                return new RuleGroupItem()
                {
                    Name = NameTb.Text.Trim(),
                    Rules = ViewModel.RuleItems.ToList()
                };
            }
            set
            {
                NameTb.Text = value.Name;
                ViewModel.RuleItems.Clear();
                foreach (var item in value.Rules)
                {
                    ViewModel.RuleItems.Add(item);
                }
            }
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTb.Text))
            {
                return;
            }
            if (ViewModel.RuleItems.Count < 1)
            {
                return;
            }
            DialogResult = true;
        }

        private void SaveRuleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PluginCb.SelectedIndex < 0)
            {
                return;
            }
            ViewModel.RuleItems.Add(new RuleItem() { 
                Name = ViewModel.PluginItems[PluginCb.SelectedIndex].Name,
                Param1 = Param1Tb.Text,
                Param2 = Param2Tb.Text,
            });
            tapClear();
        }

        private void tapClear()
        {
            PluginCb.SelectedIndex = -1;
            Param2Tb.Text = Param1Tb.Text = "";
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as MenuItem).Header as string)
            {
                case "新增":
                    tapClear();
                    break;
                case "修改":
                    var item = ViewModel.RuleItems[RuleBox.SelectedIndex];
                    PluginCb.SelectedIndex = ViewModel.PluginIndexOf(item.Name);
                    Param2Tb.Text = item.Param2;
                    Param1Tb.Text = item.Param1;
                    break;
                case "上移":
                    ViewModel.MoveUp(RuleBox.SelectedIndex);
                    break;
                case "下移":
                    ViewModel.MoveDown(RuleBox.SelectedIndex);
                    break;
                case "选中":
                    var items = new RuleItem[RuleBox.SelectedItems.Count];
                    RuleBox.SelectedItems.CopyTo(items, 0);
                    foreach (var i in items)
                    {
                        if (i == null)
                        {
                            continue;
                        }
                        ViewModel.RuleItems.Remove(i as RuleItem);
                    }
                    break;
                case "全部":
                    ViewModel.RuleItems.Clear();
                    break;
                default:
                    break;
            }
        }
    }
}
