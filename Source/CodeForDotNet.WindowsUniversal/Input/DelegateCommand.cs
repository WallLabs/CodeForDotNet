using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace CodeForDotNet.WindowsUniversal.Input
{
    /// <summary>
    /// Generic command which calls delegate function(s) to execute or get the status.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        #region Constants

        /// <summary>
        /// Used to identify the status of the command when it has no parameter.
        /// </summary>
        private const string DefaultId = "";

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance which calls the specified <see cref="Execute"/> delegate when executed.
        /// </summary>
        /// <param name="executeMethod">Method invoked by <see cref="Execute"/> to perform the command action.</param>
        public DelegateCommand(Action<object> executeMethod) : this(executeMethod, null)
        {
        }

        /// <summary>
        /// Creates an instance which calls the specified <see cref="Execute"/> and <see cref="CanExecute"/> delegates when executed.
        /// </summary>
        /// <param name="executeMethod">Method invoked by <see cref="Execute"/> to perform the command action.</param>
        /// <param name="canExecuteMethod">Optional method invoked by <see cref="CanExecute"/> to test whether the command is currently available for execution.</param>
        public DelegateCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
            _commandStatus = new Dictionary<object, bool>();
        }

        #endregion

        #region Private Fields

        private readonly Action<object> _executeMethod;
        private readonly Func<object, bool> _canExecuteMethod;
        private readonly Dictionary<object, bool> _commandStatus;

        #endregion

        #region Events

        /// <summary>
        /// Fired when the execution status has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks whether this command is currently available for execution.
        /// </summary>
        /// <param name="parameter">Optional command specific parameter.</param>
        /// <returns>True when can execute.</returns>
        public bool CanExecute(object parameter)
        {
            // Get current status
            var status = _canExecuteMethod == null || _canExecuteMethod(parameter);

            // Detect status change
            var id = parameter ?? DefaultId;
            var lastStatus = _commandStatus.ContainsKey(id) && _commandStatus[id];
            if (status != lastStatus)
            {
                // Store new status to detect next change
                // Must do before firing event else loops (stack overflow)
                _commandStatus[id] = status;
            }

            // Return result
            return status;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">Optional command specific parameter.</param>
        public void Execute(object parameter)
        {
            _executeMethod(parameter);
        }

        /// <summary>
        /// Fires the <see cref="CanExecuteChanged"/> event, causing consumers of this command to update their status.
        /// </summary>
        public void InvokeCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
