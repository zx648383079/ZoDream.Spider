using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using ZoDream.Helper.Local;
using ZoDream.Spider.Helper;
using ZoDream.Spider.Helper.Http;
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
    public class MainViewModel : ViewModelBase, IDisposable
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
        public RelayCommand NewCommand => _newCommand
                                          ?? (_newCommand = new RelayCommand(ExecuteNewCommand));

        private static void ExecuteNewCommand()
        {
            new AddView().Show();
        }

        private RelayCommand _openCommand;

        /// <summary>
        /// Gets the OpenCommand.
        /// </summary>
        public RelayCommand OpenCommand => _openCommand
                                           ?? (_openCommand = new RelayCommand(ExecuteOpenCommand));

        private void ExecuteOpenCommand()
        {
            _import(Open.ChooseFile());
        }

        private static void _import(string file)
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
        public RelayCommand<DragEventArgs> FileDrogCommand => _fileDrogCommand
                                                              ?? (_fileDrogCommand = new RelayCommand<DragEventArgs>(ExecuteFileDrogCommand));

        private static void ExecuteFileDrogCommand(DragEventArgs parameter)
        {
            var files = (Array) parameter?.Data.GetData(DataFormats.FileDrop);
            //        as FileInfo[];

            if (files == null) return;
            foreach (string item in files)
            {
                _import(item);
            }
        }

        private RelayCommand _saveCommand;

        /// <summary>
        /// Gets the SaveCommand.
        /// </summary>
        public RelayCommand SaveCommand => _saveCommand
                                           ?? (_saveCommand = new RelayCommand(ExecuteSaveCommand));

        private void ExecuteSaveCommand()
        {
            if (string.IsNullOrEmpty(_file))
            {
                _saveAs();
                return;
            }
            _save();
        }

        private RelayCommand _addCommand;

        /// <summary>
        /// Gets the AddCommand.
        /// </summary>
        public RelayCommand AddCommand => _addCommand
                                          ?? (_addCommand = new RelayCommand(ExecuteAddCommand));

        private void ExecuteAddCommand()
        {
            new UrlView().Show();
            Messenger.Default.Send(new NotificationMessageAction<IList<string>>(null, _addUrl), "url");

        }

        private RelayCommand _saveAsCommand;

        /// <summary>
        /// Gets the SaveAsCommand.
        /// </summary>
        public RelayCommand SaveAsCommand => _saveAsCommand
                                             ?? (_saveAsCommand = new RelayCommand(ExecuteSaveAsCommand));

        private void ExecuteSaveAsCommand()
        {
            _saveAs();
        }

        private RelayCommand<int> _deleteCommand;

        /// <summary>
        /// Gets the DeleteCommand.
        /// </summary>
        public RelayCommand<int> DeleteCommand => _deleteCommand
                                                  ?? (_deleteCommand = new RelayCommand<int>(ExecuteDeleteCommand));

        private void ExecuteDeleteCommand(int index)
        {
            if (index < 0 || index >= UrlList.Count) return;
            UrlList.RemoveAt(index);
        }

        private RelayCommand _deleteCompleteCommand;

        /// <summary>
        /// Gets the DeleteCompleteCommand.
        /// </summary>
        public RelayCommand DeleteCompleteCommand => _deleteCompleteCommand
                                                     ?? (_deleteCompleteCommand = new RelayCommand(ExecuteDeleteCompleteCommand));

        private void ExecuteDeleteCompleteCommand()
        {
            for (var i = UrlList.Count - 1; i >= 0; i --)
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
        public RelayCommand ClearCommand => _clearCommand
                                            ?? (_clearCommand = new RelayCommand(ExecuteClearCommand));

        private void ExecuteClearCommand()
        {
            UrlList.Clear();
        }

        private RelayCommand _startCommand;

        /// <summary>
        /// Gets the StartCommand.
        /// </summary>
        public RelayCommand StartCommand => _startCommand
                                            ?? (_startCommand = new RelayCommand(ExecuteStartCommand));

        private void ExecuteStartCommand()
        {
            _begin();
        }

        private RelayCommand _stopCommand;

        /// <summary>
        /// Gets the StopCommand.
        /// </summary>
        public RelayCommand StopCommand => _stopCommand
                                           ?? (_stopCommand = new RelayCommand(ExecuteStopCommand));

        private void ExecuteStopCommand()
        {
            _tokenSource.Cancel();
            foreach (var item in UrlList)
            {
                item.Status = UrlStatus.None;
            }
        }

        private RelayCommand _pauseCommand;

        /// <summary>
        /// Gets the PauseCommand.
        /// </summary>
        public RelayCommand PauseCommand => _pauseCommand
                                            ?? (_pauseCommand = new RelayCommand(ExecutePauseCommand));

        private void ExecutePauseCommand()
        {
            _tokenSource.Cancel();
            foreach (var item in UrlList.Where(item => item.Status == UrlStatus.Waiting))
            {
                item.Status = UrlStatus.None;
            }
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


        #region 私有属性
        /// <summary>
        /// 多线程控制
        /// </summary>
        private CancellationTokenSource _tokenSource;

        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly object _object = new object();

        #endregion

        private void _begin()
        {
            if (UrlList.Count == 0 || string.IsNullOrEmpty(SpiderHelper.BaseDirectory))
            {
                _showMessage("网址为空，或保存文件夹为空！");
                return;
            }
            #region 创造主线程，去分配多个下载线程
            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;
            Task.Factory.StartNew(() =>
            {
                var index = 0;
                while (!token.IsCancellationRequested)
                {
                    #region 创建执行下载的线程数组
                    var tasksLength = Math.Min(SpiderHelper.Count, UrlList.Count - index);
                    var tasks = new Task[tasksLength];
                    for (var i = 0; i < tasksLength; i++)
                    {
                        var index1 = index;
                        tasks[i] = new Task(() =>
                        {
                            _begin(UrlList[index1]);
                        });
                        index++;
                    }
                    #endregion

                    #region 监视线程数组完成
                    var continuation = Task.Factory.ContinueWhenAll(tasks, (task) =>
                    { }, token);
                    foreach (var task in tasks)
                    {
                        task.Start();
                    }
                    while (!continuation.IsCompleted)
                    {
                        Thread.Sleep(1000);
                    }
                    #endregion
                    _changedStatus(index - tasksLength, tasksLength, UrlStatus.Success);
                    if (index < UrlList.Count) continue;
                    _tokenSource.Cancel();
                    break;
                }
                /*Application.Current.Dispatcher.Invoke(() =>
                {
                    IsEnable = true;
                });*/
            }, token);
            #endregion
        }

        private void _changedStatus(int index, int length, UrlStatus status)
        {
            lock (_object)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    for (var i = 0; i < length; i++)
                    {
                        UrlList[index + i].Status = status;
                    }
                });

            }
        }

        private void _begin(UrlTask urlTask)
        {
            var spider = new SpiderRequest()
            {
                Url = urlTask,
                Rules = SpiderHelper.GetRules(urlTask.Url),
                Headers = SpiderHelper.Headers
            };
            spider.Start();
            if (spider.Results.Count < 1) return;
            lock (_object)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _addUrl(spider.Results);
                });

            }
        }

        private void _addUrl(IEnumerable<string> args)
        {
            foreach (var item in args)
            {
                _addUrl(item);
            }
        }

        private void _addUrl(IEnumerable<UrlTask> args)
        {
            foreach (var item in args)
            {
                _addUrl(item);
            }
        }

        private void _addUrl(string url)
        {
            _addUrl(new UrlTask(url));
        }

        private void _addUrl(UrlTask url)
        {
            if (url.Url.IndexOf("//", StringComparison.Ordinal) < 0 || !SpiderHelper.CanAdd(url.Url)) return;
            UrlList.Add(url);
            SpiderHelper.UrlList.Add(url.Url);
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_tokenSource == null) return;
            _tokenSource.Cancel();
            _tokenSource.Dispose();
            _tokenSource = null;
        }

        ~MainViewModel()
        {
            Dispose(false);
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}