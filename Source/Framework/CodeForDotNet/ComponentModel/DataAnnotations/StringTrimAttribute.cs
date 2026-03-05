using System;

namespace CodeForDotNet.ComponentModel.DataAnnotations;

/// <summary>
/// Denotes a data field that should be trimmed during binding, removing any spaces.
/// </summary>
/// <remarks>
/// <para>
/// Support for trimming is implemented in the model binder, as currently Data Annotations
/// provides no global mechanism to coerce the value.
/// </para>
/// <para>
/// This attribute does not imply that empty strings should be converted to null. Use the
/// <see cref="System.ComponentModel.DataAnnotations.DisplayFormatAttribute.ConvertEmptyStringToNull"/>
/// attribute property to control what happens with empty strings.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
[Obsolete("It is not a common issue of an entity to trim content, the binder should not inject whitespace around input fields or in data files. Remove this attribute and deal with any parsing/binding issues in those areas.")]
public sealed class StringTrimAttribute : Attribute
{
}
