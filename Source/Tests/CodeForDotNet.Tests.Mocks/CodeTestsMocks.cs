using System.Threading;
using System.Xml;
using System.Xml.Schema;

namespace CodeForDotNet.Tests.Mocks;

/// <summary>
/// Provides access to XML schema and definitions in this namespace.
/// </summary>
public static class CodeTestsMocksXsd
{
    /// <summary>
    /// Types XML namespace.
    /// </summary>
    public const string XmlNamespace = CodeXsd.XmlRootNamespace + ":" + XsdFileName;

    /// <summary>
    /// Default XML namespace prefix (preferred, not guaranteed).
    /// </summary>
    public const string XmlNamespacePrefixDefault = "test";

    /// <summary>
    /// Types XSD filename.
    /// </summary>
    public const string XsdFileName = "CodeTestsMocks.xsd";

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
    /// <remarks>
    /// The double-check locking pattern is used for performance,
    /// <see href="http://en.wikipedia.org/wiki/Double-checked_locking#Usage_in_Microsoft_.NET_.28Visual_Basic.2C_C.23.29"/>.
    /// </remarks>
    public static XmlSchemaSet GetSchema()
    {
        // Load schemata first time (when not cached)
        if (_schemas is null)
        {
            lock (SyncRoot)
            {
                if (_schemas is null)
                {
                    // Load schemata
                    var schemas = CodeXsd.GetSchema();
                    _ = schemas.Add(Read());

                    // Prevent threading issue through optimization on multi-processor systems
                    Thread.MemoryBarrier();

                    // Store singleton safely
                    _schemas = schemas;
                }
            }
        }

        // Return copy of cached value (to protect against modification)
        var copy = new XmlSchemaSet();
        copy.Add(_schemas);
        return copy;
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
        var type = typeof(CodeTestsMocksXsd);
        var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit }; // CA3075 security requirement.
        using var resource = type.Assembly.GetManifestResourceStream(type.Namespace + "." + XsdFileName)!;
        using var reader = XmlReader.Create(resource, settings);
        return XmlSchema.Read(reader, null)!;
    }
}
