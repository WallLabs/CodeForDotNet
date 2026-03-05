using System;

namespace CodeForDotNet.ComponentModel.DataAnnotations;

/// <summary>
/// Attribute which defines a logical validation group name.
/// </summary>
/// <remarks>
/// A common use case for this attribute is to support partial GUI validation, necessary to work
/// on complex entities with multiple tabs or wizard pages. Specify this attribute multiple
/// times to cover each scenario. Some standard name constants are provided in <see cref="ValidationGroupNames"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public sealed class ValidationGroupAttribute : Attribute
{
    /// <summary>
    /// Creates an instance with the required value.
    /// </summary>
    public ValidationGroupAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Name.
    /// </summary>
    public string Name { get; private set; }
}
