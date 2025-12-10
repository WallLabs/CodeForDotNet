using System;

namespace CodeForDotNet.Drawing.Printing;

/// <summary>
/// Printer settings data.
/// </summary>
[Serializable]
public class PrinterSettingsData
{
    #region Public Properties

    /// <summary>
    /// Default printer settings.
    /// </summary>
    public static PrinterSettingsData Default => new() { Collate = true, DuplexOutput = PrinterSettingsDuplex.Simplex, Copies = 1 };

    /// <summary>
    /// Collate option.
    /// </summary>
    public bool Collate { get; set; }

    /// <summary>
    /// Number of copies.
    /// </summary>
    public short Copies { get; set; }

    /// <summary>
    /// Duplex option.
    /// </summary>
    public PrinterSettingsDuplex DuplexOutput { get; set; }

    /// <summary>
    /// Printer name.
    /// </summary>
    public string? PrinterName { get; set; }

    #endregion Public Properties
}
