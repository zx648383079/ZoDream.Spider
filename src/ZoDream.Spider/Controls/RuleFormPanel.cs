using Newtonsoft.Json.Linq;
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
using ZoDream.Shared.Controls;
using ZoDream.Shared.Form;
using ZoDream.Shared.Interfaces;
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
    ///     <MyNamespace:RuleFormPanel/>
    ///
    /// </summary>
    [TemplatePart(Name = PanelName, Type = typeof(Panel))]
    public class RuleFormPanel : Control
    {
        const string PanelName = "PART_InputPanel";
        static RuleFormPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RuleFormPanel), new FrameworkPropertyMetadata(typeof(RuleFormPanel)));
        }

        public IEnumerable<IFormInput> InputItems {
            get { return (IEnumerable<IFormInput>)GetValue(InputItemsProperty); }
            set { SetValue(InputItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputItemsProperty =
            DependencyProperty.Register("InputItems", typeof(IEnumerable<IFormInput>), 
                typeof(RuleFormPanel), new PropertyMetadata(null, (d, s) => {
                    (d as RuleFormPanel)?.UpdateView();
                }));


        public IList<DataItem> DataItems {
            get { return (IList<DataItem>)GetValue(DataItemsProperty); }
            set { SetValue(DataItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataItemsProperty =
            DependencyProperty.Register("DataItems", typeof(IList<DataItem>), 
                typeof(RuleFormPanel), new FrameworkPropertyMetadata(null, (d, s) => {
                    (d as RuleFormPanel)?.UpdateView();
                }) { BindsTwoWayByDefault = true});

        private Panel? InputPanel;
        private bool IsUpdated = false;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InputPanel = GetTemplateChild(PanelName) as Panel; ;
            UpdateView();
        }

        private void UpdateView()
        {
            if (InputPanel is null || IsUpdated)
            {
                return;
            }
            if (InputItems is null)
            {
                InputPanel.Children.Clear();
                return;
            }
            var i = -1;
            foreach (var item in InputItems)
            {
                i++;
                var j = 2 * i;
                AddLabel(InputPanel.Children, item, j);
                AddInput(InputPanel.Children, item, j + 1);
            }
            i++;
            InputPanel.Children.RemoveRange(2 * i, InputPanel.Children.Count - 2 * i);
        }

        private void AddInput(UIElementCollection children, IFormInput item, int i)
        {
            if (item is Text)
            {
                AddTextInput(children, item, i);
                return;
            }
            if (item is File)
            {
                AddFileInput(children, item, i);
                return;
            }
            if (item is Numeric)
            {
                AddNumericInput(children, item, i);
                return;
            }
            if (item is Switch)
            {
                AddSwitchInput(children, item, i);
                return;
            }
            if (item is Select o)
            {
                AddSelectInput(children, o, i);
                return;
            }
        }

        private void AddTextInput(UIElementCollection children, IFormInput item, int i)
        {
            var ctl = new TextBox()
            {
                Text = GetValue<string>(item.Name) ?? string.Empty,
                MaxLines = 1,
                VerticalContentAlignment = VerticalAlignment.Center,
                Height = 32,
            };
            ctl.TextChanged += (s, o) => {
                UpdateValue(item.Name, ctl.Text.Trim());
            };
            if (children.Count <= i)
            {
                children.Add(ctl);
                return;
            }
            children.RemoveAt(i);
            children.Insert(i, ctl);
        }

        private void AddFileInput(UIElementCollection children, IFormInput item, int i)
        {
            var ctl = new FileInput()
            {
                FileName = GetValue<string>(item.Name) ?? string.Empty,
                Height = 32,
            };
            ctl.FileChanged += (s, o) => {
                UpdateValue(item.Name, ctl.FileName.Trim());
            };
            if (children.Count <= i)
            {
                children.Add(ctl);
                return;
            }
            children.RemoveAt(i);
            children.Insert(i, ctl);
        }

        private void AddNumericInput(UIElementCollection children, IFormInput item, int i)
        {
            var ctl = new NumberInput()
            {
                Value = GetValue<long>(item.Name),
                Height = 32,
            };
            ctl.ValueChanged += (s, o) => {
                UpdateValue(item.Name, ctl.Value);
            };
            if (children.Count <= i)
            {
                children.Add(ctl);
                return;
            }
            children.RemoveAt(i);
            children.Insert(i, ctl);
        }

        private void AddSwitchInput(UIElementCollection children, IFormInput item, int i)
        {
            var ctl = new SwitchInput()
            {
                Value = GetValue<bool>(item.Name),
                Height = 32,
            };
            ctl.ValueChanged += (s, o) => {
                UpdateValue(item.Name, ctl.Value);
            };
            if (children.Count <= i)
            {
                children.Add(ctl);
                return;
            }
            children.RemoveAt(i);
            children.Insert(i, ctl);
        }

        private void AddSelectInput(UIElementCollection children, Select item, int i)
        {
            var val = GetValue<object>(item.Name);
            var ctl = new ComboBox()
            {
                ItemsSource = item.Items,
                DisplayMemberPath = "Name",
                Height = 32,
            };
            for (int j = 0; j < item.Items.Length; j++)
            {
                if (item.Items[j].Value == val)
                {
                    ctl.SelectedIndex = j;
                }
            }
            ctl.SelectionChanged += (s, o) => {
                UpdateValue(item.Name, item.Items[ctl.SelectedIndex].Value);
            };
            if (children.Count <= i)
            {
                children.Add(ctl);
                return;
            }
            children.RemoveAt(i);
            children.Insert(i, ctl);
        }

        private void AddLabel(UIElementCollection children, IFormInput item, int i)
        {
            if (children.Count > i)
            {
                (children[i] as TextBlock).Text = item.Label;
            }
            else
            {
                children.Add(new TextBlock()
                {
                    Text = item.Label,
                    Margin = new Thickness(0, 10, 0, 0)
                });
            }
        }

        private T? GetValue<T>(string key)
        {
            if (DataItems is null)
            {
                return default;
            }
            foreach (var item in DataItems)
            {
                if (item.Name == key)
                {
                    return (T)item.Value;
                }
            }
            return default;
        }

        private void UpdateValue(string key, object value)
        {
            if (DataItems is null)
            {
                DataItems = new List<DataItem>() {
                    new(key, value)
                };
                return;
            }
            var found = false;
            foreach(var item in DataItems)
            {
                if (item.Name == key)
                {
                    item.Value = value;
                    found = true;
                }
            }
            if (!found)
            {
                DataItems.Add(new(key, value));
            }
            IsUpdated = true;
            SetCurrentValue(DataItemsProperty, DataItems);
            IsUpdated = false;
        }
    }
}
