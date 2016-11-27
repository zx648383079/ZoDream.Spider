using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Xml.Serialization;
using ZoDream.Helper.Local;
using ZoDream.Spider.Helper;
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
        private NotificationMessageAction _close;

        /// <summary>
        /// Initializes a new instance of the AddViewModel class.
        /// </summary>
        public AddViewModel()
        {
            Messenger.Default.Register<NotificationMessageAction>(this, "closeAdd", m =>
            {
                _close = m;
            });
            foreach (var item in SpiderHelper.UrlRegex)
            {
                UrlList.Add(item);
            }
            foreach (var item in SpiderHelper.Headers)
            {
                HeaderList.Add(item);
            }

            Count = SpiderHelper.Count;
            TimeOut = SpiderHelper.TimeOut;
            BaseDirectory = SpiderHelper.BaseDirectory;
            UseBrowser = SpiderHelper.UseBrowser;

        }

        /// <summary>
        /// The <see cref="Count" /> property's name.
        /// </summary>
        public const string CountPropertyName = "Count";

        private int _count = 100;

        /// <summary>
        /// The <see cref="UseBrowser" /> property's name.
        /// </summary>
        public const string UseBrowserPropertyName = "UseBrowser";

        private bool _useBrowser = false;

        /// <summary>
        /// Sets and gets the UseBrowser property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool UseBrowser
        {
            get
            {
                return _useBrowser;
            }
            set
            {
                Set(UseBrowserPropertyName, ref _useBrowser, value);
            }
        }

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
        /// The <see cref="TimeOut" /> property's name.
        /// </summary>
        public const string TimeOutPropertyName = "TimeOut";

        private int _timeOut = 10000;

        /// <summary>
        /// Sets and gets the TimeOut property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int TimeOut
        {
            get
            {
                return _timeOut;
            }
            set
            {
                Set(TimeOutPropertyName, ref _timeOut, value);
            }
        }

        /// <summary>
        /// The <see cref="HeaderKeys" /> property's name.
        /// </summary>
        public const string HeaderKeysPropertyName = "HeaderKeys";

        private Array _headerKeys = Enum.GetNames(typeof(HttpRequestHeader));

        /// <summary>
        /// Sets and gets the HeaderKeys property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Array HeaderKeys
        {
            get
            {
                return _headerKeys;
            }
            set
            {
                Set(HeaderKeysPropertyName, ref _headerKeys, value);
            }
        }

        /// <summary>
        /// The <see cref="UrlIndex" /> property's name.
        /// </summary>
        public const string UrlIndexPropertyName = "UrlIndex";

        private int _urlIndex = -1;

        /// <summary>
        /// Sets and gets the UrlIndex property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int UrlIndex
        {
            get
            {
                return _urlIndex;
            }
            set
            {
                Set(UrlIndexPropertyName, ref _urlIndex, value);
            }
        }

        /// <summary>
        /// The <see cref="HeaderIndex" /> property's name.
        /// </summary>
        public const string HeaderIndexPropertyName = "HeaderIndex";

        private int _headerIndex = -1;

        /// <summary>
        /// Sets and gets the HeaderIndex property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int HeaderIndex
        {
            get
            {
                return _headerIndex;
            }
            set
            {
                Set(HeaderIndexPropertyName, ref _headerIndex, value);
            }
        }


        /// <summary>
        /// The <see cref="BaseDirectory" /> property's name.
        /// </summary>
        public const string BaseDirectoryPropertyName = "BaseDirectory";

        private string _baseDirectory = string.Empty;

        /// <summary>
        /// Sets and gets the BaseDirectory property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string BaseDirectory
        {
            get
            {
                return _baseDirectory;
            }
            set
            {
                Set(BaseDirectoryPropertyName, ref _baseDirectory, value);
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

        private RelayCommand _chooseCommand;

        /// <summary>
        /// Gets the ChooseCommand.
        /// </summary>
        public RelayCommand ChooseCommand => _chooseCommand
                                             ?? (_chooseCommand = new RelayCommand(ExecuteChooseCommand));

        private void ExecuteChooseCommand()
        {
            BaseDirectory = Open.ChooseFolder();
        }


        private RelayCommand _importCommand;

        /// <summary>
        /// Gets the ImportCommand.
        /// </summary>
        public RelayCommand ImportCommand => _importCommand
                                             ?? (_importCommand = new RelayCommand(ExecuteImportCommand));

        private void ExecuteImportCommand()
        {
            _import(Open.ChooseFile());
        }

        private void _import(string file)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
            {
                return;
            }
            var xml = new XmlSerializer(typeof(List<UrlItem>));
            var stream = new FileStream(file, FileMode.Open);
            var list = (List<UrlItem>)xml.Deserialize(stream);
            stream.Close();
            foreach (var item in list)
            {
                UrlList.Add(item);
            }
        }

        private RelayCommand<DragEventArgs> _fileDrogCommand;

        /// <summary>
        /// Gets the FileDrogCommand.
        /// </summary>
        public RelayCommand<DragEventArgs> FileDrogCommand => _fileDrogCommand
                                                              ?? (_fileDrogCommand = new RelayCommand<DragEventArgs>(ExecuteFileDrogCommand));

        private void ExecuteFileDrogCommand(DragEventArgs parameter)
        {
            var files = (System.Array) parameter?.Data.GetData(DataFormats.FileDrop);
            //        as FileInfo[];

            if (files == null) return;
            foreach (string item in files)
            {
                _import(item);
            }
        }


        private RelayCommand _exportCommand;

        /// <summary>
        /// Gets the ExportCommand.
        /// </summary>
        public RelayCommand ExportCommand => _exportCommand
                                             ?? (_exportCommand = new RelayCommand(ExecuteExportCommand));

        private void ExecuteExportCommand()
        {
            var file = Open.ChooseSaveFile();
            if (string.IsNullOrEmpty(file) || File.Exists(file))
            {
                return;
            }
            var xml = new XmlSerializer(typeof(List<UrlItem>));
            var stream = new FileStream(file, FileMode.Create);
            xml.Serialize(stream, UrlList.ToList());
            stream.Close();
        }

        private RelayCommand _addCommand;

        /// <summary>
        /// Gets the AddCommand.
        /// </summary>
        public RelayCommand AddCommand => _addCommand
                                          ?? (_addCommand = new RelayCommand(ExecuteAddCommand));

        private void ExecuteAddCommand()
        {
            new RuleView().Show();
            Messenger.Default.Send(new NotificationMessageAction<UrlItem>(null, item => {
                UrlList.Add(new UrlItem(item.Url, item.Rults));
            }), "rule");
        }

        private RelayCommand _editCommand;

        /// <summary>
        /// Gets the EditCommand.
        /// </summary>
        public RelayCommand EditCommand => _editCommand
                                                ?? (_editCommand = new RelayCommand(ExecuteEditCommand));

        private void ExecuteEditCommand()
        {
            if (UrlIndex < 0 || UrlIndex >= UrlList.Count) return;
            new RuleView().Show();

            Messenger.Default.Send(new NotificationMessageAction<UrlItem>(UrlList[UrlIndex], null, item=> {
                UrlList[UrlIndex] = item;
            }), "rule");
        }

        private RelayCommand _deleteCommand;

        /// <summary>
        /// Gets the DeleteCommand.
        /// </summary>
        public RelayCommand DeleteCommand => _deleteCommand
                                                  ?? (_deleteCommand = new RelayCommand(ExecuteDeleteCommand));

        private void ExecuteDeleteCommand()
        {
            if (UrlIndex < 0 || UrlIndex >= HeaderList.Count) return;
            UrlList.RemoveAt(UrlIndex);
        }

        private RelayCommand _clearCommand;

        /// <summary>
        /// Gets the ClearCommand.
        /// </summary>
        public RelayCommand ClearCommand => _clearCommand
                                            ?? (_clearCommand = new RelayCommand(ExecuteClearCommand));

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
        public RelayCommand WebCommand => _webCommand
                                          ?? (_webCommand = new RelayCommand(ExecuteWebCommand));

        private void ExecuteWebCommand()
        {
            SpiderHelper.GetBrowser();
            Messenger.Default.Send(new NotificationMessageAction<List<HeaderItem>>(null, rules => {
                foreach (var item in rules)
                {
                    HeaderList.Add(item);
                }
            }), "web");
        }
        

        private RelayCommand _editHeaderCommand;

        /// <summary>
        /// Gets the EditHeaderCommand.
        /// </summary>
        public RelayCommand EditHeaderCommand => _editHeaderCommand
                                                      ?? (_editHeaderCommand = new RelayCommand(ExecuteEditHeaderCommand));

        private void ExecuteEditHeaderCommand()
        {
            if (HeaderIndex < 0 || HeaderIndex >= HeaderList.Count) return;
            HeaderName = HeaderList[HeaderIndex].Name.ToString();
            HeaderValue = HeaderList[HeaderIndex].Value;
        }

        private RelayCommand _deleteHeaderCommand;

        /// <summary>
        /// Gets the DeleteHeaderCommand.
        /// </summary>
        public RelayCommand DeleteHeaderCommand => _deleteHeaderCommand
                                                        ?? (_deleteHeaderCommand = new RelayCommand(ExecuteDeleteHeaderCommand));

        private void ExecuteDeleteHeaderCommand()
        {
            if (HeaderIndex < 0 || HeaderIndex >= HeaderList.Count) return;
            HeaderList.RemoveAt(HeaderIndex);
        }

        private RelayCommand _clearHeaderCommand;

        /// <summary>
        /// Gets the ClearHeaderCommand.
        /// </summary>
        public RelayCommand ClearHeaderCommand => _clearHeaderCommand
                                                  ?? (_clearHeaderCommand = new RelayCommand(ExecuteClearHeaderCommand));

        private void ExecuteClearHeaderCommand()
        {
            HeaderList.Clear();
        }

        private RelayCommand _saveHeaderCommand;

        /// <summary>
        /// Gets the SaveHeaderCommand.
        /// </summary>
        public RelayCommand SaveHeaderCommand => _saveHeaderCommand
                                                 ?? (_saveHeaderCommand = new RelayCommand(ExecuteSaveHeaderCommand));

        private void ExecuteSaveHeaderCommand()
        {
            if (string.IsNullOrWhiteSpace(HeaderName))
            {
                return;
            }
            foreach (var item in HeaderList)
            {
                if (item.Name.ToString() != HeaderName) continue;
                item.Value = HeaderValue;
                return;
            }
            HeaderList.Add(new HeaderItem(HeaderName, HeaderValue));
            HeaderName = HeaderValue = string.Empty;
        }


        private RelayCommand _saveCommand;

        /// <summary>
        /// Gets the SaveCommand.
        /// </summary>
        public RelayCommand SaveCommand => _saveCommand
                                           ?? (_saveCommand = new RelayCommand(ExecuteSaveCommand));

        private void ExecuteSaveCommand()
        {
            if (string.IsNullOrWhiteSpace(BaseDirectory))
            {
                MessageBox.Show("保持路径是必须的！");
                return;
            }
            SpiderHelper.Headers = HeaderList.ToList();
            SpiderHelper.UrlRegex = UrlList.ToList();
            SpiderHelper.Count = Count;
            SpiderHelper.TimeOut = TimeOut;
            SpiderHelper.BaseDirectory = BaseDirectory;
            SpiderHelper.UseBrowser = UseBrowser;
            _close.Execute();
        }
    }
}