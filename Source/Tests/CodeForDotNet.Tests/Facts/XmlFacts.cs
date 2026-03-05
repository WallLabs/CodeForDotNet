using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.Xsl;
using CodeForDotNet.Testing;
using CodeForDotNet.Tests.Mocks;
using CodeForDotNet.Tests.Mocks.Data;
using CodeForDotNet.Tests.Properties;
using CodeForDotNet.Xml;
using CodeForDotNet.Xml.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeForDotNet.Tests.Facts;

/// <summary>
/// Unit tests for the various XML extensions.
/// </summary>
[TestClass]
public class XmlFacts
{
    /// <summary>
    /// Tests the <see cref="XmlAnyDocument"/>.
    /// </summary>
    [TestMethod]
    [Description("XML XmlAnyDocument")]
    [Obsolete("Binary serialization is obsolete and will be removed in a future update.")]
    public void XmlAnyDocumentTest()
    {
        // Create a test object
        var xml = new XmlAnyDocument();

        // Add new data to the root
        xml.Add("<A><B><C>Test1</C></B></A>", null, null);
        Assert.AreEqual("<A><B><C>Test1</C></B></A>", xml.ToString());

        // Add data to child nodes with source and target path
        xml.Add("<Test><B>Test2</B></Test><Test />", "/Test/*", "/A");
        xml.Add("<B>Test3.1</B><B>Test3.2</B>", "B", "A");

        // Add roots
        xml.Add("<A>Test4.1</A><A>Test4.2</A>", null, null);
        xml.Add("<A>Test5.1</A><A>Test5.2</A>", null, "/New1/New2");

        // Test constructors
        const string constructedXml = "<From><A><string /></A></From>";
        var anyFromString = new XmlAnyDocument(constructedXml);
        Assert.AreEqual(constructedXml, anyFromString.ToString());
        using (var streamReader = new MemoryStream(Encoding.UTF8.GetBytes(constructedXml)))
        {
            var anyFromStream = new XmlAnyDocument(streamReader);
            Assert.AreEqual(constructedXml, anyFromStream.ToString());
        }
        using (var textReader = new StringReader(constructedXml))
        {
            var anyFromTextStream = new XmlAnyDocument(textReader);
            Assert.AreEqual(constructedXml, anyFromTextStream.ToString());
        }
        using (var xmlReader = constructedXml.CreateXmlReader())
        {
            var anyFromXmlReader = new XmlAnyDocument(xmlReader);
            Assert.AreEqual(constructedXml, anyFromXmlReader.ToString());
        }

        // Test serialization
        var xmlSerialized = xml.SerializeXml();
        var xmlDeserialized = SerializationExtensions.DeserializeXml<XmlAnyDocument>(xmlSerialized);
        Assert.AreEqual(xml, xmlDeserialized);

        // Test queries
        var result = xml.GetPath("/A/B/C");
        Assert.IsNotNull(result);
        Assert.AreEqual("Test1", result!.Value);
        result = xml.GetPath("//B[text() = 'Test3.2']");
        Assert.IsNotNull(result);
        Assert.AreEqual("Test3.2", result!.Value);
        result = xml.GetPath("A[3]");
        Assert.IsNotNull(result);
        Assert.AreEqual("Test4.2", result!.Value);
        result = xml.GetPath("New1/New2/A[2]");
        Assert.IsNotNull(result);
        Assert.AreEqual("Test5.2", result!.Value);
        result = xml.GetPath("Does/Not/Exist");
        Assert.IsNull(result);
        result = xml.GetPath("/A");    // Multiple results in single node
        Assert.IsNotNull(result);

        // Replace nodes
        xml.Replace("<B>Test3.1</B><B>Test3.2</B>", "B", "A");

        // Replace root
        xml.Replace("<A>Test4.1 Replaced</A>", null, "/A[2]");

        // Test creation of path with existing filter at parent
        Assert.IsNotNull(xml.GetPath("/New1/New2/A[2]/Created", true));

        // Delete root
        xml.Delete("/A[3]");

        // Delete all content and test empty property
        Assert.IsFalse(xml.IsEmpty);
        xml.Delete(null);
        Assert.IsTrue(xml.IsEmpty);
        Assert.AreEqual("", xml.ToString());

        // Replace empty
        xml.Clear();
        xml.Replace("<Test />", null, null);
        Assert.IsFalse(xml.IsEmpty);
        Assert.AreEqual("<Test />", xml.ToString());

        // Replace invalid
        xml.Clear();
        xml.Replace("Test non-XML at root.", null, null);
        Assert.IsFalse(xml.IsEmpty);
        Assert.AreEqual("Test non-XML at root.", xml.ToString());
    }

