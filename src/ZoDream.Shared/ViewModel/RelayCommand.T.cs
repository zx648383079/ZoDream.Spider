using System;
using System.Windows.Input;

namespace ZoDream.Shared.ViewModel
{
    public class RelayCommand<T> : ICommand
    {
        public RelayCommand(Predicate<T> canExecute, Action<T> execute) : this(execute)
        {
            CanExecuteFun = canExecute;
        }

        public RelayCommand(Action<T> execute)
        {
            ExecuteFun = execute;
        }

        private readonly Predicate<T>? CanExecuteFun;
        private readonly Action<T> ExecuteFun;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (CanExecuteFun is null)
            {
                return true;
            }
            return CanExecuteFun.Invoke((T)parameter);
        }

        public void Execute(object parameter)
        {
            ExecuteFun.Invoke((T)parameter);
        }
    }
}
