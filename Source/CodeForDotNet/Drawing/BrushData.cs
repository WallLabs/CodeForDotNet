using CodeForDotNet.Collections;
using CodeForDotNet.Xml;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Xml.Serialization;

namespace CodeForDotNet.Drawing
{
    /// <summary>
    /// Brush data.
    /// </summary>
    [Serializable]
    [SuppressMessage("Microsoft.Usage", "CA2235", Justification = "Custom member types are serializable. Rest are false positive, e.g. built-in value types do not need to be marked serializable.")]
    [SuppressMessage("Microsoft.Performance", "CA1819", Justification = "Array properties are required for serialization and it's okay if the referenced data array is changed because it is a data entity.")]
    public class BrushData : ICloneable
    {
        #region Lifetime

        /// <summary>
        /// Creates the brush.
        /// </summary>
        public BrushData()
        {
        }

        /// <summary>
        /// Creates a brush with a single color.
        /// </summary>
        public BrushData(Color startColor)
        {
            _brushType = BrushFillType.SingleColor;
            StartColor = startColor.ToArgb();
        }

        /// <summary>
        /// Creates a brush with a two color gradient.
        /// </summary>
        public BrushData(Color startColor, Color endColor, decimal angle)
        {
            _brushType = BrushFillType.TwoColorGradient;
            StartColor = startColor.ToArgb();
            EndColor = endColor.ToArgb();
            Angle = angle;
        }

        /// <summary>
        /// Creates a brush with a texture.
        /// </summary>
        public BrushData(byte[] texture, decimal angle, BrushFillWrapMode wrapMode, float scale)
        {
            _brushType = BrushFillType.Texture;
            Texture = texture;
            Angle = angle;
            WrapMode = wrapMode;
            Scale = scale;
        }

        #endregion Lifetime

        #region Properties

        /// <summary>
        /// TypeId of brush.
        /// </summary>
        public BrushFillType BrushType
        {
            get
            {
                // Return current value
                return _brushType;
            }

            set
            {
                // Clear existing values and set new brush type
                switch (value)
                {
                    case BrushFillType.SingleColor:
                        EndColor = null;
                        TextureId = null;
                        Texture = null;
                        WrapMode = null;
                        Angle = null;
                        Scale = null;
                        break;

                    case BrushFillType.TwoColorGradient:
                        TextureId = null;
                        Texture = null;
                        WrapMode = null;
                        Scale = null;
                        break;

                    case BrushFillType.Texture:
                        StartColor = null;
                        EndColor = null;
                        break;
                }
                _brushType = value;
            }
        }

        private BrushFillType _brushType;

        /// <summary>
        /// Start color, when relevant for the type.
        /// </summary>
        public int? StartColor { get; set; }

        /// <summary>
        /// Omits the <see cref="StartColor"/> property from XML serialization when empty.
        /// </summary>
        [XmlIgnore]
        public bool StartColorSpecified { get { return StartColor.HasValue; } }

        /// <summary>
        /// End color, when relevant for the type.
        /// </summary>
        public int? EndColor { get; set; }

        /// <summary>
        /// Omits the <see cref="EndColor"/> property from XML serialization when empty.
        /// </summary>
        [XmlIgnore]
        public bool EndColorSpecified { get { return EndColor.HasValue; } }

        /// <summary>
        /// Angle, when relevant for the type.
        /// </summary>
        public decimal? Angle { get; set; }

        /// <summary>
        /// Omits the <see cref="Angle"/> property from XML serialization when empty.
        /// </summary>
        [XmlIgnore]
        public bool AngleSpecified { get { return Angle.HasValue; } }

        /// <summary>
        /// Texture ID, used to correlate with media libraries,
        /// enabling re-use of images for performance and to reduce
        /// disk space when saved.
        /// </summary>
        public Guid? TextureId { get; set; }

        /// <summary>
        /// Texture, when relevant for the type.
        /// </summary>
        public byte[] Texture { get; set; }

        /// <summary>
        /// Omits the <see cref="Texture"/> property from XML serialization when empty.
        /// </summary>
        [XmlIgnore]
        public bool TextureSpecified { get { return Texture != null; } }

        /// <summary>
        /// Wrap mode, when relevant for the type, i.e. texture.
        /// </summary>
        public BrushFillWrapMode? WrapMode { get; set; }

        /// <summary>
        /// Omits the <see cref="WrapMode"/> property from XML serialization when empty.
        /// </summary>
        [XmlIgnore]
        public bool WrapModeSpecified { get { return WrapMode.HasValue; } }

        /// <summary>
        /// Scaling factor, when relevant for the type.
        /// </summary>
        public float? Scale { get; set; }

        /// <summary>
        /// Omits the <see cref="Scale"/> property from XML serialization when empty.
        /// </summary>
        [XmlIgnore]
        public bool ScaleSpecified { get { return Scale.HasValue; } }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Converts the brush to an XML string.
        /// </summary>
        public override string ToString()
        {
            return this.SerializeXml();
        }

        /// <summary>
        /// Creates an instance from a string.
        /// </summary>
        public static BrushData Parse(string value)
        {
            return XmlSerializerExtensions.DeserializeXml<BrushData>(value);
        }

        #endregion Public Methods

        #region Operators

        /// <summary>
        /// Overrides the equality operator to compare by value.
        /// </summary>
        public static bool operator ==(BrushData brush1, BrushData brush2)
        {
            return Equals(brush1, brush2);
        }

        /// <summary>
        /// Overrides the inequality operator to compare by value.
        /// </summary>
        public static bool operator !=(BrushData brush1, BrushData brush2)
        {
            return !Equals(brush1, brush2);
        }

        /// <summary>
        /// Overrides the Equals method to compare by value,
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            // Check type and nullability
            var other = obj as BrushData;
            if (other is null)
                return false;

            // Compare values
            return
                BrushType == other.BrushType &&
                StartColor == other.StartColor &&
                EndColor == other.EndColor &&
                ArrayExtensions.AreEqual(Texture, other.Texture) &&
                WrapMode == other.WrapMode &&
                Scale == other.Scale;
        }

        /// <summary>
        /// Gets a hash-code for this object based on current values.
        /// </summary>
        public override int GetHashCode()
        {
            return
                (StartColor != null ? StartColor.GetHashCode() : 0) ^
                (EndColor != null ? EndColor.GetHashCode() : 0) ^
                (Texture != null ? Texture.GetHashCode() : 0) ^
                Angle.GetHashCode() ^
                Scale.GetHashCode() ^
                WrapMode.GetHashCode() ^
                BrushType.GetHashCode();
        }

        #endregion Operators

        #region ICloneable Members

        /// <summary>
        /// Copies this brush.
        /// </summary>
        public BrushData Copy()
        {
            return new BrushData
            {
                BrushType = BrushType,
                StartColor = StartColor,
                EndColor = EndColor,
                Texture = Texture != null ? (byte[])Texture.Clone() : null,
                WrapMode = WrapMode,
                Angle = Angle,
                Scale = Scale
            };
        }

        /// <summary>
        /// Creates a copy of this object.
        /// </summary>
        public object Clone()
        {
            return Copy();
        }

        #endregion ICloneable Members
    }
}