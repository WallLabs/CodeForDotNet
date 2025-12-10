using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging;

/// <summary>
/// Managed <see cref="Wia.Property"/>.
/// </summary>
[SupportedOSPlatform("windows")]
public class WiaProperty : DisposableObject
{
    #region Private Fields

    /// <summary>
    /// Unmanaged <see cref="Wia.Property"/>.
    /// </summary>
    private readonly Wia.Property _wiaProperty;

    #endregion Private Fields

    #region Internal Constructors

    /// <summary>
    /// Creates an instance to wrap the specified unmanaged object.
    /// </summary>
    internal WiaProperty(Wia.Property property)
    {
        _wiaProperty = property;
    }

    #endregion Internal Constructors

    #region Public Properties

    /// <summary>
    /// Thread synchronization object.
    /// </summary>
    public static object SyncRoot { get; } = new object();

    /// <summary>
    /// ID.
    /// </summary>
    public int Id => _wiaProperty.PropertyID;

    /// <summary>
    /// Indicates the property is read-only.
    /// </summary>
    public bool IsReadOnly => _wiaProperty.IsReadOnly;

    /// <summary>
    /// Indicates the property is a vector.
    /// </summary>
    public bool IsVector => _wiaProperty.IsVector;

    /// <summary>
    /// Name.
    /// </summary>
    public string Name => _wiaProperty.Name;

    /// <summary>
    /// Property type.
    /// </summary>
    public int PropertyType => _wiaProperty.Type;

    /// <summary>
    /// WIA sub-type.
    /// </summary>
    public WiaSubtype Subtype => (WiaSubtype)(int)_wiaProperty.SubType;

    /// <summary>
    /// WIA sub-type default value.
    /// </summary>
    public object SubtypeDefault => _wiaProperty.SubTypeDefault;

    /// <summary>
    /// WIA sub-type maximum value.
    /// </summary>
    public int SubtypeMax => _wiaProperty.SubTypeMax;

    /// <summary>
    /// WIA sub-type minimum.
    /// </summary>
    public int SubtypeMin => _wiaProperty.SubTypeMin;

    /// <summary>
    /// WIA sub-type increment or decrement step.
    /// </summary>
    public int SubtypeStep => _wiaProperty.SubTypeStep;

    /// <summary>
    /// Property value.
    /// </summary>
    public object Value
    {
        get => _wiaProperty.get_Value(); set => _wiaProperty.set_Value(ref value);
    }

    /// <summary>
    /// WIA sub-type array values.
    /// </summary>
    public WiaVector WiaSubtypeValues
    {
        get
        {
            // Create wrapper first time
            if (field == null)
            {
                lock (SyncRoot)
                {
                    field ??= new WiaVector(_wiaProperty.SubTypeValues);
                }
            }

            // Return wrapped value
            return field;
        }
    }

    #endregion Public Properties

    #region Protected Methods

    /// <summary>
    /// Frees resources owned by this instance.
    /// </summary>
    /// <param name="disposing">True when called from <see cref="IDisposable.Dispose()"/>, false when called during finalization.</param>
    protected override void Dispose(bool disposing)
    {
        try
        {
            // Dispose unmanaged resources.
            _ = Marshal.ReleaseComObject(_wiaProperty);
        }
        finally
        {
            // Call base class method to fire events and set status properties.
            base.Dispose(disposing);
        }
    }

    #endregion Protected Methods
}
