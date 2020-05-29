using System;
using System.Linq;
using System.Runtime.InteropServices;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
	/// <summary>
	/// Encapsulates a <see cref="Wia.DeviceInfo"/> in managed code.
	/// </summary>
	public class WiaDeviceInfo : DisposableObject
	{
		#region Private Fields

		/// <summary>
		/// Unmanaged <see cref="Wia.DeviceInfo"/>.
		/// </summary>
		private readonly Wia.DeviceInfo _wiaDeviceInfo;

		private WiaPropertyCollection? _properties;

		#endregion Private Fields

		#region Internal Constructors

		/// <summary>
		/// Creates an instance to wrap the specified unmanaged object.
		/// </summary>
		internal WiaDeviceInfo(Wia.DeviceInfo deviceInfo)
		{
			_wiaDeviceInfo = deviceInfo;
		}

		#endregion Internal Constructors

		#region Public Properties

		/// <summary>
		/// Thread synchronization object.
		/// </summary>
		public static object SyncRoot { get; } = new object();

		/// <summary>
		/// Description.
		/// </summary>
		public string Description
		{
			get
			{
				return (from property in Properties
						where property.Name == "Description"
						select (string)property.Value).FirstOrDefault();
			}
		}

		/// <summary>
		/// Device type.
		/// </summary>
		public WiaDeviceType DeviceType { get { return (WiaDeviceType)(int)_wiaDeviceInfo.Type; } }

		/// <summary>
		/// Device identifier.
		/// </summary>
		public string Id { get { return _wiaDeviceInfo.DeviceID; } }

		/// <summary>
		/// Name.
		/// </summary>
		public string Name
		{
			get
			{
				return (from property in Properties
						where property.Name == "Name"
						select (string)property.Value).FirstOrDefault();
			}
		}

		/// <summary>
		/// Properties.
		/// </summary>
		public WiaPropertyCollection Properties
		{
			get
			{
				// Create wrapper first time
				if (_properties == null)
				{
					lock (SyncRoot)
					{
						if (_properties == null)
							_properties = new WiaPropertyCollection(_wiaDeviceInfo.Properties);
					}
				}

				// Return wrapped value
				return _properties;
			}
		}

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Creates a connection to this device.
		/// </summary>
		public WiaDevice Connect()
		{
			var wiaDevice = _wiaDeviceInfo.Connect();
			return new WiaDevice(wiaDevice);
		}

		#endregion Public Methods

		#region Protected Methods

		/// <summary>
		/// Frees resources owned by this instance.
		/// </summary>
		/// <param name="disposing">True when called from <see cref="IDisposable.Dispose()"/>, false when called during finalization.</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				// Dispose managed resources.
				if (disposing)
				{
					if (_properties != null) _properties.Dispose();
				}

				// Dispose unmanaged resources.
				if (_wiaDeviceInfo != null)
					Marshal.ReleaseComObject(_wiaDeviceInfo);
			}
			finally
			{
				// Call base class method to fire events and set status properties.
				base.Dispose(disposing);
			}
		}

		#endregion Protected Methods
	}
}
