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
using GalaSoft.MvvmLight.Messaging;

namespace ZoDream.Spider.View
{
    /// <summary>
    /// UrlView.xaml 的交互逻辑
    /// </summary>
    public partial class UrlView : Window
    {
        public UrlView()
        {
            InitializeComponent();
            Messenger.Default.Send(new NotificationMessageAction(null, Close), "closeAdd");
        }
        
    }
}