    /// <summary>
    /// Tests the <see cref="XmlCollection{TItem}"/>.
    /// </summary>
    [TestMethod]
    [Description("XML Collection")]
    public void XmlCollectionTest()
    {
        // Test serialization with typed collection (validates in custom namespace)
        var testTyped = new TestXmlCollection();
        for (var index = 1; index <= 10; index++)
        {
            var data = CreateTestDataSmall(index);
            testTyped.Add(data);
        }
        _ = SerializationAssert.XmlSerializeTest(testTyped,
            CodeTestsMocksXsd.GetSchema(), assertCleanNamespaces: true);

        // Test serialization with inherited collection (serializer emits XSI)
        var testInherited = new TestXmlCollectionInherited();
        for (var index = 1; index <= 10; index++)
        {
            var data = CreateTestDataInherited(index);
            testInherited.Add(data);
        }
        _ = SerializationAssert.XmlSerializeTest(testInherited,
            CodeTestsMocksXsd.GetSchema(), assertCleanNamespaces: true);

        // Test serialization with typed collection and namespace table declarations (validates
        // in custom namespace and preserves prefixes)
        var testTypedNamespace = new TestXmlCollectionWithNamespaces();
        for (var index = 1; index <= 10; index++)
        {
            var data = CreateTestDataNamespace(index);
            testTypedNamespace.Add(data);
        }
        _ = SerializationAssert.XmlSerializeTest(testTypedNamespace,
            CodeTestsMocksXsd.GetSchema(), assertCleanNamespaces: true);
    }

    /// <summary>
    /// Tests the XML serializable types in a more complex parent data structure.
    /// </summary>
    /// <remarks>
    /// Originally implemented to fix a bug where start/end elements were not being correctly
    /// read in sequence. Remains for regression test and to ensure the types behave correctly
    /// when contained within other types.
    /// </remarks>
    [TestMethod]
    [Description("XML Data Serialization")]
    public void XmlDataSerializationTest()
    {
        // Create test object
        var test = new TestXmlData {
            XmlDateTime = new XmlDateTime(DateTimeOffset.Now),
            AfterTheXmlCollection = "After the collection",
            AfterTheTypedXmlCollection = "After the typed collection",
            AfterTheInheritedXmlCollection = "After the inherited collection",
            AfterTheTypedXmlSerializableDictionary = "After the typed dictionary",
            AfterTheInheritedXmlSerializableDictionary = "After the inherited dictionary"
        };
        for (var index = 1; index <= 10; index++)
        {
            var data = CreateTestDataSmall(index);
            var dataInherited = CreateTestDataInherited(index);
            test.StandardCollection.Add(data);
            test.ATypedXmlCollection.Add(data);
            test.AnInheritedXmlCollection.Add(dataInherited);
            test.ATypedXmlSerializableDictionary.Add(data.Id, data);
            test.AnInheritedXmlSerializableDictionary.Add(dataInherited.Id, dataInherited);
        }

        // Serialize and compare
        _ = SerializationAssert.XmlSerializeTest(test, schema: CodeTestsMocksXsd.GetSchema());
    }

    /// <summary>
    /// Tests the <see cref="XmlExtensions.EscapeInvalidXmlChars"/> method.
    /// </summary>
    [TestMethod]
    [Description("XML Escape Invalid Characters")]
    public void XmlEscapeInvalidXmlCharsTest()
    {
        const string testString = "\vTest\vString\v";
        Assert.AreEqual("&#000BTest&#000BString&#000B", testString.EscapeInvalidXmlChars());
    }

