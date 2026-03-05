using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using CodeForDotNet.Xml;

namespace CodeForDotNet.Testing;

/// <summary>
/// Test helper methods for serialization functionality.
/// </summary>
public static class SerializationAssert
{
    /// <summary>
    /// Tests XML serialization, de-serialization and schema validation using the
    /// <see cref="DataContractSerializer"/>. Optionally tests comparison.
    /// </summary>
    /// <typeparam name="T">Type of the entity being tested.</typeparam>
    /// <param name="entity">Entity to test.</param>
    /// <param name="schema">Optional non-default schema to use. Leave null to not validate schema, e.g. for anonymous types.</param>
    /// <param name="name">
    /// Test entity name, defaults to type name when null. Used for file names generated during test.
    /// </param>
    /// <param name="compare">
    /// Set true to compare the serialized object after de-serialization. Best way to confirm
    /// data was written and read correctly, but requires equality operator implementation and
    /// that all data will round-trip (usual, but sometimes not implemented by design). Defaults
    /// to true.
    /// </param>
    /// <returns>New object after de-serialization.</returns>
    public static T? DataContractSerializeTest<T>(T entity, XmlSchemaSet? schema,
        string? name = null, bool compare = true)
    {
        // Validate
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        // Set defaults when required
        if (string.IsNullOrWhiteSpace(name))
            name = typeof(T).Name;

        // Serialize to XML using DataContractSerializer
        var fileName = name + " Data Contract Serialize.xml";
        using (var writer = XmlWriter.Create(fileName,
            new XmlWriterSettings { NewLineHandling = NewLineHandling.Entitize }))
            entity.SerializeXmlData(writer);

        // Load and validate the serialized XML using DataContractSerializer
        T? entity2;
        var settings = schema != null
            ? XmlExtensions.GetReaderValidationSettings(schema)
            : new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit /* CA3075 security requirement. */ };
        using (var reader = XmlReader.Create(fileName, settings))
            entity2 = SerializationExtensions.DeserializeXmlData<T>(reader);

        // Compare when requested
        if (compare)
            OperatorAssert.AssertCompare(entity, entity2);

        // Return de-serialized copy
        return entity2;
    }

    /// <summary>
    /// Tests XML serialization, de-serialization and schema validation using the
    /// <see cref="XmlSerializer"/>. Optionally tests comparison and that XML namespaces are clean/compact.
    /// </summary>
    /// <typeparam name="T">Type of the entity being tested.</typeparam>
    /// <param name="entity">Entity to test.</param>
    /// <param name="schema">Optional non-default schema to use. Leave null to not validate schema, e.g. for anonymous types.</param>
    /// <param name="name">
    /// Test entity name, defaults to type name when null. Used for file names generated during test.
    /// </param>
    /// <param name="compare">
    /// Set true to compare the serialized object after de-serialization. Best way to confirm
    /// data was written and read correctly, but requires equality operator implementation and
    /// that all data will round-trip (usual, but sometimes not implemented by design). Defaults
    /// to true.
    /// </param>
    /// <param name="assertCleanNamespaces">
    /// Asserts that all namespace declarations are clean, producing the most compact XML, i.e.
    /// not declaring the namespace more than once, by emitting it at a lower level when it
    /// is/could be defined by a parent. Ensures the XML namespaces are being handled properly
    /// throughout types and serialization code/attributes.
    /// </param>
    /// <returns>De-serialized object.</returns>
    public static T? XmlSerializeTest<T>(T entity, XmlSchemaSet? schema, string? name = null,
        bool compare = true, bool assertCleanNamespaces = true)
    {
        // Validate
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        // Set defaults when required
        if (string.IsNullOrWhiteSpace(name))
            name = typeof(T).Name;

        // Serialize to XML using XmlSerializer
        var fileName = name + " XML Serialize.xml";
        using (var writer = XmlWriter.Create(fileName,
            new XmlWriterSettings { NewLineHandling = NewLineHandling.Entitize }))
            entity.SerializeXml(writer);

        // Load and validate the serialized XML using XmlSerializer
        T? entity2;
        var settings = schema != null
            ? XmlExtensions.GetReaderValidationSettings(schema)
            : new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit /* CA3075 security requirement. */ };
        using (var reader = XmlReader.Create(fileName, settings))
            entity2 = SerializationExtensions.DeserializeXml<T>(reader);

        // Compare when requested
        if (compare)
            OperatorAssert.AssertCompare(entity, entity2);

        // Check namespace declarations are not duplicated when requested
        if (assertCleanNamespaces)
        {
            var xml = File.ReadAllText(fileName);
            XmlAssert.AssertCleanNamespaces(xml);
        }

        // Return result
        return entity2;
    }
}
