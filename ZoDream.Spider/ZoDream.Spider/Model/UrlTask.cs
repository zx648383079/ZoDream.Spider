using GalaSoft.MvvmLight;

namespace ZoDream.Spider.Model
{
    public class UrlTask: ObservableObject
    {
        public string Url { get; set; }

        private UrlStatus _status;

        /// <summary>
        /// Sets and gets the Status property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public UrlStatus Status
        {
            get
            {
                return _status;
            }

            set
            {
                if (_status == value)
                {
                    return;
                }

                _status = value;
                RaisePropertyChanged("Status");
            }
        }

        public UrlTask()
        {

        }

        public UrlTask(string url)
        {
            Url = url;
        }
    }
}
