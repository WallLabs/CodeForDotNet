using System;
using CodeForDotNet.Numerics;
using Xunit;

namespace CodeForDotNet.Tests
{
    /// <summary>
    /// Tests the numerics classes and extension.
    /// </summary>
    public class NumericsTests
    {
        /// <summary>
        /// Tests the various <see cref="Number"/> constructors.
        /// </summary>
        [Fact]
        public void NumberTestConstructors()
        {
            // Default constructor
            var number = new Number();
            Assert.True(number.IsZero);
            Assert.False(number.Signed);
            Assert.Equal(0, number.ByteSize);

            // Byte array constructor
            number = new Number(new byte[] { 1 }, true);
            Assert.False(number.IsZero);
            Assert.True(number.Signed);
            Assert.True(number.Sign);
            Assert.Equal(1, number.ByteSize);
            Assert.Equal(number[0], 1);
            number = new Number(new byte[] { 0xff }, true);
            Assert.False(number.IsZero);
            Assert.True(number.Signed);
            Assert.False(number.Sign);
            Assert.Equal(1, number.ByteSize);
            Assert.Equal(number[0], 0xff);
            number = new Number(new byte[] { 0xff }, false);
            Assert.False(number.IsZero);
            Assert.False(number.Signed);
            Assert.Equal(1, number.ByteSize);
            Assert.Equal(number[0], 0xff);

            // Decimal constructor
            number = new Number(1m);
            Assert.False(number.IsZero);
            Assert.True(number.Signed);
            Assert.True(number.Sign);
            Assert.Equal(16, number.ByteSize);
            Assert.Equal(number[0], 1);
            Assert.Equal(number[1], 0);
            Assert.Equal(number[2], 0);
            Assert.Equal(number[3], 0);
            Assert.Equal(number[4], 0);
            Assert.Equal(number[5], 0);
            Assert.Equal(number[6], 0);
            Assert.Equal(number[7], 0);
            Assert.Equal(number[8], 0);
            Assert.Equal(number[9], 0);
            Assert.Equal(number[10], 0);
            Assert.Equal(number[11], 0);
            Assert.Equal(number[12], 0);
            Assert.Equal(number[13], 0);
            Assert.Equal(number[14], 0);
            Assert.Equal(number[15], 0);
            number = new Number(-1m);
            Assert.False(number.IsZero);
            Assert.True(number.Signed);
            Assert.False(number.Sign);
            Assert.Equal(16, number.ByteSize);
            Assert.Equal(number[0], 0xff);
            Assert.Equal(number[1], 0xff);
            Assert.Equal(number[2], 0xff);
            Assert.Equal(number[3], 0xff);
            Assert.Equal(number[4], 0xff);
            Assert.Equal(number[5], 0xff);
            Assert.Equal(number[6], 0xff);
            Assert.Equal(number[7], 0xff);
            Assert.Equal(number[8], 0xff);
            Assert.Equal(number[9], 0xff);
            Assert.Equal(number[10], 0xff);
            Assert.Equal(number[11], 0xff);
            Assert.Equal(number[12], 0xff);
            Assert.Equal(number[13], 0xff);
            Assert.Equal(number[14], 0xff);
            Assert.Equal(number[15], 0xff);

            // Double constructor
            number = new Number((double)1);
            Assert.False(number.IsZero);
            Assert.True(number.Signed);
            Assert.True(number.Sign);
            Assert.Equal(8, number.ByteSize);
            Assert.Equal(number[0], 1);
            Assert.Equal(number[1], 0);
            Assert.Equal(number[2], 0);
            Assert.Equal(number[3], 0);
            Assert.Equal(number[4], 0);
            Assert.Equal(number[5], 0);
            Assert.Equal(number[6], 0);
            Assert.Equal(number[7], 0);
            number = new Number((double)-1);
            Assert.False(number.IsZero);
            Assert.True(number.Signed);
            Assert.False(number.Sign);
            Assert.Equal(8, number.ByteSize);
            Assert.Equal(number[0], 0xff);
            Assert.Equal(number[1], 0xff);
            Assert.Equal(number[2], 0xff);
            Assert.Equal(number[3], 0xff);
            Assert.Equal(number[4], 0xff);
            Assert.Equal(number[5], 0xff);
            Assert.Equal(number[6], 0xff);
            Assert.Equal(number[7], 0xff);

            // Single constructor
            number = new Number(1f);
            Assert.False(number.IsZero);
            Assert.True(number.Signed);
            Assert.True(number.Sign);
            Assert.Equal(4, number.ByteSize);
            Assert.Equal(number[0], 1);
            Assert.Equal(number[1], 0);
            Assert.Equal(number[2], 0);
            Assert.Equal(number[3], 0);
            number = new Number(-1f);
            Assert.False(number.IsZero);
            Assert.True(number.Signed);
            Assert.False(number.Sign);
            Assert.Equal(4, number.ByteSize);
            Assert.Equal(number[0], 0xff);
            Assert.Equal(number[1], 0xff);
            Assert.Equal(number[2], 0xff);
            Assert.Equal(number[3], 0xff);

            // Int64 constructor
            number = new Number((long)1);
            Assert.False(number.IsZero);
            Assert.True(number.Signed);
            Assert.True(number.Sign);
            Assert.Equal(8, number.ByteSize);
            Assert.Equal(number[0], 1);
            Assert.Equal(number[1], 0);
            Assert.Equal(number[2], 0);
            Assert.Equal(number[3], 0);
            Assert.Equal(number[4], 0);
            Assert.Equal(number[5], 0);
            Assert.Equal(number[6], 0);
            Assert.Equal(number[7], 0);
            number = new Number((long)-1);
            Assert.False(number.IsZero);
            Assert.True(number.Signed);
            Assert.False(number.Sign);
            Assert.Equal(8, number.ByteSize);
            Assert.Equal(number[0], 0xff);
            Assert.Equal(number[1], 0xff);
            Assert.Equal(number[2], 0xff);
            Assert.Equal(number[3], 0xff);
            Assert.Equal(number[4], 0xff);
            Assert.Equal(number[5], 0xff);
            Assert.Equal(number[6], 0xff);
            Assert.Equal(number[7], 0xff);

            // Int32 constructor
            number = new Number(1);
            Assert.False(number.IsZero);
            Assert.True(number.Signed);
            Assert.True(number.Sign);
            Assert.Equal(4, number.ByteSize);
            Assert.Equal(number[0], 1);
            Assert.Equal(number[1], 0);
            Assert.Equal(number[2], 0);
            Assert.Equal(number[3], 0);
            number = new Number(-1);
            Assert.False(number.IsZero);
            Assert.True(number.Signed);
            Assert.False(number.Sign);
            Assert.Equal(4, number.ByteSize);
            Assert.Equal(number[0], 0xff);
            Assert.Equal(number[1], 0xff);
            Assert.Equal(number[2], 0xff);
            Assert.Equal(number[3], 0xff);

            // Int16 constructor
            number = new Number((short)1);
            Assert.False(number.IsZero);
            Assert.True(number.Signed);
            Assert.True(number.Sign);
            Assert.Equal(2, number.ByteSize);
            Assert.Equal(number[0], 1);
            Assert.Equal(number[1], 0);
            number = new Number((short)-1);
            Assert.False(number.IsZero);
            Assert.True(number.Signed);
            Assert.False(number.Sign);
            Assert.Equal(2, number.ByteSize);
            Assert.Equal(number[0], 0xff);
            Assert.Equal(number[1], 0xff);

            // UInt64 constructor
            number = new Number((ulong)1);
            Assert.False(number.IsZero);
            Assert.False(number.Signed);
            Assert.Equal(8, number.ByteSize);
            Assert.Equal(number[0], 1);
            Assert.Equal(number[1], 0);
            Assert.Equal(number[2], 0);
            Assert.Equal(number[3], 0);
            Assert.Equal(number[4], 0);
            Assert.Equal(number[5], 0);
            Assert.Equal(number[6], 0);
            Assert.Equal(number[7], 0);

            // UInt32 constructor
            number = new Number((uint)1);
            Assert.False(number.IsZero);
            Assert.False(number.Signed);
            Assert.Equal(4, number.ByteSize);
            Assert.Equal(number[0], 1);
            Assert.Equal(number[1], 0);
            Assert.Equal(number[2], 0);
            Assert.Equal(number[3], 0);

            // Int16 constructor
            number = new Number((ushort)1);
            Assert.False(number.IsZero);
            Assert.False(number.Signed);
            Assert.Equal(2, number.ByteSize);
            Assert.Equal(number[0], 1);
            Assert.Equal(number[1], 0);
        }

