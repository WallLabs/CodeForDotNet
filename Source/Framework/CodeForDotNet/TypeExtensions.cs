using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace CodeForDotNet;

/// <summary>
/// Extensions for working with type information.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Extended <see cref="Type.Equals(Type)"/> method which supports generic types and
    /// comparison both directly (via generic arguments) or indirect (via generic restrictions).
    /// </summary>
    /// <param name="type">Type to test.</param>
    /// <param name="targetType">
    /// Target (generic) type to compare against. Will also work with non-generic types, but
    /// slightly less efficiently than the direct method.
    /// </param>
    /// <returns>True when the type matches the target type.</returns>
    public static bool EqualsGenericType(this Type type, Type targetType)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(targetType);

        // Call direct method when not generic
        if (!type.IsGenericType)
            return type.Equals(targetType);

        // Compare generic type
        if (type.DeclaringType != targetType.DeclaringType) return false;
        var argumentCount = type.GenericTypeArguments.Length;
        if (targetType.GenericTypeArguments.Length != argumentCount) return false;
        for (var index = 0; index < argumentCount; index++)
        {
            var argument = type.GenericTypeArguments[index];
            var targetArgument = targetType.GenericTypeArguments[index];
            if (!targetArgument.IsAssignableFrom(argument))
                return false;
        }

        // Found match
        return true;
    }

    /// <summary>
    /// Gets an attribute from an enumeration member.
    /// </summary>
    /// <typeparam name="T">Type of the attribute to retrieve.</typeparam>
    /// <param name="value">Enumeration value from which to get the attribute.</param>
    /// <returns>Attribute type <typeparamref name="T"/> or null when not defined.</returns>
    public static T? GetAttribute<T>(this Enum value)
        where T : Attribute
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(value);

        // Get enumeration value (field) metadata.
        var type = value.GetType();
        var name = Enum.GetName(type, value)
            ?? throw new ArgumentOutOfRangeException(nameof(value));
        var field = type.GetField(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField)!;

        // Return any defined custom attributes.
        return field.GetCustomAttribute<T>(false);
    }

    /// <summary>
    /// Gets the first custom attribute of the specified type (or any base classes), if present.
    /// </summary>
    /// <returns>Typed attribute else null when not declared.</returns>
    [SuppressMessage("Usage", "CA2263:Prefer generic overload when type is known", Justification = "Overload definition. The caller will decide which method is best.")]
    public static T? GetAttribute<T>(this MemberInfo info)
         where T : Attribute
    {
        // Call overloaded method
        return (T?)GetAttribute(info, typeof(T), true);
    }

    /// <summary>
    /// Gets the first custom attribute of the specified type (optionally including base
    /// classes), if present.
    /// </summary>
    /// <returns>Typed attribute else null when not declared.</returns>
    public static T? GetAttribute<T>(this MemberInfo info, bool inherit)
        where T : Attribute
    {
        // Call overloaded method
        return (T?)GetAttribute(info, typeof(T), inherit);
    }

    /// <summary>
    /// Gets the first custom attribute of the specified type (or any base classes), if present.
    /// </summary>
    /// <returns>Typed attribute else null when not declared.</returns>
    public static Attribute? GetAttribute(this MemberInfo info, Type attributeType)
    {
        // Call overloaded method
        return GetAttribute(info, attributeType, true);
    }

    /// <summary>
    /// Gets the first custom attribute of the specified type (optionally including base
    /// classes), if present.
    /// </summary>
    /// <returns>Typed attribute else null when not declared.</returns>
    public static Attribute? GetAttribute(this MemberInfo info, Type attributeType, bool inherit)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(info);

        // Search for attribute and return when found
        var attributes = info.GetCustomAttributes(attributeType, inherit);
        return (attributes.Length > 0) ? (Attribute)attributes[0] : null;
    }

    /// <summary>
    /// Gets the first custom attribute data of the specified type, if present.
    /// </summary>
    /// <returns>Typed attribute data else null when not declared.</returns>
    /// <remarks>
    /// In contrast to <see cref="GetAttribute{T}(MemberInfo, bool)"/> this method is required
    /// to work with assembly metadata loaded via <see cref="Assembly.ReflectionOnlyLoadFrom"/>.
    /// </remarks>
    public static CustomAttributeData? GetAttributeData(this MemberInfo info, Type attributeType)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(info);
        ArgumentNullException.ThrowIfNull(attributeType);

        // Search for attribute and return when found
        var attributes = info.GetCustomAttributesData();
        return attributes.FirstOrDefault(attribute =>
            attribute.Constructor.DeclaringType!.FullName == attributeType.FullName);
    }

    /// <summary>
    /// Gets attributes from an enumeration member.
    /// </summary>
    /// <typeparam name="T">Type of the attribute to retrieve.</typeparam>
    /// <param name="value">Enumeration value from which to get the attribute.</param>
    /// <returns>Attribute type <typeparamref name="T"/> or null when not defined.</returns>
    public static IEnumerable<T> GetAttributes<T>(this Enum value)
        where T : Attribute
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(value);

        // Get enumeration value (field) metadata.
        var type = value.GetType();
        var name = Enum.GetName(type, value)
            ?? throw new ArgumentOutOfRangeException(nameof(value));
        var field = type.GetField(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField)!;

        // Return any defined custom attributes.
        return field.GetCustomAttributes<T>(false);
    }

    /// <summary>
    /// Gets the custom attributes of the specified type (or any base classes), if present.
    /// </summary>
    /// <returns>Typed attributes if declared.</returns>
    [SuppressMessage("Usage", "CA2263:Prefer generic overload when type is known", Justification = "Overload definition. The caller will decide which method is best.")]
    public static IEnumerable<T> GetAttributes<T>(this MemberInfo info)
         where T : Attribute
    {
        // Call overloaded method
        return GetAttributes(info, typeof(T), true).Cast<T>();
    }

    /// <summary>
    /// Gets the custom attributes of the specified type (optionally including base classes), if present.
    /// </summary>
    /// <returns>Typed attributes if declared.</returns>
    public static IEnumerable<T> GetAttributes<T>(this MemberInfo info, bool inherit)
        where T : Attribute
    {
        // Call overloaded method
        return GetAttributes(info, typeof(T), inherit).Cast<T>();
    }

    /// <summary>
    /// Gets the custom attributes of the specified type (or any base classes), if present.
    /// </summary>
    /// <returns>Typed attributes if declared.</returns>
    public static IEnumerable<Attribute> GetAttributes(this MemberInfo info, Type attributeType)
    {
        // Call overloaded method
        return GetAttributes(info, attributeType, true);
    }

    /// <summary>
    /// Gets the custom attributes of the specified type (optionally including base classes), if present.
    /// </summary>
    /// <returns>Typed attributes if declared.</returns>
    public static IEnumerable<Attribute> GetAttributes(this MemberInfo info, Type attributeType, bool inherit)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(info);

        // Search for attribute and return when found
        return [.. info.GetCustomAttributes(attributeType, inherit).Cast<Attribute>()];
    }

    /// <summary>
    /// Gets the custom attributes data of the specified type, if present.
    /// </summary>
    /// <returns>Typed attribute data if declared.</returns>
    /// <remarks>
    /// In contrast to <see cref="GetAttribute{T}(MemberInfo, bool)"/> this method is required
    /// to work with assembly metadata loaded via <see cref="Assembly.ReflectionOnlyLoadFrom"/>.
    /// </remarks>
    public static IEnumerable<CustomAttributeData> GetAttributesData(this MemberInfo info, Type attributeType)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(info);
        ArgumentNullException.ThrowIfNull(attributeType);

        // Search for attribute and return when found
        var attributes = info.GetCustomAttributesData();
        return [.. attributes.Where(attribute =>
            attribute.Constructor.DeclaringType!.FullName == attributeType.FullName)];
    }

    /// <summary>
    /// Gets the default value for a given type, e.g. 0 for integers, null for reference types.
    /// </summary>
    /// <param name="type">Type to get the default for.</param>
    /// <returns>Default value.</returns>
    public static object? GetDefaultValue(this Type type)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(type);

        // Return default for value types, null for reference and nullable types
        return
            type.IsValueType && !type.IsNullable()
            ? Activator.CreateInstance(type)
            : null;
    }

    /// <summary>
    /// Searches for members with a specific attribute.
    /// </summary>
    /// <param name="type">Type to examine.</param>
    /// <typeparam name="TAttribute">Attribute type to find.</typeparam>
    /// <returns>Collection of members, empty (not null) when none exist.</returns>
    public static Dictionary<MemberInfo, TAttribute> GetMembersWithAttribute<TAttribute>(this Type type)
        where TAttribute : Attribute
    {
        // Validate
        ArgumentNullException.ThrowIfNull(type);

        // Find and return members with the attribute defined
        var result = new Dictionary<MemberInfo, TAttribute>();
        foreach (var member in type.GetMembers())
        {
            var attribute = member.GetAttribute<TAttribute>();
            if (attribute != null)
                result.Add(member, attribute);
        }
        return result;
    }

    /// <summary>
    /// Searches for members with a specific attribute (or multiple of the same type).
    /// </summary>
    /// <param name="type">Type to examine.</param>
    /// <typeparam name="TAttribute">Attribute type to find.</typeparam>
    /// <returns>Collection of members and their attributes, empty (not null) when none exist.</returns>
    public static Dictionary<MemberInfo, IEnumerable<TAttribute>> GetMembersWithAttributes<TAttribute>(this Type type)
        where TAttribute : Attribute
    {
        // Validate
        ArgumentNullException.ThrowIfNull(type);

        // Find and return members with the attribute defined
        var result = new Dictionary<MemberInfo, IEnumerable<TAttribute>>();
        foreach (var member in type.GetMembers())
        {
            var attributes = member.GetAttributes<TAttribute>().ToArray();
            if (attributes.Length > 0)
                result.Add(member, attributes);
        }
        return result;
    }

    /// <summary>
    /// Searches for properties with a specific attribute.
    /// </summary>
    /// <param name="type">Type to examine.</param>
    /// <typeparam name="TAttribute">Attribute type to find.</typeparam>
    /// <returns>
    /// Collection of properties and their attribute, empty (not null) when none exist.
    /// </returns>
    public static Dictionary<PropertyInfo, TAttribute> GetPropertiesWithAttribute<TAttribute>(this Type type)
         where TAttribute : Attribute
    {
        // Validate
        ArgumentNullException.ThrowIfNull(type);

        // Find and return properties with the attribute defined
        var result = new Dictionary<PropertyInfo, TAttribute>();
        foreach (var property in type.GetProperties())
        {
            var attribute = property.GetAttribute<TAttribute>();
            if (attribute != null)
                result.Add(property, attribute);
        }
        return result;
    }

    /// <summary>
    /// Searches for properties with a specific attribute (or multiple of the same type).
    /// </summary>
    /// <param name="type">Type to examine.</param>
    /// <typeparam name="TAttribute">Attribute type to find.</typeparam>
    /// <returns>
    /// Collection of properties and their attributes, empty (not null) when none exist.
    /// </returns>
    public static Dictionary<PropertyInfo, IEnumerable<TAttribute>> GetPropertiesWithAttributes<TAttribute>(this Type type)
         where TAttribute : Attribute
    {
        // Validate
        ArgumentNullException.ThrowIfNull(type);

        // Find and return properties with the attribute defined
        var result = new Dictionary<PropertyInfo, IEnumerable<TAttribute>>();
        foreach (var property in type.GetProperties())
        {
            var attributes = property.GetAttributes<TAttribute>().ToArray();
            if (attributes.Length > 0)
                result.Add(property, attributes);
        }
        return result;
    }

    /// <summary>
    /// Returns true when the type is a <see cref="Nullable{T}"/> type.
    /// </summary>
    public static bool IsNullable(this Type type)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(type);

        // Return true when nullable
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    /// <summary>
    /// Extended <see cref="Type.IsSubclassOf(Type)"/> method which supports generic types and
    /// comparison both direct (via generic arguments) or indirect (via generic restrictions).
    /// </summary>
    /// <param name="type">Type to test.</param>
    /// <param name="targetType">
    /// Target (generic) type to compare against. May be a non-generic type because it is
    /// possible generic types are further down in the hierarchy.
    /// </param>
    /// <returns>True when any of the base classes matching the target type.</returns>
    public static bool IsSubclassOfGenericType(this Type type, Type targetType)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(targetType);

        // Compare all types in hierarchy We also compare non-generic types as they may have a
        // generic base class in their hierarchy
        var current = type;
        while (current != null)
        {
            // Compare generic type
            if (EqualsGenericType(current, targetType))
                return true;

            // Continue with base type...
            current = current.BaseType;
        }

        // No match
        return false;
    }

    /// <summary>
    /// Recursively resolves paths on object types.
    /// </summary>
    /// <remarks>
    /// Used to lookup binding information. Paths are in dotted form, e.g. "Property1.FieldA";
    /// Arrays are supported which resolve to the target type of the array items, e.g.
    /// "ArrayProperty1[3].PropertyA" is really the "PropertyA" of the "ArrayProperty1"
    /// enumerated item type.
    /// </remarks>
    /// <param name="containerType">Type from which to resolve the path.</param>
    /// <param name="path">Path to resolve, relative to the <paramref name="containerType"/>.</param>
    /// <returns>Type information about the target member, or null when invalid.</returns>
    public static MemberInfo? ResolvePath(this Type containerType, string path)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(containerType);

        // Return null when no path specified
        if (string.IsNullOrWhiteSpace(path))
            return null;

        // Get member name
        var dotIndex = path.IndexOf('.');
        if (dotIndex == 0)
            return null;                // Invalid (starts with dot)
        var memberName = dotIndex > 0 ? path[..dotIndex] : path;

        // Check for array opening bracket
        var openBracketIndex = memberName.IndexOf('[');
        if (openBracketIndex == 0)
            return null;                // Invalid (starts with array bracket)
        var arrayIndex = -1;
        if (openBracketIndex > 0)
        {
            // Check for closing bracket
            var closeBracketIndex = memberName.IndexOf(']', openBracketIndex);
            if (closeBracketIndex <= 0)
                return null;            // Invalid (missing closing bracket or empty

            // Extract and check array index number
            var arrayIndexString = memberName.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
            if (!int.TryParse(arrayIndexString, out arrayIndex))
                return null;            // Invalid (array index not a valid number)

            // Strip brackets from member name
            memberName = memberName[..openBracketIndex];
        }

        // Lookup member type
        var memberInfo = containerType.GetMember(memberName, BindingFlags.Public | BindingFlags.Instance |
            BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase).FirstOrDefault();
        if (memberInfo is null)
            return null;                // Invalid (member not found)

        // Return member when no more children
        if (dotIndex < 0)
            return memberInfo;

        // Resolve contain for child path
        Type memberType;
        var propertyInfo = memberInfo as PropertyInfo;
        if (propertyInfo != null)
        {
            // Resolve properties to their contained type
            memberType = propertyInfo.PropertyType;
        }
        else
        {
            // Resolve arrays to their enumerated type
            memberType = memberInfo.GetType();
            if (arrayIndex >= 0)
            {
                if (!memberType.HasElementType)
                    return null;            // Invalid (not an array)
                _ = memberType.GetElementType();
            }
        }

        // Recurse to return child result
        return ResolvePath(memberType, path[(dotIndex + 1)..]);
    }
}
