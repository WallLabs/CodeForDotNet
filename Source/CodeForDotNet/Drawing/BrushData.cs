using CodeForDotNet.Collections;
using CodeForDotNet.Xml;
using System;
using System.Drawing;
using System.Xml.Serialization;

namespace CodeForDotNet.Drawing
{
    /// <summary>
    /// Brush data.
    /// </summary>
    [Serializable]
    public class BrushData : ICloneable
    {
        #region Private Fields

        private BrushFillType _brushType;

        #endregion Private Fields

        #region Public Constructors

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

        #endregion Public Constructors

        #region Public Properties

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
        /// Scaling factor, when relevant for the type.
        /// </summary>
        public float? Scale { get; set; }

        /// <summary>
        /// Omits the <see cref="Scale"/> property from XML serialization when empty.
        /// </summary>
        [XmlIgnore]
        public bool ScaleSpecified { get { return Scale.HasValue; } }

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
        /// Texture, when relevant for the type.
        /// </summary>
        public byte[]? Texture { get; set; }

        /// <summary>
        /// Texture ID, used to correlate with media libraries, enabling re-use of images for performance and to reduce disk space when saved.
        /// </summary>
        public Guid? TextureId { get; set; }

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

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Overrides the inequality operator to compare by value.
        /// </summary>
        public static bool operator !=(BrushData brush1, BrushData brush2)
        {
            return !Equals(brush1, brush2);
        }

        /// <summary>
        /// Overrides the equality operator to compare by value.
        /// </summary>
        public static bool operator ==(BrushData brush1, BrushData brush2)
        {
            return Equals(brush1, brush2);
        }

        /// <summary>
        /// Creates an instance from a string.
        /// </summary>
        public static BrushData Parse(string value)
        {
            return XmlSerializerExtensions.DeserializeXml<BrushData>(value);
        }

        /// <summary>
        /// Creates a copy of this object.
        /// </summary>
        public object Clone()
        {
            return Copy();
        }

        /// <summary>
        /// Copies this brush.
        /// </summary>
        public BrushData Copy()
        {
            return new BrushData {
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
        /// Overrides the Equals method to compare by value,
        /// </summary>
        public override bool Equals(object? other)
        {
            // Check type and nullability
            if (!(other is BrushData brush) || (brush is null))
                return false;

            // Compare values
            return
                BrushType == brush.BrushType &&
                StartColor == brush.StartColor &&
                EndColor == brush.EndColor &&
                ArrayExtensions.AreEqual(Texture, brush.Texture) &&
                WrapMode == brush.WrapMode &&
                Scale == brush.Scale;
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

        /// <summary>
        /// Converts the brush to an XML string.
        /// </summary>
        public override string ToString()
        {
            return this.SerializeXml();
        }

        #endregion Public Methods
    }
}
