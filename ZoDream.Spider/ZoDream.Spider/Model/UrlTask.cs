using GalaSoft.MvvmLight;
using System.Text.RegularExpressions;

namespace ZoDream.Spider.Model
{
    public class UrlTask: ObservableObject
    {
        public string Url { get; set; }

        public string FileName { get; set; }

        public string FullName { get; set; }

        public Regex Pattern { get; set; }

        public AssetKind Kind { get; set; }

        public string RelativeUrl { get; set; }

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

        public UrlTask(string url, string file)
        {
            Url = url;
            FullName = file;
        }

        public UrlTask(string url, string file, string fileName)
        {
            Url = url;
            FullName = file;
            FileName = fileName;
        }

        public enum AssetKind
        {
            Html,
            Js,
            Css,
            Image,
            File
        }
    }
}