        /// <summary>
        /// Tests the <see cref="Number.CompareTo"/> method.
        /// </summary>
        [Fact]
        public void NumberTestCompareTo()
        {
            // Test positive and negative ones and zeros
            Assert.Equal(0, Number.Zero.CompareTo(Number.Zero));
            Assert.Equal(-1, Number.Zero.CompareTo(Number.One));
            Assert.Equal(1, Number.Zero.CompareTo(Number.MinusOne));
            Assert.Equal(1, Number.One.CompareTo(Number.Zero));
            Assert.Equal(0, Number.One.CompareTo(Number.One));
            Assert.Equal(1, Number.One.CompareTo(Number.MinusOne));
            Assert.Equal(-1, Number.MinusOne.CompareTo(Number.Zero));
            Assert.Equal(-1, Number.MinusOne.CompareTo(Number.One));
            Assert.Equal(0, Number.MinusOne.CompareTo(Number.MinusOne));

            // Test most significant bytes used for comparison
            var number123Signed = new Number(new byte[] { 3, 2, 1 }, true);
            var number321Signed = new Number(new byte[] { 1, 2, 3 }, true);
            var number123Unsigned = new Number(new byte[] { 3, 2, 1 }, false);
            var number321Unsigned = new Number(new byte[] { 1, 2, 3 }, false);
            Assert.Equal(0, number123Signed.CompareTo(number123Signed));
            Assert.Equal(0, number123Signed.CompareTo(number123Unsigned));
            Assert.Equal(0, number321Signed.CompareTo(number321Signed));
            Assert.Equal(0, number321Signed.CompareTo(number321Unsigned));
            Assert.Equal(-1, number123Signed.CompareTo(number321Signed));
            Assert.Equal(-1, number123Signed.CompareTo(number321Unsigned));
            Assert.Equal(1, number321Signed.CompareTo(number123Signed));
            Assert.Equal(1, number321Signed.CompareTo(number123Unsigned));

            // Test comparison of different lengthed values
            var number123SignedLong = new Number(new byte[] { 3, 2, 1, 0, 0, 0, 0 }, true);
            var number123UnsignedLong = new Number(new byte[] { 3, 2, 1, 0, 0, 0, 0 }, false);
            var number321SignedLong = new Number(new byte[] { 1, 2, 3, 0, 0, 0, 0 }, true);
            var number321UnsignedLong = new Number(new byte[] { 1, 2, 3, 0, 0, 0, 0 }, false);
            Assert.Equal(0, number123SignedLong.CompareTo(number123Signed));
            Assert.Equal(0, number123SignedLong.CompareTo(number123Unsigned));
            Assert.Equal(0, number123UnsignedLong.CompareTo(number123Signed));
            Assert.Equal(0, number123UnsignedLong.CompareTo(number123Unsigned));
            Assert.Equal(0, number321SignedLong.CompareTo(number321Signed));
            Assert.Equal(0, number321SignedLong.CompareTo(number321Unsigned));
            Assert.Equal(0, number321UnsignedLong.CompareTo(number321Signed));
            Assert.Equal(0, number321UnsignedLong.CompareTo(number321Unsigned));
            Assert.Equal(-1, number123SignedLong.CompareTo(number321Signed));
            Assert.Equal(-1, number123SignedLong.CompareTo(number321Unsigned));
            Assert.Equal(-1, number123UnsignedLong.CompareTo(number321Signed));
            Assert.Equal(-1, number123UnsignedLong.CompareTo(number321Unsigned));
            Assert.Equal(1, number321SignedLong.CompareTo(number123Signed));
            Assert.Equal(1, number321SignedLong.CompareTo(number123Unsigned));
            Assert.Equal(1, number321UnsignedLong.CompareTo(number123Signed));
            Assert.Equal(1, number321UnsignedLong.CompareTo(number123Unsigned));
        }

