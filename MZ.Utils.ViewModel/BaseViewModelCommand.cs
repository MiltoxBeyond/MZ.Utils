using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MZ.Utils.ViewModel
{
    public class BaseViewModelCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public Action<object?> _action { get; }
        public Func<object?, bool>? _canExecute { get; }

        public BaseViewModelCommand(Action<object?> action, Func<object?, bool>? canExecute = null) {
            _action = action;
            _canExecute = canExecute;
        }
        public BaseViewModelCommand(Action action, Func<bool>? canExecute = null) 
            : this((_) => action(), canExecute != null ? (_) => canExecute() : null) 
        {
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute != null ? _canExecute(parameter) : true;    
        }

        public void Execute(object? parameter)
        {
            _action(parameter);
        }
    }
}
