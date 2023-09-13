using System;
using System.Windows.Input;

namespace ZoDream.Shared.ViewModel
{
    public class RelayCommand : ICommand
    {
        public RelayCommand(Predicate<object?> canExecute, Action<object?> execute) : this(execute)
        {
            CanExecuteFun = canExecute;
        }

        public RelayCommand(Action<object?> execute)
        {
            ExecuteFun = execute;
        }

        private readonly Predicate<object?>? CanExecuteFun;
        private readonly Action<object?> ExecuteFun;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            if (CanExecuteFun is null)
            {
                return true;
            }
            return CanExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            ExecuteFun(parameter);
        }
    }
}