        /// <summary>
        /// Tests the <see cref="Number"/> comparison operators (&gt; &gt;= == &lt;= &lt;).
        /// </summary>
        [Fact]
        public void NumberTestComparisonOperators()
        {
            // Avoid warning of constant result
            var otherZero = Number.Zero;

            // Test less than
            Assert.False(Number.Zero < Number.MinusOne);
            Assert.False(Number.Zero < otherZero);
            Assert.True(Number.Zero < Number.One);

            // Test less than or equal
            Assert.False(Number.Zero <= Number.MinusOne);
            Assert.True(Number.Zero <= otherZero);
            Assert.True(Number.Zero <= Number.One);

            // Test equal
            Assert.False(Number.Zero == Number.MinusOne);
            Assert.True(Number.Zero == otherZero);
            Assert.False(Number.Zero == Number.One);

            // Test greater than or equal
            Assert.True(Number.Zero >= Number.MinusOne);
            Assert.True(Number.Zero >= otherZero);
            Assert.False(Number.Zero >= Number.One);

            // Test greater than
            Assert.True(Number.Zero > Number.MinusOne);
            Assert.False(Number.Zero > otherZero);
            Assert.False(Number.Zero > Number.One);
        }

        /// <summary>
        /// Tests the <see cref="Number"/> signed and unsigned effects.
        /// </summary>
        [Fact]
        public void NumberTestSign()
        {
            // Test comparison of signed and unsigned
            var signedMin = new Number(new byte[] { 0xff, 0xff, 0xff, 0xff }, true);
            var unsignedMax = new Number(new byte[] { 0xff, 0xff, 0xff, 0xff }, false);
            Assert.False(signedMin == unsignedMax);
            Assert.True(unsignedMax > signedMin);
        }

