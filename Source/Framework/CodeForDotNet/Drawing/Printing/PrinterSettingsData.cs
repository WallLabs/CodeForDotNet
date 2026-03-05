using System;

namespace CodeForDotNet.Drawing.Printing;

/// <summary>
/// Printer settings data.
/// </summary>
[Serializable]
public class PrinterSettingsData
{
    /// <summary>
    /// Default <see cref="PrinterName"/>.
    /// </summary>
    public const string DefaultPrinterName = "Printer";

    /// <summary>
    /// Default printer settings.
    /// </summary>
    public static PrinterSettingsData Default => new() {
        PrinterName = DefaultPrinterName,
        Collate = true,
        DuplexOutput = PrinterSettingsDuplex.Simplex,
        Copies = 1
    };

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
    public required string PrinterName { get; set; }
}
