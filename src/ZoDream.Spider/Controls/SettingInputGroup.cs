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
using ZoDream.Shared.ViewModel;

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
    ///     <MyNamespace:SettingInputGroup/>
    ///
    /// </summary>
    [TemplatePart(Name = HeaderName, Type = typeof(FrameworkElement))]
    public class SettingInputGroup : ContentControl
    {
        const string HeaderName = "PART_HeaderButton";
        static SettingInputGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingInputGroup), new FrameworkPropertyMetadata(typeof(SettingInputGroup)));
        }

        public string Icon {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(SettingInputGroup), new PropertyMetadata(string.Empty));




        public string Header {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(SettingInputGroup), new PropertyMetadata(string.Empty));



        public string Meta {
            get { return (string)GetValue(MetaProperty); }
            set { SetValue(MetaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Meta.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MetaProperty =
            DependencyProperty.Register("Meta", typeof(string), typeof(SettingInputGroup), new PropertyMetadata(string.Empty));




        public double IconFontSize {
            get { return (double)GetValue(IconFontSizeProperty); }
            set { SetValue(IconFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconFontSizeProperty =
            DependencyProperty.Register("IconFontSize", typeof(double), typeof(SettingInputGroup), new PropertyMetadata(40.0));



        public double MetaFontSize {
            get { return (double)GetValue(MetaFontSizeProperty); }
            set { SetValue(MetaFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MetaFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MetaFontSizeProperty =
            DependencyProperty.Register("MetaFontSize", typeof(double), typeof(SettingInputGroup), new PropertyMetadata(12.0));




        public bool IsOpen {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(SettingInputGroup), 
                new FrameworkPropertyMetadata(false)
                {
                    BindsTwoWayByDefault = true,
                });


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild(HeaderName) is FrameworkElement header)
            {
                header.MouseDown += (o, e) => {
                    IsOpen = !IsOpen;
                };
            }
        }

    }
}
