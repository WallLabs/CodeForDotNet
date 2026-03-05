using System;

namespace CodeForDotNet.ComponentModel.DataAnnotations;

/// <summary>
/// Defines options for formatting a <see cref="DateTime"/> derived data field.
/// </summary>
/// <remarks>
/// Support for trimming is implemented in the model binder, as currently Data Annotations
/// provide no global mechanism to coerce the value.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class DateTimeFormatAttribute : Attribute
{
    /// <summary>
    /// Creates an instance with default values, i.e. display local, store universal.
    /// </summary>
    public DateTimeFormatAttribute()
    {
        Display = DateTimeKind.Local;
        Store = DateTimeKind.Utc;
    }

    /// <summary>
    /// Initializes a new instance with required values.
    /// </summary>
    public DateTimeFormatAttribute(DateTimeKind display, DateTimeKind store)
    {
        Display = display;
        Store = store;
    }

    /// <summary>
    /// Display format.
    /// </summary>
    public DateTimeKind Display { get; set; }

    /// <summary>
    /// Store format.
    /// </summary>
    public DateTimeKind Store { get; set; }
}