        /// <summary>
        /// Tests the <see cref="Number.Negate"/> method and operator.
        /// </summary>
        [Fact]
        public void NumberTestNegate()
        {
            // Test negate
            var one = new Number(1);
            var minusOne = -one;
            Assert.True(minusOne.Signed);
            Assert.False(minusOne.Sign);
            Assert.Equal(one.ByteSize, minusOne.ByteSize);
            Assert.Equal(-1, minusOne);
            var unsignedMax = new Number((ushort)0xFFFF);
            var negativeOverflow = -unsignedMax;
            Assert.True(negativeOverflow.Signed);
            Assert.False(negativeOverflow.Sign);
            Assert.Equal(unsignedMax.ByteSize + 1, negativeOverflow.ByteSize);
            Assert.Equal(unchecked((int)0xFFFF0001), negativeOverflow);
        }

        /// <summary>
        /// Tests the <see cref="Number.Add"/> method and operator.
        /// </summary>
        [Fact]
        public void NumberTestAdd()
        {
            // Test simple addition
            var one = (Number)1;
            var two = one + one;
            Assert.Equal(2, two);
            Assert.Equal(3, two + one);
            Assert.Equal(4, two + two);

            // Test addition with negative
            var minusOne = -(Number)1;
            Assert.Equal(-1, minusOne);
            var zero = one + minusOne;
            Assert.Equal(0, zero);
            Assert.True(zero.IsZero);
            var minusTwo = minusOne + minusOne;
            Assert.Equal(-2, minusTwo);
            Assert.Equal(-1, zero + minusOne);
            Assert.Equal(-4, minusTwo + minusTwo);

            // Test addition of unsigned values with overflow carried
            var unsignedMax = new Number((ushort)0xFFFF);
            Assert.False(unsignedMax.Signed);
            Assert.True(unsignedMax.Sign);
            var unsignedPositiveOverflow = unsignedMax + unsignedMax;
            Assert.False(unsignedPositiveOverflow.Signed);
            Assert.True(unsignedPositiveOverflow.Sign);
            Assert.Equal(3, unsignedPositiveOverflow.ByteSize);
            Assert.Equal(new Number((uint)0x0001FFFE), unsignedPositiveOverflow);

            // Test addition of signed values with overflow carried
            var signedPositiveMax = new Number((short)0x7FFF);
            Assert.True(signedPositiveMax.Signed);
            Assert.True(signedPositiveMax.Sign);
            var signedPositiveOverflow = signedPositiveMax + signedPositiveMax;
            Assert.True(signedPositiveOverflow.Signed);
            Assert.True(signedPositiveOverflow.Sign);
            Assert.Equal(3, signedPositiveOverflow.ByteSize);
            Assert.Equal(new Number(0x0000FFFE), signedPositiveOverflow);
            var signedNegativeMax = new Number(unchecked((short)0x8001));
            Assert.True(signedNegativeMax.Signed);
            Assert.False(signedNegativeMax.Sign);
            var signedNegativeOverflow = signedNegativeMax + signedNegativeMax;
            Assert.True(signedNegativeOverflow.Signed);
            Assert.False(signedNegativeOverflow.Sign);
            Assert.Equal(3, signedNegativeOverflow.ByteSize);
            Assert.Equal(new Number(unchecked((int)0xFFFF0002)), signedNegativeOverflow);
        }

