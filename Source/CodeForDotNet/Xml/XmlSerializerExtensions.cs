using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using CodeForDotNet.Collections;

namespace CodeForDotNet.Xml
{
    /// <summary>
    /// XML serialization extensions.
    /// </summary>
    public static class XmlSerializerExtensions
    {
        /// <summary>
        /// Determines if the specified type is XML serializable.
        /// </summary>
        public static bool IsXmlSerializable(this Type type)
        {
            // Validate
            if (type == null) throw new ArgumentNullException(nameof(type));

            // Use reflection to see if XML serialization interface or attributes implemented
            var typeInfo = type.GetTypeInfo();
            return typeInfo.ImplementedInterfaces.Contains(typeof(IXmlSerializable)) ||
                   typeInfo.GetAttribute<XmlRootAttribute>() != null;
        }

        /// <summary>
        /// Serializes the object to an XML string using the <see cref="XmlSerializer"/>,
        /// with optional formatting.
        /// </summary>
        /// <param name="value">XML serializable object.</param>
        public static string SerializeXml(this object value)
        {
            // Call overloaded method
            return SerializeXml(value, null, false);
        }

        /// <summary>
        /// Serializes the object to an XML string using the <see cref="XmlSerializer"/>,
        /// with optional formatting.
        /// </summary>
        /// <param name="value">XML serializable object.</param>
        /// <param name="format">Set true to format the XML by indenting each parent-child element on new lines.</param>
        public static string SerializeXml(this object value, bool format)
        {
            // Call overloaded method
            return SerializeXml(value, null, format);
        }

        /// <summary>
        /// Serializes the object to an XML string using the <see cref="XmlSerializer"/>.
        /// </summary>
        /// <param name="value">XML serializable object.</param>
        /// <param name="extraTypes">Additional types the serializer must know.</param>
        public static string SerializeXml(this object value, Type[] extraTypes)
        {
            // Call overloaded method
            return SerializeXml(value, extraTypes, false);
        }

        /// <summary>
        /// Serializes the object to an XML string using the <see cref="XmlSerializer"/>.
        /// </summary>
        /// <param name="value">XML serializable object.</param>
        /// <param name="extraTypes">Additional types the serializer must know.</param>
        /// <param name="format">Set true to format the XML by indenting each parent-child element on new lines.</param>
        public static string SerializeXml(this object value, Type[] extraTypes, bool format)
        {
            // Use consistent line endings so that serialize/de-serialize round-trips
            // In testing it was proven that the .NET framework uses Unix line endings (CR without LF)
            // mostly and when replace is not specified certain circumstances (combination of
            // IXmlSerialiable and other serializer attributes) would not round trip
            // because in other places the CR and LF would be used by default. Hence we fix
            // the Unix style line endings. Source solution with issues was Reaction Server v1.5.
            // Possibly re-test with newer version or when more time to investigate.
            var settings = new XmlWriterSettings
            {
                Indent = format,
                NewLineHandling = format ? NewLineHandling.Replace : NewLineHandling.Entitize
            };

            // Serialize to XML string buffer
            var buffer = new StringBuilder();
            using (var writer = XmlWriter.Create(buffer, settings))
            {
                // Call overloaded method
                SerializeXml(value, writer, extraTypes);
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
            // Call overloaded method
            SerializeXml(value, writer, null);
        }

        /// <summary>
        /// Serializes the object to an <see cref="XmlWriter"/> using the <see cref="XmlSerializer"/>.
        /// </summary>
        /// <param name="value">XML serializable object.</param>
        /// <param name="writer">XML writer to serialize to. It's settings may be modified.</param>
        /// <param name="extraTypes">Additional types the serializer must know.</param>
        public static void SerializeXml(this object value, XmlWriter writer, Type[] extraTypes)
        {
            // Validate
            if (ReferenceEquals(value, null)) throw new ArgumentNullException(nameof(value));

            // Create serializer with extra types when specified
            var serializer = extraTypes == null || extraTypes.Length == 0 ?
                new XmlSerializer(value.GetType()) :
                new XmlSerializer(value.GetType(), extraTypes);

            // Serialize to writer
            serializer.Serialize(writer, value);
        }

        /// <summary>
        /// De-serializes an object from a string using the <see cref="XmlSerializer"/>.
        /// </summary>
        public static T DeserializeXml<T>(string xml)
        {
            // Call overloaded method
            return DeserializeXml<T>(xml, null);
        }

        /// <summary>
        /// De-serializes an object from a string using the <see cref="XmlSerializer"/>.
        /// </summary>
        public static T DeserializeXml<T>(string xml, Type[] extraTypes)
        {
            // Create XML reader for string
            using (var reader = new StringReader(xml))
            {
                var xmlReader = XmlReader.Create(reader);

                // Call overloaded method
                return DeserializeXml<T>(xmlReader, extraTypes);
            }
        }

        /// <summary>
        /// De-serializes an object from an <see cref="XmlReader"/> using the <see cref="XmlSerializer"/>.
        /// </summary>
        public static T DeserializeXml<T>(XmlReader xml)
        {
            // Call overloaded method
            return DeserializeXml<T>(xml, null);
        }

        /// <summary>
        /// De-serializes an object from an <see cref="XmlReader"/> using the <see cref="XmlSerializer"/>.
        /// </summary>
        public static T DeserializeXml<T>(XmlReader xmlReader, Type[] extraTypes)
        {
            // Call overloaded method
            return (T)DeserializeXml(typeof(T), xmlReader, extraTypes);
        }

        /// <summary>
        /// De-serializes an object from a string using the <see cref="XmlSerializer"/>.
        /// </summary>
        public static object DeserializeXml(Type type, string xml)
        {
            // Create XML reader for string
            using (var reader = new StringReader(xml))
            {
                var xmlReader = XmlReader.Create(reader);

                // Call overloaded method
                return DeserializeXml(type, xmlReader, null);
            }
        }

        /// <summary>
        /// De-serializes an object from an <see cref="XmlReader"/> using the <see cref="XmlSerializer"/>.
        /// </summary>
        public static object DeserializeXml(Type type, XmlReader xmlReader)
        {
            // Call overloaded method
            return DeserializeXml(type, xmlReader, null);
        }

        /// <summary>
        /// De-serializes an object from an <see cref="XmlReader"/> using the <see cref="XmlSerializer"/>.
        /// </summary>
        public static object DeserializeXml(Type type, XmlReader xmlReader, Type[] extraTypes)
        {
            // Validate
            if (xmlReader == null) throw new ArgumentNullException(nameof(xmlReader));

            // Create serializer with extra types when specified
            var serializer = extraTypes == null || extraTypes.Length == 0 ?
                new XmlSerializer(type) :
                new XmlSerializer(type, extraTypes);

            // Check start element
            var hasEnd = !xmlReader.IsEmptyElement;
            var depth = xmlReader.Depth;
            var rootElementName = xmlReader.Name;

            // De-serialize element
            var result = serializer.Deserialize(xmlReader);

            // Read past end element if serializer does not do this (occurs in some cases)
            if (hasEnd && xmlReader.Depth == depth && xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == rootElementName)
                xmlReader.ReadEndElement();

            // Return result
            return result;
        }
    }
}
