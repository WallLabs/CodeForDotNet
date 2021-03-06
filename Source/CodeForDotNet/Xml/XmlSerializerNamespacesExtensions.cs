using System;
using System.Xml.Serialization;

namespace CodeForDotNet.Xml
{
	/// <summary>
	/// Extensions for working with the <see cref="XmlSerializer"/>.
	/// </summary>
	public static class XmlSerializerNamespacesExtensions
	{
		#region Public Methods

		/// <summary>
		/// Merges namespaces.
		/// </summary>
		public static void Merge(this XmlSerializerNamespaces xmlns, XmlSerializerNamespaces other)
		{
			// Validate
			if (xmlns == null) throw new ArgumentNullException(nameof(xmlns));
			if (other == null) throw new ArgumentNullException(nameof(other));

			// Merge...
			foreach (var qname in other.ToArray())
				xmlns.Add(qname.Name, qname.Namespace);
		}

		#endregion Public Methods
	}
}
