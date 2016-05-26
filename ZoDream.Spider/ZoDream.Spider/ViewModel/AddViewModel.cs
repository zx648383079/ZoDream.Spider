using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using ZoDream.Spider.Model;
using ZoDream.Spider.View;

namespace ZoDream.Spider.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class AddViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the AddViewModel class.
        /// </summary>
        public AddViewModel()
        {
        }

        /// <summary>
        /// The <see cref="BaseUrl" /> property's name.
        /// </summary>
        public const string BaseUrlPropertyName = "BaseUrl";

        private string _baseUrl = string.Empty;

        /// <summary>
        /// Sets and gets the BaseUrl property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string BaseUrl
        {
            get
            {
                return _baseUrl;
            }
            set
            {
                Set(BaseUrlPropertyName, ref _baseUrl, value);
            }
        }

        /// <summary>
        /// The <see cref="Count" /> property's name.
        /// </summary>
        public const string CountPropertyName = "Count";

        private int _count = 100;

        /// <summary>
        /// Sets and gets the Count property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                Set(CountPropertyName, ref _count, value);
            }
        }

        /// <summary>
        /// The <see cref="UrlList" /> property's name.
        /// </summary>
        public const string UrlListPropertyName = "UrlList";

        private ObservableCollection<UrlItem> _urlList = new ObservableCollection<UrlItem>();

        /// <summary>
        /// Sets and gets the UrlList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<UrlItem> UrlList
        {
            get
            {
                return _urlList;
            }
            set
            {
                Set(UrlListPropertyName, ref _urlList, value);
            }
        }


        private RelayCommand _importCommand;

        /// <summary>
        /// Gets the ImportCommand.
        /// </summary>
        public RelayCommand ImportCommand
        {
            get
            {
                return _importCommand
                    ?? (_importCommand = new RelayCommand(ExecuteImportCommand));
            }
        }

        private void ExecuteImportCommand()
        {

        }


        private RelayCommand _exportCommand;

        /// <summary>
        /// Gets the ExportCommand.
        /// </summary>
        public RelayCommand ExportCommand
        {
            get
            {
                return _exportCommand
                    ?? (_exportCommand = new RelayCommand(ExecuteExportCommand));
            }
        }

        private void ExecuteExportCommand()
        {

        }

        private RelayCommand _addCommand;

        /// <summary>
        /// Gets the AddCommand.
        /// </summary>
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand
                    ?? (_addCommand = new RelayCommand(ExecuteAddCommand));
            }
        }

        private void ExecuteAddCommand()
        {
            new RuleView().Show();
            Messenger.Default.Send(new NotificationMessageAction<UrlItem>(null, item=> {
                UrlList.Add(new UrlItem(item.Url, item.Rults));
            }));
        }

        private RelayCommand<int> _editCommand;

        /// <summary>
        /// Gets the EditCommand.
        /// </summary>
        public RelayCommand<int> EditCommand
        {
            get
            {
                return _editCommand
                    ?? (_editCommand = new RelayCommand<int>(ExecuteEditCommand));
            }
        }

        private void ExecuteEditCommand(int index)
        {
            if (index < 0 || index >= UrlList.Count) return;
            new RuleView().Show();

            Messenger.Default.Send(new NotificationMessageAction<UrlItem>(UrlList[index], null, item=> {
                UrlList[index] = item;
            }));
        }

        private RelayCommand<int> _deleteCommand;

        /// <summary>
        /// Gets the DeleteCommand.
        /// </summary>
        public RelayCommand<int> DeleteCommand
        {
            get
            {
                return _deleteCommand
                    ?? (_deleteCommand = new RelayCommand<int>(ExecuteDeleteCommand));
            }
        }

        private void ExecuteDeleteCommand(int index)
        {
            if (index < 0 || index >= HeaderList.Count) return;
            UrlList.RemoveAt(index);
        }

        private RelayCommand _clearCommand;

        /// <summary>
        /// Gets the ClearCommand.
        /// </summary>
        public RelayCommand ClearCommand
        {
            get
            {
                return _clearCommand
                    ?? (_clearCommand = new RelayCommand(ExecuteClearCommand));
            }
        }

        private void ExecuteClearCommand()
        {
            UrlList.Clear();
        }

        /// <summary>
        /// The <see cref="HeaderList" /> property's name.
        /// </summary>
        public const string HeaderListPropertyName = "HeaderList";

        private ObservableCollection<HeaderItem> _headerList = new ObservableCollection<HeaderItem>();

        /// <summary>
        /// Sets and gets the HeaderList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<HeaderItem> HeaderList
        {
            get
            {
                return _headerList;
            }
            set
            {
                Set(HeaderListPropertyName, ref _headerList, value);
            }
        }

        /// <summary>
        /// The <see cref="HeaderName" /> property's name.
        /// </summary>
        public const string HeaderNamePropertyName = "HeaderName";

        private string _headerName = string.Empty;

        /// <summary>
        /// Sets and gets the HeaderName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string HeaderName
        {
            get
            {
                return _headerName;
            }
            set
            {
                Set(HeaderNamePropertyName, ref _headerName, value);
            }
        }

        /// <summary>
        /// The <see cref="HeaderValue" /> property's name.
        /// </summary>
        public const string HeaderValuePropertyName = "HeaderValue";

        private string _headerValue = string.Empty;

        /// <summary>
        /// Sets and gets the HeaderValue property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string HeaderValue
        {
            get
            {
                return _headerValue;
            }
            set
            {
                Set(HeaderValuePropertyName, ref _headerValue, value);
            }
        }
        

        private RelayCommand _webCommand;

        /// <summary>
        /// Gets the WebCommand.
        /// </summary>
        public RelayCommand WebCommand
        {
            get
            {
                return _webCommand
                    ?? (_webCommand = new RelayCommand(ExecuteWebCommand));
            }
        }

        private void ExecuteWebCommand()
        {
            new WebView().Show();
        }
        

        private RelayCommand<int> _editHeaderCommand;

        /// <summary>
        /// Gets the EditHeaderCommand.
        /// </summary>
        public RelayCommand<int> EditHeaderCommand
        {
            get
            {
                return _editHeaderCommand
                    ?? (_editHeaderCommand = new RelayCommand<int>(ExecuteEditHeaderCommand));
            }
        }

        private void ExecuteEditHeaderCommand(int index)
        {
            if (index < 0 || index >= HeaderList.Count) return;
            HeaderName = HeaderList[index].Name;
            HeaderValue = HeaderList[index].Value;
        }

        private RelayCommand<int> _deleteHeaderCommand;

        /// <summary>
        /// Gets the DeleteHeaderCommand.
        /// </summary>
        public RelayCommand<int> DeleteHeaderCommand
        {
            get
            {
                return _deleteHeaderCommand
                    ?? (_deleteHeaderCommand = new RelayCommand<int>(ExecuteDeleteHeaderCommand));
            }
        }

        private void ExecuteDeleteHeaderCommand(int index)
        {
            if (index < 0 || index >= HeaderList.Count) return;
            HeaderList.RemoveAt(index);
        }

        private RelayCommand _clearHeaderCommand;

        /// <summary>
        /// Gets the ClearHeaderCommand.
        /// </summary>
        public RelayCommand ClearHeaderCommand
        {
            get
            {
                return _clearHeaderCommand
                    ?? (_clearHeaderCommand = new RelayCommand(ExecuteClearHeaderCommand));
            }
        }

        private void ExecuteClearHeaderCommand()
        {
            HeaderList.Clear();
        }

        private RelayCommand _saveHeaderCommand;

        /// <summary>
        /// Gets the SaveHeaderCommand.
        /// </summary>
        public RelayCommand SaveHeaderCommand
        {
            get
            {
                return _saveHeaderCommand
                    ?? (_saveHeaderCommand = new RelayCommand(ExecuteSaveHeaderCommand));
            }
        }

        private void ExecuteSaveHeaderCommand()
        {
            if (string.IsNullOrWhiteSpace(HeaderName))
            {
                return;
            }
            foreach (var item in HeaderList)
            {
                if (item.Name == HeaderName)
                {
                    item.Value = HeaderValue;
                    return;
                }
            }
            HeaderList.Add(new HeaderItem(HeaderName, HeaderValue));
            HeaderName = HeaderValue = string.Empty;
        }


        private RelayCommand _saveCommand;

        /// <summary>
        /// Gets the SaveCommand.
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand
                    ?? (_saveCommand = new RelayCommand(ExecuteSaveCommand));
            }
        }

        private void ExecuteSaveCommand()
        {

        }
    }
}