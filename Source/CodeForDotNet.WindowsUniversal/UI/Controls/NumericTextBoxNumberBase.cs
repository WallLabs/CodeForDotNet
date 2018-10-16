using System.Diagnostics.CodeAnalysis;

namespace CodeForDotNet.WindowsUniversal.UI.Controls
{
    /// <summary>
    /// Number base.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1720", Justification = "Name is preferred for an intuitive object model and would not conflict when used as an enumeration member.")]
    public enum NumericTextBoxNumberBase
    {
        /// <summary>
        /// Binary base 2.
        /// </summary>
        Binary = 2,

        /// <summary>
        /// Decimal base 10.
        /// </summary>
        Decimal = 10,

        /// <summary>
        /// Hexadecimal base 16.
        /// </summary>
        Hexadecimal = 16
    }
}