        /// <summary>
        /// Tests the <see cref="Number.Subtract"/> method and operator.
        /// </summary>
        [Fact]
        public void NumberTestSubtract()
        {
            // Test simple subtraction
            var two = (Number)2;
            var one = two - 1;
            Assert.Equal(1, one);
            Assert.Equal(1, two - one);
            Assert.Equal(0, two - two);

            // Test subtraction with negative
            var minusOne = -(Number)1;
            Assert.Equal(-1, minusOne);
            var zero = minusOne + one;
            Assert.Equal(0, zero);
            Assert.True(zero.IsZero);
            var minusTwo = minusOne - 1;
            Assert.Equal(-2, minusTwo);
            Assert.Equal(-1, zero - one);
            Assert.Equal(1, zero - minusOne);
            Assert.Equal(-4, minusTwo - two);

            // Test subtraction of unsigned values with overflow
            var unsignedSmall = new Number((ushort)0x0102);
            Assert.False(unsignedSmall.Signed);
            Assert.True(unsignedSmall.Sign);
            var unsignedMax = new Number((ushort)0xFFFF);
            Assert.False(unsignedMax.Signed);
            Assert.True(unsignedMax.Sign);
            var unsignedNegativeOverflow = unsignedSmall - unsignedMax;
            Assert.True(unsignedNegativeOverflow.Signed);
            Assert.False(unsignedNegativeOverflow.Sign);
            Assert.Equal(3, unsignedNegativeOverflow.ByteSize);
            Assert.Equal(new Number(unchecked((int)0xFFFF0103)), unsignedNegativeOverflow);

            // Test subtraction of signed values with overflow carried
            var signedNegativeMax = new Number(unchecked((short)0x8001));
            Assert.True(signedNegativeMax.Signed);
            Assert.False(signedNegativeMax.Sign);
            var signedPositiveMax = new Number((short)0x7FFF);
            Assert.True(signedPositiveMax.Signed);
            Assert.True(signedPositiveMax.Sign);
            var signedNegativeOverflow = signedNegativeMax - signedPositiveMax;
            Assert.True(signedNegativeOverflow.Signed);
            Assert.False(signedNegativeOverflow.Sign);
            Assert.Equal(3, signedNegativeOverflow.ByteSize);
            Assert.Equal(new Number(unchecked((int)0xFFFF0002)), signedNegativeOverflow);
            var signedPositiveOverflow = signedPositiveMax - signedNegativeMax;
            Assert.True(signedPositiveOverflow.Signed);
            Assert.True(signedPositiveOverflow.Sign);
            Assert.Equal(3, signedPositiveOverflow.ByteSize);
            Assert.Equal(new Number(0x0000FFFE), signedPositiveOverflow);
        }

        /// <summary>
        /// Tests the <see cref="Number.Multiply"/> method and operator.
        /// </summary>
        [Fact]
        public void NumberTestMultiply()
        {
            // Test simple multiplication
            var zero = Number.Zero;
            var one = (Number)1;
            var two = (Number)2;
            Assert.Equal(0, one * zero);
            Assert.Equal(1, one * one);
            Assert.Equal(2, one * two);
            Assert.Equal(4, two * two);
            Assert.Equal(624, (Number)12 * 52);

            // Test multiplication of negative
            var minusOne = -(Number)1;
            Assert.Equal(-1, one * minusOne);
            Assert.Equal(-1, minusOne * one);
            Assert.Equal(0, minusOne * zero);
            Assert.Equal(1, minusOne * minusOne);

            // Test multiplication of unsigned values with overflow carried
            var positiveOverflow1 = new Number((ushort)0xFFFF);
            Assert.False(positiveOverflow1.Signed);
            Assert.True(positiveOverflow1.Sign);
            var positiveOverflow2 = new Number((ushort)0xFFFF);
            Assert.False(positiveOverflow2.Signed);
            Assert.True(positiveOverflow2.Sign);
            var positiveOverflowResult = positiveOverflow1 * positiveOverflow2;
            Assert.False(positiveOverflowResult.Signed);
            Assert.True(positiveOverflowResult.Sign);
            Assert.Equal(4, positiveOverflowResult.ByteSize);
            Assert.Equal(new Number(0xFFFE0001), positiveOverflowResult);
        }

