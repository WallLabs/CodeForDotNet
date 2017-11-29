using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace CodeForDotNet.Xml
{
    /// <summary>
    /// Container for unqualified arbitrary XML data which is serializable
    /// as child element of a qualified type, i.e. an "xs:any" container.
    /// </summary>
    /// <remarks>
    /// The default root element is "root" when serialized standalone.
    /// Normally this is a property of another serializable type, in which case the root
    /// is set by the XML serialization attributes of the parent property or in the
    /// custom serialization code of the parent class if implemented.
    /// </remarks>
    [Serializable]
    [XmlRoot(XmlRootName, Namespace = XmlNamespace)]
    [XmlSchemaProvider("GetSchema")]
    public sealed class XmlAnyDocument : ISerializable, IXmlSerializable, IDisposable
    {
        #region Constants

        /// <summary>
        /// XML root element name.
        /// </summary>
        public const string XmlRootName = "xml";

        /// <summary>
        /// XML type name.
        /// </summary>
        public const string XmlTypeName = "xmlAnyDocumentType";

        /// <summary>
        /// XML namespace.
        /// </summary>
        public const string XmlNamespace = ""; /* unqualified/anonymous type when serialized as document */

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an empty instance.
        /// </summary>
        public XmlAnyDocument()
        {
            _xml = new XmlDocument().CreateDocumentFragment();
        }

        #region ISerializable Members

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        private XmlAnyDocument(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException("info");
            _xml = new XmlDocument().CreateDocumentFragment();
            string xml = info.GetString("Xml");
            if (!String.IsNullOrEmpty(xml))
                Add(info.GetString("Xml"), null, null);
        }

        /// <summary>
        /// Called by the binary serializer to get object data.
        /// </summary>
        /// <remarks>
        /// We need to control serialization because the <see cref="XmlDocumentFragment"/> class is not serializable.
        /// </remarks>
        [SecurityCritical]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Validate
            if (info == null) throw new ArgumentNullException("info");

            // Add properties
            info.AddValue("Xml", GetRoot().OuterXml);
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// Gets the schema and XML type of this class.
        /// </summary>
        public static XmlQualifiedName GetSchema(XmlSchemaSet schemaSet)
        {
            return new XmlQualifiedName(XmlTypeName, XmlNamespace);
        }

        /// <summary>
        /// Returns the XML schema for this type.
        /// </summary>
        public XmlSchema GetSchema()
        {
            return new XmlSchema();
        }

        /// <summary>
        /// Reads properties of this object from XML during de-serialization.
        /// </summary>
        public void ReadXml(XmlReader reader)
        {
            // Validate
            if (reader == null) throw new ArgumentNullException("reader");

            // Remove any existing content
            Clear();

            // Skip any white space (possible when serialized stand-alone)
            if (reader.NodeType != XmlNodeType.Element)
                reader.MoveToContent();

            // Read root element
            reader.ReadStartElement();

            // Add inner XML
            Add(new XPathDocument(reader).CreateNavigator(), null, null);

            // Read root end
            reader.ReadEndElement();
        }

        /// <summary>
        /// Writes properties of this object to XML during serialization.
        /// </summary>
        public void WriteXml(XmlWriter writer)
        {
            GetRoot().WriteSubtree(writer);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Calls <see cref="Dispose(bool)"/> during finalization to free resources in case it was forgotten.
        /// </summary>
        ~XmlAnyDocument()
        {
            // Partial dispose
            Dispose(false);
        }

        /// <summary>
        /// Proactively frees resources.
        /// </summary>
        public void Dispose()
        {
            // Full dispose
            Dispose(true);

            // Suppress finalization as no longer needed
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Frees resources owned by this object.
        /// </summary>
        /// <param name="disposing">
        /// True when called proactively by <see cref="Dispose()"/>.
        /// False when called during finalization.
        /// </param>
        void Dispose(bool disposing)
        {
            try
            {
                // Dispose managed resources during dispose
                if (disposing)
                    Clear();
            }
            finally
            {
                // Release references
                _xml = null;
            }
        }

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(XmlAnyDocument data1, XmlAnyDocument data2)
        {
            return !ReferenceEquals(data1, null)
                ? data1.Equals(data2) : ReferenceEquals(data2, null);
        }

        /// <summary>
        /// Tests two objects of this type for inequality by value.
        /// </summary>
        public static bool operator !=(XmlAnyDocument data1, XmlAnyDocument data2)
        {
            if (!ReferenceEquals(data1, null))
                return !data1.Equals(data2);
            return !ReferenceEquals(data2, null);
        }

        /// <summary>
        /// Compares this object with another by value.
        /// </summary>
        public override bool Equals(object obj)
        {
            // Compare nullability and type
            var other = obj as XmlAnyDocument;
            if (ReferenceEquals(other, null))
                return false;

            // Compare values
            return
                Xml != null && other.Xml != null &&
                Xml.CreateNavigator().OuterXml == other.Xml.CreateNavigator().OuterXml;
        }

        /// <summary>
        /// Returns a hash-code based on the current value of this object.
        /// </summary>
        public override int GetHashCode()
        {
            return Xml.GetHashCode();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// XML data.
        /// </summary>
        public IXPathNavigable Xml { get { return _xml; } }
        XmlDocumentFragment _xml;

        /// <summary>
        /// Indicates whether the XML is currently empty.
        /// </summary>
        public bool IsEmpty { get { return _xml == null || _xml.ChildNodes.Count == 0; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Removing all content.
        /// </summary>
        public void Clear()
        {
            _xml.RemoveAll();
        }

        /// <summary>
        /// Returns an <see cref="XPathNavigator"/> positioned at the root of the data.
        /// </summary>
        public XPathNavigator GetRoot()
        {
            return Xml.CreateNavigator();
        }

        /// <summary>
        /// Returns an <see cref="XPathNavigator"/> positioned within the data.
        /// The path is not created.
        /// </summary>
        /// <remarks>
        /// Even if this instance was serialized standalone the default "root" element is not part of the path because it is stripped during de-serialization.
        /// </remarks>
        /// <param name="path">Optional XPath expression to select the sub-path. Set null to select the root element.</param>
        /// <returns><see cref="XPathNavigator"/> positioned at the path or null when not found.</returns>
        public XPathNavigator GetPath(string path)
        {
            // Call overloaded method
            return GetPath(path, false);
        }

        /// <summary>
        /// Returns an <see cref="XPathNavigator"/> positioned within the data.
        /// Parent elements in the path without filters will be created if the <paramref name="create"/> option is set.
        /// (up to the first filter).
        /// </summary>
        /// <remarks>
        /// Even if this instance was serialized standalone the default "root" element is not part of the path because it is stripped during de-serialization.
        /// </remarks>
        /// <param name="path">Optional XPath expression to select the sub-path. Set null to select the root element.</param>
        /// <param name="create">Create the path if it doesn't exist.</param>
        /// <returns><see cref="XPathNavigator"/> positioned at the path or null when not found and <paramref name="create"/> was not set true.</returns>
        public XPathNavigator GetPath(string path, bool create)
        {
            // Return document root when no path
            if (String.IsNullOrEmpty(path))
                return Xml.CreateNavigator();

            // Attempt to get path
            var result = Xml.CreateNavigator().SelectSingleNode(path);
            if (create && result == null)
            {
                // Create path when not found and requested
                result = Xml.CreateNavigator();
                foreach (var pathPart in path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    // Check if next part exists
                    if (result != null)
                    {
                        var child = result.SelectSingleNode(pathPart);
                        if (child == null)
                        {
                            // Return null when path includes filter (cannot automatically create conditional content)
                            if (pathPart.IndexOf('[') >= 0)
                                return null;

                            // Create path parts which do not exist
                            result.AppendChildElement("", pathPart, "", null);
                            child = result.SelectSingleNode(pathPart);
                        }

                        // Next/last part
                        result = child;
                    }
                }
            }

            // Return result
            return result;
        }

        /// <summary>
        /// Adds XML to the specified data path.
        /// </summary>
        /// <param name="sourceXml">XML to add at the path.</param>
        /// <param name="sourcePath">Source XPath, or null for root.</param>
        /// <param name="targetPath">Target XPath, or null for root.</param>
        public void Add(string sourceXml, string sourcePath, string targetPath)
        {
            // Call overloaded method
            Add(sourceXml, sourcePath, targetPath, false);
        }

        /// <summary>
        /// Adds XML to the specified data path.
        /// </summary>
        /// <param name="sourceXml">XML to add at the path.</param>
        /// <param name="sourcePath">Source XPath, or null for root.</param>
        /// <param name="targetPath">Target XPath, or null for root.</param>
        /// <param name="overwrite">Set true to delete data first.</param>
        public void Add(string sourceXml, string sourcePath, string targetPath, bool overwrite)
        {
            // Validate
            if (String.IsNullOrEmpty(sourceXml))
                throw new ArgumentNullException("sourceXml");

            // Call overloaded method
            Add(sourceXml.CreateXPathDocument(), sourcePath, targetPath, overwrite);
        }

        /// <summary>
        /// Adds XML to the specified data path.
        /// </summary>
        /// <param name="sourceXml">XML to add at the path.</param>
        /// <param name="sourcePath">Source XPath, or null for root.</param>
        /// <param name="targetPath">Target XPath, or null for root.</param>
        public void Add(IXPathNavigable sourceXml, string sourcePath, string targetPath)
        {
            // Call overloaded method
            Add(sourceXml, sourcePath, targetPath, false);
        }

        /// <summary>
        /// Adds XML to the specified data path.
        /// </summary>
        /// <param name="sourceXml">XML to add at the path.</param>
        /// <param name="sourcePath">Source XPath, or null for root.</param>
        /// <param name="targetPath">Target XPath, or null for root.</param>
        /// <param name="overwrite">Set true to delete data first.</param>
        public void Add(IXPathNavigable sourceXml, string sourcePath, string targetPath, bool overwrite)
        {
            // Validate
            if (sourceXml == null) throw new ArgumentNullException("sourceXml");

            // Use root elements as source when null or root
            if (String.IsNullOrEmpty(sourcePath) || sourcePath == "/") sourcePath = "/*";

            // Read source XML and make anonymous
            var buffer = new StringBuilder();
            var readSettings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };
            var writeSettings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using (var reader = XmlReader.Create(sourceXml.CreateNavigator().ReadSubtree(), readSettings))
            using (var writer = XmlWriter.Create(buffer, writeSettings))
                XmlFullExtensions.FormatXmlAnyTransform.Transform(reader, writer);

            // Find or create target path
            var target = GetPath(targetPath, true);

            // Append or replace source XML depending on overwrite option
            using (var reader = XmlReader.Create(new StringReader(buffer.ToString()), readSettings))
            {
                var sourceNodes = new XPathDocument(reader).CreateNavigator().Select(sourcePath);
                while (sourceNodes.MoveNext())
                {
                    Debug.Assert(sourceNodes.Current != null);
                    if (overwrite)
                        target.ReplaceSelf(sourceNodes.Current);
                    else
                        target.AppendChild(sourceNodes.Current);
                }
            }
        }

        /// <summary>
        /// Deletes XML from the specified path.
        /// </summary>
        /// <param name="path">XPath target path, null for root.</param>
        public void Delete(string path)
        {
            // Find or create target path
            var target = GetPath(path, false);
            if (target != null)
            {
                // Delete
                target.DeleteSelf();
            }
        }

        #endregion
    }
}
