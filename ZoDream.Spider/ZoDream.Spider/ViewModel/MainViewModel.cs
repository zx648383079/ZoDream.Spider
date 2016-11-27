using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
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
            get { return _message; }
            set { Set(MessagePropertyName, ref _message, value); }
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
            get { return _urlList; }
            set { Set(UrlListPropertyName, ref _urlList, value); }
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

        private void _import(string file)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
            {
                return;
            }
            _file = file;
            using (var reader = Open.Reader(file))
            {
                var tag = "URL";
                var xml = new StringBuilder();
                string line;
                var urls = new List<string>();
                string[] args;
                var regex = new Regex(@"^\[(\w+)\]$");
                while (null != (line = reader.ReadLine()))
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    if (regex.IsMatch(line))
                    {
                        tag = regex.Match(line).Groups[1].Value.ToUpper();
                        continue;
                    }
                    switch (tag)
                    {
                        case "URL":
                            urls.Add(line);
                            break;
                        case "COUNT":
                            SpiderHelper.Count = Convert.ToInt32(line);
                            break;
                        case "TIMEOUT":
                            SpiderHelper.TimeOut = Convert.ToInt32(line);
                            break;
                        case "USEBROWSER":
                            SpiderHelper.UseBrowser = line == "Y";
                            break;
                        case "HEADER":
                            args = line.Split(new[] {'='}, 2);
                            if (args.Length < 2)
                            {
                                continue;
                            }
                            SpiderHelper.Headers.Add(new HeaderItem(args[0], args[1]));
                            break;
                        case "DIRECTORY":
                            SpiderHelper.BaseDirectory = line.TrimEnd('\\');
                            break;
                        case "REGEX":
                            xml.AppendLine(line);
                            break;
                    }
                }
                var text = xml.ToString();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    var xmler = new XmlSerializer(typeof(List<UrlItem>));
                    SpiderHelper.UrlRegex = (List<UrlItem>) xmler.Deserialize(new StringReader(text));
                }
                foreach (var url in urls)
                {
                    args = url.Split(new[] { '=' }, 3);
                    if (args.Length < 3 || string.IsNullOrWhiteSpace(args[2]))
                    {
                        continue;
                    }
                    _addUrl(new UrlTask(args[2])
                    {
                        Kind = (AssetKind)Enum.Parse(typeof(AssetKind), args[1]),
                        Status = (UrlStatus)Enum.Parse(typeof(UrlStatus), args[0])
                    });
                }
            }
            _showMessage("导入完成！");
        }

        private RelayCommand<DragEventArgs> _fileDrogCommand;

        /// <summary>
        /// Gets the FileDrogCommand.
        /// </summary>
        public RelayCommand<DragEventArgs> FileDrogCommand => _fileDrogCommand
                                                              ??
                                                              (_fileDrogCommand =
                                                                  new RelayCommand<DragEventArgs>(ExecuteFileDrogCommand))
            ;

        private void ExecuteFileDrogCommand(DragEventArgs parameter)
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
            if (SpiderHelper.UrlRegex.Count == 0)
            {
                _showMessage("请设置规则！");
                return;
            }
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

        private RelayCommand _deleteCommand;

        /// <summary>
        /// Gets the DeleteCommand.
        /// </summary>
        public RelayCommand DeleteCommand => _deleteCommand
                                                  ?? (_deleteCommand = new RelayCommand(ExecuteDeleteCommand));

        private void ExecuteDeleteCommand()
        {
            if (UrlIndex < 0 || UrlIndex >= UrlList.Count) return;
            UrlList.RemoveAt(UrlIndex);
        }

        private RelayCommand _deleteCompleteCommand;

        /// <summary>
        /// Gets the DeleteCompleteCommand.
        /// </summary>
        public RelayCommand DeleteCompleteCommand => _deleteCompleteCommand
                                                     ??
                                                     (_deleteCompleteCommand =
                                                         new RelayCommand(ExecuteDeleteCompleteCommand));

        private void ExecuteDeleteCompleteCommand()
        {
            for (var i = UrlList.Count - 1; i >= 0; i--)
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
            _isStop = false;
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
            _isStop = true;
            _tokenSource.Cancel();
            _showMessage("程序已停止！");
        }

        private RelayCommand _resetCommand;

        /// <summary>
        /// Gets the ResetCommand.
        /// </summary>
        public RelayCommand ResetCommand => _resetCommand
                                            ?? (_resetCommand = new RelayCommand(ExecuteResetCommand));

        private void ExecuteResetCommand()
        {
            _isStop = true;
            _tokenSource.Cancel();
            foreach (var urlTask in UrlList)
            {
                urlTask.Status = UrlStatus.None;
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
            _isStop = true;
            _tokenSource.Cancel();
            foreach (var item in UrlList.Where(item => item.Status == UrlStatus.Waiting))
            {
                item.Status = UrlStatus.None;
            }
            _showMessage("程序已暂停！");
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
            _showMessage("程序启动。。。");
            
            if (SpiderHelper.UseBrowser)
            {
                _useBrowser(_getStart());
                return;
            }

            #region 创造主线程，去分配多个下载线程

            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;
            Task.Factory.StartNew(() =>
            {
                var index = _getStart();
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
                            _changedStatus(index1, UrlStatus.Waiting);
                            try
                            {
                                _begin(UrlList[index1]);
                                _changedStatus(index1, UrlStatus.Success);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"{index1}, {ex.Message},{ex.TargetSite}");
                                _changedStatus(index1, UrlStatus.Failure);
                            }
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
                    if (index < UrlList.Count) continue;
                    _tokenSource.Cancel();
                    _showMessage("程序执行完成。。。");
                    break;
                }
                /*Application.Current.Dispatcher.Invoke(() =>
                {
                    IsEnable = true;
                });*/
            }, token);
            #endregion
        }

        private bool _isStop = true;

        /// <summary>
        /// 使用浏览器下载
        /// </summary>
        /// <param name="index"></param>
        private void _useBrowser(int index)
        {
            var urlTask = UrlList[index];
            urlTask.Status = UrlStatus.Waiting;
            if (urlTask.Kind != AssetKind.Html)
            {
                try
                {
                    _begin(urlTask);
                    _changedStatus(index, UrlStatus.Success);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{index}, {ex.Message}");
                    _changedStatus(index, UrlStatus.Failure);
                }
                index++;
                if (index >= UrlList.Count)
                {
                    _showMessage("下载完成！");
                    return;
                }
                if (_isStop) return;
                _useBrowser(index);
                return;
            }
            SpiderHelper.GetBrowser().HtmlCallback = html =>
            {
                var spider = new SpiderRequest()
                {
                    Url = urlTask,
                    Rules = SpiderHelper.GetRules(urlTask.Url),
                };
                spider.DealHtml(html);
                if (spider.Results.Count > 0)
                {
                    _addUrl(spider.Results);
                }
                urlTask.Status = UrlStatus.Success;
                index++;
                if (index >= UrlList.Count)
                {
                    _showMessage("下载完成！");
                    return;
                }
                if (_isStop) return;
                _useBrowser(index);
            };
            SpiderHelper.GetBrowser().NavigateUrl(urlTask.Url);
        }

        private void _changedStatus(int index, UrlStatus status)
        {
            lock (_object)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UrlList[index].Status = status;
                });

            }
        }

        private void _begin(UrlTask urlTask)
        {
            var spider = new SpiderRequest()
            {
                Url = urlTask,
                Rules = SpiderHelper.GetRules(urlTask.Url),
                Headers = SpiderHelper.Headers,
                TimeOut = SpiderHelper.TimeOut
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

        private int _getStart()
        {
            for (var i = 0; i < UrlList.Count; i++)
            {
                if (UrlList[i].Status == UrlStatus.None || UrlList[i].Status == UrlStatus.Waiting)
                {
                    return i;
                }
            }
            return 0;
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
            if (url.Url.IndexOf("//", StringComparison.Ordinal) < 0 
                || !SpiderHelper.CanAdd(url.Url)) return;
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
            using (var sw = new StreamWriter(_file, false, new UTF8Encoding(false)))
            {
                sw.WriteLine("[URL]");
                foreach (var urlTask in UrlList)
                {
                    sw.WriteLine($"{urlTask.Status}={urlTask.Kind}={urlTask.Url}");
                }
                sw.WriteLine();
                sw.WriteLine("[COUNT]");
                sw.WriteLine(SpiderHelper.Count);
                sw.WriteLine();
                sw.WriteLine("[TIMEOUT]");
                sw.WriteLine(SpiderHelper.TimeOut);
                sw.WriteLine();
                sw.WriteLine("[USEBROWSER]");
                sw.WriteLine(SpiderHelper.UseBrowser ? "Y" : "N");
                sw.WriteLine();
                sw.WriteLine("[DIRECTORY]");
                sw.WriteLine(SpiderHelper.BaseDirectory);
                sw.WriteLine();
                sw.WriteLine("[HEADER]");
                foreach (var item in SpiderHelper.Headers)
                {
                    sw.WriteLine($"{item.Name}={item.Value}");
                }
                sw.WriteLine();
                sw.WriteLine("[REGEX]");
                var xml = new XmlSerializer(typeof(List<UrlItem>));
                xml.Serialize(sw, SpiderHelper.UrlRegex.ToList());
            }
            _showMessage("已成功保存到：" + _file);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _isStop = true;
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

        public override void Cleanup()
        {
            if (SpiderHelper.Browser != null)
            {
                SpiderHelper.Browser.Close();
            }
            base.Cleanup();
        }
    }
}