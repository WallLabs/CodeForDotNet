namespace CodeForDotNet.Drawing
{
	/// <summary>
	/// View quadrant, used for layout. Quadrants go clockwise from 12 o'clock so they can be mapped to 90 degree portions of 360 degrees. i.e. can be used to
	/// translate an angle into a quadrant.
	/// </summary>
	public enum Quadrant
	{
		/// <summary>
		/// 0 to 89 degrees
		/// </summary>
		TopRight,

		/// <summary>
		/// 90 to 179 degrees
		/// </summary>
		BottomRight,

		/// <summary>
		/// 180 to 269 degrees
		/// </summary>
		BottomLeft,

		/// <summary>
		/// 270 to 359 degrees
		/// </summary>
		TopLeft
	}
}
