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
    public class LogicalBrush : ICloneable
    {
        #region Lifetime

        /// <summary>
        /// Creates the brush.
        /// </summary>
        public LogicalBrush()
        {
        }

        /// <summary>
        /// Creates a brush with a single color.
        /// </summary>
        public LogicalBrush(Color startColor)
        {
            _brushType = LogicalBrushType.SingleColor;
            StartColor = startColor.ToArgb();
        }

        /// <summary>
        /// Creates a brush with a two color gradient.
        /// </summary>
        public LogicalBrush(Color startColor, Color endColor, Decimal angle)
        {
            _brushType = LogicalBrushType.TwoColorGradient;
            StartColor = startColor.ToArgb();
            EndColor = endColor.ToArgb();
            Angle = angle;
        }

        /// <summary>
        /// Creates a brush with a texture.
        /// </summary>
        public LogicalBrush(byte[] texture, Decimal angle, LogicalBrushWrapMode wrapMode, float scale)
        {
            _brushType = LogicalBrushType.Texture;
            Texture = texture;
            Angle = angle;
            WrapMode = wrapMode;
            Scale = scale;
        }

        #endregion

        #region Properties

        /// <summary>
        /// TypeId of brush.
        /// </summary>
        public LogicalBrushType BrushType
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
                    case LogicalBrushType.SingleColor:
                        EndColor = null;
                        TextureId = null;
                        Texture = null;
                        WrapMode = null;
                        Angle = null;
                        Scale = null;
                        break;

                    case LogicalBrushType.TwoColorGradient:
                        TextureId = null;
                        Texture = null;
                        WrapMode = null;
                        Scale = null;
                        break;

                    case LogicalBrushType.Texture:
                        StartColor = null;
                        EndColor = null;
                        break;
                }
                _brushType = value;
            }
        }
        LogicalBrushType _brushType;

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
        public Decimal? Angle { get; set; }

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
        public LogicalBrushWrapMode? WrapMode { get; set; }

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

        #endregion

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
        public static LogicalBrush Parse(string value)
        {
            return XmlSerializerExtensions.DeserializeXml<LogicalBrush>(value);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Overrides the equality operator to compare by value.
        /// </summary>
        public static bool operator ==(LogicalBrush brush1, LogicalBrush brush2)
        {
            return Equals(brush1, brush2);
        }

        /// <summary>
        /// Overrides the inequality operator to compare by value.
        /// </summary>
        public static bool operator !=(LogicalBrush brush1, LogicalBrush brush2)
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
            var other = obj as LogicalBrush;
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

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Copies this brush.
        /// </summary>
        public LogicalBrush Copy()
        {
            return new LogicalBrush
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

        #endregion
    }
}