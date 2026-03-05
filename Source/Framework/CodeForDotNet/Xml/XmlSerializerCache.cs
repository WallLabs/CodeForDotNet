using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using CodeForDotNet.Collections;
using CodeForDotNet.Properties;

namespace CodeForDotNet.Xml;

/// <summary>
/// <see cref="XmlSerializer"/> cache. A factory for XML serializers which stores created
/// instances in memory for performance.
/// </summary>
public static class XmlSerializerCache
{
    /// <summary>
    /// Types included by the framework.
    /// </summary>
    /// <remarks>
    /// It is necessary to add some types here which were forgotten by Microsoft in the standard
    /// XML serializer. Originally it was supposed to support all built-in types without any
    /// special declarations.
    /// </remarks>
    private static readonly Type[] _builtInTypes;

    /// <summary>
    /// Dictionary of cached XML serializers indexed by <see cref="Type.FullName"/> or a custom key.
    /// </summary>
    private static readonly Dictionary<string, XmlSerializer> _cache;

    /// <summary>
    /// Collection of additional XML types which should be registered with the serializer.
    /// </summary>
    /// <remarks>
    /// Includes <see cref="_builtInTypes"/> by default or when <see cref="Flush()"/> is called.
    /// </remarks>
    private static Type[] _xmlTypes;

    /// <summary>
    /// Static initializer.
    /// </summary>
    static XmlSerializerCache()
    {
        // Initialize members
        _cache = [];
        _builtInTypes = [typeof(DateTimeOffset)];

        // Add built-in types
        _xmlTypes = _builtInTypes;
        _cache.Add(typeof(DateTimeOffset).FullName!, new XmlSerializer(typeof(XmlDateTime)));
    }

    /// <summary>
    /// Creates or returns a cached XML serializer.
    /// </summary>
    /// <param name="serializedType">Type of the serializer.</param>
    /// <param name="key">
    /// Optional key for the serializer, required when a custom serializer is being cached.
    /// Defaults to <paramref name="serializedType.FullName"/>.
    /// </param>
    /// <param name="constructor">
    /// Optional constructor to call when the serializer is created. Not necessary for standard
    /// type serializers, used to create custom types, e.g. with serializer overloads.
    /// </param>
    /// <returns>XML serializer for the type.</returns>
    public static XmlSerializer Create(Type serializedType, string? key = null, Func<XmlSerializer>? constructor = null)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(serializedType);

        // Use default key when not specified
        if (string.IsNullOrWhiteSpace(key))
            key = serializedType.FullName;

        // Use standard constructor when not specified
        constructor ??= () => { return new XmlSerializer(serializedType, _xmlTypes); };

        // Create serializer first time
        XmlSerializer? serializer = null;
        if (!_cache.ContainsKey(key!))
        {
            lock (_cache)
            {
                // Thread safe double-check lock
                if (!_cache.ContainsKey(key!))
                {
                    // Create serializer
                    serializer = constructor();

                    // Add to cache
                    _cache.Add(key!, serializer);
                }
            }
        }

        // Use cached serializer
        serializer ??= _cache[key!];

        // Return result
        return serializer;
    }

    /// <summary>
    /// Flushes all serializers from the cache.
    /// </summary>
    public static void Flush()
    {
        // Flush cache
        lock (_cache)
        {
            // Clear cache
            _cache.Clear();

            // Add built-in types
            _xmlTypes = _builtInTypes;
            _cache.Add(typeof(DateTimeOffset).FullName!, new XmlSerializer(typeof(XmlDateTime)));
        }
    }

    /// <summary>
    /// Flushes any cached serializer for the specified type.
    /// </summary>
    /// <param name="serializedType">Serialized type to flush from the cache, if present.</param>
    public static void Flush(Type serializedType)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(serializedType);

        // Do not allow built-in types to be removed
        if (_builtInTypes.ContainsItem(serializedType))
        {
            throw new ArgumentException(
            Resources.XmlSerializerCacheErrorFlushBuiltInType, serializedType.FullName);
        }

        // Get the key
        var key = serializedType.FullName!;

        // Call overloaded method
        Flush(key);
    }

    /// <summary>
    /// Flushes any cached serializer with the specified key.
    /// </summary>
    /// <param name="key">Key of the serializer to flush from the cache, if present.</param>
    public static void Flush(string key)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

        // Do not allow built-in types to be removed
        if (_builtInTypes.ContainsItem((item) => { return item.FullName!.Equals(key, StringComparison.OrdinalIgnoreCase); }))
            throw new ArgumentException(Resources.XmlSerializerCacheErrorFlushBuiltInType, key);

        // Check if exists
        if (_cache.ContainsKey(key))
        {
            lock (_cache)
            {
                // Thread safe double-check lock
                if (_cache.ContainsKey(key))
                {
                    // Remove from cache
                    _ = _cache.Remove(key);
                }
            }
        }
    }

    /// <summary>
    /// Registers additional types to be supported during XML serialization.
    /// </summary>
    /// <param name="types">Types to register.</param>
    /// <remarks>
    /// Does not extend already cached serializers. To ensure new types are supported by all
    /// existing serialized types call <see cref="Flush(Type)"/>.
    /// </remarks>
    public static void RegisterTypes(IEnumerable<Type> types)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(types);

        // Thread-safe lock
        lock (_xmlTypes)
        {
            // Merged types and detect any change
            var register = new List<Type>(_xmlTypes);
            var changed = false;
            foreach (var type in types)
            {
                if (!register.Contains(type))
                {
                    register.Add(type);
                    changed = true;
                }
            }

            // Update type array when changed
            if (changed)
                _xmlTypes = [.. register];
        }
    }
}
