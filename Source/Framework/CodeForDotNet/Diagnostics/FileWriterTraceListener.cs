using System;
using System.Diagnostics;
using System.IO;

namespace CodeForDotNet.Diagnostics;

/// <summary>
/// Writes trace output to a file, supporting environment variables in the filename and lazy open of the file.
/// </summary>
public class FileWriterTraceListener(string fileName) : TraceListener
{
    /// <summary>
    /// Filename to write to.
    /// </summary>
    private readonly string _fileName = Environment.ExpandEnvironmentVariables(fileName);

    /// <summary>
    /// Output file stream.
    /// </summary>
    private StreamWriter? _stream;

    /// <summary>
    /// Closes the output stream (if open).
    /// </summary>
    public override void Close()
    {
        _stream?.Dispose();
    }

    /// <summary>
    /// Flushes any buffered data to the file (if open).
    /// </summary>
    public override void Flush()
    {
        _stream?.Flush();
    }

    /// <summary>
    /// Writes to the output stream.
    /// </summary>
    public override void Write(string? message)
    {
        // Do nothing when no message (questionable design but base class allows this).
        if (string.IsNullOrWhiteSpace(message))
            return;

        // Lazy create/open file.
        OpenFile();

        // Write to file.
        _stream?.Write(message);
    }

    /// <summary>
    /// Writes to the output stream followed by a new line.
    /// </summary>
    public override void WriteLine(string? message)
    {
        // Do nothing when no message (questionable design but base class allows this).
        if (string.IsNullOrWhiteSpace(message))
            return;

        // Lazy create/open file.
        OpenFile();

        // Write to file.
        _stream?.WriteLine(message);
    }

    /// <summary>
    /// Cleans-up resources.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        try
        {
            // Dispose unmanaged resources
            _stream?.Dispose();
        }
        finally
        {
            // Dispose base class
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Opens the stream when it is needed (lazy open).
    /// </summary>
    private void OpenFile()
    {
        // Create directory if not exists
        var directory = Path.GetDirectoryName(_fileName);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            _ = Directory.CreateDirectory(directory);

        // Create or open file
        _stream ??= new StreamWriter(_fileName, true);
    }
}
