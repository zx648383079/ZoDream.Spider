using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.Spider.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.Spider.Controls;assembly=ZoDream.Spider.Controls"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:RadioGroupPanel/>
    ///
    /// </summary>
    [TemplatePart(Name = PanelName, Type = typeof(Panel))]
    public class RadioGroupPanel : Control
    {
        const string PanelName = "PART_InnerPanel";
        static RadioGroupPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadioGroupPanel), new FrameworkPropertyMetadata(typeof(RadioGroupPanel)));
        }

        public object Value {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(RadioGroupPanel), 
                new FrameworkPropertyMetadata(null, (d, s) => {
                    (d as RadioGroupPanel)?.UpdateView();
                }) { BindsTwoWayByDefault = true});




        public object ItemsSource {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), 
                typeof(RadioGroupPanel), new PropertyMetadata(null, (d, s) => {
                    (d as RadioGroupPanel)?.UpdateView();
                }));


        private Panel? InnerPanel;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InnerPanel = GetTemplateChild(PanelName) as Panel;
            UpdateView();
        }

        private void UpdateView()
        {
            if (InnerPanel == null)
            {
                return;
            }
            if (Value is not RuleMatchType val)
            {
                InnerPanel.Children.Clear();
                return;
            }
            var items = Enum.GetValues<RuleMatchType>();
            var i = -1;
            foreach (var item in items)
            {
                i++;
                RadioGroupItem node;
                if (InnerPanel.Children.Count > i)
                {
                    node = InnerPanel.Children[i] as RadioGroupItem;
                } else
                {
                    node = new RadioGroupItem();
                    InnerPanel.Children.Add(node);
                }
                if (node is null)
                {
                    continue;
                }
                node.Text = FormatText(item);
                node.Value = item;
                node.IsChecked = item == val;
                node.MouseDown -= Node_MouseDown;
                node.MouseDown += Node_MouseDown;
            }
        }

        private void Node_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is RadioGroupItem o) {
                Value = o.Value;
            }
        }

        private static string FormatText(RuleMatchType type)
        {
            return type switch {
                RuleMatchType.All => "全部",
                RuleMatchType.Event =>  "事件",
                RuleMatchType.Regex => "网址正则匹配",
                RuleMatchType.Contains => "网址包含字符串",
                RuleMatchType.StartWith => "网址匹配前缀",
                RuleMatchType.Host =>  "匹配域名",
                RuleMatchType.Page => "仅网页",
                RuleMatchType.None => "单页",
                _ => "-",
            };
        }
    }
}