        /// <summary>
        /// Tests the <see cref="Number.Divide(Number, Number, out Number)"/> method and operator.
        /// </summary>
        [Fact]
        public void NumberTestDivide()
        {
            // Test simple division
            var zero = Number.Zero;
            var one = (Number)1;
            var two = (Number)2;
            try { zero = one / zero; }
            catch (DivideByZeroException) {/* Successful */}
            Assert.Equal(1, one / one);
            Assert.Equal(2, one / two);
            Assert.Equal(1, two / two);
            Assert.Equal(12, (Number)624 / 52);

            // Test division of negative
            var minusOne = -(Number)1;
            Assert.Equal(-1, one / minusOne);
            Assert.Equal(-1, minusOne / one);
            try { zero = minusOne / zero; }
            catch (DivideByZeroException) {/* Successful */}
            Assert.Equal(1, minusOne / minusOne);

            // Test division with remainder
            Number remainder;
            Assert.Equal(5, Number.Divide(26, 5, out remainder));
            Assert.Equal(1, remainder);
        }

        /// <summary>
        /// Tests the <see cref="Number.Power"/> method.
        /// </summary>
        [Fact]
        public void NumberTestPower()
        {
            Assert.Equal(0, Number.Power(1, 0));
            Assert.Equal(1, Number.Power(1, 1));
            Assert.Equal(1, Number.Power(1, 2));
            Assert.Equal(1, Number.Power(1, 3));
            Assert.Equal(1, Number.Power(1, 4));
            Assert.Equal(1, Number.Power(1, 5));
            Assert.Equal(0, Number.Power(2, 0));
            Assert.Equal(2, Number.Power(2, 1));
            Assert.Equal(4, Number.Power(2, 2));
            Assert.Equal(8, Number.Power(2, 3));
            Assert.Equal(16, Number.Power(2, 4));
            Assert.Equal(32, Number.Power(2, 5));
            Assert.Equal(0, Number.Power(3, 0));
            Assert.Equal(3, Number.Power(3, 1));
            Assert.Equal(9, Number.Power(3, 2));
            Assert.Equal(27, Number.Power(3, 3));
            Assert.Equal(81, Number.Power(3, 4));
            Assert.Equal(243, Number.Power(3, 5));
        }

        /// <summary>
        /// Tests the <see cref="Number.LeftShift"/> method and operator.
        /// </summary>
        [Fact]
        public void NumberTestLeftShift()
        {
            Assert.Equal(0xffffffff, new Number(0xffffffff) << 0);
            Assert.Equal(0x01fffffffe, new Number(0xffffffff) << 1);
            Assert.Equal(0x03fffffffc, new Number(0xffffffff) << 2);
            Assert.Equal(0x07fffffff8, new Number(0xffffffff) << 3);
            Assert.Equal(0x0ffffffff0, new Number(0xffffffff) << 4);
            Assert.Equal(0x1fffffffe0, new Number(0xffffffff) << 5);
            Assert.Equal(0x3fffffffc0, new Number(0xffffffff) << 6);
            Assert.Equal(0x7fffffff80, new Number(0xffffffff) << 7);
            Assert.Equal(0xffffffff00, new Number(0xffffffff) << 8);
            Assert.Equal(0x01fffffffe00, new Number(0xffffffff) << 9);
            Assert.Equal(0x03fffffffc00, new Number(0xffffffff) << 10);
            Assert.Equal(0x07fffffff800, new Number(0xffffffff) << 11);
            Assert.Equal(0x0ffffffff000, new Number(0xffffffff) << 12);
            Assert.Equal(0x1fffffffe000, new Number(0xffffffff) << 13);
            Assert.Equal(0x3fffffffc000, new Number(0xffffffff) << 14);
            Assert.Equal(0x7fffffff8000, new Number(0xffffffff) << 15);
            Assert.Equal(0xffffffff0000, new Number(0xffffffff) << 16);
            Assert.Equal(0x0ffffffff00000, new Number(0xffffffff) << 20);
            Assert.Equal(0xffffffff000000, new Number(0xffffffff) << 24);
            Assert.Equal(0x0ffffffff0000000, new Number(0xffffffff) << 28);
            Assert.Equal(0xffffffff00000000, new Number(0xffffffff) << 32);
        }

