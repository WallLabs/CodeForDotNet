using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using CodeForDotNet.Xml;

namespace CodeForDotNet;

/// <summary>
/// Serialization extensions and helper methods.
/// </summary>
public static class SerializationExtensions
{
    /// <summary>
    /// De-serializes an object from a string using the <see cref="XmlSerializer"/>.
    /// </summary>
    public static T? DeserializeXml<T>(string xml)
    {
        // Call overloaded method
        return (T?)DeserializeXml(typeof(T), xml);
    }

    /// <summary>
    /// De-serializes an object from an <see cref="XmlReader"/> using the <see cref="XmlSerializer"/>.
    /// </summary>
    public static T? DeserializeXml<T>(XmlReader xml)
    {
        // Call overloaded method
        return (T?)DeserializeXml(typeof(T), xml);
    }

    /// <summary>
    /// De-serializes an object from an <see cref="XmlReader"/> using the
    /// <see cref="XmlSerializer"/> and settings.
    /// </summary>
    public static T? DeserializeXml<T>(string xml, XmlReaderSettings settings)
    {
        // Call overloaded method
        return (T?)DeserializeXml(typeof(T), xml, settings);
    }

    /// <summary>
    /// De-serializes an object from a string using the <see cref="XmlSerializer"/>.
    /// </summary>
    public static object? DeserializeXml(Type type, string xml, XmlReaderSettings? settings = null)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(type);
        if (string.IsNullOrWhiteSpace(xml)) throw new ArgumentNullException(nameof(xml));

