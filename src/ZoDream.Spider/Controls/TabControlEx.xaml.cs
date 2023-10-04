using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZoDream.Spider.Controls
{
    /// <summary>
    /// TabControlEx.xaml 的交互逻辑
    /// https://www.cnblogs.com/s0611163/p/9994594.html
    /// </summary>
    public partial class TabControlEx : TabControl
    {
        public TabControlEx()
        {
            InitializeComponent();
        }

        /// <summary>
        /// TabItem右键菜单源
        /// </summary>
        private TabItem? _contextMenuSource;


        private void tabItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void tabItem_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _contextMenuSource = (sender as Grid)?.TemplatedParent as TabItem;
            this.menu.PlacementTarget = sender as Grid;
            this.menu.Placement = PlacementMode.MousePoint;
            this.menu.IsOpen = true;
        }

        #region TabItem右键菜单点击事件
        private void menuItemClick(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as MenuItem;
            int data = Convert.ToInt32(btn?.CommandParameter.ToString());

            if (_contextMenuSource != null)
            {
                List<TabItem> tabItemList = new();
                if (data == 0)
                {
                    tabItemList.Add(_contextMenuSource);
                }
                if (data == 1)
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        var tabItem = Items[i] as TabItem;
                        if (tabItem != _contextMenuSource && tabItem is not null)
                        {
                            tabItemList.Add(tabItem);
                        }
                    }
                }
                if (data == 2)
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        var tabItem = Items[i] as TabItem;
                        if (tabItem != _contextMenuSource && tabItem is not null)
                        {
                            tabItemList.Add(tabItem);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (data == 3)
                {
                    for (int i = Items.Count - 1; i >= 0; i--)
                    {
                        var tabItem = Items[i] as TabItem;
                        if (tabItem != _contextMenuSource && tabItem is not null)
                        {
                            tabItemList.Add(tabItem);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                foreach (TabItem tabItem in tabItemList)
                {
                    CloseTabItem(tabItem);
                }
            }
        }
        #endregion

        private void btnTabItemClose_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var tmpParent = (btn?.Parent as Grid)?.TemplatedParent;
            var tabItem = tmpParent as TabItem;
            if (tabItem is null)
            {
                return;
            }
            CloseTabItem(tabItem);
        }

        #region 关闭TabItem
        /// <summary>
        /// 关闭TabItem
        /// </summary>
        private void CloseTabItem(TabItem tabItem)
        {
            //if (tabItem.Content is BrowserTabItem o)
            //{
                
            //    tabItem.Content = null;
            //}
            Items.Remove(tabItem);
        }
        #endregion

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (TabItem tabItem in e.RemovedItems)
            {
                Panel.SetZIndex(tabItem, 99);
            }
            foreach (TabItem tabItem in e.AddedItems)
            {
                Panel.SetZIndex(tabItem, 999);
            }
        }
    }
}
