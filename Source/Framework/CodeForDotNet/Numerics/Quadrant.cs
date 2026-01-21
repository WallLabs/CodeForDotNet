namespace CodeForDotNet.Numerics
{
	/// <summary>
	/// Quadrant of an angle.
	/// </summary>
	public enum Quadrant
	{
		/// <summary>
		/// First quadrant from 0 to 89 degrees or viewed as "top right".
		/// </summary>
		First = 1,

		/// <summary>
		/// Fourth quadrant from 90 to 179 degrees or viewed as "bottom right".
		/// </summary>
		Second = 2,

		/// <summary>
		/// Third quadrant from 180 to 269 degrees or viewed as "bottom left".
		/// </summary>
		Third = 3,

		/// <summary>
		/// Second quadrant from 270 to 359 degrees or viewed as "top left".
		/// </summary>
		Fourth = 4
	}
}
