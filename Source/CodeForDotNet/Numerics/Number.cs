using CodeForDotNet.Properties;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CodeForDotNet.Numerics
{
    /// <summary>
    /// Number of unlimited size, based on a variable length byte array, signed or unsigned.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1720", Justification = "Use of the word \"signed\" is preferable for an intuitive interface.")]
    public struct Number : IComparable<Number>, IEquatable<Number>
    {
        #region Private Fields

        /// <summary>
        /// Valid number digits in order of value starting at zero.
        /// </summary>
        private const string NumberDigits = "0123456789ABCDEF";

        /// <summary>
        /// The internal numeric value, stored as a byte array ordered from least significant (LSB) to most significant (MSB), i.e. the LSB is at index 0.
        /// </summary>
        private readonly byte[] _bytes;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Creates a value based on an existing array of bytes (not copied).
        /// </summary>
        public Number(byte[] value, bool signed)
            : this()
        {
            // Validate
            if (value is null) throw new ArgumentNullException(nameof(value));

            // Set value
            _bytes = value;
            Signed = signed;
        }

        /// <summary>
        /// Creates a value based on an existing <see cref="byte"/>.
        /// </summary>
        public Number(byte value)
            : this()
        {
            _bytes = new[] { value };
            Signed = false;
        }

        /// <summary>
        /// Creates a value based on an existing <see cref="sbyte"/>.
        /// </summary>
        [CLSCompliant(false)]
        public Number(sbyte value)
            : this()
        {
            unchecked
            {
                _bytes = new[] { (byte)value };
                Signed = true;
            }
        }

        /// <summary>
        /// Creates a value based on an existing <see cref="short"/>.
        /// </summary>
        public Number(short value)
            : this()
        {
            _bytes = BitConverter.GetBytes(value);
            Signed = true;
        }

        /// <summary>
        /// Creates a value based on an existing <see cref="ushort"/>.
        /// </summary>
        [CLSCompliant(false)]
        public Number(ushort value)
            : this()
        {
            _bytes = BitConverter.GetBytes(value);
            Signed = false;
        }

        /// <summary>
        /// Creates a value based on an existing <see cref="int"/>.
        /// </summary>
        public Number(int value)
            : this()
        {
            _bytes = BitConverter.GetBytes(value);
            Signed = true;
        }

        /// <summary>
        /// Creates a value based on an existing <see cref="uint"/>.
        /// </summary>
        [CLSCompliant(false)]
        public Number(uint value)
            : this()
        {
            _bytes = BitConverter.GetBytes(value);
            Signed = false;
        }

        /// <summary>
        /// Creates a value based on an existing <see cref="long"/>.
        /// </summary>
        public Number(long value)
            : this()
        {
            _bytes = BitConverter.GetBytes(value);
            Signed = true;
        }

        /// <summary>
        /// Creates a value based on an existing <see cref="ulong"/>.
        /// </summary>
        [CLSCompliant(false)]
        public Number(ulong value)
            : this()
        {
            _bytes = BitConverter.GetBytes(value);
            Signed = false;
        }

        /// <summary>
        /// Creates a value based on an existing <see cref="float"/>.
        /// </summary>
        public Number(float value)
            : this()
        {
            // Validate
            if (float.IsInfinity(value) || float.IsNaN(value))
                throw new OverflowException();

            // Set value
            _bytes = BitConverter.GetBytes((int)Math.Floor(value));
            Signed = true;
        }

        /// <summary>
        /// Creates a value based on an existing <see cref="double"/>.
        /// </summary>
        public Number(double value)
            : this()
        {
            // Validate
            if (double.IsInfinity(value) || double.IsNaN(value))
                throw new OverflowException();

            // Set value
            _bytes = BitConverter.GetBytes((long)Math.Floor(value));
            Signed = true;
        }

        /// <summary>
        /// Creates a value based on an existing <see cref="decimal"/>.
        /// </summary>
        public Number(decimal value)
            : this()
        {
            unchecked
            {
                // Split decimal into parts and detect sign
                value = decimal.Truncate(value);
                var decimalIntegers = decimal.GetBits(value);
                var signScaleAndHighWord = decimalIntegers[3];
                decimalIntegers[3] = signScaleAndHighWord & 0x0000ffff;
                var negative = (signScaleAndHighWord & 0x80000000) != 0;
                var scale = (short)((signScaleAndHighWord >> 16) & 0x7fff);

                // Calculate binary scale
                var bitScale = scale * (10 / 2);
                var byteScale = bitScale / 8;
                var byteSize = bitScale + (4 * 4);

                // Allocate buffer for conversion and initialize
                var bytes = new byte[byteSize];
                if (negative)
                {
                    // Set scale bytes negative when necessary
                    for (var index = 0; index < byteScale; index++)
                        bytes[index] = 0xff;
                }

                // Convert value part
                var bitShift = bitScale - (byteScale * 8);
                var carry = 0;
                for (int bytesIndex = byteScale, decimalIndex = 0;
                    bytesIndex < byteSize;
                    bytesIndex += 4, decimalIndex++)
                {
                    var intPlusCarryLong = (long)(decimalIntegers[decimalIndex] << bitShift) + carry;
                    var intValue = (int)((negative
                                              ? intPlusCarryLong > 0 ? -intPlusCarryLong : -1
                                              : intPlusCarryLong) & 0xffffffff);
                    Array.Copy(BitConverter.GetBytes(intValue), 0, bytes, bytesIndex, 4);
                    carry = (int)(intPlusCarryLong >> 32);
                }

                // Set value
                _bytes = bytes;
                Signed = true;
            }
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Minus-one signed value.
        /// </summary>
        public static Number MinusOne
        {
            get { return new Number(new byte[] { 0xff }, true); }
        }

        /// <summary>
        /// One signed value.
        /// </summary>
        public static Number One
        {
            get { return new Number(new byte[] { 0x01 }, true); }
        }

        /// <summary>
        /// Zero signed value.
        /// </summary>
        public static Number Zero
        {
            get { return new Number(Array.Empty<byte>(), true); }
        }

        /// <summary>
        /// Size in bytes.
        /// </summary>
        public int ByteSize { get { return _bytes != null ? _bytes.Length : 0; } }

        /// <summary>
        /// Indicates the current value is zero.
        /// </summary>
        public bool IsZero
        {
            get
            {
                // Zero when empty
                if (_bytes == null || _bytes.Length == 0)
                    return true;

                // Zero when all bytes are zero
                return _bytes.All(byteValue => byteValue == 0);
            }
        }

        /// <summary>
        /// Indicates the sign, true for positive, false for negative. Always true when unsigned.
        /// </summary>
        public bool Sign
        {
            get
            {
                // Always positive when unsigned
                if (!Signed) return true;

                // Use sign bit when signed
                var size = ByteSize;
                return size == 0 || (_bytes[size - 1] & 0x80) == 0;
            }
        }

        /// <summary>
        /// Enables or disables signed number processing.
        /// </summary>
        /// <remarks>The internal value is not changed, only its interpretation.</remarks>
        public bool Signed { get; private set; }

        #endregion Public Properties

        #region Public Indexers

        /// <summary>
        /// Gets the byte at the specified index.
        /// </summary>
        public byte this[int index]
        {
            get { return _bytes[index]; }
        }

        #endregion Public Indexers

        #region Public Methods

        /// <summary>
        /// Gets the absolute value of the number, basically converting negative to positive when necessary.
        /// </summary>
        public static Number Absolute(Number value)
        {
            return value.Sign || value.IsZero ? value : Negate(value);
        }

        /// <summary>
        /// Adds two numbers together.
        /// </summary>
        public static Number Add(Number left, Number right)
        {
            unchecked
            {
                // Calculate size and sign
                var leftSize = left.ByteSize;
                var rightSize = right.ByteSize;
                var leftSigned = left.Signed;
                var rightSigned = right.Signed;
                var resultSigned = leftSigned || rightSigned;
                var resultSize = rightSize > leftSize ? rightSize : leftSize;
                var leftBytes = left._bytes;
                var rightBytes = right._bytes;
                if (leftSigned != rightSigned &&
                    ((leftSize > 0 && (leftBytes[leftSize - 1] & 0x80) != 0) ||
                    (rightSize > 0 && (rightBytes[rightSize - 1] & 0x80) != 0)))
                {
                    // Extend by one byte when part unsigned and last bit used
                    resultSize++;
                }

                // Add bytes with carry
                var resultBytes = new byte[resultSize];
                var leftSign = left.Sign;
                var rightSign = right.Sign;
                byte carry = 0;
                for (var index = 0; index < resultSize; index++)
                {
                    // Get sign extended bytes
                    var leftValue = index < leftSize ? leftBytes[index] : leftSign ? (byte)0 : (byte)0xFF;
                    var rightValue = index < rightSize ? rightBytes[index] : rightSign ? (byte)0 : (byte)0xFF;

                    // Add with carry
                    var resultAndCarry = (ushort)(leftValue + carry + rightValue);
                    carry = (byte)(resultAndCarry >> 8);
                    var result = (byte)resultAndCarry;
                    resultBytes[index] = result;
                }

                // Extend overflow and/or sign when necessary
                var lastBitSet = (resultBytes[resultSize - 1] & 0x80) != 0;
                if ((!resultSigned && carry == 1) ||
                    (resultSigned && (
                        (leftSign == rightSign && ((lastBitSet && carry == 0) || (!lastBitSet && carry == 1)))) ||
                        (leftSign != rightSign && ((lastBitSet && carry == 1) || (!lastBitSet && carry == 0)))))
                {
                    Array.Resize(ref resultBytes, resultSize + 1);
                    resultBytes[resultSize] = !resultSigned
                        ? carry             // Unsigned overflow
                        : carry != 0        // Sign extension
                            ? (byte)0xff
                            : (byte)0;
                }

                // Return result
                return new Number(resultBytes, resultSigned);
            }
        }

        /// <summary>
        /// Performs a bitwise AND on two <see cref="Number"/> values.
        /// </summary>
        public static Number BitwiseAnd(Number left, Number right)
        {
            // Get sizes
            var leftSize = left.ByteSize;
            var rightSize = right.ByteSize;
            var resultSize = rightSize > leftSize ? rightSize : leftSize;

            // Apply bitwise operator to each byte
            var leftBytes = left._bytes;
            var rightBytes = right._bytes;
            var resultBytes = new byte[resultSize];
            for (var index = 0; index < resultSize; index++)
            {
                var leftValue = index < leftSize ? leftBytes[index] : left.Sign ? (byte)0 : (byte)0xFF;
                var rightValue = index < rightSize ? rightBytes[index] : right.Sign ? (byte)0 : (byte)0xFF;
                resultBytes[index] = (byte)(leftValue & rightValue);
            }

            // Preserve sign flag
            var resultSigned = left.Signed || right.Signed;

            // Return result
            return new Number(resultBytes, resultSigned);
        }

        /// <summary>
        /// Performs a bitwise OR on two <see cref="Number"/> values.
        /// </summary>
        public static Number BitwiseOr(Number left, Number right)
        {
            // Get sizes
            var leftSize = left.ByteSize;
            var rightSize = right.ByteSize;
            var resultSize = rightSize > leftSize ? rightSize : leftSize;

            // Apply bitwise operator to each byte
            var leftBytes = left._bytes;
            var rightBytes = right._bytes;
            var resultBytes = new byte[resultSize];
            for (var index = 0; index < resultSize; index++)
            {
                var leftValue = index < leftSize ? leftBytes[index] : left.Sign ? (byte)0 : (byte)0xFF;
                var rightValue = index < rightSize ? rightBytes[index] : right.Sign ? (byte)0 : (byte)0xFF;
                resultBytes[index] = (byte)(leftValue | rightValue);
            }

            // Preserve sign flag
            var resultSigned = left.Signed || right.Signed;

            // Return result
            return new Number(resultBytes, resultSigned);
        }

        /// <summary>
        /// Decrements a <see cref="Number"/> by one.
        /// </summary>
        public static Number Decrement(Number value)
        {
            return value - (byte)1;
        }

        /// <summary>
        /// Divides one <see cref="Number"/> by another.
        /// </summary>
        public static Number Divide(Number left, Number right)
        {
            return Divide(left, right, out _);
        }

        /// <summary>
        /// Divides one <see cref="Number"/> by another, including remainder.
        /// </summary>
        public static Number Divide(Number left, Number right, out Number remainder)
        {
            // Throw divide by zero error when any part is zero
            if (left.IsZero || right.IsZero)
                throw new DivideByZeroException();

            // Return other value when any part is one
            remainder = Zero;
            if (left == 1) return right;
            if (right == 1) return left;

            // Return negated value when any part is minus one
            if (left == -1) return -right;
            if (right == -1) return -left;

            // Calculate size and sign
            var dividendBytes = (byte[])left._bytes.Clone();
            var dividendSize = dividendBytes.Length;
            var divisorBytes = right._bytes;
            var divisorSize = dividendBytes.Length;
            var resultSigned = left.Signed || right.Signed;

            // Divide bytes with carry
            var resultBytes = Array.Empty<byte>();
            var resultSize = 0;
            uint carry = 0;
            for (var divisorIndex = divisorSize - 1; divisorIndex >= 0; divisorIndex--)
            {
                // Get each divisor byte
                var divisorValue = divisorBytes[divisorIndex];

                // Divide each byte of dividend with shifted power of divisor from MSB to LSB
                for (int dividendIndex = dividendSize - 1, resultIndex = dividendIndex; dividendIndex >= 0; dividendIndex--, resultIndex--)
                {
                    // Get remainder byte
                    var remainderValue = dividendBytes[dividendIndex];

                    // Divide with carry
                    var resultAndCarry = (carry + remainderValue);
                    var result = (byte)0;
                    carry = 0;
                    if (divisorValue != 0)
                    {
                        var digitResult = resultAndCarry / divisorValue;
                        result = (byte)digitResult;
                        var digitRemainder = resultAndCarry % divisorValue;
                        carry = digitRemainder << 8;
                        remainderValue = (byte)digitRemainder;
                    }
                    dividendBytes[dividendIndex] = remainderValue;

                    // Store result, extending when necessary
                    if (result != 0)
                    {
                        if (resultIndex >= resultSize)
                        {
                            resultSize = resultIndex + 1;
                            Array.Resize(ref resultBytes, resultSize);
                        }
                        resultBytes[resultIndex] = result;
                    }
                }
            }

            // Return result
            remainder = new Number(dividendBytes, resultSigned);
            return new Number(resultBytes, resultSigned);
        }

        /// <summary>
        /// Explicitly converts a <see cref="Number"/> to a <see cref="byte"/> (unsigned).
        /// </summary>
        public static explicit operator byte(Number value)
        {
            var byteLength = value.ByteSize;
            if (byteLength > 1 || value.Signed) throw new OverflowException();
            return byteLength == 0 ? (byte)0 : value._bytes[0];
        }

        /// <summary>
        /// Explicitly converts a <see cref="Number"/> to a <see cref="decimal"/> (signed).
        /// </summary>
        public static explicit operator decimal(Number value)
        {
            var byteLength = value.ByteSize;
            if (byteLength > 3) throw new OverflowException();

            // TODO: Support truncation with exponent for larger numbers
            if (byteLength == 0)
                return 0;
            return new decimal(value._bytes[0],
                byteLength > 1 ? value._bytes[1] : 0,
                byteLength > 2 ? value._bytes[2] : 0,
                !value.Sign, 0);
        }

        /// <summary>
        /// Explicitly converts a <see cref="Number"/> to a <see cref="int"/> (signed).
        /// </summary>
        public static explicit operator int(Number value)
        {
            var byteLength = value.ByteSize;
            if (byteLength > 4) throw new OverflowException();
            if (byteLength == 0)
                return 0;
            var result = (int)value._bytes[0];
            for (var index = 1; index <= 3; index++)
                result &= value._bytes[0] << (8 * index);
            return result;
        }

        /// <summary>
        /// Explicitly converts a <see cref="Number"/> to a <see cref="long"/> (signed).
        /// </summary>
        public static explicit operator long(Number value)
        {
            var byteLength = value.ByteSize;
            if (byteLength > 8) throw new OverflowException();
            if (byteLength == 0)
                return 0;
            var result = (long)value._bytes[0];
            for (var index = 1; index <= 7; index++)
                result &= value._bytes[0] << (8 * index);
            return result;
        }

        /// <summary>
        /// Explicitly converts a <see cref="Number"/> to a <see cref="sbyte"/> (signed).
        /// </summary>
        [CLSCompliant(false)]
        public static explicit operator sbyte(Number value)
        {
            var byteLength = value.ByteSize;
            if (byteLength > 1) throw new OverflowException();
            if (byteLength == 0)
                return 0;
            return (sbyte)value._bytes[0];
        }

        /// <summary>
        /// Explicitly converts a <see cref="Number"/> to a <see cref="short"/> (signed).
        /// </summary>
        public static explicit operator short(Number value)
        {
            var byteLength = value.ByteSize;
            if (byteLength > 1) throw new OverflowException();
            if (byteLength == 0)
                return 0;
            var result = (short)value._bytes[0];
            if (byteLength == 1)
                result &= (short)(value._bytes[0] << 8);
            return result;
        }

        /// <summary>
        /// Explicitly converts a <see cref="Number"/> to a <see cref="uint"/> (unsigned).
        /// </summary>
        [CLSCompliant(false)]
        public static explicit operator uint(Number value)
        {
            var byteLength = value.ByteSize;
            if (byteLength > 4 || value.Signed) throw new OverflowException();
            if (byteLength == 0)
                return 0;
            var result = (uint)value._bytes[0];
            for (var index = 1; index <= 3; index++)
                result &= (uint)value._bytes[0] << (8 * index);
            return result;
        }

        /// <summary>
        /// Explicitly converts a <see cref="Number"/> to a <see cref="ulong"/> (unsigned).
        /// </summary>
        [CLSCompliant(false)]
        public static explicit operator ulong(Number value)
        {
            var byteLength = value.ByteSize;
            if (byteLength > 8 || value.Signed) throw new OverflowException();
            if (byteLength == 0)
                return 0;
            var result = (ulong)value._bytes[0];
            for (var index = 1; index <= 7; index++)
                result &= (ulong)value._bytes[0] << (8 * index);
            return result;
        }

        /// <summary>
        /// Explicitly converts a <see cref="Number"/> to a <see cref="ushort"/> (unsigned).
        /// </summary>
        [CLSCompliant(false)]
        public static explicit operator ushort(Number value)
        {
            var byteLength = value.ByteSize;
            if (byteLength > 2 || value.Signed) throw new OverflowException();
            if (byteLength == 0)
                return 0;
            var result = (ushort)value._bytes[0];
            if (byteLength == 1)
                result &= (ushort)(value._bytes[0] << 8);
            return result;
        }

        /// <summary>
        /// Converts an (unsigned) <see cref="byte"/> to a <see cref="Number"/>.
        /// </summary>
        public static Number FromByte(byte value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Converts an (signed) <see cref="decimal"/> to a <see cref="Number"/>.
        /// </summary>
        public static Number FromDecimal(decimal value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Converts an (signed) <see cref="double"/> to a <see cref="Number"/>.
        /// </summary>
        public static Number FromDouble(double value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Converts an (signed) <see cref="short"/> to a <see cref="Number"/>.
        /// </summary>
        public static Number FromInt16(short value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Converts an (signed) <see cref="int"/> to a <see cref="Number"/>.
        /// </summary>
        public static Number FromInt32(int value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Converts an (signed) <see cref="long"/> to a <see cref="Number"/>.
        /// </summary>
        public static Number FromInt64(long value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Converts an (signed) <see cref="sbyte"/> to a <see cref="Number"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static Number FromSByte(sbyte value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Converts an (signed) <see cref="float"/> to a <see cref="Number"/>.
        /// </summary>
        public static Number FromSingle(float value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Converts an (unsigned) <see cref="ushort"/> to a <see cref="Number"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static Number FromUInt16(ushort value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Converts an (unsigned) <see cref="uint"/> to a <see cref="Number"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static Number FromUInt32(uint value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Converts an (unsigned) <see cref="ulong"/> to a <see cref="Number"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static Number FromUInt64(ulong value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Implicitly converts an (unsigned) <see cref="byte"/> to a <see cref="Number"/>.
        /// </summary>
        public static implicit operator Number(byte value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Implicitly converts an (signed) <see cref="sbyte"/> to a <see cref="Number"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static implicit operator Number(sbyte value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Implicitly converts an (unsigned) <see cref="ushort"/> to a <see cref="Number"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static implicit operator Number(ushort value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Implicitly converts an (signed) <see cref="short"/> to a <see cref="Number"/>.
        /// </summary>
        public static implicit operator Number(short value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Implicitly converts an (unsigned) <see cref="uint"/> to a <see cref="Number"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static implicit operator Number(uint value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Implicitly converts an (signed) <see cref="int"/> to a <see cref="Number"/>.
        /// </summary>
        public static implicit operator Number(int value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Implicitly converts an (unsigned) <see cref="ulong"/> to a <see cref="Number"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static implicit operator Number(ulong value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Implicitly converts an (signed) <see cref="long"/> to a <see cref="Number"/>.
        /// </summary>
        public static implicit operator Number(long value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Implicitly converts an (signed) <see cref="float"/> to a <see cref="Number"/>.
        /// </summary>
        public static implicit operator Number(float value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Implicitly converts an (signed) <see cref="double"/> to a <see cref="Number"/>.
        /// </summary>
        public static implicit operator Number(double value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Implicitly converts an (signed) <see cref="decimal"/> to a <see cref="Number"/>.
        /// </summary>
        public static implicit operator Number(decimal value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Increments a <see cref="Number"/> by one.
        /// </summary>
        public static Number Increment(Number value)
        {
            return value + (byte)1;
        }

        /// <summary>
        /// Performs a bitwise left shift of a <see cref="Number"/> value.
        /// </summary>
        public static Number LeftShift(Number value, int shift)
        {
            unchecked
            {
                // Return same value when no shift
                if (shift == 0) return value;

                // Return opposite shift when negative (code only shifts positive)
                if (shift < 0)
                    return value >> -shift;

                // Get sizes
                var shiftWholeBytes = shift / 8;                // Get whole bytes shifted
                var shiftBits = shift % 8;                      // Get any remaining bits shifted
                var valueSize = value.ByteSize;
                var resultSize = shiftWholeBytes + valueSize;   // Add space for whole shifted bytes
                if (shiftBits > 0) resultSize++;                // Add space for carry bits
                var resultBytes = new byte[resultSize];         // Allocate buffer

                // Apply bitwise operator to each byte
                var valueBytes = value._bytes;
                byte carry = 0;
                for (int resultIndex = shiftWholeBytes, valueIndex = 0;
                    resultIndex < resultSize; resultIndex++, valueIndex++)
                {
                    var valueByte = valueIndex < valueSize ? valueBytes[valueIndex] : (byte)0;
                    resultBytes[resultIndex] = (byte)((valueByte << shiftBits) | carry);
                    carry = (byte)(valueByte >> (8 - shiftBits));
                }

                // Return result, preserving sign
                return new Number(resultBytes, value.Signed);
            }
        }

        /// <summary>
        /// Multiplies one <see cref="Number"/> by another.
        /// </summary>
        public static Number Multiply(Number left, Number right)
        {
            unchecked
            {
                // Return zero when any part is zero
                if (left.IsZero || right.IsZero)
                    return Zero;

                // Return other value when any part is one
                if (left == 1) return right;
                if (right == 1) return left;

                // Return negated value when any part is minus one
                if (left == -1) return -right;
                if (right == -1) return -left;

                // Calculate size and sign
                var leftSize = left.ByteSize;
                var leftSigned = left.Signed;
                var leftSign = left.Sign;
                var rightSigned = right.Signed;
                var rightSign = right.Sign;
                var resultSigned = leftSigned || rightSigned;
                var leftBytes = left._bytes;
                var factorBytes = right._bytes;
                var factorSize = factorBytes.Length;

                // Multiply bytes with carry
                var resultBytes = Array.Empty<byte>();
                var resultSize = resultBytes.Length;
                ushort carry = 0;
                for (var factorIndex = 0; factorIndex < factorSize; factorIndex++)
                {
                    // Get each factor byte
                    var factorDigit = factorBytes[factorIndex];

                    // Multiply to each byte of product with shifted power
                    for (int leftIndex = 0, resultIndex = factorIndex; leftIndex < leftSize; leftIndex++, resultIndex++)
                    {
                        // Get sign extended accumulator byte
                        var resultValue = resultIndex < resultSize
                            ? resultBytes[resultIndex]
                            : leftSign ? (byte)0 : (byte)0xFF;

                        // Get product byte
                        var leftValue = leftBytes[leftIndex];

                        // Multiply with carry
                        ulong resultAndCarry;
                        if (factorDigit != 0)
                        {
                            // Add previous digit result, multiple then add carry when factor is non-zero
                            resultAndCarry = (((ulong)resultValue + leftValue) * factorDigit) + carry;
                        }
                        else
                        {
                            // Just add carry when factor digit is zero
                            resultAndCarry = (ulong)resultValue + carry;
                        }
                        carry = (ushort)(resultAndCarry >> 8);
                        resultValue = (byte)resultAndCarry;

                        // Store result, extending when necessary
                        if (resultIndex >= resultSize)
                        {
                            resultSize = resultIndex + 1;
                            Array.Resize(ref resultBytes, resultSize);
                        }
                        resultBytes[resultIndex] = resultValue;
                    }
                }

                // Extend overflow and/or sign when necessary
                var lastBitSet = resultSize > 0 && (resultBytes[resultSize - 1] & 0x80) != 0;
                if ((!resultSigned && carry != 0) ||
                    (resultSigned && (
                        (leftSign == rightSign && ((lastBitSet && carry == 0) || (!lastBitSet && carry != 0)))) ||
                     (leftSign != rightSign && ((lastBitSet && carry != 0) || (!lastBitSet && carry == 0)))))
                {
                    if (!resultSigned)
                    {
                        // Unsigned overflow
                        if ((carry & 0xff00) == 0)
                        {
                            Array.Resize(ref resultBytes, resultSize + 1);
                            resultBytes[resultSize] = (byte)carry;
                        }
                        else
                        {
                            Array.Resize(ref resultBytes, resultSize + 2);
                            resultBytes[resultSize] = (byte)carry;
                            resultBytes[resultSize + 1] = (byte)(carry >> 8);
                        }
                    }
                    else
                    {
                        // Sign extension
                        Array.Resize(ref resultBytes, resultSize + 1);
                        resultBytes[resultSize] = carry != 0
                            ? (byte)0xff
                            : (byte)0;
                    }
                }

                // Return result
                return new Number(resultBytes, resultSigned);
            }
        }

        /// <summary>
        /// Performs a negation of a <see cref="Number"/>, also known as the "two's complement".
        /// </summary>
        public static Number Negate(Number value)
        {
            // Ensure signed, convert and extend when necessary
            if (!value.Signed)
                value = value.ToSigned(true);

            // Negate signed value (NOT + 1)
            return ~value + One;
        }

        /// <summary>
        /// Performs a bitwise NOT on a <see cref="Number"/> value, also known as the "one's complement".
        /// </summary>
        public static Number OnesComplement(Number value)
        {
            unchecked
            {
                // Get sizes (ensure at least one byte for result)
                var valueSize = value.ByteSize;
                var resultSize = valueSize > 0 ? valueSize : 1;
                var resultBytes = new byte[resultSize];

                // Apply bitwise operator to each byte
                var valueBytes = value._bytes;
                for (var index = 0; index < resultSize; index++)
                    resultBytes[index] = (byte)(valueSize > index ? ~valueBytes[index] : 0);

                // Return result, preserving sign
                return new Number(resultBytes, value.Signed);
            }
        }

        /// <summary>
        /// Performs a negation of a <see cref="Number"/>, also known as the "two's complement".
        /// </summary>
        public static Number operator -(Number value)
        {
            return Negate(value);
        }

        /// <summary>
        /// Subtracts two numbers.
        /// </summary>
        public static Number operator -(Number left, Number right)
        {
            return Subtract(left, right);
        }

        /// <summary>
        /// Decrements a <see cref="Number"/> by one.
        /// </summary>
        public static Number operator --(Number value)
        {
            return Decrement(value);
        }

        /// <summary>
        /// Compares two objects of this type for equality.
        /// </summary>
        public static bool operator !=(Number left, Number right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Performs a bitwise AND on two <see cref="Number"/> values.
        /// </summary>
        public static Number operator &(Number left, Number right)
        {
            return BitwiseAnd(left, right);
        }

        /// <summary>
        /// Multiplies one <see cref="Number"/> by another.
        /// </summary>
        public static Number operator *(Number left, Number right)
        {
            return Multiply(left, right);
        }

        /// <summary>
        /// Divides one <see cref="Number"/> by another.
        /// </summary>
        public static Number operator /(Number left, Number right)
        {
            return Divide(left, right);
        }

        /// <summary>
        /// Performs a bitwise XOR on two <see cref="Number"/> values.
        /// </summary>
        public static Number operator ^(Number left, Number right)
        {
            return Xor(left, right);
        }

        /// <summary>
        /// Performs a bitwise OR on two <see cref="Number"/> values.
        /// </summary>
        public static Number operator |(Number left, Number right)
        {
            return BitwiseOr(left, right);
        }

        /// <summary>
        /// Performs a bitwise NOT on a <see cref="Number"/> value, also known as the "one's complement".
        /// </summary>
        public static Number operator ~(Number value)
        {
            return OnesComplement(value);
        }

        /// <summary>
        /// Returns the value of a <see cref="Number"/> (no change to sign).
        /// </summary>
        public static Number operator +(Number value)
        {
            return Plus(value);
        }

        /// <summary>
        /// Adds two numbers together.
        /// </summary>
        public static Number operator +(Number left, Number right)
        {
            return Add(left, right);
        }

        /// <summary>
        /// Increments a <see cref="Number"/> by one.
        /// </summary>
        public static Number operator ++(Number value)
        {
            return Increment(value);
        }

        /// <summary>
        /// Tests if one <see cref="Number"/> is less than another.
        /// </summary>
        public static bool operator <(Number left, Number right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Performs a bitwise left shift of a <see cref="Number"/> value.
        /// </summary>
        public static Number operator <<(Number value, int shift)
        {
            return LeftShift(value, shift);
        }

        /// <summary>
        /// Tests if one <see cref="Number"/> is less than or equal to another.
        /// </summary>
        public static bool operator <=(Number left, Number right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Compares two objects of this type for equality.
        /// </summary>
        public static bool operator ==(Number left, Number right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests if one <see cref="Number"/> is greater than another.
        /// </summary>
        public static bool operator >(Number left, Number right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Tests if one <see cref="Number"/> is greater than or equal to another.
        /// </summary>
        public static bool operator >=(Number left, Number right)
        {
            return left.CompareTo(right) >= 0;
        }

        /// <summary>
        /// Performs a bitwise right shift of a <see cref="Number"/> value.
        /// </summary>
        public static Number operator >>(Number value, int shift)
        {
            return RightShift(value, shift);
        }

        /// <summary>
        /// Converts a string to a signed <see cref="Number"/> of the specified base.
        /// </summary>
        /// <exception cref="FormatException">Thrown when conversion is not possible because the string value has an invalid format.</exception>
        public static Number Parse(string value, int numberBase)
        {
            return Parse(value, numberBase, true);
        }

        /// <summary>
        /// Converts a string to a signed or unsigned <see cref="Number"/> of the specified base.
        /// </summary>
        /// <exception cref="FormatException">Thrown when conversion is not possible because the string value has an invalid format.</exception>
        public static Number Parse(string value, int numberBase, bool signed)
        {
            // Parse number and return when successful
            if (TryParse(value, numberBase, out var result, signed))
                return result;

            // Throw detailed error message when failed
            var message = string.Format(CultureInfo.CurrentCulture, signed
                ? Resources.NumberParseFormatErrorSigned
                : Resources.NumberParseFormatErrorUnsigned,
                value, numberBase);
            throw new FormatException(message);
        }

        /// <summary>
        /// Returns the value of a <see cref="Number"/> (no change to sign).
        /// </summary>
        public static Number Plus(Number value)
        {
            return value;
        }

        /// <summary>
        /// Calculates the exponent, multiplying a <see cref="Number"/> by itself a number of times.
        /// </summary>
        /// <remarks>Also known as raising the <paramref name="value"/> to the specified <paramref name="exponent"/> "power". http://en.wikipedia.org/wiki/Exponentiation_by_squaring</remarks>
        public static Number Power(Number value, Number exponent)
        {
            // Return zero when value or power is zero or negative
            if (exponent.IsZero || value.IsZero || !exponent.Sign || !value.Sign)
                return Zero;

            // Return same value when one
            if (exponent == (byte)1)
                return value;

            // Perform binary exponent (square method)
            var result = One;
            while (!exponent.IsZero)    // Process each exponent bit until zero
            {
                // Multiply value by current square when bit set
                if ((exponent[0] & 1) != 0)
                {
                    result *= value;
                    exponent--;
                }

                // Square value at every bit (even if we don't use it)
                value *= value;

                // Next bit...
                exponent >>= 1;
            }

            // Return result
            return result;
        }

        /// <summary>
        /// Performs a bitwise right shift of a <see cref="Number"/> value.
        /// </summary>
        public static Number RightShift(Number value, int shift)
        {
            unchecked
            {
                // Return same value when no shift
                if (shift == 0) return value;

                // Return opposite shift when negative (code only shifts positive)
                if (shift < 0)
                    return value << -shift;

                // Get sizes (do nothing when zero)
                var shiftWholeBytes = shift / 8;                // Get whole bytes shifted
                var shiftBits = shift % 8;                      // Get any remaining bits shifted
                var valueSize = value.ByteSize;
                if (shiftWholeBytes >= valueSize)               // Return zero when entire value shifted out
                    return new Number(Array.Empty<byte>(), value.Signed);
                var resultSize = valueSize - shiftWholeBytes;   // Calculate size without whole shifted bytes
                var resultBytes = new byte[resultSize];         // Allocate buffer

                // Apply bitwise operator to each byte
                var valueBytes = value._bytes;
                byte carry = 0;
                for (int resultIndex = resultSize - 1, valueIndex = valueSize - 1;
                    resultIndex >= 0; resultIndex--, valueIndex--)
                {
                    var valueByte = valueBytes[valueIndex];
                    resultBytes[resultIndex] = (byte)((valueByte >> shiftBits) | carry);
                    carry = (byte)(valueByte & (0xff << (8 - shiftBits)));
                }

                // Return result, preserving sign
                return new Number(resultBytes, value.Signed);
            }
        }

        /// <summary>
        /// Subtracts two numbers.
        /// </summary>
        public static Number Subtract(Number left, Number right)
        {
            unchecked
            {
                // Calculate size and sign
                var leftSize = left.ByteSize;
                var rightSize = right.ByteSize;
                var leftSigned = left.Signed;
                var rightSigned = right.Signed;
                var resultSigned = leftSigned || rightSigned;
                var resultSize = rightSize > leftSize ? rightSize : leftSize;
                var leftBytes = left._bytes;
                var rightBytes = right._bytes;
                if (leftSigned != rightSigned &&
                    ((leftSize > 0 && (leftBytes[leftSize - 1] & 0x80) != 0) ||
                     (rightSize > 0 && (rightBytes[rightSize - 1] & 0x80) != 0)))
                {
                    // Extend by one byte when part unsigned and last bit used
                    resultSize++;
                }

                // Subtract bytes with borrow
                var resultBytes = new byte[resultSize];
                var leftSign = left.Sign;
                var rightSign = right.Sign;
                sbyte borrow = 0;
                for (var index = 0; index < resultSize; index++)
                {
                    // Get sign extended bytes
                    var leftValue = index < leftSize ? leftBytes[index] : leftSign ? (byte)0 : (byte)0xFF;
                    var rightValue = index < rightSize ? rightBytes[index] : rightSign ? (byte)0 : (byte)0xFF;

                    // Add with borrow
                    var resultAndBorrow = (ushort)(leftValue + borrow - rightValue);
                    borrow = (sbyte)(resultAndBorrow >> 8);
                    var result = (byte)resultAndBorrow;
                    resultBytes[index] = result;
                }

                // Extend overflow and/or sign when necessary
                var lastBitSet = (resultBytes[resultSize - 1] & 0x80) != 0;
                if ((!resultSigned && borrow == -1) ||
                    (resultSigned && (
                        (leftSign == rightSign && ((lastBitSet && borrow == 0) || (!lastBitSet && borrow == -1)))) ||
                        (leftSign != rightSign && ((lastBitSet && borrow == -1) || (!lastBitSet && borrow == 0)))))
                {
                    Array.Resize(ref resultBytes, resultSize + 1);
                    resultBytes[resultSize] = !resultSigned
                        ? (byte)borrow             // Unsigned overflow
                        : borrow != (byte)0        // Sign extension
                            ? (byte)0
                            : (byte)0xff;
                    resultSigned = true;
                }

                // Return result
                return new Number(resultBytes, resultSigned);
            }
        }

        /// <summary>
        /// Attempts conversion of a string to a signed <see cref="Number"/> of the specified base.
        /// </summary>
        public static bool TryParse(string value, int numberBase, out Number result)
        {
            return TryParse(value, numberBase, out result, true);
        }

        /// <summary>
        /// Attempts conversion of a string to a signed or unsigned <see cref="Number"/> of the specified base.
        /// </summary>
        public static bool TryParse(string value, int numberBase, out Number result, bool signed)
        {
            // Validate
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));
            if (numberBase < 2 || numberBase > 16) throw new ArgumentOutOfRangeException(nameof(numberBase));

            // Prepare result
            result = new Number(Array.Empty<byte>(), signed);

            // Decide how to handle sign according to base
            var bitBased = true;
            var bitsPerDigit = 1;
            if (numberBase > 2 && numberBase % 2 == 0)
            {
                var baseRoot = Math.Sqrt(numberBase);
                bitsPerDigit = (int)Math.Floor(baseRoot);
                bitBased = !(baseRoot - bitsPerDigit > 0);
            }
            var digitsPerByte = 8 / bitsPerDigit;

            // Detect negative when signed
            var valueLength = value.Length;
            var negative = false;
            var firstDigitIndex = 0;
            if (signed)
            {
                if (!bitBased)
                {
                    if (value[0] == '-')
                    {
                        negative = true;
                        firstDigitIndex = 1;
                    }
                }
                else
                {
                    var indexChar = value[0];
#if !NETSTANDARD2_0
                    var indexValue = NumberDigits.IndexOf(indexChar, StringComparison.OrdinalIgnoreCase);
#else
                    var indexValue = NumberDigits.IndexOf(indexChar);
#endif
                    if (indexValue == numberBase - 1)
                        negative = true;
                }
            }

            // Parse value
            var resultBytes = result._bytes;
            var lastDigitIndex = valueLength - 1;
            for (var valueIndex = lastDigitIndex; valueIndex >= firstDigitIndex; valueIndex--)
            {
                // Get next character value (return false when invalid)
                var digitChar = value[valueIndex];
#if !NETSTANDARD2_0
                var digitValueIndex = NumberDigits.IndexOf(digitChar, StringComparison.OrdinalIgnoreCase);
#else
                var digitValueIndex = NumberDigits.IndexOf(digitChar);
#endif
                if (digitValueIndex < 0)
                    return false;
                var digitValue = (byte)digitValueIndex;

                // Append to result
                var digitPower = lastDigitIndex - valueIndex;
                if (bitBased)
                {
                    // Append bits to result
                    var digitByteIndex = digitPower / digitsPerByte;
                    var digitBitIndex = (digitPower % digitsPerByte) * bitsPerDigit;
                    var requiredSize = digitByteIndex + 1;
                    if (resultBytes.Length < requiredSize)
                        Array.Resize(ref resultBytes, requiredSize);
                    resultBytes[digitByteIndex] |= digitBitIndex > 0 ? (byte)(digitValue << digitBitIndex) : digitValue;
                }
                else
                {
                    // Append value to result

                    // Multiply to the power of base value when not the last (lowest) digit
                    var digitNumber = digitPower == 0
                        ? digitValue
                        : digitPower == 1
                            ? digitValue * numberBase
                            : digitValue * Power(numberBase, digitPower);

                    // Add or subtract value to/from result depending on sign
                    if (negative)
                        result -= digitNumber;
                    else
                        result += digitNumber;
                }
            }

            // Return result
            if (bitBased)
                result = new Number(resultBytes, signed);
            return true;
        }

        /// <summary>
        /// Performs a bitwise XOR on two <see cref="Number"/> values.
        /// </summary>
        public static Number Xor(Number left, Number right)
        {
            // Get sizes
            var leftSize = left.ByteSize;
            var rightSize = right.ByteSize;
            var resultSize = rightSize > leftSize ? rightSize : leftSize;

            // Apply bitwise operator to each byte
            var leftBytes = left._bytes;
            var rightBytes = right._bytes;
            var resultBytes = new byte[resultSize];
            for (var index = 0; index < resultSize; index++)
            {
                var leftValue = index < leftSize ? leftBytes[index] : left.Sign ? (byte)0 : (byte)0xFF;
                var rightValue = index < rightSize ? rightBytes[index] : right.Sign ? (byte)0 : (byte)0xFF;
                resultBytes[index] = (byte)(leftValue ^ rightValue);
            }

            // Preserve sign flag
            var resultSigned = left.Signed || right.Signed;

            // Return result
            return new Number(resultBytes, resultSigned);
        }

        /// <summary>
        /// Compares this <see cref="Number"/> with another.
        /// </summary>
        public int CompareTo(Number other)
        {
            // Compare zero and sign quickly
            if (IsZero) return !other.IsZero ? other.Sign ? -1 : 1 : 0;
            if (other.IsZero || Sign != other.Sign) return Sign ? 1 : -1;

            // Compare value when both are same sign and non-zero
            int byteSize = ByteSize, otherByteSize = other.ByteSize;
            var maxByteSize = byteSize > otherByteSize ? byteSize : otherByteSize;
            var sign = Sign;
            var otherSign = other.Sign;
            for (var index = maxByteSize - 1; index >= 0; index--)
            {
                var value = index < byteSize ? _bytes[index] : sign ? 0 : 0xFF;
                var otherValue = index < otherByteSize ? other._bytes[index] : otherSign ? 0 : 0xFF;
                if (value != otherValue)
                {
                    // Return result using sign of most significant byte which differs
                    return (sign ? value : ~value + 1) > (otherSign ? otherValue : ~otherValue + 1) ? 1 : -1;
                }
            }

            // Return same when all bytes match
            return 0;
        }

        /// <summary>
        /// Compares this object with another.
        /// </summary>
        public override bool Equals(object something)
        {
            return something is Number other && Equals(other);
        }

        /// <summary>
        /// Compares this object with another of the same type.
        /// </summary>
        public bool Equals(Number other)
        {
            return CompareTo(other) == 0;
        }

        /// <summary>
        /// Gets a read-only reference to the internal bytes of this value.
        /// </summary>
        /// <remarks>
        /// The numeric value is stored in this byte array ordered from least significant (LSB) to most significant (MSB), i.e. the LSB is at index 0.
        /// </remarks>
        public ReadOnlyCollection<byte> GetBytes()
        {
            return new ReadOnlyCollection<byte>(_bytes);
        }

        /// <summary>
        /// Gets a hash based on the current value.
        /// </summary>
        public override int GetHashCode()
        {
            return _bytes.Aggregate(0, (current, byteValue) => current ^ byteValue);
        }

        /// <summary>
        /// Returns a <see cref="Number"/> which has been resized with sign extension when <see cref="Signed"/>.
        /// </summary>
        /// <param name="size">New size in bytes.</param>
        public Number Resize(int size)
        {
            return Resize(size, Signed);
        }

        /// <summary>
        /// Returns a <see cref="Number"/> which has been resized with optional sign extension.
        /// </summary>
        /// <param name="size">New size in bytes.</param>
        /// <param name="extendSign">When true the sign will be extended when setting to a larger value.</param>
        /// <returns>New value which is truncated or extended as specified. Any overflow is discarded.</returns>
        public Number Resize(int size, bool extendSign)
        {
            // Resize
            var oldSize = _bytes.Length;
            var resizedBytes = new byte[size];
            var signed = Signed;
            var sign = Sign;
            var extended = oldSize < size;
            Array.Copy(_bytes, resizedBytes, extended ? oldSize : size);

            // Extend sign when necessary
            if (extended && signed && extendSign)
            {
                for (var index = oldSize; index < size; index++)
                    _bytes[index] = sign ? (byte)0x00 : (byte)0xFF;
            }

            // Return result
            return new Number(resizedBytes, signed);
        }

        /// <summary>
        /// Converts the current value to a <see cref="byte"/> (unsigned).
        /// </summary>
        public byte ToByte()
        {
            var byteLength = ByteSize;
            if (byteLength > 1 || Signed) throw new OverflowException();
            return byteLength == 0 ? (byte)0 : _bytes[0];
        }

        /// <summary>
        /// Converts the current value to a <see cref="decimal"/> (signed).
        /// </summary>
        public decimal ToDecimal()
        {
            var byteLength = ByteSize;
            if (byteLength > 3) throw new OverflowException();

            // TODO: Support truncation with exponent for larger numbers
            if (byteLength == 0)
                return 0;
            return new decimal(_bytes[0],
                byteLength > 1 ? _bytes[1] : 0,
                byteLength > 2 ? _bytes[2] : 0,
                !Sign, 0);
        }

        /// <summary>
        /// Converts the current value to a <see cref="short"/> (signed).
        /// </summary>
        public short ToInt16()
        {
            var byteLength = ByteSize;
            if (byteLength > 1) throw new OverflowException();
            if (byteLength == 0)
                return 0;
            var result = (short)_bytes[0];
            if (byteLength == 1)
                result &= (short)(_bytes[0] << 8);
            return result;
        }

        /// <summary>
        /// Converts the current value to a <see cref="int"/> (signed).
        /// </summary>
        public int ToInt32()
        {
            var byteLength = ByteSize;
            if (byteLength > 4) throw new OverflowException();
            if (byteLength == 0)
                return 0;
            var result = (int)_bytes[0];
            for (var index = 1; index <= 3; index++)
                result &= _bytes[0] << (8 * index);
            return result;
        }

        /// <summary>
        /// Converts the current value to a <see cref="long"/> (signed).
        /// </summary>
        public long ToInt64()
        {
            var byteLength = ByteSize;
            if (byteLength > 8) throw new OverflowException();
            if (byteLength == 0)
                return 0;
            var result = (long)_bytes[0];
            for (var index = 1; index <= 7; index++)
                result &= _bytes[0] << (8 * index);
            return result;
        }

        /// <summary>
        /// Converts the current value to a <see cref="sbyte"/> (signed).
        /// </summary>
        [CLSCompliant(false)]
        public sbyte ToSByte()
        {
            var byteLength = ByteSize;
            if (byteLength > 1) throw new OverflowException();
            if (byteLength == 0)
                return 0;
            return (sbyte)_bytes[0];
        }

        /// <summary>
        /// Creates a signed copy of this value, optionally extending to preserve sign.
        /// </summary>
        /// <param name="extend">
        /// Set true to preserve sign by extending when converting from unsigned to signed and the most significant (sign) bit is already used by the original value.
        /// </param>
        /// <returns>New <see cref="Number"/> which is signed and has the same value.</returns>
        public Number ToSigned(bool extend)
        {
            // Return same value (copy) when already signed
            if (Signed)
                return this;

            // Return signed zero when empty
            if (_bytes == null) return Zero;
            var size = _bytes.Length;
            if (size == 0) return Zero;

            // Extend sign if necessary
            var resultSize = size;
            if (extend)
            {
                var signBit = ((_bytes[size - 1] & 0x80) != 0);
                if (signBit)
                    resultSize = size + 1;
            }

            // Copy bytes (leaving any extended byte zero)
            var signedBytes = new byte[resultSize];
            Array.Copy(_bytes, signedBytes, size);

            // Return result
            return new Number(signedBytes, true);
        }

        /// <summary>
        /// Converts the current value to a signed decimal string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // Call overloaded method with defaults
            return ToString(10, 0);
        }

        /// <summary>
        /// Converts the number to a string of the specified base.
        /// </summary>
        public string ToString(int numberBase)
        {
            return ToString(numberBase, 0);
        }

        /// <summary>
        /// Converts the number to a string of the specified base.
        /// </summary>
        public string ToString(int numberBase, int minimumLength)
        {
            unchecked
            {
                // Validate
                if (numberBase < 2 || numberBase > 16) throw new ArgumentOutOfRangeException(nameof(numberBase));
                if (minimumLength < 0) throw new ArgumentOutOfRangeException(nameof(minimumLength));
                var signed = Signed;
                var negative = !Sign;

                // Decide how to handle sign according to base
                var bitBased = true;
                var bitsPerDigit = 1;
                if (numberBase > 2 && numberBase % 2 == 0)
                {
                    var baseRoot = Math.Sqrt(numberBase);
                    bitsPerDigit = (int)Math.Floor(baseRoot);
                    bitBased = !(baseRoot - bitsPerDigit > 0);
                }
                var minimumDigitsPerByte = bitBased ? (int)Math.Ceiling(8d / bitsPerDigit) : 0;
                if (minimumDigitsPerByte > 0 && minimumLength > 0)
                    minimumLength = (int)Math.Ceiling((double)minimumLength / minimumDigitsPerByte) * minimumDigitsPerByte;

                // Process whole number...
                var result = new StringBuilder(minimumLength);
                var negate = false;
                if (negative && !bitBased)
                {
                    // Just prepend negative sign to positive number for non-bit based values
                    result.Append('-');
                    negate = true;
                }
                var firstDigitIndex = result.Length;
                var byteLength = _bytes.Length;
                var digitValue = 0;
                var digits = 0;
                var negateCarry = (sbyte)0;
                for (var byteIndex = 0; byteIndex < byteLength; byteIndex++)
                {
                    // Get value (negating when necessary)
                    var byteValue = _bytes[byteIndex];
                    if (negate)
                    {
                        var negateAndCarry = -byteValue + negateCarry;
                        byteValue = (byte)negateAndCarry;
                        negateCarry = (sbyte)(negateAndCarry >> 8);
                    }

                    // Skip unnecessary byte
                    if (byteValue == 0 && digits >= minimumLength)
                        continue;

                    // Divide into string of digits...
                    var digitByteValue = byteValue;
                    var digitsThisByte = 0;
                    do
                    {
                        // Divide byte value/dividend by base to extract digit value as remainder
                        var baseDividend = decimal.Floor(digitByteValue / (decimal)numberBase);
                        var baseRemainder = digitByteValue - numberBase * baseDividend;
                        digitValue = (int)baseRemainder;
                        digitByteValue = (byte)baseDividend;

                        // Use remainder as digit from right to left
                        result.Insert(firstDigitIndex, new[] { NumberDigits[digitValue] });
                        digits++;
                        digitsThisByte++;
                    } while (digitByteValue > 0 || digitsThisByte < minimumDigitsPerByte);
                }

                // Add padding at last position of bit based values when necessary
                if (signed && bitBased)
                {
                    if (negative)
                    {
                        // Ensure negative binary values have at least 2 bits (one for sign)
                        if (bitsPerDigit == 1 && result.Length == 1 && minimumLength < 2)
                            minimumLength = 2;
                    }
                    else
                    {
                        // Pad with zero when non-negative but MSB set in value
                        var negativeBitValue = 1 << (bitsPerDigit - 1);
                        var negativeBitSet = (negativeBitValue & digitValue) != 0;
                        if (negativeBitSet)
                        {
                            var positiveWidth = result.Length - firstDigitIndex + minimumDigitsPerByte;
                            if (minimumLength < positiveWidth)
                                minimumLength = positiveWidth;
                        }
                    }
                }

                // Pad string to desired width
                while (result.Length - firstDigitIndex < minimumLength)
                    result.Insert(firstDigitIndex, new[] { negative && !negate ? NumberDigits[numberBase - 1] : NumberDigits[0] });

                // Return result
                return result.ToString();
            }
        }

        /// <summary>
        /// Converts the current value to a <see cref="ushort"/> (unsigned).
        /// </summary>
        [CLSCompliant(false)]
        public ushort ToUInt16()
        {
            var byteLength = ByteSize;
            if (byteLength > 2 || Signed) throw new OverflowException();
            if (byteLength == 0)
                return 0;
            var result = (ushort)_bytes[0];
            if (byteLength == 1)
                result &= (ushort)(_bytes[0] << 8);
            return result;
        }

        /// <summary>
        /// Converts the current value to a <see cref="uint"/> (unsigned).
        /// </summary>
        [CLSCompliant(false)]
        public uint ToUInt32()
        {
            var byteLength = ByteSize;
            if (byteLength > 4 || Signed) throw new OverflowException();
            if (byteLength == 0)
                return 0;
            var result = (uint)_bytes[0];
            for (var index = 1; index <= 3; index++)
                result &= (uint)_bytes[0] << (8 * index);
            return result;
        }

        /// <summary>
        /// Converts the current value to a <see cref="ulong"/> (unsigned).
        /// </summary>
        [CLSCompliant(false)]
        public ulong ToUInt64()
        {
            var byteLength = ByteSize;
            if (byteLength > 8 || Signed) throw new OverflowException();
            if (byteLength == 0)
                return 0;
            var result = (ulong)_bytes[0];
            for (var index = 1; index <= 7; index++)
                result &= (ulong)_bytes[0] << (8 * index);
            return result;
        }

        /// <summary>
        /// Creates an unsigned copy of this value.
        /// </summary>
        /// <param name="positiveOnly">Set true to ensure a positive result by returning zero when negative.</param>
        /// <returns>
        /// New <see cref="Number"/> which is unsigned and has the same value when positive, and optionally the same bit value (but different interpreted value)
        /// or zero when negative (according to the <see paramref="positiveOnly"/> option).
        /// </returns>
        public Number ToUnsigned(bool positiveOnly)
        {
            // Return save value (copy) when already unsigned
            if (!Signed)
                return this;

            // Return unsigned zero when empty
            if (_bytes == null) return new Number((byte)0);
            var size = _bytes.Length;
            if (size == 0) return new Number((byte)0);

            // Return zero when negative and positive only option set
            if (positiveOnly && !Sign)
                return new Number((byte)0);

            // Return unsigned copy
            return new Number(_bytes, false);
        }

        #endregion Public Methods
    }
}
