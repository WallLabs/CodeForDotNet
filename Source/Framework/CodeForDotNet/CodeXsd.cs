using System.Threading;
using System.Xml;
using System.Xml.Schema;

namespace CodeForDotNet;

/// <summary>
/// Provides access to XML schema and definitions in this namespace.
/// </summary>
public static class CodeXsd
{
    /// <summary>
    /// Types XML namespace.
    /// </summary>
    public const string XmlNamespace = XmlRootNamespace + ":" + XsdFileName;

    /// <summary>
    /// Default XML namespace prefix (preferred, not guaranteed).
    /// </summary>
    public const string XmlNamespacePrefixDefault = "code";

    /// <summary>
    /// Root XML namespace.
    /// </summary>
    /// <remarks>
    /// URN is used because we will never publish this schema on the Internet. Child namespaces
    /// must use the ":" URN separator, not "/" as in URLs. e.g.
    /// "urn:grandparent:parent:child:type".
    /// </remarks>
    public const string XmlRootNamespace = "urn:Code";

    /// <summary>
    /// XML XSD filename.
    /// </summary>
    public const string XmlXsdFileName = "Xml.xsd";

    /// <summary>
    /// Types XSD filename.
    /// </summary>
    public const string XsdFileName = "Code.xsd";

    /// <summary>
    /// XSLT XSD filename.
    /// </summary>
    public const string XsltXsdFileName = "Xslt.xsd";

    /// <summary>
    /// Synchronization object.
    /// </summary>
    private static readonly Lock SyncRoot = new();

    /// <summary>
    /// Schema cache for performance.
    /// </summary>
    private static XmlSchemaSet? _schemas;

    /// <summary>
    /// Gets the <see cref="XmlSchemaSet"/> necessary to validate types in this assembly.
    /// Includes schemata from imported types.
    /// </summary>
    /// <returns>XML schema for this assembly.</returns>
    public static XmlSchemaSet GetSchema()
    {
        // Load schemata first time (when not cached).
        // Use double-check-locking pattern for performance.
        if (_schemas is null)
        {
            lock (SyncRoot)
            {
                if (_schemas is null)
                {
                    // Load schemata.
                    var schemas = new XmlSchemaSet();
                    _ = schemas.Add(Read());

                    // Prevent threading issue through optimization on multi-processor systems.
                    Thread.MemoryBarrier();

                    // Store singleton safely.
                    _schemas = schemas;
                }
            }
        }

        // Return copy of cached value (to protect against modification).
        var copy = new XmlSchemaSet();
        copy.Add(_schemas);
        return copy;
    }

    /// <summary>
    /// Gets the <see cref="XmlSchema"/> which defines W3C XML types.
    /// </summary>
    /// <returns>XML schema for this assembly.</returns>
    public static XmlSchema GetXmlSchema()
    {
        // Load schema from resource
        var type = typeof(CodeXsd);
        var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit }; // CA3075 security requirement.
        using var resource = type.Assembly.GetManifestResourceStream(type.Namespace + "." + XmlXsdFileName)!;
        using var xmlReader = XmlReader.Create(resource, settings);
        return XmlSchema.Read(xmlReader, null)!;
    }

    /// <summary>
    /// Gets the <see cref="XmlSchema"/> which defines W3C XSLT documents. This may not be
    /// sufficient for validation because it does not include any imported types.
    /// </summary>
    /// <returns>Standard XML schema for XSLT documents as defined by the W3C.</returns>
    public static XmlSchema GetXsltSchema()
    {
        // Load schema from resource
        var type = typeof(CodeXsd);
        var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit }; // CA3075 security requirement.
        using var resource = type.Assembly.GetManifestResourceStream(type.Namespace + "." + XsltXsdFileName)!;
        using var xmlReader = XmlReader.Create(resource, settings);
        return XmlSchema.Read(xmlReader, null)!;
    }

    /// <summary>
    /// Reads the XSD file which defines types in this assembly. This may not be sufficient for
    /// validation because it does not include any imported types. Call
    /// <see cref="GetSchema()"/> to get the full schema.
    /// </summary>
    /// <returns>XML schema for this assembly only excluding any dependencies.</returns>
    private static XmlSchema Read()
    {
        // Load schema from resource
        var type = typeof(CodeXsd);
        var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit }; // CA3075 security requirement.
        using var resource = type.Assembly.GetManifestResourceStream(type.Namespace + "." + XsdFileName)!;
        using var xmlReader = XmlReader.Create(resource, settings);
        return XmlSchema.Read(xmlReader, null)!;
    }
}
