using System;
using System.Drawing.Printing;
using CodeForDotNet.Drawing.Printing;

namespace CodeForDotNet.Windows.Drawing.Printing;

/// <summary>
/// Extensions for work with types in the <see cref="System.Drawing.Printing"/> namespace.
/// </summary>
public static class PrintingExtensions
{
    #region Public Methods

    /// <summary>
    /// Converts a .NET PrinterSettings class to a data structure of this type.
    /// </summary>
    public static PrinterSettingsData ToData(this PrinterSettings source)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(source);

        // Convert...
        return new PrinterSettingsData {
            Collate = source.Collate,
            Copies = source.Copies,
            DuplexOutput = (PrinterSettingsDuplex)source.Duplex,
            PrinterName = source.PrinterName
        };
    }

    /// <summary>
    /// Creates a data structure of this type from .NET PageSettings.
    /// </summary>
    public static PageSettingsData ToData(this PageSettings source)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(source);

        // Convert...
        var target = new PageSettingsData {
            Color = source.Color,
            Landscape = source.Landscape,
            Margins = source.Margins.ToData(),
            PaperWidth = source.PaperSize.Width,
            PaperHeight = source.PaperSize.Height,
            PaperName = source.PaperSize.PaperName,
            PaperSizeKind = (int)source.PaperSize.Kind,
            PrinterName = source.PrinterSettings.PrinterName
        };
        return target;
    }

    /// <summary>
    /// Converts this data to a .NET PageSettings class.
    /// </summary>
    public static PageSettings ToPageSettings(this PageSettingsData source)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(source);

        // Create system printer settings
        var target = new PageSettings {
            Color = source.Color,
            Landscape = source.Landscape,
            Margins = source.Margins.ToMargins(),
        };

        // Find and set paper size when available in system printer settings
        var printer = new PrinterSettings { PrinterName = source.PrinterName };
        foreach (PaperSize paperSize in printer.PaperSizes)
        {
            if (paperSize.PaperName == source.PaperName)
                target.PaperSize = paperSize;
        }

        // Return complete settings
        return target;
    }

    /// <summary>
    /// Converts this data to a .NET PrinterSettings class.
    /// </summary>
    public static PrinterSettings ToPrinterSettings(this PrinterSettingsData source)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(source);

        // Convert...
        return new PrinterSettings {
            Collate = source.Collate,
            Copies = source.Copies,
            Duplex = (Duplex)source.DuplexOutput,
            PrinterName = source.PrinterName
        };
    }

    #endregion Public Methods
}
