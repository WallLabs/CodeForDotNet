using System.Net.NetworkInformation;

namespace CodeForDotNet.Net
{
	/// <summary>
	/// Networking extensions.
	/// </summary>
	public static class NetworkExtensions
	{
		#region Public Methods

		/// <summary>
		/// Gets the host name and any domain (the FQDN) of the local computer.
		/// </summary>
		public static string GetFullHostName()
		{
			var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
			return string.IsNullOrWhiteSpace(ipProperties.DomainName)
					   ? ipProperties.HostName
					   : ipProperties.HostName + "." + ipProperties.DomainName;
		}

		#endregion Public Methods
	}
}
