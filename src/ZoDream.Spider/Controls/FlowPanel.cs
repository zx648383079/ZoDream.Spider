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
    ///     <MyNamespace:FlowPanel/>
    ///
    /// </summary>
    public class FlowPanel : ContentControl
    {
        static FlowPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlowPanel), new FrameworkPropertyMetadata(typeof(FlowPanel)));
        }

        public FlowPanel()
        {
            BackCommand ??= new RelayCommand(_ => {
                IsOpen = false;
            });
        }


        public string Title {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(FlowPanel), new PropertyMetadata(string.Empty));




        public string SubTitle {
            get { return (string)GetValue(SubTitleProperty); }
            set { SetValue(SubTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubTitleProperty =
            DependencyProperty.Register("SubTitle", typeof(string), typeof(FlowPanel), new PropertyMetadata(string.Empty));



        public bool BackVisible {
            get { return (bool)GetValue(BackVisibleProperty); }
            set { SetValue(BackVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackVisibleProperty =
            DependencyProperty.Register("BackVisible", typeof(bool), typeof(FlowPanel), new PropertyMetadata(false));



        public bool AddVisible {
            get { return (bool)GetValue(AddVisibleProperty); }
            set { SetValue(AddVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddVisibleProperty =
            DependencyProperty.Register("AddVisible", typeof(bool), typeof(FlowPanel), new PropertyMetadata(false));




        public Brush MaskColor {
            get { return (Brush)GetValue(MaskColorProperty); }
            set { SetValue(MaskColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaskColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaskColorProperty =
            DependencyProperty.Register("MaskColor", typeof(Brush), typeof(FlowPanel), new PropertyMetadata(null));


        public bool IsOpen {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(FlowPanel),
                new FrameworkPropertyMetadata(false, (d, s) => {
                    (d as FlowPanel)!.Visibility = (bool)s.NewValue ? Visibility.Visible : Visibility.Collapsed;
                })
                { BindsTwoWayByDefault = true });

        public ICommand BackCommand {
            get { return (ICommand)GetValue(BackCommandProperty); }
            set { SetValue(BackCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackCommandProperty =
            DependencyProperty.Register("BackCommand", typeof(ICommand), typeof(FlowPanel), new PropertyMetadata(null));



        public ICommand AddCommand {
            get { return (ICommand)GetValue(AddCommandProperty); }
            set { SetValue(AddCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(FlowPanel), new PropertyMetadata(null));


    }
}
