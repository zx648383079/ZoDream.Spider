using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ZoDream.Helper.Local;
using ZoDream.Spider.Model;
using ZoDream.Spider.View;

namespace ZoDream.Spider.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private string _file;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {

        }

        /// <summary>
        /// The <see cref="Message" /> property's name.
        /// </summary>
        public const string MessagePropertyName = "Message";

        private string _message = string.Empty;

        /// <summary>
        /// Sets and gets the Message property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                Set(MessagePropertyName, ref _message, value);
            }
        }

        /// <summary>
        /// The <see cref="UrlList" /> property's name.
        /// </summary>
        public const string UrlListPropertyName = "UrlList";

        private ObservableCollection<UrlTask> _urlList = new ObservableCollection<UrlTask>();

        /// <summary>
        /// Sets and gets the UrlList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<UrlTask> UrlList
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


        private RelayCommand _newCommand;

        /// <summary>
        /// Gets the NewCommand.
        /// </summary>
        public RelayCommand NewCommand
        {
            get
            {
                return _newCommand
                    ?? (_newCommand = new RelayCommand(ExecuteNewCommand));
            }
        }

        private void ExecuteNewCommand()
        {
            new AddView().Show();
        }

        private RelayCommand _openCommand;

        /// <summary>
        /// Gets the OpenCommand.
        /// </summary>
        public RelayCommand OpenCommand
        {
            get
            {
                return _openCommand
                    ?? (_openCommand = new RelayCommand(ExecuteOpenCommand));
            }
        }

        private void ExecuteOpenCommand()
        {
            _import(Open.ChooseFile());
        }

        private void _import(string file)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
            {
                return;
            }
        }

        private RelayCommand<DragEventArgs> _fileDrogCommand;

        /// <summary>
        /// Gets the FileDrogCommand.
        /// </summary>
        public RelayCommand<DragEventArgs> FileDrogCommand
        {
            get
            {
                return _fileDrogCommand
                    ?? (_fileDrogCommand = new RelayCommand<DragEventArgs>(ExecuteFileDrogCommand));
            }
        }

        private void ExecuteFileDrogCommand(DragEventArgs parameter)
        {
            if (parameter == null)
            {
                return;
            }
            var files = (System.Array)parameter.Data.GetData(DataFormats.FileDrop);
            //        as FileInfo[];

            foreach (string item in files)
            {
                _import(item);
            }
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
            if (string.IsNullOrEmpty(_file))
            {
                _saveAs();
                return;
            }
            _save();
        }

        private void _saveAs()
        {
            _file = Open.ChooseSaveFile();
            _save();
        }

        private void _save()
        {
            if (string.IsNullOrEmpty(_file))
            {
                return;
            }
        }

        private RelayCommand _saveAsCommand;

        /// <summary>
        /// Gets the SaveAsCommand.
        /// </summary>
        public RelayCommand SaveAsCommand
        {
            get
            {
                return _saveAsCommand
                    ?? (_saveAsCommand = new RelayCommand(ExecuteSaveAsCommand));
            }
        }

        private void ExecuteSaveAsCommand()
        {
            _saveAs();
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
            if (index < 0 || index >= UrlList.Count) return;
            UrlList.RemoveAt(index);
        }

        private RelayCommand _deleteCompleteCommand;

        /// <summary>
        /// Gets the DeleteCompleteCommand.
        /// </summary>
        public RelayCommand DeleteCompleteCommand
        {
            get
            {
                return _deleteCompleteCommand
                    ?? (_deleteCompleteCommand = new RelayCommand(ExecuteDeleteCompleteCommand));
            }
        }

        private void ExecuteDeleteCompleteCommand()
        {
            for (int i = UrlList.Count - 1; i >= 0; i --)
            {
                if (UrlList[i].Status == UrlStatus.Success)
                {
                    UrlList.RemoveAt(i);
                }
            }
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


        private void _showMessage(string message)
        {
            Message = message;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(3000);
                Message = string.Empty;
            });
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}