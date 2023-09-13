using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
    ///     <MyNamespace:StepPanel/>
    ///
    /// </summary>
    public class StepPanel : Control
    {
        static StepPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StepPanel), new FrameworkPropertyMetadata(typeof(StepPanel)));
        }



        public IList<string> ItemsSource {
            get { return (IList<string>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList<string>), typeof(StepPanel), 
                new PropertyMetadata(null, (d, s) => {
                    (d as StepPanel)?.InvalidateVisual();
                }));




        public double Gap {
            get { return (double)GetValue(GapProperty); }
            set { SetValue(GapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Gap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GapProperty =
            DependencyProperty.Register("Gap", typeof(double), typeof(StepPanel), 
                new PropertyMetadata(5.0, (d, s) => {
                    (d as StepPanel)?.InvalidateVisual();
                }));



        public int SelectedIndex {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(StepPanel), 
                new PropertyMetadata(-1, (d, s) => {
                    (d as StepPanel)?.InvalidateVisual();
                }));




        public Brush SelectedBackground {
            get { return (Brush)GetValue(SelectedBackgroundProperty); }
            set { SetValue(SelectedBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedBackgroundProperty =
            DependencyProperty.Register("SelectedBackground", typeof(Brush), typeof(StepPanel), new PropertyMetadata(null));




        public Brush SelectedForeground {
            get { return (Brush)GetValue(SelectedForegroundProperty); }
            set { SetValue(SelectedForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty.Register("SelectedForeground", typeof(Brush), typeof(StepPanel), new PropertyMetadata(null));




        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (ActualWidth <= 0 || ActualHeight <= 0 || ItemsSource is null || ItemsSource.Count < 1)
            {
                return;
            }
            var itemWidth = (ActualWidth - (ItemsSource.Count - 1) * Gap) / ItemsSource.Count;
            var halfHeight = ActualHeight / 2;
            var cornerWidth = Math.Min(Math.Tan((30 * Math.PI) / 180) * halfHeight, halfHeight);
            var pen = new Pen(BorderBrush, BorderThickness.Left);
            var font = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            for (int i = 0; i < ItemsSource.Count; i++)
            {
                var item = ItemsSource[i];
                var path = new StreamGeometry();
                var x = (itemWidth + Gap) * i;
                var middleRight = x + itemWidth;
                var topRight = middleRight - cornerWidth;
                using (var ctx = path.Open())
                {
                    if (i < 1)
                    {
                        ctx.BeginFigure(new Point(x, 0), true, true);
                        ctx.LineTo(new Point(topRight, 0), true, false);
                        ctx.LineTo(new Point(middleRight, halfHeight), true, false);
                        ctx.LineTo(new Point(topRight, ActualHeight), true, false);
                        ctx.LineTo(new Point(x, ActualHeight), true, false);
                    } else
                    {
                        var topLeft = x - cornerWidth;
                        ctx.BeginFigure(new Point(topLeft, 0), true, true);
                        ctx.LineTo(new Point(topRight, 0), true, false);
                        ctx.LineTo(new Point(x + itemWidth, halfHeight), true, false);
                        ctx.LineTo(new Point(topRight, ActualHeight), true, false);
                        ctx.LineTo(new Point(topLeft, ActualHeight), true, false);
                        ctx.LineTo(new Point(x, halfHeight), true, false);
                    }
                }
                path.Freeze();
                drawingContext.DrawGeometry(i == SelectedIndex && SelectedBackground is not null ? SelectedBackground : Background, 
                    pen, path);
                var format = new FormattedText(item, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                            font, FontSize, i == SelectedIndex && SelectedForeground is not null ? SelectedForeground : Foreground, 
                            1.25);
                drawingContext.DrawText(format, new Point(
                    Math.Max(x, (x + topRight - format.Width) / 2), 
                    halfHeight - format.Height / 2
                    ));
            }
        }

    }
}