        /// <summary>
        /// Tests the <see cref="Number.RightShift"/> method and operator.
        /// </summary>
        [Fact]
        public void NumberTestRightShift()
        {
            Assert.Equal(0xffffffff, new Number(0xffffffff) >> 0);
            Assert.Equal(0x7fffffff, new Number(0xffffffff) >> 1);
            Assert.Equal(0x3fffffff, new Number(0xffffffff) >> 2);
            Assert.Equal(0x1fffffff, new Number(0xffffffff) >> 3);
            Assert.Equal(0x0fffffff, new Number(0xffffffff) >> 4);
            Assert.Equal(0x07ffffff, new Number(0xffffffff) >> 5);
            Assert.Equal(0x03ffffff, new Number(0xffffffff) >> 6);
            Assert.Equal(0x01ffffff, new Number(0xffffffff) >> 7);
            Assert.Equal(0x00ffffff, new Number(0xffffffff) >> 8);
            Assert.Equal(0x007fffff, new Number(0xffffffff) >> 9);
            Assert.Equal(0x003fffff, new Number(0xffffffff) >> 10);
            Assert.Equal(0x001fffff, new Number(0xffffffff) >> 11);
            Assert.Equal(0x000fffff, new Number(0xffffffff) >> 12);
            Assert.Equal(0x0007ffff, new Number(0xffffffff) >> 13);
            Assert.Equal(0x0003ffff, new Number(0xffffffff) >> 14);
            Assert.Equal(0x0001ffff, new Number(0xffffffff) >> 15);
            Assert.Equal(0x0000ffff, new Number(0xffffffff) >> 16);
            Assert.Equal(0x00000fff, new Number(0xffffffff) >> 20);
            Assert.Equal(0x000000ff, new Number(0xffffffff) >> 24);
            Assert.Equal(0x0000000f, new Number(0xffffffff) >> 28);
            Assert.Equal(0x00000000, new Number(0xffffffff) >> 32);
            Assert.Equal(0x00000000, new Number(0xffffffff) >> 33);
            Assert.Equal(0x00000000, new Number(0xffffffff) >> 64);
        }