    /// <summary>
    /// Tests the
    /// <see cref="XmlExtensions.FormatXml(string,Encoding,XmlFormatOptions)"/> method.
    /// </summary>
    [TestMethod]
    [Description("XML Format")]
    public void XmlFormatTest()
    {
        // Load test data
        var xmlDocument = File.ReadAllText(Settings.Default.TestFormatXmlInputFilePath);

        // Format XML with different options, checking expected output
        var formattedXml = xmlDocument.FormatXml(Encoding.UTF8, XmlFormatOptions.Indent);
        Assert.AreEqual(File.ReadAllText(Settings.Default.TestFormatXmlOutputIndentFilePath).Trim(), formattedXml);
        formattedXml = xmlDocument.FormatXml(Encoding.UTF8, XmlFormatOptions.Indent | XmlFormatOptions.OmitComments);
        Assert.AreEqual(File.ReadAllText(Settings.Default.TestFormatXmlOutputIndentOmitCommentsFilePath).Trim(), formattedXml);
        formattedXml = xmlDocument.FormatXml(Encoding.UTF8);
        Assert.AreEqual(File.ReadAllText(Settings.Default.TestFormatXmlOutputIndentTrimFilePath).Trim(), formattedXml);
        formattedXml = xmlDocument.FormatXml(Encoding.UTF8,
            XmlFormatOptions.Indent | XmlFormatOptions.Trim | XmlFormatOptions.OmitComments);
        Assert.AreEqual(File.ReadAllText(Settings.Default.TestFormatXmlOutputIndentOmitCommentsTrimFilePath).Trim(), formattedXml);
        formattedXml = xmlDocument.FormatXml(Encoding.Unicode, XmlFormatOptions.Trim);
        Assert.AreEqual(File.ReadAllText(Settings.Default.TestFormatXmlOutputTrimUtf16FilePath).Trim(), formattedXml);

        // Load test data with multiple roots (XML fragment)
        var xmlFragment = File.ReadAllText(Settings.Default.TestFormatXmlInputFragmentFilePath).Trim();
        formattedXml = xmlFragment.FormatXml(Encoding.Unicode, XmlFormatOptions.Trim);
        Assert.AreEqual(File.ReadAllText(Settings.Default.TestFormatXmlOutputFragmentFilePath).Trim(), formattedXml);
    }

    /// <summary>
    /// Tests the <see cref="XmlDictionary{TKey, TValue}"/>.
    /// </summary>
    [TestMethod]
    [Description("XML Dictionary")]
    public void XmlSerializableDictionaryTest()
    {
        // Test serialization with typed dictionary (validates in custom namespace)
        var testTyped = new TestXmlDictionary();
        for (var index = 1; index <= 10; index++)
        {
            var data = CreateTestDataSmall(index);
            testTyped.Add(data.Id, data);
        }
        _ = SerializationAssert.XmlSerializeTest(testTyped,
            CodeTestsMocksXsd.GetSchema(), assertCleanNamespaces: true);

        // Test serialization with inherited dictionary (serializer emits XSI)
        var testInherited = new TestXmlDictionaryInherited();
        for (var index = 1; index <= 10; index++)
        {
            var data = CreateTestDataInherited(index);
            testInherited.Add(data.Id, data);
        }
        _ = SerializationAssert.XmlSerializeTest(testInherited,
            CodeTestsMocksXsd.GetSchema(), assertCleanNamespaces: true);
    }

    /// <summary>
    /// Tests the <see cref="XmlSerializerCache"/>.
    /// </summary>
    [TestMethod]
    [Description("XML Serializer Cache")]
    public void XmlSerializerCacheTest()
    {
        // Constants
        const int iterations = 100;

        // Create a test object
        var test = CreateTestData();

        // Test with extra XML types, where the caching benefit really kicks-in!
        var extraTypes = new[] { typeof(TestData), typeof(XmlDateTime) };

        // Serialize test method
        var fileName = typeof(TestData).Name + " Serialize Cache Test.xml";
        void Serialize(XmlSerializer serializer, TestData source)
        {
            // Serialize
            using (var writer = XmlWriter.Create(fileName))
                serializer.Serialize(writer, source);

            // De-serialize
            var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit }; // CA3075 security requirement.
            using var reader = XmlReader.Create(fileName, settings);
            _ = serializer.Deserialize(reader);
        }

