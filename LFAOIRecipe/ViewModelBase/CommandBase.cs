using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LFAOIRecipe
{
    public class CommandBase : ICommand
    {
        private readonly Action<object> _executeCommand = null;
        private readonly Func<object, bool> _canExecuteCommand = null;

        public CommandBase(Action<object> executeMethod)
            : this(executeMethod, null)
        { }

        public CommandBase(Action executeMethod)
        { }

        public CommandBase(Action<object> executeCommand, Func<object, bool> canExecuteCommand)
        {
            if (executeCommand == null)
                throw new ArgumentNullException("executeMethod");
            this._executeCommand = executeCommand;
            this._canExecuteCommand = canExecuteCommand;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (this._canExecuteCommand != null)
            {
                return this._canExecuteCommand(parameter);
            }
            return true;
        }

        public void Execute(object parameter)
        {
            if (this._executeCommand != null)
            {
                this._executeCommand(parameter);
            }
        }
    }
}
