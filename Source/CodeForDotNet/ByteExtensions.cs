using System;

namespace CodeForDotNet
{
    /// <summary>
    /// Contains extensions for working with the <see cref="byte"/> structure.
    /// </summary>
    public static class ByteExtensions
    {
        /// <summary>
        /// Returns a value with all bits reversed.
        /// </summary>
        public static byte Reverse(this byte value)
        {
            return (byte)(
                ((0x80 /* 10000000 */ & value) >> 7 /* 00000001 */) |
                ((0x40 /* 01000000 */ & value) >> 5 /* 00000010 */) |
                ((0x20 /* 00100000 */ & value) >> 3 /* 00000100 */) |
                ((0x10 /* 00010000 */ & value) >> 1 /* 00001000 */) |
                ((0x08 /* 00001000 */ & value) << 1 /* 00010000 */) |
                ((0x04 /* 00000100 */ & value) << 3 /* 00100000 */) |
                ((0x02 /* 00000010 */ & value) << 5 /* 01000000 */) |
                ((0x01 /* 00000001 */ & value) << 7 /* 10000000 */));
        }
    }
}