        /// <summary>
        /// Tests the conversion to and from strings of different bases.
        /// </summary>
        [Fact]
        public void NumberTestConvertStringAndBase()
        {
            // Test big integers (positive, negative and unsigned)
            Number number = 255;
            Assert.Equal("00FF", number.ToString(16));
            Assert.Equal("00FF", number.ToString(16, 4));
            Assert.Equal("0000FF", number.ToString(16, 5));                      // Minimum length rounded-up to nearest byte when bit based
            Assert.Equal("255", number.ToString(10));
            Assert.Equal("0000000011111111", number.ToString(2));
            Assert.Equal("0000000011111111", number.ToString(2, 16));
            Assert.Equal("000000000000000011111111", number.ToString(2, 17));    // Minimum length rounded-up to nearest byte when bit based
            Assert.True(Number.TryParse("00FF", 16, out number));
            Assert.Equal(255, number);
            Assert.True(Number.TryParse("FF", 16, out number));
            Assert.Equal(-1, number);
            Assert.True(Number.TryParse("255", 10, out number));
            Assert.Equal(255, number);
            Assert.True(Number.TryParse("-1", 10, out number));
            Assert.Equal(-1, number);
            Assert.True(Number.TryParse("11111111", 2, out number, false));
            Assert.Equal(255, number);
            Assert.True(Number.TryParse("11111111", 2, out number, true));
            Assert.Equal(-1, number);
            number = (sbyte)-1;
            Assert.Equal("FF", number.ToString(16));
            Assert.Equal("FFFF", number.ToString(16, 4));
            Assert.Equal("FFFFFF", number.ToString(16, 5));                      // Minimum length rounded-up to nearest byte when bit based
            Assert.Equal("-1", number.ToString(10));
            Assert.Equal("11111111", number.ToString(2));
            Assert.Equal("1111111111111111", number.ToString(2, 16));
            Assert.Equal("111111111111111111111111", number.ToString(2, 17));    // Minimum length rounded-up to nearest byte when bit based
            Assert.True(Number.TryParse("FF", 16, out number, true));
            Assert.Equal(-1, number);
            Assert.True(Number.TryParse("-1", 10, out number, true));
            Assert.Equal(-1, number);
            Assert.True(Number.TryParse("11111111", 2, out number, true));
            Assert.Equal(-1, number);

            // Test signed longs (positive and negative)
            var signedLong = 255L;
            Assert.Equal("00FF", signedLong.ToString(16));
            Assert.Equal("00FF", signedLong.ToString(16, 4));
            Assert.Equal("0000FF", signedLong.ToString(16, 5));                      // Minimum length rounded-up to nearest byte when bit based
            Assert.Equal("255", signedLong.ToString(10));
            Assert.Equal("0000000011111111", signedLong.ToString(2));
            Assert.Equal("0000000011111111", signedLong.ToString(2, 16));
            Assert.Equal("000000000000000011111111", signedLong.ToString(2, 17));    // Minimum length rounded-up to nearest byte when bit based
            Assert.True(Number.TryParse("0FF", 16, out number));
            Assert.Equal(255, number);
            Assert.True(Number.TryParse("255", 10, out number));
            Assert.Equal(255, number);
            Assert.True(Number.TryParse("0000000011111111", 2, out number));
            Assert.Equal(255, number);
            signedLong = -1;
            Assert.Equal("FFFFFFFFFFFFFFFF", signedLong.ToString(16));
            Assert.Equal("FFFFFFFFFFFFFFFF", signedLong.ToString(16, 4));            // Minimum length cannot be less than underlying size
            Assert.Equal("FFFFFFFFFFFFFFFFFFFF", signedLong.ToString(16, 20));       // ...but it can be greater
            Assert.Equal("-1", signedLong.ToString(10));
            Assert.Equal("1111111111111111111111111111111111111111111111111111111111111111", signedLong.ToString(2));
            Assert.Equal("1111111111111111111111111111111111111111111111111111111111111111", signedLong.ToString(2, 8));           // Minimum length cannot be less than underlying size
            Assert.Equal("111111111111111111111111111111111111111111111111111111111111111111111111", signedLong.ToString(2, 65));   // ...but it can be greater
            Assert.True(Number.TryParse("FF", 16, out number));
            Assert.Equal(-1, number);
            Assert.True(Number.TryParse("-1", 10, out number));
            Assert.Equal(-1, number);
            Assert.True(Number.TryParse("11111111", 2, out number));
            Assert.Equal(-1, number);

            // Test unsigned longs
            const ulong unsignedLong = 255UL;
            Assert.Equal("FF", unsignedLong.ToString(16));
            Assert.Equal("00FF", unsignedLong.ToString(16, 4));
            Assert.Equal("0000FF", unsignedLong.ToString(16, 5));            // Minimum length rounded-up to nearest byte when bit based
            Assert.Equal("255", unsignedLong.ToString(10));
            Assert.Equal("11111111", unsignedLong.ToString(2));
            Assert.Equal("11111111", unsignedLong.ToString(2, 4));           // Minimum length cannot be less than underlying size
            Assert.Equal("0000000011111111", unsignedLong.ToString(2, 16));  // ...but it can be greater
            Assert.True(Number.TryParse("FF", 16, out number, false));
            Assert.Equal((ulong)255, number);
            Assert.True(Number.TryParse("255", 10, out number, false));
            Assert.Equal((ulong)255, number);
            Assert.True(Number.TryParse("11111111", 2, out number, false));
            Assert.Equal((ulong)255, number);

            // Test invalid characters
            Assert.False(Number.TryParse("980&'=%", 16, out number));
            Assert.Equal(0, number);
            Assert.False(Number.TryParse("980&'=%", 10, out number));
            Assert.Equal(0, number);
            Assert.False(Number.TryParse("980&'=%", 2, out number));
            Assert.Equal(0, number);
        }
    }
}
