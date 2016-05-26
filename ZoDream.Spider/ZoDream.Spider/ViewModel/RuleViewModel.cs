using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Linq;
using ZoDream.Spider.Model;

namespace ZoDream.Spider.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class RuleViewModel : ViewModelBase
    {
        private NotificationMessageAction<UrlItem> _callBack;

        private int _index = -1;
        /// <summary>
        /// Initializes a new instance of the RuleViewModel class.
        /// </summary>
        public RuleViewModel()
        {
            Messenger.Default.Register<NotificationMessageAction<UrlItem>>(this, m=>
            {
                _callBack = m;
                if (m.Sender == null)
                {
                    return;
                }
                var item = m.Sender as UrlItem;
                Url = item.Url;
                foreach (var i in item.Rults)
                {
                    RuleList.Add(i);
                }
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

        /// <summary>
        /// The <see cref="RuleList" /> property's name.
        /// </summary>
        public const string RuleListPropertyName = "RuleList";

        private ObservableCollection<RuleItem> _ruleList = new ObservableCollection<RuleItem>();

        /// <summary>
        /// Sets and gets the RuleList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<RuleItem> RuleList
        {
            get
            {
                return _ruleList;
            }
            set
            {
                Set(RuleListPropertyName, ref _ruleList, value);
            }
        }

        /// <summary>
        /// The <see cref="Kind" /> property's name.
        /// </summary>
        public const string KindPropertyName = "Kind";

        private int _kind = 0;

        /// <summary>
        /// Sets and gets the Kind property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int Kind
        {
            get
            {
                return _kind;
            }
            set
            {
                Set(KindPropertyName, ref _kind, value);
            }
        }

        /// <summary>
        /// The <see cref="Value1" /> property's name.
        /// </summary>
        public const string Value1PropertyName = "Value1";

        private string _value1 = string.Empty;

        /// <summary>
        /// Sets and gets the Value1 property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Value1
        {
            get
            {
                return _value1;
            }
            set
            {
                Set(Value1PropertyName, ref _value1, value);
            }
        }

        /// <summary>
        /// The <see cref="Value2" /> property's name.
        /// </summary>
        public const string Value2PropertyName = "Value2";

        private string _value2 = string.Empty;

        /// <summary>
        /// Sets and gets the Value2 property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Value2
        {
            get
            {
                return _value2;
            }
            set
            {
                Set(Value2PropertyName, ref _value2, value);
            }
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
            var item = new RuleItem((RuleKinds)Kind, Value1, Value2);
            if (_index < 0 || _index >= RuleList.Count)
            {
                RuleList.Add(item);
            } else
            {
                RuleList[_index] = item;
                _index = 0;
            }
            Kind = 0;
            Value1 = Value2 = string.Empty;
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
            if (string.IsNullOrWhiteSpace(Url)) return;
            _callBack.Execute(new UrlItem(Url, RuleList.ToList()));
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
            _index = -1;
            Kind = 0;
            Value2 = Value1 = string.Empty;
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
            if (index < 0 || index >= RuleList.Count) return;
            _index = index;
            Kind = (int)RuleList[index].Kind;
            Value1 = RuleList[index].Value1;
            Value2 = RuleList[index].Value2;
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
            if (index < 0 || index >= RuleList.Count) return;
            RuleList.RemoveAt(index);
        }


        private RelayCommand<int> _moveUpCommand;

        /// <summary>
        /// Gets the MoveUpCommand.
        /// </summary>
        public RelayCommand<int> MoveUpCommand
        {
            get
            {
                return _moveUpCommand
                    ?? (_moveUpCommand = new RelayCommand<int>(ExecuteMoveUpCommand));
            }
        }

        private void ExecuteMoveUpCommand(int index)
        {
            if (index <= 0 || index >= RuleList.Count) return;
            var item = RuleList[index];
            RuleList[index] = RuleList[index - 1];
            RuleList[index - 1] = item;
        }

        private RelayCommand<int> _moveDownCommand;

        /// <summary>
        /// Gets the MoveDownCommand.
        /// </summary>
        public RelayCommand<int> MoveDownCommand
        {
            get
            {
                return _moveDownCommand
                    ?? (_moveDownCommand = new RelayCommand<int>(ExecuteMoveDownCommand));
            }
        }

        private void ExecuteMoveDownCommand(int index)
        {
            if (index < 0 || index > RuleList.Count - 2) return;
            var item = RuleList[index];
            RuleList[index] = RuleList[index + 1];
            RuleList[index + 1] = item;
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
            RuleList.Clear();
        }
    }
}