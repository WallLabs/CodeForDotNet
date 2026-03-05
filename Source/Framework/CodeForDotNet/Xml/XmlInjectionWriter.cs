using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Schema;

namespace CodeForDotNet.Xml;

/// <summary>
/// <see cref="XmlWriter"/> wrapper which injects additional content into the stream as it is written.
/// </summary>
/// <remarks>
/// Initializes a new instance wrapping/extending the specified <see cref="XmlWriter"/>.
/// </remarks>
/// <param name="writer">XML writer to extend. It is not disposed.</param>
[SuppressMessage("Naming", "CA1725:Parameter names should match base declaration", Justification = "Readability.")]
public class XmlInjectionWriter(XmlWriter writer) : XmlWriter
{
    /// <summary>
    /// Dictionary of XML namespace prefixes known by this writer (key = namespace, value = prefix).
    /// </summary>
    /// <remarks>
    /// Extends the default behavior to also record namespaces as written or in advance to avoid
    /// unnecessary duplication of namespace declarations.
    /// </remarks>
    private readonly StringDictionary _namespacePrefixes = [];

    /// <summary>
    /// Dictionary of XML namespace declarations to override (key = prefix, value = replacement).
    /// </summary>
    /// <remarks>
    /// If the replacement prefix is null or empty, attempts to write the it (both declaration
    /// and use) are ignored.
    /// </remarks>
    private readonly StringDictionary _renamePrefixes = [];

    /// <summary>
    /// An XSI type name which should be added to the next element written with <see cref="WriteStartElement(string, string, string)"/>.
    /// </summary>
    private XmlQualifiedName? _addXsiType;

    /// <summary>
    /// Set true after the first call to <see cref="WriteStartElement(string, string, string)"/>
    /// following <see cref="AddXsiType(XmlQualifiedName)"/>.
    /// </summary>
    /// <remarks>
    /// Necessary to ensure that we do not write the XSI type when the <see cref="WriteState"/>
    /// is still positioned on the (already started but not closed) parent element.
    /// </remarks>
    private bool _addXsiTypeStarted;

    /// <summary>
    /// Set true when attempts to write the XSI type attribute must be ignored, i.e. because it
    /// has already been written.
    /// </summary>
    /// <remarks>
    /// Once the flag has been set it is cleared in <see cref="WriteEndElement"/>,
    /// <see cref="WriteFullEndElement"/> (end of current element) or
    /// <see cref="WriteStartElement(string, string, string)"/> (start of child element).
    /// </remarks>
    private bool _blockXsiType;

    /// <summary>
    /// Replacement local name for the next element written with <see cref="WriteStartElement(string, string, string)"/>.
    /// </summary>
    private string? _renameElementLocalName;

    /// <summary>
    /// Replacement namespace for the next element written with <see cref="WriteStartElement(string, string, string)"/>.
    /// </summary>
    private string? _renameElementNamespace;

    /// <summary>
    /// Replacement prefix for the next element written with <see cref="WriteStartElement(string, string, string)"/>.
    /// </summary>
    private string? _renameElementPrefix;

    /// <summary>
    /// Set true when a <see cref="WriteStartAttribute(string, string, string)"/> call has been
    /// ignored, indicating the following <see cref="WriteEndAttribute"/> should also be ignored.
    /// </summary>
    private bool _skipAttribute;

    /// <summary>
    /// Gets the <see cref="XmlWriter"/> wrapped by this instance.
    /// </summary>
    public XmlWriter InnerWriter { get; private set; } = writer;

    /// <summary>
    /// Gets the state of the writer.
    /// </summary>
    public override WriteState WriteState => InnerWriter.WriteState;

    /// <summary>
    /// Ensures that a prefix has been assigned and written, if necessary assigning a new prefix
    /// and writing the namespace declaration at the current element.
    /// </summary>
    /// <param name="namespace">Namespace to add or suppress.</param>
    /// <param name="preferredPrefix">Preferred prefix to use when required but not yet declared.</param>
    /// <returns>Prefix used or empty when it's the current (default) namespace (not necessary).</returns>
    public string AssertPrefixWritten(string @namespace, string preferredPrefix)
    {
        // Check if already written, then return that prefix.
        var prefix = LookupPrefix(@namespace, written: true);
        if (prefix is not null)
            return prefix;

        // Lookup existing or assign preferred prefix.
        prefix = LookupPrefix(@namespace);
        prefix ??= AssertPrefix(@namespace, preferredPrefix);

        // Write namespace (prefix) declaration.
        WriteAttributeString(XmlExtensions.XmlDeclarationPrefix, prefix, null, @namespace);

        // Return newly declared (written) prefix.
        return prefix;
    }

