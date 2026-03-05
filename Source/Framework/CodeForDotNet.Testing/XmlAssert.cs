using System;
using System.Collections.Generic;
using CodeForDotNet.Xml;

namespace CodeForDotNet.Testing;

/// <summary>
/// Test helper methods for XML functionality and data.
/// </summary>
public static class XmlAssert
{
    /// <summary>
    /// Ensures that XML namespace declarations are optimized, i.e. only declared once.
    /// </summary>
    /// <param name="xml">XML to check.</param>
    public static void AssertCleanNamespaces(string xml)
    {
        // Validate.
        if (string.IsNullOrWhiteSpace(xml)) throw new ArgumentNullException(nameof(xml));

        // Check XML...
        var namespaces = new List<string>();
        var index = 0;
        while (index < xml.Length)
        {
            // Find next declaration.
            var declarationIndex = xml.IndexOf(XmlExtensions.XmlDeclarationPrefix + ":",
                index, StringComparison.OrdinalIgnoreCase);
            if (declarationIndex < 0)
            {
                // No more found.
                break;
            }

            // Get namespace.
            var namespaceIndex = xml.IndexOf('"', declarationIndex);
            var prefix = xml[declarationIndex..namespaceIndex].TrimEnd('=').Trim();
            var declarationEnd = xml.IndexOf('"', namespaceIndex + 1);
            var @namespace = xml[namespaceIndex..declarationEnd].Trim([' ', '"']);

            // Allow multiple default namespace changes.
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                // Check it has not been defined more than once.
                if (namespaces.Contains(@namespace))
                    throw new RankException(nameof(namespaces));

                // Record namespace (so it cannot be added again).
                namespaces.Add(@namespace);
            }

            // Continue search...
            index = declarationEnd + 1;
        }
    }
}
