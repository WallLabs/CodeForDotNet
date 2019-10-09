using CodeForDotNet.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace CodeForDotNet.Tests.Facts
{
	/// <summary>
	/// Tests the numerics classes and extension.
	/// </summary>
	[TestClass]
	public class NumericsFacts
	{
		#region Public Methods

		/// <summary>
		/// Tests the <see cref="Numerics.Number.Add"/> method and operator.
		/// </summary>
		[TestMethod]
		public void NumberTestAdd()
		{
			// Test simple addition
			var one = (Number)1;
			var two = one + one;
			Assert.AreEqual(2, two);
			Assert.AreEqual(3, two + one);
			Assert.AreEqual(4, two + two);

			// Test addition with negative
			var minusOne = -(Number)1;
			Assert.AreEqual(-1, minusOne);
			var zero = one + minusOne;
			Assert.AreEqual(0, zero);
			Assert.IsTrue(zero.IsZero);
			var minusTwo = minusOne + minusOne;
			Assert.AreEqual(-2, minusTwo);
			Assert.AreEqual(-1, zero + minusOne);
			Assert.AreEqual(-4, minusTwo + minusTwo);

			// Test addition of unsigned values with overflow carried
			var unsignedMax = new Number((ushort)0xFFFF);
			Assert.IsFalse(unsignedMax.Signed);
			Assert.IsTrue(unsignedMax.Sign);
			var unsignedPositiveOverflow = unsignedMax + unsignedMax;
			Assert.IsFalse(unsignedPositiveOverflow.Signed);
			Assert.IsTrue(unsignedPositiveOverflow.Sign);
			Assert.AreEqual(3, unsignedPositiveOverflow.ByteSize);
			Assert.AreEqual(new Number((uint)0x0001FFFE), unsignedPositiveOverflow);

			// Test addition of signed values with overflow carried
			var signedPositiveMax = new Number((short)0x7FFF);
			Assert.IsTrue(signedPositiveMax.Signed);
			Assert.IsTrue(signedPositiveMax.Sign);
			var signedPositiveOverflow = signedPositiveMax + signedPositiveMax;
			Assert.IsTrue(signedPositiveOverflow.Signed);
			Assert.IsTrue(signedPositiveOverflow.Sign);
			Assert.AreEqual(3, signedPositiveOverflow.ByteSize);
			Assert.AreEqual(new Number(0x0000FFFE), signedPositiveOverflow);
			var signedNegativeMax = new Number(unchecked((short)0x8001));
			Assert.IsTrue(signedNegativeMax.Signed);
			Assert.IsFalse(signedNegativeMax.Sign);
			var signedNegativeOverflow = signedNegativeMax + signedNegativeMax;
			Assert.IsTrue(signedNegativeOverflow.Signed);
			Assert.IsFalse(signedNegativeOverflow.Sign);
			Assert.AreEqual(3, signedNegativeOverflow.ByteSize);
			Assert.AreEqual(new Number(unchecked((int)0xFFFF0002)), signedNegativeOverflow);
		}

		/// <summary>
		/// Tests the <see cref="Numerics.Number.CompareTo"/> method.
		/// </summary>
		[TestMethod]
		public void NumberTestCompareTo()
		{
			// Test positive and negative ones and zeros
			Assert.AreEqual(0, Number.Zero.CompareTo(Number.Zero));
			Assert.AreEqual(-1, Number.Zero.CompareTo(Number.One));
			Assert.AreEqual(1, Number.Zero.CompareTo(Number.MinusOne));
			Assert.AreEqual(1, Number.One.CompareTo(Number.Zero));
			Assert.AreEqual(0, Number.One.CompareTo(Number.One));
			Assert.AreEqual(1, Number.One.CompareTo(Number.MinusOne));
			Assert.AreEqual(-1, Number.MinusOne.CompareTo(Number.Zero));
			Assert.AreEqual(-1, Number.MinusOne.CompareTo(Number.One));
			Assert.AreEqual(0, Number.MinusOne.CompareTo(Number.MinusOne));

			// Test most significant bytes used for comparison
			var number123Signed = new Number(new byte[] { 3, 2, 1 }, true);
			var number321Signed = new Number(new byte[] { 1, 2, 3 }, true);
			var number123Unsigned = new Number(new byte[] { 3, 2, 1 }, false);
			var number321Unsigned = new Number(new byte[] { 1, 2, 3 }, false);
			Assert.AreEqual(0, number123Signed.CompareTo(number123Signed));
			Assert.AreEqual(0, number123Signed.CompareTo(number123Unsigned));
			Assert.AreEqual(0, number321Signed.CompareTo(number321Signed));
			Assert.AreEqual(0, number321Signed.CompareTo(number321Unsigned));
			Assert.AreEqual(-1, number123Signed.CompareTo(number321Signed));
			Assert.AreEqual(-1, number123Signed.CompareTo(number321Unsigned));
			Assert.AreEqual(1, number321Signed.CompareTo(number123Signed));
			Assert.AreEqual(1, number321Signed.CompareTo(number123Unsigned));

			// Test comparison of different length values
			var number123SignedLong = new Number(new byte[] { 3, 2, 1, 0, 0, 0, 0 }, true);
			var number123UnsignedLong = new Number(new byte[] { 3, 2, 1, 0, 0, 0, 0 }, false);
			var number321SignedLong = new Number(new byte[] { 1, 2, 3, 0, 0, 0, 0 }, true);
			var number321UnsignedLong = new Number(new byte[] { 1, 2, 3, 0, 0, 0, 0 }, false);
			Assert.AreEqual(0, number123SignedLong.CompareTo(number123Signed));
			Assert.AreEqual(0, number123SignedLong.CompareTo(number123Unsigned));
			Assert.AreEqual(0, number123UnsignedLong.CompareTo(number123Signed));
			Assert.AreEqual(0, number123UnsignedLong.CompareTo(number123Unsigned));
			Assert.AreEqual(0, number321SignedLong.CompareTo(number321Signed));
			Assert.AreEqual(0, number321SignedLong.CompareTo(number321Unsigned));
			Assert.AreEqual(0, number321UnsignedLong.CompareTo(number321Signed));
			Assert.AreEqual(0, number321UnsignedLong.CompareTo(number321Unsigned));
			Assert.AreEqual(-1, number123SignedLong.CompareTo(number321Signed));
			Assert.AreEqual(-1, number123SignedLong.CompareTo(number321Unsigned));
			Assert.AreEqual(-1, number123UnsignedLong.CompareTo(number321Signed));
			Assert.AreEqual(-1, number123UnsignedLong.CompareTo(number321Unsigned));
			Assert.AreEqual(1, number321SignedLong.CompareTo(number123Signed));
			Assert.AreEqual(1, number321SignedLong.CompareTo(number123Unsigned));
			Assert.AreEqual(1, number321UnsignedLong.CompareTo(number123Signed));
			Assert.AreEqual(1, number321UnsignedLong.CompareTo(number123Unsigned));
		}

		/// <summary>
		/// Tests the <see cref="Numerics.Number"/> comparison operators (&gt; &gt;= == &lt;= &lt;).
		/// </summary>
		[TestMethod]
		public void NumberTestComparisonOperators()
		{
			// Avoid warning of constant result
			var otherZero = Number.Zero;

			// Test less than
			Assert.IsFalse(Number.Zero < Number.MinusOne);
			Assert.IsFalse(Number.Zero < otherZero);
			Assert.IsTrue(Number.Zero < Number.One);

			// Test less than or equal
			Assert.IsFalse(Number.Zero <= Number.MinusOne);
			Assert.IsTrue(Number.Zero <= otherZero);
			Assert.IsTrue(Number.Zero <= Number.One);

			// Test equal
			Assert.IsFalse(Number.Zero == Number.MinusOne);
			Assert.IsTrue(Number.Zero == otherZero);
			Assert.IsFalse(Number.Zero == Number.One);

			// Test greater than or equal
			Assert.IsTrue(Number.Zero >= Number.MinusOne);
			Assert.IsTrue(Number.Zero >= otherZero);
			Assert.IsFalse(Number.Zero >= Number.One);

			// Test greater than
			Assert.IsTrue(Number.Zero > Number.MinusOne);
			Assert.IsFalse(Number.Zero > otherZero);
			Assert.IsFalse(Number.Zero > Number.One);
		}

		/// <summary>
		/// Tests the various <see cref="Numerics.Number"/> constructors.
		/// </summary>
		[TestMethod]
		public void NumberTestConstructors()
		{
			// Default constructor
			var number = new Number();
			Assert.IsTrue(number.IsZero);
			Assert.IsFalse(number.Signed);
			Assert.AreEqual(0, number.ByteSize);

			// Byte array constructor
			number = new Number(new byte[] { 1 }, true);
			Assert.IsFalse(number.IsZero);
			Assert.IsTrue(number.Signed);
			Assert.IsTrue(number.Sign);
			Assert.AreEqual(1, number.ByteSize);
			Assert.AreEqual(number[0], 1);
			number = new Number(new byte[] { 0xff }, true);
			Assert.IsFalse(number.IsZero);
			Assert.IsTrue(number.Signed);
			Assert.IsFalse(number.Sign);
			Assert.AreEqual(1, number.ByteSize);
			Assert.AreEqual(number[0], 0xff);
			number = new Number(new byte[] { 0xff }, false);
			Assert.IsFalse(number.IsZero);
			Assert.IsFalse(number.Signed);
			Assert.AreEqual(1, number.ByteSize);
			Assert.AreEqual(number[0], 0xff);

			// Decimal constructor
			number = new Number(1m);
			Assert.IsFalse(number.IsZero);
			Assert.IsTrue(number.Signed);
			Assert.IsTrue(number.Sign);
			Assert.AreEqual(16, number.ByteSize);
			Assert.AreEqual(number[0], 1);
			Assert.AreEqual(number[1], 0);
			Assert.AreEqual(number[2], 0);
			Assert.AreEqual(number[3], 0);
			Assert.AreEqual(number[4], 0);
			Assert.AreEqual(number[5], 0);
			Assert.AreEqual(number[6], 0);
			Assert.AreEqual(number[7], 0);
			Assert.AreEqual(number[8], 0);
			Assert.AreEqual(number[9], 0);
			Assert.AreEqual(number[10], 0);
			Assert.AreEqual(number[11], 0);
			Assert.AreEqual(number[12], 0);
			Assert.AreEqual(number[13], 0);
			Assert.AreEqual(number[14], 0);
			Assert.AreEqual(number[15], 0);
			number = new Number(-1m);
			Assert.IsFalse(number.IsZero);
			Assert.IsTrue(number.Signed);
			Assert.IsFalse(number.Sign);
			Assert.AreEqual(16, number.ByteSize);
			Assert.AreEqual(number[0], 0xff);
			Assert.AreEqual(number[1], 0xff);
			Assert.AreEqual(number[2], 0xff);
			Assert.AreEqual(number[3], 0xff);
			Assert.AreEqual(number[4], 0xff);
			Assert.AreEqual(number[5], 0xff);
			Assert.AreEqual(number[6], 0xff);
			Assert.AreEqual(number[7], 0xff);
			Assert.AreEqual(number[8], 0xff);
			Assert.AreEqual(number[9], 0xff);
			Assert.AreEqual(number[10], 0xff);
			Assert.AreEqual(number[11], 0xff);
			Assert.AreEqual(number[12], 0xff);
			Assert.AreEqual(number[13], 0xff);
			Assert.AreEqual(number[14], 0xff);
			Assert.AreEqual(number[15], 0xff);

			// Double constructor
			number = new Number((double)1);
			Assert.IsFalse(number.IsZero);
			Assert.IsTrue(number.Signed);
			Assert.IsTrue(number.Sign);
			Assert.AreEqual(8, number.ByteSize);
			Assert.AreEqual(number[0], 1);
			Assert.AreEqual(number[1], 0);
			Assert.AreEqual(number[2], 0);
			Assert.AreEqual(number[3], 0);
			Assert.AreEqual(number[4], 0);
			Assert.AreEqual(number[5], 0);
			Assert.AreEqual(number[6], 0);
			Assert.AreEqual(number[7], 0);
			number = new Number((double)-1);
			Assert.IsFalse(number.IsZero);
			Assert.IsTrue(number.Signed);
			Assert.IsFalse(number.Sign);
			Assert.AreEqual(8, number.ByteSize);
			Assert.AreEqual(number[0], 0xff);
			Assert.AreEqual(number[1], 0xff);
			Assert.AreEqual(number[2], 0xff);
			Assert.AreEqual(number[3], 0xff);
			Assert.AreEqual(number[4], 0xff);
			Assert.AreEqual(number[5], 0xff);
			Assert.AreEqual(number[6], 0xff);
			Assert.AreEqual(number[7], 0xff);

			// Single constructor
			number = new Number(1f);
			Assert.IsFalse(number.IsZero);
			Assert.IsTrue(number.Signed);
			Assert.IsTrue(number.Sign);
			Assert.AreEqual(4, number.ByteSize);
			Assert.AreEqual(number[0], 1);
			Assert.AreEqual(number[1], 0);
			Assert.AreEqual(number[2], 0);
			Assert.AreEqual(number[3], 0);
			number = new Number(-1f);
			Assert.IsFalse(number.IsZero);
			Assert.IsTrue(number.Signed);
			Assert.IsFalse(number.Sign);
			Assert.AreEqual(4, number.ByteSize);
			Assert.AreEqual(number[0], 0xff);
			Assert.AreEqual(number[1], 0xff);
			Assert.AreEqual(number[2], 0xff);
			Assert.AreEqual(number[3], 0xff);

			// Int64 constructor
			number = new Number((long)1);
			Assert.IsFalse(number.IsZero);
			Assert.IsTrue(number.Signed);
			Assert.IsTrue(number.Sign);
			Assert.AreEqual(8, number.ByteSize);
			Assert.AreEqual(number[0], 1);
			Assert.AreEqual(number[1], 0);
			Assert.AreEqual(number[2], 0);
			Assert.AreEqual(number[3], 0);
			Assert.AreEqual(number[4], 0);
			Assert.AreEqual(number[5], 0);
			Assert.AreEqual(number[6], 0);
			Assert.AreEqual(number[7], 0);
			number = new Number((long)-1);
			Assert.IsFalse(number.IsZero);
			Assert.IsTrue(number.Signed);
			Assert.IsFalse(number.Sign);
			Assert.AreEqual(8, number.ByteSize);
			Assert.AreEqual(number[0], 0xff);
			Assert.AreEqual(number[1], 0xff);
			Assert.AreEqual(number[2], 0xff);
			Assert.AreEqual(number[3], 0xff);
			Assert.AreEqual(number[4], 0xff);
			Assert.AreEqual(number[5], 0xff);
			Assert.AreEqual(number[6], 0xff);
			Assert.AreEqual(number[7], 0xff);

			// Int32 constructor
			number = new Number(1);
			Assert.IsFalse(number.IsZero);
			Assert.IsTrue(number.Signed);
			Assert.IsTrue(number.Sign);
			Assert.AreEqual(4, number.ByteSize);
			Assert.AreEqual(number[0], 1);
			Assert.AreEqual(number[1], 0);
			Assert.AreEqual(number[2], 0);
			Assert.AreEqual(number[3], 0);
			number = new Number(-1);
			Assert.IsFalse(number.IsZero);
			Assert.IsTrue(number.Signed);
			Assert.IsFalse(number.Sign);
			Assert.AreEqual(4, number.ByteSize);
			Assert.AreEqual(number[0], 0xff);
			Assert.AreEqual(number[1], 0xff);
			Assert.AreEqual(number[2], 0xff);
			Assert.AreEqual(number[3], 0xff);

			// Int16 constructor
			number = new Number((short)1);
			Assert.IsFalse(number.IsZero);
			Assert.IsTrue(number.Signed);
			Assert.IsTrue(number.Sign);
			Assert.AreEqual(2, number.ByteSize);
			Assert.AreEqual(number[0], 1);
			Assert.AreEqual(number[1], 0);
			number = new Number((short)-1);
			Assert.IsFalse(number.IsZero);
			Assert.IsTrue(number.Signed);
			Assert.IsFalse(number.Sign);
			Assert.AreEqual(2, number.ByteSize);
			Assert.AreEqual(number[0], 0xff);
			Assert.AreEqual(number[1], 0xff);

			// UInt64 constructor
			number = new Number((ulong)1);
			Assert.IsFalse(number.IsZero);
			Assert.IsFalse(number.Signed);
			Assert.AreEqual(8, number.ByteSize);
			Assert.AreEqual(number[0], 1);
			Assert.AreEqual(number[1], 0);
			Assert.AreEqual(number[2], 0);
			Assert.AreEqual(number[3], 0);
			Assert.AreEqual(number[4], 0);
			Assert.AreEqual(number[5], 0);
			Assert.AreEqual(number[6], 0);
			Assert.AreEqual(number[7], 0);

			// UInt32 constructor
			number = new Number((uint)1);
			Assert.IsFalse(number.IsZero);
			Assert.IsFalse(number.Signed);
			Assert.AreEqual(4, number.ByteSize);
			Assert.AreEqual(number[0], 1);
			Assert.AreEqual(number[1], 0);
			Assert.AreEqual(number[2], 0);
			Assert.AreEqual(number[3], 0);

			// Int16 constructor
			number = new Number((ushort)1);
			Assert.IsFalse(number.IsZero);
			Assert.IsFalse(number.Signed);
			Assert.AreEqual(2, number.ByteSize);
			Assert.AreEqual(number[0], 1);
			Assert.AreEqual(number[1], 0);
		}

		/// <summary>
		/// Tests the conversion to and from strings of different bases.
		/// </summary>
		[TestMethod]
		public void NumberTestConvertStringAndBase()
		{
			// Test big integers (positive, negative and unsigned)
			Number number = 255;
			Assert.AreEqual("00FF", number.ToString(16));
			Assert.AreEqual("00FF", number.ToString(16, 4));
			Assert.AreEqual("0000FF", number.ToString(16, 5));                      // Minimum length rounded-up to nearest byte when bit based
			Assert.AreEqual("255", number.ToString(10));
			Assert.AreEqual("0000000011111111", number.ToString(2));
			Assert.AreEqual("0000000011111111", number.ToString(2, 16));
			Assert.AreEqual("000000000000000011111111", number.ToString(2, 17));    // Minimum length rounded-up to nearest byte when bit based
			Assert.IsTrue(Number.TryParse("00FF", 16, out number));
			Assert.AreEqual(255, number);
			Assert.IsTrue(Number.TryParse("FF", 16, out number));
			Assert.AreEqual(-1, number);
			Assert.IsTrue(Number.TryParse("255", 10, out number));
			Assert.AreEqual(255, number);
			Assert.IsTrue(Number.TryParse("-1", 10, out number));
			Assert.AreEqual(-1, number);
			Assert.IsTrue(Number.TryParse("11111111", 2, out number, false));
			Assert.AreEqual(255, number);
			Assert.IsTrue(Number.TryParse("11111111", 2, out number, true));
			Assert.AreEqual(-1, number);
			number = (sbyte)-1;
			Assert.AreEqual("FF", number.ToString(16));
			Assert.AreEqual("FFFF", number.ToString(16, 4));
			Assert.AreEqual("FFFFFF", number.ToString(16, 5));                      // Minimum length rounded-up to nearest byte when bit based
			Assert.AreEqual("-1", number.ToString(10));
			Assert.AreEqual("11111111", number.ToString(2));
			Assert.AreEqual("1111111111111111", number.ToString(2, 16));
			Assert.AreEqual("111111111111111111111111", number.ToString(2, 17));    // Minimum length rounded-up to nearest byte when bit based
			Assert.IsTrue(Number.TryParse("FF", 16, out number, true));
			Assert.AreEqual(-1, number);
			Assert.IsTrue(Number.TryParse("-1", 10, out number, true));
			Assert.AreEqual(-1, number);
			Assert.IsTrue(Number.TryParse("11111111", 2, out number, true));
			Assert.AreEqual(-1, number);

			// Test signed longs (positive and negative)
			var signedLong = 255L;
			Assert.AreEqual("00FF", signedLong.ToString(16));
			Assert.AreEqual("00FF", signedLong.ToString(16, 4));
			Assert.AreEqual("0000FF", signedLong.ToString(16, 5));                      // Minimum length rounded-up to nearest byte when bit based
			Assert.AreEqual("255", signedLong.ToString(10));
			Assert.AreEqual("0000000011111111", signedLong.ToString(2));
			Assert.AreEqual("0000000011111111", signedLong.ToString(2, 16));
			Assert.AreEqual("000000000000000011111111", signedLong.ToString(2, 17));    // Minimum length rounded-up to nearest byte when bit based
			Assert.IsTrue(Number.TryParse("0FF", 16, out number));
			Assert.AreEqual(255, number);
			Assert.IsTrue(Number.TryParse("255", 10, out number));
			Assert.AreEqual(255, number);
			Assert.IsTrue(Number.TryParse("0000000011111111", 2, out number));
			Assert.AreEqual(255, number);
			signedLong = -1;
			Assert.AreEqual("FFFFFFFFFFFFFFFF", signedLong.ToString(16));
			Assert.AreEqual("FFFFFFFFFFFFFFFF", signedLong.ToString(16, 4));            // Minimum length cannot be less than underlying size
			Assert.AreEqual("FFFFFFFFFFFFFFFFFFFF", signedLong.ToString(16, 20));       // ...but it can be greater
			Assert.AreEqual("-1", signedLong.ToString(10));
			Assert.AreEqual("1111111111111111111111111111111111111111111111111111111111111111", signedLong.ToString(2));
			Assert.AreEqual("1111111111111111111111111111111111111111111111111111111111111111", signedLong.ToString(2, 8));           // Minimum length cannot be less than underlying size
			Assert.AreEqual("111111111111111111111111111111111111111111111111111111111111111111111111", signedLong.ToString(2, 65));   // ...but it can be greater
			Assert.IsTrue(Number.TryParse("FF", 16, out number));
			Assert.AreEqual(-1, number);
			Assert.IsTrue(Number.TryParse("-1", 10, out number));
			Assert.AreEqual(-1, number);
			Assert.IsTrue(Number.TryParse("11111111", 2, out number));
			Assert.AreEqual(-1, number);

			// Test unsigned longs
			const ulong unsignedLong = 255UL;
			Assert.AreEqual("FF", unsignedLong.ToString(16));
			Assert.AreEqual("00FF", unsignedLong.ToString(16, 4));
			Assert.AreEqual("0000FF", unsignedLong.ToString(16, 5));            // Minimum length rounded-up to nearest byte when bit based
			Assert.AreEqual("255", unsignedLong.ToString(10));
			Assert.AreEqual("11111111", unsignedLong.ToString(2));
			Assert.AreEqual("11111111", unsignedLong.ToString(2, 4));           // Minimum length cannot be less than underlying size
			Assert.AreEqual("0000000011111111", unsignedLong.ToString(2, 16));  // ...but it can be greater
			Assert.IsTrue(Number.TryParse("FF", 16, out number, false));
			Assert.AreEqual((ulong)255, number);
			Assert.IsTrue(Number.TryParse("255", 10, out number, false));
			Assert.AreEqual((ulong)255, number);
			Assert.IsTrue(Number.TryParse("11111111", 2, out number, false));
			Assert.AreEqual((ulong)255, number);

			// Test invalid characters
			Assert.IsFalse(Number.TryParse("980&'=%", 16, out number));
			Assert.AreEqual(0, number);
			Assert.IsFalse(Number.TryParse("980&'=%", 10, out number));
			Assert.AreEqual(0, number);
			Assert.IsFalse(Number.TryParse("980&'=%", 2, out number));
			Assert.AreEqual(0, number);
		}

		/// <summary>
		/// Tests the <see cref="Numerics.Number.Divide(Numerics.Number, Numerics.Number, out Numerics.Number)"/> method and operator.
		/// </summary>
		[TestMethod]
		[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Test.")]
		public void NumberTestDivide()
		{
			// Test simple division
			var zero = Number.Zero;
			var one = (Number)1;
			var two = (Number)2;
			try { zero = one / zero; }
			catch (DivideByZeroException) {/* Successful */}
			Assert.AreEqual(1, one / one);
			Assert.AreEqual(2, one / two);
			Assert.AreEqual(1, two / two);
			Assert.AreEqual(12, (Number)624 / 52);

			// Test division of negative
			var minusOne = -(Number)1;
			Assert.AreEqual(-1, one / minusOne);
			Assert.AreEqual(-1, minusOne / one);
			try { zero = minusOne / zero; }
			catch (System.DivideByZeroException) {/* Successful */}
			Assert.AreEqual(1, minusOne / minusOne);

			// Test division with remainder
			Assert.AreEqual(5, Number.Divide(26, 5, out var remainder));
			Assert.AreEqual(1, remainder);
		}

		/// <summary>
		/// Tests the <see cref="Numerics.Number.LeftShift"/> method and operator.
		/// </summary>
		[TestMethod]
		public void NumberTestLeftShift()
		{
			Assert.AreEqual(0xffffffff, new Number(0xffffffff) << 0);
			Assert.AreEqual(0x01fffffffe, new Number(0xffffffff) << 1);
			Assert.AreEqual(0x03fffffffc, new Number(0xffffffff) << 2);
			Assert.AreEqual(0x07fffffff8, new Number(0xffffffff) << 3);
			Assert.AreEqual(0x0ffffffff0, new Number(0xffffffff) << 4);
			Assert.AreEqual(0x1fffffffe0, new Number(0xffffffff) << 5);
			Assert.AreEqual(0x3fffffffc0, new Number(0xffffffff) << 6);
			Assert.AreEqual(0x7fffffff80, new Number(0xffffffff) << 7);
			Assert.AreEqual(0xffffffff00, new Number(0xffffffff) << 8);
			Assert.AreEqual(0x01fffffffe00, new Number(0xffffffff) << 9);
			Assert.AreEqual(0x03fffffffc00, new Number(0xffffffff) << 10);
			Assert.AreEqual(0x07fffffff800, new Number(0xffffffff) << 11);
			Assert.AreEqual(0x0ffffffff000, new Number(0xffffffff) << 12);
			Assert.AreEqual(0x1fffffffe000, new Number(0xffffffff) << 13);
			Assert.AreEqual(0x3fffffffc000, new Number(0xffffffff) << 14);
			Assert.AreEqual(0x7fffffff8000, new Number(0xffffffff) << 15);
			Assert.AreEqual(0xffffffff0000, new Number(0xffffffff) << 16);
			Assert.AreEqual(0x0ffffffff00000, new Number(0xffffffff) << 20);
			Assert.AreEqual(0xffffffff000000, new Number(0xffffffff) << 24);
			Assert.AreEqual(0x0ffffffff0000000, new Number(0xffffffff) << 28);
			Assert.AreEqual(0xffffffff00000000, new Number(0xffffffff) << 32);
		}

		/// <summary>
		/// Tests the <see cref="Numerics.Number.Multiply"/> method and operator.
		/// </summary>
		[TestMethod]
		public void NumberTestMultiply()
		{
			// Test simple multiplication
			var zero = Number.Zero;
			var one = (Number)1;
			var two = (Number)2;
			Assert.AreEqual(0, one * zero);
			Assert.AreEqual(1, one * one);
			Assert.AreEqual(2, one * two);
			Assert.AreEqual(4, two * two);
			Assert.AreEqual(624, (Number)12 * 52);

			// Test multiplication of negative
			var minusOne = -(Number)1;
			Assert.AreEqual(-1, one * minusOne);
			Assert.AreEqual(-1, minusOne * one);
			Assert.AreEqual(0, minusOne * zero);
			Assert.AreEqual(1, minusOne * minusOne);

			// Test multiplication of unsigned values with overflow carried
			var positiveOverflow1 = new Number((ushort)0xFFFF);
			Assert.IsFalse(positiveOverflow1.Signed);
			Assert.IsTrue(positiveOverflow1.Sign);
			var positiveOverflow2 = new Number((ushort)0xFFFF);
			Assert.IsFalse(positiveOverflow2.Signed);
			Assert.IsTrue(positiveOverflow2.Sign);
			var positiveOverflowResult = positiveOverflow1 * positiveOverflow2;
			Assert.IsFalse(positiveOverflowResult.Signed);
			Assert.IsTrue(positiveOverflowResult.Sign);
			Assert.AreEqual(4, positiveOverflowResult.ByteSize);
			Assert.AreEqual(new Number(0xFFFE0001), positiveOverflowResult);
		}

		/// <summary>
		/// Tests the <see cref="Numerics.Number.Negate"/> method and operator.
		/// </summary>
		[TestMethod]
		public void NumberTestNegate()
		{
			// Test negate
			var one = new Number(1);
			var minusOne = -one;
			Assert.IsTrue(minusOne.Signed);
			Assert.IsFalse(minusOne.Sign);
			Assert.AreEqual(one.ByteSize, minusOne.ByteSize);
			Assert.AreEqual(-1, minusOne);
			var unsignedMax = new Number((ushort)0xFFFF);
			var negativeOverflow = -unsignedMax;
			Assert.IsTrue(negativeOverflow.Signed);
			Assert.IsFalse(negativeOverflow.Sign);
			Assert.AreEqual(unsignedMax.ByteSize + 1, negativeOverflow.ByteSize);
			Assert.AreEqual(unchecked((int)0xFFFF0001), negativeOverflow);
		}

		/// <summary>
		/// Tests the <see cref="Numerics.Number.Power"/> method.
		/// </summary>
		[TestMethod]
		public void NumberTestPower()
		{
			Assert.AreEqual(0, Number.Power(1, 0));
			Assert.AreEqual(1, Number.Power(1, 1));
			Assert.AreEqual(1, Number.Power(1, 2));
			Assert.AreEqual(1, Number.Power(1, 3));
			Assert.AreEqual(1, Number.Power(1, 4));
			Assert.AreEqual(1, Number.Power(1, 5));
			Assert.AreEqual(0, Number.Power(2, 0));
			Assert.AreEqual(2, Number.Power(2, 1));
			Assert.AreEqual(4, Number.Power(2, 2));
			Assert.AreEqual(8, Number.Power(2, 3));
			Assert.AreEqual(16, Number.Power(2, 4));
			Assert.AreEqual(32, Number.Power(2, 5));
			Assert.AreEqual(0, Number.Power(3, 0));
			Assert.AreEqual(3, Number.Power(3, 1));
			Assert.AreEqual(9, Number.Power(3, 2));
			Assert.AreEqual(27, Number.Power(3, 3));
			Assert.AreEqual(81, Number.Power(3, 4));
			Assert.AreEqual(243, Number.Power(3, 5));
		}

		/// <summary>
		/// Tests the <see cref="Numerics.Number.RightShift"/> method and operator.
		/// </summary>
		[TestMethod]
		public void NumberTestRightShift()
		{
			Assert.AreEqual(0xffffffff, new Number(0xffffffff) >> 0);
			Assert.AreEqual(0x7fffffff, new Number(0xffffffff) >> 1);
			Assert.AreEqual(0x3fffffff, new Number(0xffffffff) >> 2);
			Assert.AreEqual(0x1fffffff, new Number(0xffffffff) >> 3);
			Assert.AreEqual(0x0fffffff, new Number(0xffffffff) >> 4);
			Assert.AreEqual(0x07ffffff, new Number(0xffffffff) >> 5);
			Assert.AreEqual(0x03ffffff, new Number(0xffffffff) >> 6);
			Assert.AreEqual(0x01ffffff, new Number(0xffffffff) >> 7);
			Assert.AreEqual(0x00ffffff, new Number(0xffffffff) >> 8);
			Assert.AreEqual(0x007fffff, new Number(0xffffffff) >> 9);
			Assert.AreEqual(0x003fffff, new Number(0xffffffff) >> 10);
			Assert.AreEqual(0x001fffff, new Number(0xffffffff) >> 11);
			Assert.AreEqual(0x000fffff, new Number(0xffffffff) >> 12);
			Assert.AreEqual(0x0007ffff, new Number(0xffffffff) >> 13);
			Assert.AreEqual(0x0003ffff, new Number(0xffffffff) >> 14);
			Assert.AreEqual(0x0001ffff, new Number(0xffffffff) >> 15);
			Assert.AreEqual(0x0000ffff, new Number(0xffffffff) >> 16);
			Assert.AreEqual(0x00000fff, new Number(0xffffffff) >> 20);
			Assert.AreEqual(0x000000ff, new Number(0xffffffff) >> 24);
			Assert.AreEqual(0x0000000f, new Number(0xffffffff) >> 28);
			Assert.AreEqual(0x00000000, new Number(0xffffffff) >> 32);
			Assert.AreEqual(0x00000000, new Number(0xffffffff) >> 33);
			Assert.AreEqual(0x00000000, new Number(0xffffffff) >> 64);
		}

		/// <summary>
		/// Tests the <see cref="Number()"/> signed and unsigned effects.
		/// </summary>
		[TestMethod]
		public void NumberTestSign()
		{
			// Test comparison of signed and unsigned
			var signedMin = new Number(new byte[] { 0xff, 0xff, 0xff, 0xff }, true);
			var unsignedMax = new Number(new byte[] { 0xff, 0xff, 0xff, 0xff }, false);
			Assert.IsFalse(signedMin == unsignedMax);
			Assert.IsTrue(unsignedMax > signedMin);
		}

		/// <summary>
		/// Tests the <see cref="Numerics.Number.Subtract"/> method and operator.
		/// </summary>
		[TestMethod]
		public void NumberTestSubtract()
		{
			// Test simple subtraction
			var two = (Number)2;
			var one = two - 1;
			Assert.AreEqual(1, one);
			Assert.AreEqual(1, two - one);
			Assert.AreEqual(0, two - two);

			// Test subtraction with negative
			var minusOne = -(Number)1;
			Assert.AreEqual(-1, minusOne);
			var zero = minusOne + one;
			Assert.AreEqual(0, zero);
			Assert.IsTrue(zero.IsZero);
			var minusTwo = minusOne - 1;
			Assert.AreEqual(-2, minusTwo);
			Assert.AreEqual(-1, zero - one);
			Assert.AreEqual(1, zero - minusOne);
			Assert.AreEqual(-4, minusTwo - two);

			// Test subtraction of unsigned values with overflow
			var unsignedSmall = new Number((ushort)0x0102);
			Assert.IsFalse(unsignedSmall.Signed);
			Assert.IsTrue(unsignedSmall.Sign);
			var unsignedMax = new Number((ushort)0xFFFF);
			Assert.IsFalse(unsignedMax.Signed);
			Assert.IsTrue(unsignedMax.Sign);
			var unsignedNegativeOverflow = unsignedSmall - unsignedMax;
			Assert.IsTrue(unsignedNegativeOverflow.Signed);
			Assert.IsFalse(unsignedNegativeOverflow.Sign);
			Assert.AreEqual(3, unsignedNegativeOverflow.ByteSize);
			Assert.AreEqual(new Number(unchecked((int)0xFFFF0103)), unsignedNegativeOverflow);

			// Test subtraction of signed values with overflow carried
			var signedNegativeMax = new Number(unchecked((short)0x8001));
			Assert.IsTrue(signedNegativeMax.Signed);
			Assert.IsFalse(signedNegativeMax.Sign);
			var signedPositiveMax = new Number((short)0x7FFF);
			Assert.IsTrue(signedPositiveMax.Signed);
			Assert.IsTrue(signedPositiveMax.Sign);
			var signedNegativeOverflow = signedNegativeMax - signedPositiveMax;
			Assert.IsTrue(signedNegativeOverflow.Signed);
			Assert.IsFalse(signedNegativeOverflow.Sign);
			Assert.AreEqual(3, signedNegativeOverflow.ByteSize);
			Assert.AreEqual(new Number(unchecked((int)0xFFFF0002)), signedNegativeOverflow);
			var signedPositiveOverflow = signedPositiveMax - signedNegativeMax;
			Assert.IsTrue(signedPositiveOverflow.Signed);
			Assert.IsTrue(signedPositiveOverflow.Sign);
			Assert.AreEqual(3, signedPositiveOverflow.ByteSize);
			Assert.AreEqual(new Number(0x0000FFFE), signedPositiveOverflow);
		}

		#endregion Public Methods
	}
}
