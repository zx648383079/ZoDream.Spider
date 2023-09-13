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
using ZoDream.Shared.Controls;

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
    ///     <MyNamespace:LargeIconRadio/>
    ///
    /// </summary>
    public class LargeIconRadio : ButtonBase
    {
        static LargeIconRadio()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LargeIconRadio), new FrameworkPropertyMetadata(typeof(LargeIconRadio)));
        }

        public string Icon {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(LargeIconRadio), new PropertyMetadata(string.Empty));




        public string Header {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(LargeIconRadio), new PropertyMetadata(string.Empty));



        public string Meta {
            get { return (string)GetValue(MetaProperty); }
            set { SetValue(MetaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Meta.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MetaProperty =
            DependencyProperty.Register("Meta", typeof(string), typeof(LargeIconRadio), new PropertyMetadata(string.Empty));




        public double IconFontSize {
            get { return (double)GetValue(IconFontSizeProperty); }
            set { SetValue(IconFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconFontSizeProperty =
            DependencyProperty.Register("IconFontSize", typeof(double), typeof(LargeIconRadio), new PropertyMetadata(40.0));



        public double MetaFontSize {
            get { return (double)GetValue(MetaFontSizeProperty); }
            set { SetValue(MetaFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MetaFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MetaFontSizeProperty =
            DependencyProperty.Register("MetaFontSize", typeof(double), typeof(LargeIconRadio), new PropertyMetadata(12.0));
    }
}
