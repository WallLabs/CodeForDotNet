using CodeForDotNet.Diagnostics;
using CodeForDotNet.Drawing;
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
            var font1 = new FontData("Verdana", 10, (int)FontStyle.Regular);
            var font1Xml = font1.SerializeXml();

            // De-serialize the font and check contents match
            var font2 = XmlSerializerExtensions.DeserializeXml<FontData>(font1Xml);
            Assert.AreEqual(font1, font2);

            // Serialize again and check the XML matches
            var font2Xml = font2.SerializeXml();
            Assert.AreEqual(font1Xml, font2Xml);
        }

        /// <summary>
        /// Tests the <see cref="Vector2"/> class.
        /// </summary>
        [TestMethod]
        public void DrawingTestVector2()
        {
            // Draw a circle in a bitmap using the vector class
            using (var canvas = new Bitmap(1001, 1001))
            {
                var vector = new Vector2(0, 500);
                using (var graphics = Graphics.FromImage(canvas))
                using (var path = new GraphicsPath())
                {
                    graphics.TranslateTransform(500, 500);
                    graphics.FillRectangle(Brushes.White, 0, 0, canvas.Width, canvas.Height);
                    path.StartFigure();
                    var lastPoint = vector.ToPointF();
                    do
                    {
                        var currentPoint = vector.ToPointF();
                        path.AddLine(lastPoint, currentPoint);
                        vector.Angle += 0.01f;
                        lastPoint = currentPoint;
                    } while (vector.Angle < 360);
                    path.CloseFigure();
                    graphics.DrawPath(Pens.Black, path);
                }
                canvas.Save("TestVector2.bmp");
            }
            ProcessExtensions.Run("mspaint.exe", "TestVector2.bmp", null, 5);
        }
    }
}