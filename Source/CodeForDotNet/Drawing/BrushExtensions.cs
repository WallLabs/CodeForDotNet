using CodeForDotNet.Drawing;
using CodeForDotNet.Properties;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;

namespace CodeForDotNet.Drawing
{
    /// <summary>
    /// Extensions for work with <see cref="Brush"/> types.
    /// </summary>
    public static class BrushExtensions
    {
        #region Brush Conversion

        /// <summary>
        /// Creates a .NET brush based on the current brush specification.
        /// </summary>
        public static Brush MakeBrush(this BrushData data, Rectangle bounds)
        {
            return MakeBrush(data, bounds, false);
        }

        /// <summary>
        /// Creates a .NET brush based on the current brush specification,
        /// optionally flipping the start and end colors.
        /// </summary>
        public static Brush MakeBrush(this BrushData data, Rectangle bounds, bool reverse)
        {
            // Validate
            if (data == null) throw new ArgumentNullException(nameof(data));

            // Make brush (according to type)
            switch (data.BrushType)
            {
                case BrushFillType.SingleColor:
                    {
                        // Single Color
                        if (!data.StartColor.HasValue) throw new ArgumentNullException(
                            string.Format(CultureInfo.CurrentCulture, Resources.PropertyRequired, nameof(BrushData.StartColor)));
                        return new SolidBrush(Color.FromArgb(data.StartColor.Value));
                    }

                case BrushFillType.TwoColorGradient:
                    {
                        // Two color gradient
                        if (!data.StartColor.HasValue) throw new ArgumentNullException(
                            string.Format(CultureInfo.CurrentCulture, Resources.PropertyRequired, nameof(BrushData.StartColor)));
                        if (!data.EndColor.HasValue) throw new ArgumentNullException(
                            string.Format(CultureInfo.CurrentCulture, Resources.PropertyRequired, nameof(BrushData.EndColor)));
                        if (!data.Angle.HasValue) throw new ArgumentNullException(
                            string.Format(CultureInfo.CurrentCulture, Resources.PropertyRequired, nameof(BrushData.Angle)));
                        Color startColor, endColor;
                        if (reverse)
                        {
                            startColor = Color.FromArgb(data.EndColor.Value);
                            endColor = Color.FromArgb(data.StartColor.Value);
                        }
                        else
                        {
                            startColor = Color.FromArgb(data.StartColor.Value);
                            endColor = Color.FromArgb(data.EndColor.Value);
                        }
                        return new LinearGradientBrush(bounds, startColor,
                                                         endColor, decimal.ToSingle(data.Angle.Value));
                    }

                case BrushFillType.Texture:
                    {
                        // Texture
                        if (!data.Angle.HasValue) throw new ArgumentNullException(
                            string.Format(CultureInfo.CurrentCulture, Resources.PropertyRequired, nameof(BrushData.Angle)));
                        if (!data.WrapMode.HasValue) throw new ArgumentNullException(
                            string.Format(CultureInfo.CurrentCulture, Resources.PropertyRequired, nameof(BrushData.WrapMode)));
                        if (!data.Scale.HasValue) throw new ArgumentNullException(
                            string.Format(CultureInfo.CurrentCulture, Resources.PropertyRequired, nameof(BrushData.Scale)));

                        // Make texture
                        Image image;
                        using (var reader = new MemoryStream(data.Texture))
                            image = Image.FromStream(reader);
                        var brush = new TextureBrush(image, (WrapMode)(int)data.WrapMode.Value);

                        // Rotate
                        brush.RotateTransform(decimal.ToSingle(data.Angle.Value), MatrixOrder.Append);

                        // Scale image to fit
                        var scaleX = 1.0F;
                        if (image.Width > bounds.Width)
                            scaleX = bounds.Width / (float)image.Width;
                        var scaleY = 1.0F;
                        if (image.Height > bounds.Height)
                            scaleY = bounds.Height / (float)image.Height;
                        var fScale = 1.0F;
                        if ((scaleX < 1) || (scaleY < 1))
                            fScale = scaleX < scaleY ? scaleX : scaleY;

                        // Scale image
                        var fFinalScale = data.Scale.Value * fScale;
                        brush.ScaleTransform(fFinalScale, fFinalScale, MatrixOrder.Append);

                        // Return result
                        return brush;
                    }

                default:
                    // Unsupported type
                    throw new NotSupportedException();
            }
        }

        #endregion Brush Conversion
    }
}