    /// <summary>
    /// Registers an XSI type to add to the next element written with <see cref="WriteStartElement(string, string, string)"/>.
    /// </summary>
    /// <param name="xsiType">Fully qualified XSD type name (QName).</param>
    public void AddXsiType(XmlQualifiedName xsiType)
    {
        _addXsiType = xsiType;
        _blockXsiType = false;
    }

    /// <summary>
    /// Blocks the XSI type from being written in the next element.
    /// </summary>
    public void BlockXsiType()
    {
        _blockXsiType = true;
    }

    /// <summary>
    /// Looks-up or adds a namespace, returning the prefix.
    /// </summary>
    /// <param name="namespace">Namespace to find or add.</param>
    /// <param name="preferredPrefix">
    /// Preferred prefix to assign when a new namespace entry is made.
    /// </param>
    /// <returns>
    /// Existing or <paramref name="preferredPrefix"/>, depending which was used (if existed).
    /// An empty string means the namespace is the current (default) namespace.
    /// </returns>
    public string AssertPrefix(string @namespace, string preferredPrefix)
    {
        // Validate
        ArgumentNullException.ThrowIfNullOrWhiteSpace(@namespace);

        // Check if already exists
        var prefix = LookupPrefix(@namespace);

        // Return when found (empty is valid).
        if (prefix is not null)
            return prefix;

        // Set preferred prefix when new.
        SetPrefix(@namespace, preferredPrefix);

        // Return set prefix.
        return preferredPrefix;
    }

    /// <summary>
    /// When overridden in a derived class, flushes whatever is in the buffer to the underlying
    /// streams and also flushes the underlying stream.
    /// </summary>
    public override void Flush()
    {
        InnerWriter.Flush();
    }

    /// <summary>
    /// Returns the closest prefix defined in the current namespace scope for the namespace URI.
    /// </summary>
    /// <param name="namespace">The namespace URI whose prefix you want to find.</param>
    /// <returns>
    /// Important: empty is not the same as null! Null when no prefix registered for the given
    /// namespace URI. Empty when the namespace is the current namespace (no prefix required).
    /// Prefix string when the namespace is registered.
    /// </returns>
    public override string? LookupPrefix(string @namespace)
    {
        // Call overloaded method
        return LookupPrefix(@namespace, false);
    }

    /// <summary>
    /// Returns the closest prefix defined in the current namespace scope for the namespace URI,
    /// or the renamed or removed prefix (null) which this writer has been requested to override.
    /// </summary>
    /// <param name="namespace">The namespace URI whose prefix you want to find.</param>
    /// <param name="written">
    /// Set true to only return prefixes from the internal writer, i.e. usually means those
    /// which have already been written. Default is false, which also returns prefixes which
    /// this writer will rename or remove.
    /// </param>
    /// <returns>
    /// Important: empty is not the same as null! Null when no prefix registered for the given
    /// namespace URI. Empty when the namespace is the current namespace (no prefix required).
    /// Prefix string when the namespace is registered.
    /// </returns>
    public string? LookupPrefix(string @namespace, bool written = false)
    {
        // Check our additional prefixes first
        if (!written && _namespacePrefixes.ContainsKey(@namespace))
            return _namespacePrefixes[@namespace];

        // Call inner stream method to lookup from internal namespace table
        return InnerWriter.LookupPrefix(@namespace);
    }

    /// <summary>
    /// Changes the prefix, name and/or namespace of the next element written with <see cref="WriteStartElement(string, string, string)"/>.
    /// </summary>
    /// <param name="prefix">Replacement prefix. No change when null or empty.</param>
    /// <param name="localName">Replacement local name. No change when null or empty.</param>
    /// <param name="namespace">
    /// Replacement namespace. No change when null. Empty/unqualified namespace when empty.
    /// </param>
    public void RenameElement(string? prefix, string? localName, string? @namespace)
    {
        // Set flags
        _renameElementPrefix = StringExtensions.NullWhenEmpty(prefix);
        _renameElementLocalName = StringExtensions.NullWhenEmpty(localName);
        _renameElementNamespace = @namespace;
    }

