using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace CodeForDotNet.Xml
{
    /// <summary>
    /// Creates extensions and helper methods for working with XML.
    /// </summary>
    public static class XmlExtensions
    {
        #region String Extensions

        /// <summary>
        /// Removes any invalid XML characters from a string.
        /// </summary>
        /// <param name="value">XML string.</param>
        /// <returns>Valid XML string.</returns>
        public static string StripInvalidXmlChars(this string value)
        {
            // Validate
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            // Return string with invalid characters removed
            return new string(value.ToCharArray().Where(IsValidInXml).ToArray());
        }

        /// <summary>
        /// Checks whether a character is valid according to the XML v1.0 standard,
        /// e.g. most control characters are not allowed.
        /// </summary>
        /// <param name="value">Character to check.</param>
        /// <returns>True if the character is allowed else false.</returns>
        public static bool IsValidInXml(this char value)
        {
            return
            (
                 // Check valid control characters...
                 value == 0x9 ||    // Horizontal tab (ASCII 9 or '\t').
                 value == 0xA ||    // New line (ASCII 10 or '\n').
                 value == 0xD ||    // Carriage (ASCII 13 or '\r').

                // Check valid alphabet, whitespace, accents and signs...
                (value >= 0x20 && value <= 0xD7FF) ||
                (value >= 0xE000 && value <= 0xFFFD)
            );
        }

        #endregion String Extensions

        #region XmlReader Extensions

        /// <summary>
        /// Overload of the <see cref="XmlReader.ReadElementContentAsInt()"/> which behaves similarly to the XmlReader.ReadElementString() method.
        /// Required because the <see cref="XmlReader.ReadElementContentAsInt()"/> is not robust enough because it does not move to the next element before reading,
        /// so fails if any other content (whitespace, comments, etc...) appears at the current position, just before the next element to read.
        /// </summary>
        /// <param name="reader">Extension target.</param>
        /// <param name="localName">
        /// Optional, when both this and the <paramref name="namespace"/> parameter are specified
        /// the <see cref="XmlReader.ReadStartElement(string, string)"/> method is called before reading the content.
        /// If only this parameter is specified the <see cref="XmlReader.ReadStartElement(string)"/> overload is called,
        /// otherwise the <see cref="XmlReader.ReadStartElement()"/>.</param>
        /// <param name="namespace">Optional, when specified causes the <see cref="XmlReader.ReadStartElement(string, string)"/> to be called.</param>
        /// <return>Value read from the current or specified (name/namespace) element.</return>
        public static int ReadElementInt32(this XmlReader reader, string localName = null, string @namespace = null)
        {
            // Validate
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (@namespace != null && localName == null) throw new ArgumentNullException(nameof(localName));

            // Read element start (moving past any other non-element content)
            // using the appropriate overload depending on parameters passed
            if (@namespace != null)
                reader.ReadStartElement(localName, @namespace);
            else if (localName != null)
                reader.ReadStartElement(localName);
            else
                reader.ReadStartElement();

            // Read value
            var value = reader.ReadContentAsInt();

            // Read element end
            reader.ReadEndElement();

            // Return result
            return value;
        }

        /// <summary>
        /// Overload of the <see cref="XmlReader.ReadElementContentAsLong()"/> which behaves similarly to the XmlReader.ReadElementString() method.
        /// Required because the <see cref="XmlReader.ReadElementContentAsLong()"/> is not robust enough because it does not move to the next element before reading,
        /// so fails if any other content (whitespace, comments, etc...) appears at the current position, just before the next element to read.
        /// </summary>
        /// <param name="reader">Extension target.</param>
        /// <param name="localName">
        /// Optional, when both this and the <paramref name="namespace"/> parameter are specified
        /// the <see cref="XmlReader.ReadStartElement(string, string)"/> method is called before reading the content.
        /// If only this parameter is specified the <see cref="XmlReader.ReadStartElement(string)"/> overload is called,
        /// otherwise the <see cref="XmlReader.ReadStartElement()"/>.</param>
        /// <param name="namespace">Optional, when specified causes the <see cref="XmlReader.ReadStartElement(string, string)"/> to be called.</param>
        /// <return>Value read from the current or specified (name/namespace) element.</return>
        public static long ReadElementInt64(this XmlReader reader, string localName = null, string @namespace = null)
        {
            // Validate
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (@namespace != null && localName == null) throw new ArgumentNullException(nameof(localName));

            // Read element start (moving past any other non-element content)
            // using the appropriate overload depending on parameters passed
            if (@namespace != null)
                reader.ReadStartElement(localName, @namespace);
            else if (localName != null)
                reader.ReadStartElement(localName);
            else
                reader.ReadStartElement();

            // Read value
            var value = reader.ReadContentAsLong();

            // Read element end
            reader.ReadEndElement();

            // Return result
            return value;
        }

        /// <summary>
        /// Overload of the <see cref="XmlReader.ReadElementContentAsDecimal()"/> which behaves similarly to the XmlReader.ReadElementString() method.
        /// Required because the <see cref="XmlReader.ReadElementContentAsDecimal()"/> is not robust enough because it does not move to the next element before reading,
        /// so fails if any other content (whitespace, comments, etc...) appears at the current position, just before the next element to read.
        /// </summary>
        /// <param name="reader">Extension target.</param>
        /// <param name="localName">
        /// Optional, when both this and the <paramref name="namespace"/> parameter are specified
        /// the <see cref="XmlReader.ReadStartElement(string, string)"/> method is called before reading the content.
        /// If only this parameter is specified the <see cref="XmlReader.ReadStartElement(string)"/> overload is called,
        /// otherwise the <see cref="XmlReader.ReadStartElement()"/>.</param>
        /// <param name="namespace">Optional, when specified causes the <see cref="XmlReader.ReadStartElement(string, string)"/> to be called.</param>
        /// <return>Value read from the current or specified (name/namespace) element.</return>
        public static decimal ReadElementDecimal(this XmlReader reader, string localName = null, string @namespace = null)
        {
            // Validate
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (@namespace != null && localName == null) throw new ArgumentNullException(nameof(localName));

            // Read element start (moving past any other non-element content)
            // using the appropriate overload depending on parameters passed
            if (@namespace != null)
                reader.ReadStartElement(localName, @namespace);
            else if (localName != null)
                reader.ReadStartElement(localName);
            else
                reader.ReadStartElement();

            // Read value
            var value = reader.ReadContentAsDecimal();

            // Read element end
            reader.ReadEndElement();

            // Return result
            return value;
        }

        /// <summary>
        /// Overload of the <see cref="XmlReader.ReadElementContentAsFloat()"/> which behaves similarly to the XmlReader.ReadElementString() method.
        /// Required because the <see cref="XmlReader.ReadElementContentAsFloat()"/> is not robust enough because it does not move to the next element before reading,
        /// so fails if any other content (whitespace, comments, etc...) appears at the current position, just before the next element to read.
        /// </summary>
        /// <param name="reader">Extension target.</param>
        /// <param name="localName">
        /// Optional, when both this and the <paramref name="namespace"/> parameter are specified
        /// the <see cref="XmlReader.ReadStartElement(string, string)"/> method is called before reading the content.
        /// If only this parameter is specified the <see cref="XmlReader.ReadStartElement(string)"/> overload is called,
        /// otherwise the <see cref="XmlReader.ReadStartElement()"/>.</param>
        /// <param name="namespace">Optional, when specified causes the <see cref="XmlReader.ReadStartElement(string, string)"/> to be called.</param>
        /// <return>Value read from the current or specified (name/namespace) element.</return>
        public static float ReadElementSingle(this XmlReader reader, string localName = null, string @namespace = null)
        {
            // Validate
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (@namespace != null && localName == null) throw new ArgumentNullException(nameof(localName));

            // Read element start (moving past any other non-element content)
            // using the appropriate overload depending on parameters passed
            if (@namespace != null)
                reader.ReadStartElement(localName, @namespace);
            else if (localName != null)
                reader.ReadStartElement(localName);
            else
                reader.ReadStartElement();

            // Read value
            var value = reader.ReadContentAsFloat();

            // Read element end
            reader.ReadEndElement();

            // Return result
            return value;
        }

        /// <summary>
        /// Overload of the <see cref="XmlReader.ReadElementContentAsDouble()"/> which behaves similarly to the XmlReader.ReadElementString() method.
        /// Required because the <see cref="XmlReader.ReadElementContentAsDouble()"/> is not robust enough because it does not move to the next element before reading,
        /// so fails if any other content (whitespace, comments, etc...) appears at the current position, just before the next element to read.
        /// </summary>
        /// <param name="reader">Extension target.</param>
        /// <param name="localName">
        /// Optional, when both this and the <paramref name="namespace"/> parameter are specified
        /// the <see cref="XmlReader.ReadStartElement(string, string)"/> method is called before reading the content.
        /// If only this parameter is specified the <see cref="XmlReader.ReadStartElement(string)"/> overload is called,
        /// otherwise the <see cref="XmlReader.ReadStartElement()"/>.</param>
        /// <param name="namespace">Optional, when specified causes the <see cref="XmlReader.ReadStartElement(string, string)"/> to be called.</param>
        /// <return>Value read from the current or specified (name/namespace) element.</return>
        public static double ReadElementDouble(this XmlReader reader, string localName = null, string @namespace = null)
        {
            // Validate
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (@namespace != null && localName == null) throw new ArgumentNullException(nameof(localName));

            // Read element start (moving past any other non-element content)
            // using the appropriate overload depending on parameters passed
            if (@namespace != null)
                reader.ReadStartElement(localName, @namespace);
            else if (localName != null)
                reader.ReadStartElement(localName);
            else
                reader.ReadStartElement();

            // Read value
            var value = reader.ReadContentAsDouble();

            // Read element end
            reader.ReadEndElement();

            // Return result
            return value;
        }

        /// <summary>
        /// Overload of the <see cref="XmlReader.ReadElementContentAsBoolean()"/> which behaves similarly to the XmlReader.ReadElementString() method.
        /// Required because the <see cref="XmlReader.ReadElementContentAsBoolean()"/> is not robust enough because it does not move to the next element before reading,
        /// so fails if any other content (whitespace, comments, etc...) appears at the current position, just before the next element to read.
        /// </summary>
        /// <param name="reader">Extension target.</param>
        /// <param name="localName">
        /// Optional, when both this and the <paramref name="namespace"/> parameter are specified
        /// the <see cref="XmlReader.ReadStartElement(string, string)"/> method is called before reading the content.
        /// If only this parameter is specified the <see cref="XmlReader.ReadStartElement(string)"/> overload is called,
        /// otherwise the <see cref="XmlReader.ReadStartElement()"/>.</param>
        /// <param name="namespace">Optional, when specified causes the <see cref="XmlReader.ReadStartElement(string, string)"/> to be called.</param>
        /// <return>Value read from the current or specified (name/namespace) element.</return>
        public static bool ReadElementBoolean(this XmlReader reader, string localName = null, string @namespace = null)
        {
            // Validate
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (@namespace != null && localName == null) throw new ArgumentNullException(nameof(localName));

            // Read element start (moving past any other non-element content)
            // using the appropriate overload depending on parameters passed
            if (@namespace != null)
                reader.ReadStartElement(localName, @namespace);
            else if (localName != null)
                reader.ReadStartElement(localName);
            else
                reader.ReadStartElement();

            // Read value
            var value = reader.ReadContentAsBoolean();

            // Read element end
            reader.ReadEndElement();

            // Return result
            return value;
        }

        /// <summary>
        /// Overload of the <see cref="XmlReader.ReadElementContentAsObject()"/> which behaves similarly to the XmlReader.ReadElementString() method.
        /// Required because the <see cref="XmlReader.ReadElementContentAsObject()"/> is not robust enough because it does not move to the next element before reading,
        /// so fails if any other content (whitespace, comments, etc...) appears at the current position, just before the next element to read.
        /// </summary>
        /// <param name="reader">Extension target.</param>
        /// <param name="localName">
        /// Optional, when both this and the <paramref name="namespace"/> parameter are specified
        /// the <see cref="XmlReader.ReadStartElement(string, string)"/> method is called before reading the content.
        /// If only this parameter is specified the <see cref="XmlReader.ReadStartElement(string)"/> overload is called,
        /// otherwise the <see cref="XmlReader.ReadStartElement()"/>.</param>
        /// <param name="namespace">Optional, when specified causes the <see cref="XmlReader.ReadStartElement(string, string)"/> to be called.</param>
        /// <return>Value read from the current or specified (name/namespace) element.</return>
        public static T ReadElement<T>(this XmlReader reader, string localName = null, string @namespace = null)
        {
            // Validate
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (@namespace != null && localName == null) throw new ArgumentNullException(nameof(localName));

            // Read element start (moving past any other non-element content)
            // using the appropriate overload depending on parameters passed
            if (@namespace != null)
                reader.ReadStartElement(localName, @namespace);
            else if (localName != null)
                reader.ReadStartElement(localName);
            else
                reader.ReadStartElement();

            // Read value
            var value = (T)reader.ReadContentAsObject();

            // Read element end
            reader.ReadEndElement();

            // Return result
            return value;
        }

        /// <summary>
        /// Overload of the <see cref="XmlReader.ReadElementContentAsBase64(byte[], int, int)"/> which behaves similarly to the XmlReader.ReadElementString() method.
        /// Required because the <see cref="XmlReader.ReadElementContentAsBase64(byte[], int, int)"/> is not robust enough because it does not move to the next element before reading,
        /// so fails if any other content (whitespace, comments, etc...) appears at the current position, just before the next element to read.
        /// </summary>
        /// <param name="reader">Extension target.</param>
        /// <param name="buffer">Buffer into which to copy the result.</param>
        /// <param name="index">Offset in <paramref name="buffer"/> at which data should be read.</param>
        /// <param name="count">Amount of data to read (if available).</param>
        /// <param name="localName">
        /// Optional, when both this and the <paramref name="namespace"/> parameter are specified
        /// the <see cref="XmlReader.ReadStartElement(string, string)"/> method is called before reading the content.
        /// If only this parameter is specified the <see cref="XmlReader.ReadStartElement(string)"/> overload is called,
        /// otherwise the <see cref="XmlReader.ReadStartElement()"/>.</param>
        /// <param name="namespace">Optional, when specified causes the <see cref="XmlReader.ReadStartElement(string, string)"/> to be called.</param>
        /// <return>Number of bytes read (may be less then <paramref name="count"/> when less content exists than requested).</return>
        public static int ReadElementBase64(this XmlReader reader, byte[] buffer, int index, int count, string localName = null, string @namespace = null)
        {
            // Validate
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (@namespace != null && localName == null) throw new ArgumentNullException(nameof(localName));

            // Read element start (moving past any other non-element content)
            // using the appropriate overload depending on parameters passed
            if (@namespace != null)
                reader.ReadStartElement(localName, @namespace);
            else if (localName != null)
                reader.ReadStartElement(localName);
            else
                reader.ReadStartElement();

            // Read value
            var read = reader.ReadContentAsBase64(buffer, index, count);

            // Read element end
            reader.ReadEndElement();

            // Return result
            return read;
        }

        /// <summary>
        /// Overload of the <see cref="XmlReader.ReadElementContentAsBinHex(byte[], int, int)"/> which behaves similarly to the XmlReader.ReadElementString() method.
        /// Required because the <see cref="XmlReader.ReadElementContentAsBinHex(byte[], int, int)"/> is not robust enough because it does not move to the next element before reading,
        /// so fails if any other content (whitespace, comments, etc...) appears at the current position, just before the next element to read.
        /// </summary>
        /// <param name="reader">Extension target.</param>
        /// <param name="buffer">Buffer into which to copy the result.</param>
        /// <param name="index">Offset in <paramref name="buffer"/> at which data should be read.</param>
        /// <param name="count">Amount of data to read (if available).</param>
        /// <param name="localName">
        /// Optional, when both this and the <paramref name="namespace"/> parameter are specified
        /// the <see cref="XmlReader.ReadStartElement(string, string)"/> method is called before reading the content.
        /// If only this parameter is specified the <see cref="XmlReader.ReadStartElement(string)"/> overload is called,
        /// otherwise the <see cref="XmlReader.ReadStartElement()"/>.</param>
        /// <param name="namespace">Optional, when specified causes the <see cref="XmlReader.ReadStartElement(string, string)"/> to be called.</param>
        /// <return>Number of bytes read (may be less then <paramref name="count"/> when less content exists than requested).</return>
        public static int ReadElementBinHex(this XmlReader reader, byte[] buffer, int index, int count, string localName = null, string @namespace = null)
        {
            // Validate
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (@namespace != null && localName == null) throw new ArgumentNullException(nameof(localName));

            // Read element start (moving past any other non-element content)
            // using the appropriate overload depending on parameters passed
            if (@namespace != null)
                reader.ReadStartElement(localName, @namespace);
            else if (localName != null)
                reader.ReadStartElement(localName);
            else
                reader.ReadStartElement();

            // Read value
            var read = reader.ReadContentAsBinHex(buffer, index, count);

            // Read element end
            reader.ReadEndElement();

            // Return result
            return read;
        }

        /// <summary>
        /// Overload of the <see cref="XmlReader.ReadElementContentAs(Type, IXmlNamespaceResolver)"/> which behaves similarly to the XmlReader.ReadElementString() method.
        /// Required because the <see cref="XmlReader.ReadElementContentAs(Type, IXmlNamespaceResolver)"/> is not robust enough because it does not move to the next element before reading,
        /// so fails if any other content (whitespace, comments, etc...) appears at the current position, just before the next element to read.
        /// </summary>
        /// <param name="reader">Extension target.</param>
        /// <param name="namespaceResolver">See <see cref="XmlReader.ReadElementContentAs(Type, IXmlNamespaceResolver)"/>.</param>
        /// <param name="localName">
        /// Optional, when both this and the <paramref name="namespace"/> parameter are specified
        /// the <see cref="XmlReader.ReadStartElement(string, string)"/> method is called before reading the content.
        /// If only this parameter is specified the <see cref="XmlReader.ReadStartElement(string)"/> overload is called,
        /// otherwise the <see cref="XmlReader.ReadStartElement()"/>.</param>
        /// <param name="namespace">Optional, when specified causes the <see cref="XmlReader.ReadStartElement(string, string)"/> to be called.</param>
        /// <return>Value read from the current or specified (name/namespace) element.</return>
        public static T ReadElement<T>(this XmlReader reader, IXmlNamespaceResolver namespaceResolver, string localName = null, string @namespace = null)
        {
            // Validate
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (@namespace != null && localName == null) throw new ArgumentNullException(nameof(localName));

            // Read element start (moving past any other non-element content)
            // using the appropriate overload depending on parameters passed
            if (@namespace != null)
                reader.ReadStartElement(localName, @namespace);
            else if (localName != null)
                reader.ReadStartElement(localName);
            else
                reader.ReadStartElement();

            // Read value
            var value = (T)reader.ReadContentAs(typeof(T), namespaceResolver);

            // Read element end
            reader.ReadEndElement();

            // Return result
            return value;
        }

        #endregion XmlReader Extensions

        #region XmlWriter Extensions

        /// <summary>
        /// Overload of the <see cref="XmlWriter.WriteValue(object)"/> which behaves similarly to the <see cref="XmlWriter.WriteElementString(string, string)"/> method.
        /// Helps reduce the amount of code necessary to write element values other than string types.
        /// </summary>
        /// <param name="writer">Extension target.</param>
        /// <param name="localName">Element name to write.</param>
        /// <param name="value">Value to write.</param>
        public static void WriteElementValue(this XmlWriter writer, string localName, object value)
        {
            // Call overloaded method
            WriteElementValue(writer, localName, null, value);
        }

        /// <summary>
        /// Overload of the <see cref="XmlWriter.WriteValue(object)"/> which behaves similarly to the <see cref="XmlWriter.WriteElementString(string, string, string)"/> method.
        /// Helps reduce the amount of code necessary to write element values other than string types.
        /// </summary>
        /// <param name="writer">Extension target.</param>
        /// <param name="localName">Element name to write.</param>
        /// <param name="namespace">Optional element namespace to write.</param>
        /// <param name="value">Value to write.</param>
        public static void WriteElementValue(this XmlWriter writer, string localName, string @namespace, object value)
        {
            // Validate
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (localName == null) throw new ArgumentNullException(nameof(localName));

            // Write element start (moving past any other non-element content)
            // using the appropriate overload depending on parameters passed
            if (@namespace == null)
                writer.WriteStartElement(localName);
            else
                writer.WriteStartElement(localName, @namespace);

            // Write value
            writer.WriteValue(value);

            // Write element end
            writer.WriteEndElement();
        }

        #endregion XmlWriter Extensions
    }
}