using System.IO;
using System.Reflection;
using System.Xml.Schema;

namespace CodeForDotNet
{
	/// <summary>
	/// Core XML constants and helper methods.
	/// </summary>
	public static class CodeForDotNetXsd
	{
		#region Public Fields

		/// <summary>
		/// XML XSD filename.
		/// </summary>
		public const string XmlXsdFileName = "Xml.xsd";

		/// <summary>
		/// Types XSD filename.
		/// </summary>
		public const string XsdFileName = "CodeForDotNet.xsd";

		/// <summary>
		/// XSLT XSD filename.
		/// </summary>
		public const string XsltXsdFileName = "Xslt.xsd";

		#endregion Public Fields

		#region Public Methods

		/// <summary>
		/// Gets the <see cref="XmlSchema"/> which defines types in this assembly. This may not be sufficient for validation because it does not include any
		/// imported types.
		/// </summary>
		public static Stream GetSchemaFile()
		{
			// Load schema from resource
			var type = typeof(CodeForDotNetXsd);
			var assembly = Assembly.Load(new AssemblyName(type.AssemblyQualifiedName));
			return assembly.GetManifestResourceStream(type.Namespace + "." + XsdFileName);
		}

		#endregion Public Methods
	}
}
