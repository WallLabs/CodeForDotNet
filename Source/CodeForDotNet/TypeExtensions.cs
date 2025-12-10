using System;
using System.Linq;
using System.Reflection;

namespace CodeForDotNet;

/// <summary>
/// Extensions for working with type information.
/// </summary>
public static class TypeExtensions
{
    #region Public Methods

    /// <summary>
    /// Gets the first custom attribute of the specified type, if present.
    /// </summary>
    /// <returns>Typed attribute else null when not declared.</returns>
    public static T? GetAttribute<T>(this MemberInfo info, bool inherit = false)
        where T : Attribute
    {
        // Validate
        ArgumentNullException.ThrowIfNull(info);

        // Search for attribute and return when found
        var attributes = info.GetCustomAttributes(typeof(T), inherit).ToArray();
        return (attributes.Length > 0) ? (T)attributes[0] : null;
    }

    #endregion Public Methods
}
