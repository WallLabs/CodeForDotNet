using System;

namespace CodeForDotNet.WindowsUniversal.Runtime;

/// <summary>
/// An error occurring during session state management in the <see cref="SuspensionManager"/> class.
/// </summary>
public class SuspensionManagerException : Exception
{
    /// <summary>
    /// Creates an empty instance.
    /// </summary>
    public SuspensionManagerException()
    {
    }

    /// <summary>
    /// Creates an instance with the specified message.
    /// </summary>
    public SuspensionManagerException(string message)
        : base(message)
    { }

    /// <summary>
    /// Creates an instance with the specified message and inner exception.
    /// </summary>
    public SuspensionManagerException(string message, Exception error)
        : base(message, error)
    {
    }
}