    /// <summary>
    /// Changes all writes of the specified namespace prefix including declaration.
    /// </summary>
    /// <param name="prefix">Existing or future prefix to rename.</param>
    /// <param name="newPrefix">New prefix to change to. Set empty to remove the prefix.</param>
    public void RenamePrefix(string prefix, string newPrefix)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(prefix);
        ArgumentNullException.ThrowIfNull(newPrefix);

        // Add or update rename list
        if (_renamePrefixes.ContainsKey(prefix))
            _renamePrefixes[prefix] = newPrefix;
        else
            _renamePrefixes.Add(prefix, newPrefix);

        // Update our lookup list
        if (_namespacePrefixes.ContainsKey(prefix))
        {
            var @namespace = _namespacePrefixes[prefix];
            _namespacePrefixes.Remove(prefix);
            if (_namespacePrefixes.ContainsKey(newPrefix))
                _namespacePrefixes[newPrefix] = @namespace;
            else
                _namespacePrefixes.Add(newPrefix, @namespace);
        }
    }

    /// <summary>
    /// Registers a new prefix or renames the namespace of an existing prefix.
    /// </summary>
    /// <param name="namespace">Namespace that the prefix references.</param>
    /// <param name="prefix">Prefix to register or rename the namespace of.</param>
    public void SetPrefix(string @namespace, string prefix)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(prefix);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(@namespace);

        // Use new name if renamed
        if (_renamePrefixes.ContainsKey(prefix))
            prefix = _renamePrefixes[prefix]!;

        // Add or update prefix
        if (_namespacePrefixes.ContainsKey(@namespace))
            _namespacePrefixes[@namespace] = prefix;
        else
            _namespacePrefixes.Add(@namespace, prefix);
    }

    /// <summary>
    /// When overridden in a derived class, encodes the specified binary bytes as Base64 and
    /// writes out the resulting text.
    /// </summary>
    /// <param name="buffer">Byte array to encode.</param>
    /// <param name="index">
    /// The position in the buffer indicating the start of the bytes to write.
    /// </param>
    /// <param name="count">The number of bytes to write.</param>
    public override void WriteBase64(byte[] buffer, int index, int count)
    {
        // Check for close of start element and call handler when necessary
        if (WriteState == WriteState.Element)
            OnStartElementClose();

        // Call inner stream method to write content
        InnerWriter.WriteBase64(buffer, index, count);
    }

    /// <summary>
    /// When overridden in a derived class, writes out a &lt;![CDATA[...]]&gt; block containing
    /// the specified text.
    /// </summary>
    /// <param name="text">The text to place inside the CDATA block.</param>
    public override void WriteCData(string? text)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(text);

        // Check for close of start element and call handler when necessary
        if (WriteState == WriteState.Element)
            OnStartElementClose();

        // Call inner stream method to write content
        InnerWriter.WriteCData(text);
    }

    /// <summary>
    /// When overridden in a derived class, forces the generation of a character entity for the
    /// specified Unicode character value.
    /// </summary>
    /// <param name="character">The Unicode character for which to generate a character entity.</param>
    public override void WriteCharEntity(char character)
    {
        // Check for close of start element and call handler when necessary
        if (WriteState == WriteState.Element)
            OnStartElementClose();

        // Call inner stream method to write content
        InnerWriter.WriteCharEntity(character);
    }

    /// <summary>
    /// When overridden in a derived class, writes text one buffer at a time.
    /// </summary>
    /// <param name="buffer">Character array containing the text to write.</param>
    /// <param name="index">
    /// The position in the buffer indicating the start of the text to write.
    /// </param>
    /// <param name="count">The number of characters to write.</param>
    public override void WriteChars(char[] buffer, int index, int count)
    {
        // Check for close of start element and call handler when necessary
        if (WriteState == WriteState.Element)
            OnStartElementClose();

        // Call inner stream method to write content
        InnerWriter.WriteChars(buffer, index, count);
    }

    /// <summary>
    /// When overridden in a derived class, writes out a comment &lt;!--...--&gt; containing the
    /// specified text.
    /// </summary>
    /// <param name="text">Text to place inside the comment.</param>
    public override void WriteComment(string? text)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(text);

        // Check for close of start element and call handler when necessary
        if (WriteState == WriteState.Element)
            OnStartElementClose();

        // Call inner stream method to write content
        InnerWriter.WriteComment(text);
    }

    /// <summary>
    /// When overridden in a derived class, writes the DOCTYPE declaration with the specified
    /// name and optional attributes.
    /// </summary>
    /// <param name="name">The name of the DOCTYPE. This must be non-empty.</param>
    /// <param name="publicId">
    /// If non-null it also writes PUBLIC "pubid" "sysid" where <paramref name="publicId"/> and
    /// <paramref name="systemId"/> are replaced with the value of the given arguments.
    /// </param>
    /// <param name="systemId">
    /// If <paramref name="publicId"/> is null and <paramref name="systemId"/> is non-null it
    /// writes SYSTEM "sysid" where <paramref name="systemId"/> is replaced with the value of
    /// this argument.
    /// </param>
    /// <param name="subset">
    /// If non-null it writes [subset] where subset is replaced with the value of this argument.
    /// </param>
    public override void WriteDocType(string name, string? publicId, string? systemId, string? subset)
    {
        InnerWriter.WriteDocType(name, publicId, systemId, subset);
    }

    /// <summary>
    /// When overridden in a derived class, closes the previous
    /// <see cref="XmlWriter.WriteStartAttribute(string,string)"/> call.
    /// </summary>
    public override void WriteEndAttribute()
    {
        // Do nothing when attribute has been ignored
        if (_skipAttribute)
        {
            // Clear flags and return
            _skipAttribute = false;
            return;
        }

        // Call inner stream method to write end attribute
        InnerWriter.WriteEndAttribute();
    }

    /// <summary>
    /// When overridden in a derived class, closes any open elements or attributes and puts the
    /// writer back in the Start state.
    /// </summary>
    public override void WriteEndDocument()
    {
        // Check for close of start element and call handler when necessary
        if (WriteState == WriteState.Element)
            OnStartElementClose();

        // Call inner stream method to write content
        InnerWriter.WriteEndDocument();
    }

    /// <summary>
    /// When overridden in a derived class, closes one element and pops the corresponding
    /// namespace scope.
    /// </summary>
    /// <remarks>
    /// Additionally writes the XSI type attribute when
    /// <see cref="AddXsiType(XmlQualifiedName)"/> has been called.
    /// </remarks>
    public override void WriteEndElement()
    {
        // Call handler
        OnStartElementClose();

        // Call inner steam method to write end element
        InnerWriter.WriteEndElement();
    }

    /// <summary>
    /// When overridden in a derived class, writes out an entity reference as &amp;name;.
    /// </summary>
    /// <param name="name">The name of the entity reference.</param>
    public override void WriteEntityRef(string name)
    {
        InnerWriter.WriteEntityRef(name);
    }

    /// <summary>
    /// When overridden in a derived class, closes one element and pops the corresponding
    /// namespace scope.
    /// </summary>
    /// <remarks>
    /// Additionally writes the XSI type attribute when
    /// <see cref="AddXsiType(XmlQualifiedName)"/> has been called.
    /// </remarks>
    public override void WriteFullEndElement()
    {
        // Call handler
        OnStartElementClose();

        // Call inner steam method to write full end element
        InnerWriter.WriteFullEndElement();
    }

    /// <summary>
    /// When overridden in a derived class, writes out a processing instruction with a space
    /// between the name and text as follows: &lt;?name text?&gt;.
    /// </summary>
    /// <param name="name">The name of the processing instruction.</param>
    /// <param name="text">The text to include in the processing instruction.</param>
    public override void WriteProcessingInstruction(string name, string? text)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(text);

        // Check for close of start element and call handler when necessary
        if (WriteState == WriteState.Element)
            OnStartElementClose();

        // Call inner stream method to write content
        InnerWriter.WriteProcessingInstruction(name, text);
    }

    /// <summary>
    /// When overridden in a derived class, writes raw markup manually from a string.
    /// </summary>
    /// <param name="data">String containing the text to write.</param>
    public override void WriteRaw(string data)
    {
        // Check for close of start element and call handler when necessary
        if (WriteState == WriteState.Element)
            OnStartElementClose();

        // Call inner stream method to write content
        InnerWriter.WriteRaw(data);
    }

    /// <summary>
    /// When overridden in a derived class, writes raw markup manually from a character buffer.
    /// </summary>
    /// <param name="buffer">Character array containing the text to write.</param>
    /// <param name="index">
    /// The position within the buffer indicating the start of the text to write.
    /// </param>
    /// <param name="count">The number of characters to write.</param>
    public override void WriteRaw(char[] buffer, int index, int count)
    {
        // Check for close of start element and call handler when necessary
        if (WriteState == WriteState.Element)
            OnStartElementClose();

        // Call inner stream method to write content
        InnerWriter.WriteRaw(buffer, index, count);
    }

    /// <summary>
    /// When overridden in a derived class, writes the start of an attribute with the specified
    /// prefix, local name, and namespace URI.
    /// </summary>
    /// <param name="prefix">The namespace prefix of the attribute.</param>
    /// <param name="localName">The local name of the attribute.</param>
    /// <param name="namespace">The namespace URI for the attribute.</param>
    public override void WriteStartAttribute(string? prefix, string localName, string? @namespace)
    {
        // Detect namespace declarations
        if (prefix == XmlExtensions.XmlDeclarationPrefix)
        {
            // Check for rename or remove of prefix
            if (_renamePrefixes.ContainsKey(localName))
            {
                var newPrefix = _renamePrefixes[localName];
                if (string.IsNullOrWhiteSpace(newPrefix))
                {
                    // Skip when renamed to empty (default) or removed (null)
                    _skipAttribute = true;
                    return;
                }

                // Rename prefix
                prefix = newPrefix;
            }
        }
        else
        {
            // Check for known prefix and use that first
            if (@namespace != null && _namespacePrefixes.ContainsKey(@namespace))
            {
                var knownPrefix = _namespacePrefixes[@namespace];
                if (knownPrefix != null)
                {
                    // Use existing prefix
                    prefix = knownPrefix;

                    // Add to renames if not already present
                    if (!_renamePrefixes.ContainsKey(prefix) && knownPrefix != prefix)
                        _renamePrefixes.Add(prefix, knownPrefix);
                }
            }

            // Check for rename or remove of prefix
            if (prefix != null && _renamePrefixes.ContainsKey(prefix))
            {
                var newPrefix = _renamePrefixes[localName];
                if (string.IsNullOrWhiteSpace(newPrefix))
                {
                    // Remove prefix when renamed to null or empty
                    prefix = "";
                }
                else
                {
                    // Rename prefix
                    prefix = newPrefix;
                }
            }

            // Skip duplicate attributes
            if (@namespace == XmlSchema.InstanceNamespace && localName == "type")
            {
                if (_blockXsiType)
                {
                    // Flag end-attribute should be skipped
                    _skipAttribute = true;
                    _blockXsiType = false;
                    return;
                }
                else
                {
                    // Clear flags to prevent duplicate XSI type
                    _addXsiType = null;
                }
            }
        }

        // Call stream method to write attribute
        InnerWriter.WriteStartAttribute(prefix, localName, @namespace);
    }

    /// <summary>
    /// When overridden in a derived class, writes the XML declaration with the version "1.0".
    /// </summary>
    public override void WriteStartDocument()
    {
        InnerWriter.WriteStartDocument();
    }

    /// <summary>
    /// When overridden in a derived class, writes the XML declaration with the version "1.0"
    /// and the standalone attribute.
    /// </summary>
    /// <param name="standalone">If true, it writes "standalone=yes"; if false, it writes "standalone=no".</param>
    public override void WriteStartDocument(bool standalone)
    {
        InnerWriter.WriteStartDocument(standalone);
    }

    /// <summary>
    /// When overridden in a derived class, writes the specified start tag and associates it
    /// with the given namespace and prefix.
    /// </summary>
    /// <param name="prefix">The namespace prefix of the element.</param>
    /// <param name="localName">The local name of the element.</param>
    /// <param name="namespace">The namespace URI to associate with the element.</param>
    public override void WriteStartElement(string? prefix, string localName, string? @namespace)
    {
        // Check for close of start element and call handler when necessary
        if (WriteState == WriteState.Element)
            OnStartElementClose();

        // Rename when specified
        if (!string.IsNullOrWhiteSpace(_renameElementPrefix))
        {
            prefix = _renameElementPrefix!;
            _renameElementPrefix = null;
        }
        if (!string.IsNullOrWhiteSpace(_renameElementLocalName))
        {
            localName = _renameElementLocalName!;
            _renameElementLocalName = null;
        }
        if (_renameElementNamespace != null)
        {
            @namespace = _renameElementNamespace;
            _renameElementNamespace = null;
        }

        // Check for known prefix and use that first
        if (@namespace != null && _namespacePrefixes.ContainsKey(@namespace))
        {
            // Add to renames if not already present
            var knownPrefix = _namespacePrefixes[@namespace];
            if (prefix != null && !_renamePrefixes.ContainsKey(prefix) && knownPrefix != prefix)
                _renamePrefixes.Add(prefix, knownPrefix);

            // Use existing prefix (unless removed)
            if (knownPrefix != null)
                prefix = knownPrefix;
        }

        // Check for rename or remove of prefix
        if (prefix != null && _renamePrefixes.ContainsKey(prefix))
        {
            var newPrefix = _renamePrefixes[localName];
            if (string.IsNullOrWhiteSpace(newPrefix))
            {
                // Remove prefix when renamed to null or empty
                prefix = "";
            }
            else
            {
                // Rename prefix
                prefix = newPrefix;
            }
        }

        // Call stream method to write the start element
        InnerWriter.WriteStartElement(prefix, localName, @namespace);

        // Flag start of injection
        if (_addXsiType != null)
            _addXsiTypeStarted = true;
    }

    /// <summary>
    /// When overridden in a derived class, writes the given text content.
    /// </summary>
    /// <param name="text">The text to write.</param>
    public override void WriteString(string? text)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(text);

        // Do not write skipped attribute text.
        if (_skipAttribute)
            return;

        // Extend functionality
        switch (WriteState)
        {
            case WriteState.Element:

                // Complete start element (writing string moves to content)
                OnStartElementClose();
                break;
        }

        // Call inner stream method to write content
        InnerWriter.WriteString(text);
    }

    /// <summary>
    /// When overridden in a derived class, generates and writes the surrogate character entity
    /// for the surrogate character pair.
    /// </summary>
    /// <param name="low">The low surrogate. This must be a value between 0xDC00 and 0xDFFF.</param>
    /// <param name="high">The high surrogate. This must be a value between 0xD800 and 0xDBFF.</param>
    public override void WriteSurrogateCharEntity(char low, char high)
    {
        // Check for close of start element and call handler when necessary
        if (WriteState == WriteState.Element)
            OnStartElementClose();

        // Call inner stream method to write content
        InnerWriter.WriteSurrogateCharEntity(low, high);
    }

    /// <summary>
    /// When overridden in a derived class, writes out the given white space.
    /// </summary>
    /// <param name="space">The string of white space characters.</param>
    public override void WriteWhitespace(string? space)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(space);

        // Check for close of start element and call handler when necessary
        if (WriteState == WriteState.Element)
            OnStartElementClose();

        // Call inner stream method to write content
        InnerWriter.WriteWhitespace(space);
    }

    /// <summary>
    /// Called when an element end occurs directly or indirectly, i.e. writing child content
    /// after a start element.
    /// </summary>
    private void OnStartElementClose()
    {
        // Append the XSI type when specified
        if (_addXsiTypeStarted && _addXsiType != null)
        {
            // Write the "xsi:type" attribute
            InnerWriter.WriteStartAttribute("type", XmlSchema.InstanceNamespace);
            InnerWriter.WriteQualifiedName(_addXsiType.Name, _addXsiType.Namespace);
            InnerWriter.WriteEndAttribute();

            // Clear all flags
            _addXsiType = null;
            _addXsiTypeStarted = false;
            _blockXsiType = false;
        }

        // Clear element scoped flags
        _skipAttribute = false;
    }
}
