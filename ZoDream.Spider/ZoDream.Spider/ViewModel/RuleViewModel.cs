using System;
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

        private NotificationMessageAction _close;

        private int _index = -1;
        /// <summary>
        /// Initializes a new instance of the RuleViewModel class.
        /// </summary>
        public RuleViewModel()
        {
            
            Messenger.Default.Register<NotificationMessageAction<UrlItem>>(this, "rule", m=>
            {
                _callBack = m;
                var item = m.Sender as UrlItem;
                if (item == null) return;
                Url = item.Url;
                foreach (var i in item.Rults)
                {
                    RuleList.Add(i);
                }
            });

            Messenger.Default.Register<NotificationMessageAction>(this, "close", m =>
            {
                _close = m;
            });
        }

        /// <summary>
        /// The <see cref="KindName" /> property's name.
        /// </summary>
        public const string KindNamePropertyName = "KindName";

        private Array _kindName = Enum.GetNames(typeof(RuleKinds));

        /// <summary>
        /// Sets and gets the KindName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Array KindName
        {
            get
            {
                return _kindName;
            }
            set
            {
                Set(KindNamePropertyName, ref _kindName, value);
            }
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

        /// <summary>
        /// The <see cref="RuleIndex" /> property's name.
        /// </summary>
        public const string RuleIndexPropertyName = "RuleIndex";

        private int _ruleIndex = -1;

        /// <summary>
        /// Sets and gets the RuleIndex property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int RuleIndex
        {
            get
            {
                return _ruleIndex;
            }
            set
            {
                Set(RuleIndexPropertyName, ref _ruleIndex, value);
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
                _index = -1;
            }
            Kind = 0;
            Value1 = Value2 = string.Empty;
        }

        private RelayCommand _saveCommand;

        /// <summary>
        /// Gets the SaveCommand.
        /// </summary>
        public RelayCommand SaveCommand => _saveCommand
                                           ?? (_saveCommand = new RelayCommand(ExecuteSaveCommand));

        private void ExecuteSaveCommand()
        {
            if (string.IsNullOrWhiteSpace(Url)) return;
            _callBack.Execute(new UrlItem(Url, RuleList.ToList()));
            Url = string.Empty;
            RuleList.Clear();
            _index = -1;
            Kind = 0;
            Value2 = Value1 = string.Empty;
            _close.Execute();
        }

        private RelayCommand _newCommand;

        /// <summary>
        /// Gets the NewCommand.
        /// </summary>
        public RelayCommand NewCommand => _newCommand
                                          ?? (_newCommand = new RelayCommand(ExecuteNewCommand));

        private void ExecuteNewCommand()
        {
            _index = -1;
            Kind = 0;
            Value2 = Value1 = string.Empty;
        }

        private RelayCommand _editCommand;

        /// <summary>
        /// Gets the EditCommand.
        /// </summary>
        public RelayCommand EditCommand => _editCommand
                                                ?? (_editCommand = new RelayCommand(ExecuteEditCommand));

        private void ExecuteEditCommand()
        {
            if (RuleIndex < 0 || RuleIndex >= RuleList.Count) return;
            _index = RuleIndex;
            Kind = (int)RuleList[RuleIndex].Kind;
            Value1 = RuleList[RuleIndex].Value1;
            Value2 = RuleList[RuleIndex].Value2;
        }

        private RelayCommand _deleteCommand;

        /// <summary>
        /// Gets the DeleteCommand.
        /// </summary>
        public RelayCommand DeleteCommand => _deleteCommand
                                                  ?? (_deleteCommand = new RelayCommand(ExecuteDeleteCommand));

        private void ExecuteDeleteCommand()
        {
            if (RuleIndex < 0 || RuleIndex >= RuleList.Count) return;
            RuleList.RemoveAt(RuleIndex);
        }


        private RelayCommand _moveUpCommand;

        /// <summary>
        /// Gets the MoveUpCommand.
        /// </summary>
        public RelayCommand MoveUpCommand => _moveUpCommand
                                                  ?? (_moveUpCommand = new RelayCommand(ExecuteMoveUpCommand));

        private void ExecuteMoveUpCommand()
        {
            var index = RuleIndex;
            if (index <= 1) return;
            var item = RuleList[index];
            RuleList[index] = RuleList[index - 1];
            RuleList[index - 1] = item;
        }

        private RelayCommand _moveDownCommand;

        /// <summary>
        /// Gets the MoveDownCommand.
        /// </summary>
        public RelayCommand MoveDownCommand => _moveDownCommand
                                                    ?? (_moveDownCommand = new RelayCommand(ExecuteMoveDownCommand));

        private void ExecuteMoveDownCommand()
        {
            var index = RuleIndex;
            if (index < 0 || index > RuleList.Count - 2) return;
            var item = RuleList[index];
            RuleList[index] = RuleList[index + 1];
            RuleList[index + 1] = item;
        }

        private RelayCommand _clearCommand;

        /// <summary>
        /// Gets the ClearCommand.
        /// </summary>
        public RelayCommand ClearCommand => _clearCommand
                                            ?? (_clearCommand = new RelayCommand(ExecuteClearCommand));

        private void ExecuteClearCommand()
        {
            RuleList.Clear();
        }
    }
}