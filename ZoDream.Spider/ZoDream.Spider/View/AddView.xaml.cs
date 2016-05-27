using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace ZoDream.Spider.View
{
    /// <summary>
    /// Description for AddView.
    /// </summary>
    public partial class AddView : Window
    {
        /// <summary>
        /// Initializes a new instance of the AddView class.
        /// </summary>
        public AddView()
        {
            InitializeComponent();
            Messenger.Default.Send(new NotificationMessageAction(null, Close), "closeAdd");
        }
    }
}