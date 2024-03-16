using System.Windows.Input;

namespace BookObserver.Infrastructure.Commands.Base
{
    abstract class Command : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        private bool _executeble = true;
        public bool Executable
        {
            get => _executeble;
            set
            {
                if(_executeble == value) return;
                _executeble = value;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        bool ICommand.CanExecute(object? parameter) => _executeble && CanExecute(parameter);
        void ICommand.Execute(object? parameter)
        {
            if(CanExecute(parameter))
                Execute(parameter);
        }

        public virtual bool CanExecute(object? parameter) => true;

        public abstract void Execute(object? parameter);
    }
}