        // Use default settings when null
        settings ??= new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Auto };

        // Create XML reader for string
        using var reader = new StringReader(xml);
        using var xmlReader = XmlReader.Create(reader, settings);

        // Call overloaded method
        return DeserializeXml(type, xmlReader);
    }

    /// <summary>
    /// De-serializes an object from an <see cref="XmlReader"/> using the <see cref="XmlSerializer"/>.
    /// </summary>
    public static object? DeserializeXml(this Type type, XmlReader xmlReader)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(xmlReader);

        // Create cached serializer
        var serializer = XmlSerializerCache.Create(type);

        // Move to start element if necessary
        if (!xmlReader.IsStartElement())
            throw new InvalidOperationException();

        // De-serialize element
        return serializer.Deserialize(xmlReader);
    }

    /// <summary>
    /// De-serializes an object from a string using the <see cref="DataContractSerializer"/>.
    /// </summary>
    public static T? DeserializeXmlData<T>(string xml)
    {
        // Call overloaded method
        return DeserializeXmlData<T>(xml, null);
    }

    /// <summary>
    /// De-serializes an object from a string using the <see cref="DataContractSerializer"/>.
    /// </summary>
    public static T? DeserializeXmlData<T>(string xml, Type[]? extraTypes)
    {
        // Set XML options
        var settings = new XmlReaderSettings {
            ConformanceLevel = ConformanceLevel.Auto
        };

        // Create XML reader for string
        using var reader = new StringReader(xml);
        using var xmlReader = XmlReader.Create(reader, settings);

        // Call overloaded method
        return DeserializeXmlData<T>(xmlReader, extraTypes);
    }

    /// <summary>
    /// De-serializes an object from an <see cref="XmlReader"/> using the <see cref="DataContractSerializer"/>.
    /// </summary>
    public static T? DeserializeXmlData<T>(XmlReader xml)
    {
        // Call overloaded method
        return DeserializeXmlData<T>(xml, null);
    }

    /// <summary>
    /// De-serializes an object from an <see cref="XmlReader"/> using the <see cref="DataContractSerializer"/>.
    /// </summary>
    public static T? DeserializeXmlData<T>(XmlReader xmlReader, Type[]? extraTypes)
    {
        // Call overloaded method
        return (T?)DeserializeXmlData(typeof(T), xmlReader, extraTypes);
    }

    /// <summary>
    /// De-serializes an object from a string using the <see cref="DataContractSerializer"/>.
    /// </summary>
    public static object? DeserializeXmlData(Type type, string xml)
    {
        // Set XML options
        var settings = new XmlReaderSettings {
            ConformanceLevel = ConformanceLevel.Auto
        };

        // Create XML reader for string
        using var reader = new StringReader(xml);
        using var xmlReader = XmlReader.Create(reader, settings);

        // Call overloaded method
        return DeserializeXmlData(type, xmlReader, null);
    }

    /// <summary>
    /// De-serializes an object from an <see cref="XmlReader"/> using the <see cref="DataContractSerializer"/>.
    /// </summary>
    public static object? DeserializeXmlData(Type type, XmlReader xmlReader)
    {
        // Call overloaded method
        return DeserializeXmlData(type, xmlReader, null);
    }

    /// <summary>
    /// De-serializes an object from an <see cref="XmlReader"/> using the <see cref="DataContractSerializer"/>.
    /// </summary>
    public static object? DeserializeXmlData(Type type, XmlReader xmlReader, Type[]? extraTypes)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(xmlReader);

        // Create serializer with extra types when specified
        var serializer = extraTypes is null || extraTypes.Length == 0 ?
            new DataContractSerializer(type) :
            new DataContractSerializer(type, extraTypes);

        // Move to start element if necessary
        if (!xmlReader.IsStartElement())
            throw new InvalidOperationException();

        // De-serialize element
        var result = serializer.ReadObject(xmlReader);

        // Return result
        return result;
    }

    /// <summary>
    /// Determines if the specified type is XML serializable.
    /// </summary>
    public static bool IsXmlSerializable(this Type type)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(type);

        // Use reflection to see if XML serialization interface or attributes implemented
        return type.GetInterface(typeof(IXmlSerializable).FullName!) is not null ||
               type.GetCustomAttributes(typeof(XmlRootAttribute), true).Length > 0;
    }

    /// <summary>
    /// Serializes the object to an XML string using the <see cref="XmlSerializer"/>, with
    /// optional formatting.
    /// </summary>
    /// <param name="value">XML serializable object.</param>
    /// <param name="format">
    /// Set true to format the XML by indenting each parent-child element on new lines.
    /// </param>
    public static string SerializeXml(this object value, bool format = false)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(value);

        // Set XML options New line characters and handling must be explicitly set to support
        // round-tripping (an exact/comparable copy). The XML 1.0 specification requires that
        // CRLF is converted to LF and CR alone to LF.
        var settings = new XmlWriterSettings {
            ConformanceLevel = ConformanceLevel.Auto,
            NewLineHandling = NewLineHandling.Entitize,
            Indent = format
        };

        // Serialize to XML string buffer
        var buffer = new StringBuilder();
        using (var writer = XmlWriter.Create(buffer, settings))
        {
            // Call overloaded method
            SerializeXml(value, writer);
        }

        // Return result
        return buffer.ToString();
    }

    /// <summary>
    /// Serializes the object to an <see cref="XmlWriter"/> using the <see cref="XmlSerializer"/>.
    /// </summary>
    /// <param name="value">XML serializable object.</param>
    /// <param name="writer">XML writer to serialize to. It's settings may be modified.</param>
    public static void SerializeXml(this object value, XmlWriter writer)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(writer);

        // Create cached serializer
        var serializer = XmlSerializerCache.Create(value.GetType());

        // Serialize to writer
        serializer.Serialize(writer, value);
    }

    /// <summary>
    /// Serializes the object to an XML string using the <see cref="DataContractSerializer"/>,
    /// with optional formatting.
    /// </summary>
    /// <param name="value">XML serializable object.</param>
    public static string SerializeXmlData(this object value)
    {
        // Call overloaded method
        return SerializeXmlData(value, null, false);
    }

    /// <summary>
    /// Serializes the object to an XML string using the <see cref="DataContractSerializer"/>,
    /// with optional formatting.
    /// </summary>
    /// <param name="value">XML serializable object.</param>
    /// <param name="format">
    /// Set true to format the XML by indenting each parent-child element on new lines.
    /// </param>
    public static string SerializeXmlData(this object value, bool format)
    {
        // Call overloaded method
        return SerializeXmlData(value, null, format);
    }

    /// <summary>
    /// Serializes the object to an XML string using the <see cref="DataContractSerializer"/>.
    /// </summary>
    /// <param name="value">XML serializable object.</param>
    /// <param name="extraTypes">Additional types the serializer must know.</param>
    public static string SerializeXmlData(this object value, Type[] extraTypes)
    {
        // Call overloaded method
        return SerializeXmlData(value, extraTypes, false);
    }

    /// <summary>
    /// Serializes the object to an XML string using the <see cref="DataContractSerializer"/>.
    /// </summary>
    /// <param name="value">XML serializable object.</param>
    /// <param name="extraTypes">Additional types the serializer must know.</param>
    /// <param name="format">
    /// Set true to format the XML by indenting each parent-child element on new lines.
    /// </param>
    public static string SerializeXmlData(this object value, Type[]? extraTypes, bool format)
    {
        // Set XML options New line characters and handling must be explicitly preserved to
        // support round-tripping (an exact/comparable copy). The XML 1.0 specification requires
        // that CRLF is converted to LF and CR alone to LF.
        var settings = new XmlWriterSettings {
            ConformanceLevel = ConformanceLevel.Auto,
            NewLineHandling = NewLineHandling.Entitize,
            Indent = format
        };

        // Serialize to XML string buffer
        var buffer = new StringBuilder();
        using (var writer = XmlWriter.Create(buffer, settings))
        {
            // Call overloaded method
            SerializeXmlData(value, writer, extraTypes);
        }

        // Return result
        return buffer.ToString();
    }

    /// <summary>
    /// Serializes the object to an <see cref="XmlWriter"/> using the <see cref="DataContractSerializer"/>.
    /// </summary>
    /// <param name="value">XML serializable object.</param>
    /// <param name="writer">XML writer to serialize to. It's settings may be modified.</param>
    public static void SerializeXmlData(this object value, XmlWriter writer)
    {
        // Call overloaded method
        SerializeXmlData(value, writer, null);
    }

    /// <summary>
    /// Serializes the object to an <see cref="XmlWriter"/> using the <see cref="DataContractSerializer"/>.
    /// </summary>
    /// <param name="value">XML serializable object.</param>
    /// <param name="writer">XML writer to serialize to. It's settings may be modified.</param>
    /// <param name="extraTypes">Additional types the serializer must know.</param>
    public static void SerializeXmlData(this object value, XmlWriter writer, Type[]? extraTypes)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(value);

        // Create serializer with extra types when specified
        var serializer = extraTypes is null || extraTypes.Length == 0 ?
            new DataContractSerializer(value.GetType()) :
            new DataContractSerializer(value.GetType(), extraTypes);

        // Serialize to writer
        serializer.WriteObject(writer, value);
    }
}
