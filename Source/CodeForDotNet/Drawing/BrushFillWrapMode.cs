namespace CodeForDotNet.Drawing
{
	/// <summary>
	/// Defines a texture or gradient wrapping mode agnostic to any API for future-proof data serialization and cross-platform.
	/// </summary>
	public enum BrushFillWrapMode
	{
		/// <summary>
		/// The texture or gradient is not tiled.
		/// </summary>
		Clamp,

		/// <summary>
		/// Tiles the gradient or texture.
		/// </summary>
		Tile,

		/// <summary>
		/// Reverses the texture or gradient horizontally and then tiles the texture or gradient.
		/// </summary>
		TileFlipX,

		/// <summary>
		/// Reverses the texture or gradient horizontally and vertically and then tiles the texture or gradient.
		/// </summary>
		TileFlipXY,

		/// <summary>
		/// Reverses the texture or gradient vertically and then tiles the texture or gradient.
		/// </summary>
		TileFlipY
	}
}
