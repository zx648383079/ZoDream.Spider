using System.Collections.Generic;
using System.Diagnostics;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ZoDream.Spider.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class UrlViewModel : ViewModelBase
    {
        private NotificationMessageAction _close;
        private NotificationMessageAction<IList<string>> _callback;

        /// <summary>
        /// Initializes a new instance of the UrlViewModel class.
        /// </summary>
        public UrlViewModel()
        {
            Messenger.Default.Register<NotificationMessageAction<IList<string>>>(this, "url", m =>
            {
                _callback = m;
            });
            Messenger.Default.Register<NotificationMessageAction>(this, "closeAdd", m =>
            {
                _close = m;
            });
        }

        /// <summary>
        /// The <see cref="Url" /> property's name.
        /// </summary>
        public const string UrlPropertyName = "Url";

        private string _url = string.Empty;

        /// <summary>
        /// Sets and gets the Url property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                Set(UrlPropertyName, ref _url, value);
            }
        }

        private RelayCommand _yesCommand;

        /// <summary>
        /// Gets the YesCommand.
        /// </summary>
        public RelayCommand YesCommand => _yesCommand
                                          ?? (_yesCommand = new RelayCommand(ExecuteYesCommand));

        private void ExecuteYesCommand()
        {
            _callback.Execute(Url.Split('\n'));
            _close.Execute();
        }
    }
}