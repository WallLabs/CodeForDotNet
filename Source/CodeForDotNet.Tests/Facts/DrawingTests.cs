using CodeForDotNet.Diagnostics;
using CodeForDotNet.Drawing;
using CodeForDotNet.Numerics;
using CodeForDotNet.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CodeForDotNet.Tests.Facts
{
    /// <summary>
    /// Tests classes in the drawing namespace.
    /// </summary>
    [TestClass]
    public class DrawingTests
    {
        /// <summary>
        /// Tests the <see cref="BrushData"/> XML serialization.
        /// </summary>
        [TestMethod]
        public void DrawingTestBrushDataXmlSerialize()
        {
            // Create a test object and serialize
            var brush1 = new BrushData(Color.Black, Color.Blue, 45m);
            var brush1Xml = brush1.SerializeXml();

            // De-serialize the brush and check contents match
            var brush2 = XmlSerializerExtensions.DeserializeXml<BrushData>(brush1Xml);
            Assert.AreEqual(brush1, brush2);

            // Serialize again and check the XML matches
            var brush2Xml = brush2.SerializeXml();
            Assert.AreEqual(brush1Xml, brush2Xml);
        }

        /// <summary>
        /// Tests the <see cref="BrushData"/> type conversion.
        /// </summary>
        [TestMethod]
        public void DrawingTestBrushDataConversion()
        {
            // Create a test brush
            var brush1 = new BrushData(Color.Yellow);

            // Convert to string
            var brush1String = brush1.ToString();

            // Convert from string
            var brush2 = BrushData.Parse(brush1String);

            // Validate result
            Assert.AreEqual(brush1, brush2);
        }

        /// <summary>
        /// Tests the <see cref="FontData"/> XML serialization.
        /// </summary>
        [TestMethod]
        public void DrawingTestFontDataXmlSerialize()
        {
            // Create a test object and serialize
            var font1 = new FontData("Verdana", 10, (int)System.Drawing.FontStyle.Regular);
            var font1Xml = font1.SerializeXml();

            // De-serialize the font and check contents match
            var font2 = XmlSerializerExtensions.DeserializeXml<FontData>(font1Xml);
            Assert.AreEqual(font1, font2);

            // Serialize again and check the XML matches
            var font2Xml = font2.SerializeXml();
            Assert.AreEqual(font1Xml, font2Xml);
        }

		/// <summary>
		/// Tests the <see cref="AngleVector2"/> class.
		/// </summary>
		/// <remarks>
		/// <list type="number">
		/// <listheader>Expected Results</listheader>
		/// <item>
		/// A bitmap file is generated then displayed showing the results of the drawing test.
		/// </item>
		///	<item>
		/// A red circle is drawn using the vector class to convert angle vectors to points.
		/// If the calculation is correct, with a constant length with an increasing angle
		/// then a perfect circle should be drawn.
		///	</item>
		///	<item>
		/// A green circle is drawn using the resulting points from the first angle conversion
		/// round-tripped back to an angle then point. If the calculation is accurate,
		/// the red circle will be completely hidden. Calculation errors would hence be
		/// shown in red.
		///	</item>
		///	<item>
		///	In testing this calculation has prooven to be 100% round-trip capable at least visibly,
		///	meaning only a green circle should be visible without any red pixels.
		///	</item>
		/// </list>
		/// </remarks>
		[TestMethod]
        public void DrawingTestAngleVector2()
        {
			// Create drawing.
			using (var canvas = new Bitmap(720, 720))
            {
                using (var graphics = Graphics.FromImage(canvas))
                using (var angleToPointCircle = new GraphicsPath())
				using (var pointToAngleCircle = new GraphicsPath())
				{
					// Set origin to center.
					graphics.TranslateTransform(359.5f, 359.5f);

					// Fill background.
                    graphics.FillRectangle(Brushes.White, 0, 0, canvas.Width, canvas.Height);

					// Start drawing.
                    angleToPointCircle.StartFigure();
					pointToAngleCircle.StartFigure();
					var vector = new AngleVector2(0, 359.5f);
					var lastPoint = vector.ToPointF();
					var lastRoundTripPoint = new AngleVector2(lastPoint).ToPointF();
                    do
                    {
						// Calculate current positions.
                        var currentPoint = vector.ToPointF();
						var currentRoundTripPoint = new AngleVector2(currentPoint).ToPointF();

						// Draw position.
                        angleToPointCircle.AddLine(lastPoint, currentPoint);
						pointToAngleCircle.AddLine(lastRoundTripPoint, currentRoundTripPoint);

						// Next...
						vector.Angle += 0.01f;
						lastPoint = currentPoint;
						lastRoundTripPoint = currentRoundTripPoint;
                    }
					while (vector.Angle < 360);

					// End drawing.
                    angleToPointCircle.CloseFigure();
					pointToAngleCircle.CloseFigure();
                    graphics.DrawPath(Pens.Red, angleToPointCircle);
					graphics.DrawPath(Pens.Green, pointToAngleCircle);
                }

				// Save to bitmap.
                canvas.Save("TestAngleVector2.bmp");
            }

			// Display bitmap.
            ProcessExtensions.Run("mspaint.exe", "TestAngleVector2.bmp", timeout: 5);
        }
    }
}
