using System.Drawing;

namespace CodeForDotNet.Drawing
{
	/// <summary>
	/// Provides useful extension methods to the <see cref="Point"/> class.
	/// </summary>
	public static class PointExtensions
	{
		#region Public Methods

		/// <summary>
		/// Adds two <see cref="Point"/> s together.
		/// </summary>
		public static Point Add(this Point point1, Point point2)
		{
			return new Point(point1.X + point2.X, point1.Y + point2.Y);
		}

		/// <summary>
		/// Adds a <see cref="PointF"/> to a PointF.
		/// </summary>
		public static PointF Add(this PointF point1, PointF point2)
		{
			return new PointF(point1.X + point2.X, point1.Y + point2.Y);
		}

		/// <summary>
		/// Subtracts one <see cref="Point"/> from another.
		/// </summary>
		public static Point Subtract(this Point point1, Point point2)
		{
			return new Point(point1.X - point2.X, point1.Y - point2.Y);
		}

		/// <summary>
		/// Subtracts a <see cref="PointF"/> from a <see cref="PointF"/>.
		/// </summary>
		public static PointF Subtract(this PointF point1, PointF point2)
		{
			return new PointF(point1.X - point2.X, point1.Y - point2.Y);
		}

		#endregion Public Methods
	}
}
