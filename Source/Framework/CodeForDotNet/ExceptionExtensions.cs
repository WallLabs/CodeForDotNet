using System;
using System.Text;

namespace CodeForDotNet;

/// <summary>
/// Contains extensions for working with Exceptions.
/// </summary>
public static class ExceptionExtensions
{
    #region Public Methods

    /// <summary>
    /// Gets the whole message (including inner exceptions) from an <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">Exception on which this extension method applies.</param>
    public static string GetFullMessage(this Exception exception)
    {
        // Call overloaded message (default is no stack trace)
        return GetFullMessage(exception, false);
    }

    /// <summary>
    /// Gets the whole message (including inner exceptions) from an <see cref="Exception"/> and optional debug information (stack trace).
    /// </summary>
    /// <param name="exception">Exception on which this extension method applies.</param>
    /// <param name="debug">Set true to include debug information (Exception.ToString() which includes stack.trace).</param>
    public static string GetFullMessage(this Exception exception, bool debug)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(exception);

        // Get full message by adding all messages from inner exceptions
        var message = new StringBuilder();
        var innerException = exception;
        do
        {
            // Add message
            _ = message.AppendLine(innerException.Message);
            innerException = innerException.InnerException;
        }
        while (innerException != null);

        // Add stack trace when debug information requested
        if (debug)
            _ = message.AppendLine(exception.StackTrace);

        // Return result
        return message.ToString();
    }

    #endregion Public Methods
}