        // Serialize without cache
        var serializeTime = Stopwatch.StartNew();
        for (var count = 0; count < iterations; count++)
        {
            var serializer = new XmlSerializer(typeof(TestData), extraTypes);
            Serialize(serializer, test);
        }
        serializeTime.Stop();

        // Serialize with cache
        var cacheTime = Stopwatch.StartNew();
        XmlSerializerCache.RegisterTypes(extraTypes);
        for (var count = 0; count < iterations; count++)
        {
            var serializer = XmlSerializerCache.Create(typeof(TestData));
            Serialize(serializer, test);
        }
        cacheTime.Stop();

        // Should be quicker
        Assert.IsLessThan(serializeTime.Elapsed, cacheTime.Elapsed);
    }

    /// <summary>
    /// Tests the XML serializer extensions with inheritance.
    /// </summary>
    [TestMethod]
    [Description("XML Serializer Inherited")]
    public void XmlSerializerInheritedTest()
    {
        // Prepare cache
        XmlSerializerCache.RegisterTypes([typeof(TestXmlDataInherited), typeof(TestXmlDataAbstractBase), typeof(TestXmlDataAbstractInherited)]);

        // Test inheritance
        var inherited = new TestXmlDataInherited {
            Id = Guid.NewGuid(),
            Name = "Inherited",
            Description = "Description"
        };
        _ = SerializationAssert.XmlSerializeTest(inherited, CodeTestsMocksXsd.GetSchema());

        // Test abstract base
        var abstractBased = new TestXmlDataAbstractInherited {
            BaseProperty = "Base",
            Description = "Description"
        };
        _ = SerializationAssert.XmlSerializeTest(abstractBased, CodeTestsMocksXsd.GetSchema());
    }

    /// <summary>
    /// Tests new line handling of the
    /// <see cref="SerializationExtensions.SerializeXml(object, bool)"/> method.
    /// </summary>
    [TestMethod]
    [Description("XML Serializer New Lines")]
    public void XmlSerializerNewLinesTest()
    {
        // Test serialization with Windows new lines
        var windowsNewLines = "Line 1\r\nLine 2\r\nLine 3\r\n";
        var windowsNewLinesSerialized = windowsNewLines.SerializeXml();
        var windowsNewLinesDeserialized = SerializationExtensions.DeserializeXml<string>(windowsNewLinesSerialized);
        Assert.AreEqual(windowsNewLines, windowsNewLinesDeserialized);

        // Test serialization with Linux new lines
        var linuxNewLines = "Line 1\nLine 2\nLine 3\n";
        var linuxNewLinesSerialized = linuxNewLines.SerializeXml();
        var linuxNewLinesDeserialized = SerializationExtensions.DeserializeXml<string>(linuxNewLinesSerialized);
        Assert.AreEqual(linuxNewLines, linuxNewLinesDeserialized);
    }

    /// <summary>
    /// Tests the <see cref="XmlExtensions.StripInvalidXmlChars"/> method.
    /// </summary>
    [TestMethod]
    [Description("XML Strip Invalid Characters")]
    public void XmlStripInvalidXmlCharsTest()
    {
        const string testString = "\vTest\vString\v";
        Assert.AreEqual("TestString", testString.StripInvalidXmlChars());
    }

    /// <summary>
    /// Tests the <see cref="XmlExtensions.GetReaderValidationSettings"/> method.
    /// </summary>
    [TestMethod]
    [Description("XML Validate")]
    public void XmlValidateTest()
    {
        // Create good and bad XML
        const string goodXml = "<A><B>Hello</B></A>";
        const string badXml = "<A>\nB>Goodbye</B>\n</A>";

        // Load schema
        var schemas = new XmlSchemaSet();
        XmlSchema schema;
        var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit }; // CA3075 security requirement.
        using (var file = File.OpenRead(Settings.Default.TestXmlValidationSchemaFilePath))
        using (var xmlReader = XmlReader.Create(file, settings))
            schema = XmlSchema.Read(xmlReader, null)!;
        _ = schemas.Add(schema);

        // Validate good XML (successful)
        var xml = new XmlDocument { XmlResolver = null };
        settings = XmlExtensions.GetReaderValidationSettings(schemas);
        settings.DtdProcessing = DtdProcessing.Prohibit; // CA3075 security requirement.
        using (var stringReader = new StringReader(goodXml))
        using (var xmlReader = XmlReader.Create(stringReader))
        using (var validatingReader = XmlReader.Create(xmlReader, settings))
            xml.Load(validatingReader);

        // Validate bad XML (fails)
        using (var stringReader = new StringReader(badXml))
        using (var xmlReader = XmlReader.Create(stringReader))
        using (var validatingReader = XmlReader.Create(xmlReader, settings))
        {
            var formatError = Assert.Throws<FormatException>(() => xml.Load(validatingReader));
            Assert.AreEqual("XML validation error at line 1 character 4. The element 'A' cannot contain text. List of possible elements expected: 'B'.",
                formatError.Message);
        }
    }

    /// <summary>
    /// Tests the <see cref="XPathFunctions"/> directly, to ensure they match the
    /// behavior and test case examples defined in the W3C specification, regardless
    /// of any external influences, e.g. XSLT transform engine.
    /// </summary>
    [TestMethod]
    [Description("XPath Functions Extension")]
    public void XPathFunctionsTestApi()
    {
        // Create extension.
        var extension = new XPathFunctions();

        // Ensure trimming has not been added to workaround incorrect usage.
        Assert.IsFalse(extension.matches("   Test  ", "^Test$"),
            message: "W3C standard violated by trimming input!");

        // Execute W3C documented test cases.
        // See examples at: https://www.w3.org/TR/xpath-functions-31/#func-matches
        // The expression fn: matches("abracadabra", "bra") returns true().
        Assert.IsTrue(extension.matches("abracadabra", "bra"));
        // The expression fn: matches("abracadabra", "^a.*a$") returns true().
        Assert.IsTrue(extension.matches("abracadabra", "^a.*a$"));
        // The expression fn: matches("abracadabra", "^bra") returns false().
        Assert.IsFalse(extension.matches("abracadabra", "^bra"));
        // Given the source document:
        var source = "<poem author=\"Wilhelm Busch\">\nKaum hat dies der Hahn gesehen,\nFängt er auch schon an zu krähen:\nKikeriki! Kikikerikih!!\nTak, tak, tak! - da kommen sie.\n</poem>";
        // the following function calls produce the following results, with the poem element as the context node:
        // The expression fn: matches($poem, "Kaum.*krähen") returns false().
        Assert.IsFalse(extension.matches(source, "Kaum.*krähen"));
        // The expression fn: matches($poem, "Kaum.*krähen", "s") returns true().
        Assert.IsTrue(extension.matches(source, "Kaum.*krähen", "s"));
        // The expression fn: matches($poem, "^Kaum.*gesehen,$", "m") returns true().
        // Note, this will work with the official test data above as it only uses Unix line endings "\n",
        // but it won't work with Windows text file data which includes the carriage-return "\r" before each new line.
        // The .NET Framework does not ignore them, in order to follow the specification.
        // When reading Windows text file data, they must either be removed by the program, at the source,
        // or the expression "\r?$" added in the expression to support both formats.
        Assert.IsTrue(extension.matches(source, "^Kaum.*gesehen,$", "m"));
        // The expression fn: matches($poem, "^Kaum.*gesehen,$") returns false().
        Assert.IsFalse(extension.matches(source, "^Kaum.*gesehen,$"));
        // The expression fn: matches($poem, "kiki", "i") returns true().
        Assert.IsTrue(extension.matches(source, "kiki", "i"));
    }

    /// <summary>
    /// Tests the <see cref="XPathFunctions"/> used when executing an XSLT.
    /// </summary>
    [TestMethod]
    [Description("XPath Functions Extension")]
    public void XPathFunctionsTestTransform()
    {
        // Create extension and validate basic functionality directly (without XSL).
        var extension = new XPathFunctions();
        Assert.IsFalse(extension.matches("   Test  ", "^Test$"),
            message: "W3C standard violated by trimming input!");

        // Create and register extension functions.
        var arguments = new XsltArgumentList();
        arguments.AddExtensionObject(XPathFunctions.XmlNamespace, extension);

        // Create XML reader settings with secure defaults.
        var readerSettings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit };

        // Load style-sheet.
        var transform = new XslCompiledTransform(true /* Enable debugging */);
        using (var stylesheetReader = XmlReader.Create(Settings.Default.TestXPathFunctionsExecuteFilePath, readerSettings))
            transform.Load(stylesheetReader);

        // Read test file and normalize line endings (in case checked-out on Windows machine).
        var inputXml = File.ReadAllText(Settings.Default.TestXPathFunctionsInputFilePath).Replace("\r\n", "\n");

        // Transform XML.
        var resultsFileName = Settings.Default.TestXPathFunctionsOutputFilePath;
        using (var stringReader = new StringReader(inputXml))
        using (var inputReader = XmlReader.Create(stringReader, readerSettings))
        using (var resultsWriter = XmlWriter.Create(resultsFileName))
            transform.Transform(inputReader, arguments, resultsWriter);

        // Load and validate results.
        var resultsText = File.ReadAllText(resultsFileName);
        var resultsXml = new XmlAnyDocument(resultsText);
        var failures = resultsXml.GetRoot()
                .Select("//node()[boolean(@output) and boolean(@expected) and not(@output = @expected)]");
        Assert.IsEmpty(failures);
    }

    /// <summary>
    /// Creates a <see cref="TestData"/>.
    /// </summary>
    private static TestData CreateTestData()
    {
        return new TestData {
            Id = Guid.NewGuid(),
            Guid = Guid.NewGuid(),
            GuidRequired = Guid.NewGuid(),
            Boolean = true,
            BooleanRequired = true,
            ChangedByUserName = "User2",
            ChangedDate = DateTime.Now,
            CreatedByUserName = "User1",
            CreatedDate = DateTime.Now,
            LocalTime = DateTime.Now,
            LocalTimeWithOffset = DateTimeOffset.Now,
            Name = "Name",
            NumberDecimal = decimal.MaxValue,
            NumberInteger = int.MaxValue,
            NumberLong = long.MaxValue,
            SerializableLocalTimeWithOffset = new XmlDateTime(DateTimeOffset.Now),
            Text = "Text",
            TextWithMaxLength = "Text...",
            TextWithStringLength = "Text...",
            UniversalTime = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a <see cref="TestXmlDataInherited"/> with the specified number.
    /// </summary>
    private static TestXmlDataInherited CreateTestDataInherited(int number)
    {
        return new TestXmlDataInherited {
            Id = Guid.NewGuid(),
            Name = string.Format(CultureInfo.InvariantCulture, "Name {0}", number),
            Description = string.Format(CultureInfo.InvariantCulture, "Description {0}", number)
        };
    }

    /// <summary>
    /// Creates a <see cref="TestXmlDataNamespace"/> with the specified number.
    /// </summary>
    private static TestXmlDataNamespace CreateTestDataNamespace(int number)
    {
        return new TestXmlDataNamespace {
            Id = Guid.NewGuid(),
            Name = string.Format(CultureInfo.InvariantCulture, "Name {0}", number)
        };
    }

    /// <summary>
    /// Creates a <see cref="TestXmlDataSmall"/> with the specified number.
    /// </summary>
    private static TestXmlDataSmall CreateTestDataSmall(int number)
    {
        return new TestXmlDataSmall {
            Id = Guid.NewGuid(),
            Name = string.Format(CultureInfo.InvariantCulture, "Name {0}", number)
        };
    }
}
