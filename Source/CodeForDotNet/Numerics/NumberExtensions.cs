using System;

namespace CodeForDotNet.Numerics
{
	/// <summary>
	/// Number extensions.
	/// </summary>
	public static class NumberExtensions
	{
		#region Public Methods

		/// <summary>
		/// Converts the number to a string of the specified base.
		/// </summary>
		public static string ToString(this long value, int numberBase)
		{
			return ToString(value, numberBase, 0);
		}

		/// <summary>
		/// Converts the number to a string of the specified base.
		/// </summary>
		public static string ToString(this long value, int numberBase, int minWidth)
		{
			return ((Number)value).ToString(numberBase, minWidth);
		}

		/// <summary>
		/// Converts the number to a string of the specified base.
		/// </summary>
		[CLSCompliant(false)]
		public static string ToString(this ulong value, int numberBase)
		{
			return ToString(value, numberBase, 0);
		}

		/// <summary>
		/// Converts the number to a string of the specified base.
		/// </summary>
		[CLSCompliant(false)]
		public static string ToString(this ulong value, int numberBase, int minWidth)
		{
			return ((Number)value).ToString(numberBase, minWidth);
		}

		#endregion Public Methods
	}
}
