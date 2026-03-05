using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace CodeForDotNet.UI.Input;

/// <summary>
/// Generic command which calls delegate function(s) to execute or get the status.
/// </summary>
/// <remarks>
/// Creates an instance which calls the specified <see cref="Execute"/> and <see cref="CanExecute"/> delegates when executed.
/// </remarks>
/// <param name="executeMethod">Method invoked by <see cref="Execute"/> to perform the command action.</param>
/// <param name="canExecuteMethod">Optional method invoked by <see cref="CanExecute"/> to test whether the command is currently available for execution.</param>
public class DelegateCommand(Action<object?> executeMethod, Func<object?, bool>? canExecuteMethod) : ICommand
{
    /// <summary>
    /// Used to identify the status of the command when it has no parameter.
    /// </summary>
    private const string DefaultId = "";

    /// <summary>
    /// Exection status delegate.
    /// </summary>
    private readonly Func<object?, bool>? _canExecuteMethod = canExecuteMethod;

    /// <summary>
    /// Stpres the last known execution status for each command.
    /// </summary>
    private readonly Dictionary<object, bool> _commandStatus = [];

    /// <summary>
    /// Execute delegate.
    /// </summary>
    private readonly Action<object?> _executeMethod = executeMethod;

    /// <summary>
    /// Creates an instance which calls the specified <see cref="Execute"/> delegate when executed.
    /// </summary>
    /// <param name="executeMethod">Method invoked by <see cref="Execute"/> to perform the command action.</param>
    public DelegateCommand(Action<object?> executeMethod) : this(executeMethod, null)
    {
    }

    /// <summary>
    /// Fired when the execution status has changed.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Checks whether this command is currently available for execution.
    /// </summary>
    /// <param name="parameter">Optional command specific parameter.</param>
    /// <returns>True when can execute.</returns>
    public bool CanExecute(object? parameter)
    {
        // Get current status
        var status = _canExecuteMethod == null || _canExecuteMethod(parameter);

        // Detect status change
        var id = parameter ?? DefaultId;
        var lastStatus = _commandStatus.ContainsKey(id) && _commandStatus[id];
        if (status != lastStatus)
        {
            // Store new status to detect next change Must do before firing event else loops (stack overflow)
            _commandStatus[id] = status;
        }

        // Return result
        return status;
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameter">Optional command specific parameter.</param>
    public void Execute(object? parameter)
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
}
