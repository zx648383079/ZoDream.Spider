using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace ZoDream.Spider.View
{
    /// <summary>
    /// Description for RuleView.
    /// </summary>
    public partial class RuleView : Window
    {
        /// <summary>
        /// Initializes a new instance of the RuleView class.
        /// </summary>
        public RuleView()
        {
            InitializeComponent();
            Messenger.Default.Send(new NotificationMessageAction(null, Close), "close");
        }
    }
}