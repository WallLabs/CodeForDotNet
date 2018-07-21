﻿using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace CodeForDotNet.Runtime.Serialization
{
    /// <summary>
    /// Extensions for working with <see cref="DataContractJsonSerializer"/>.
    /// </summary>
    public static class DataContractJsonSerializerExtensions
    {
        /// <summary>
        /// Serializes this object as a JSON string.
        /// </summary>
        public static string SerializeJsonString<T>(this T value)
        {
            // Validate
            if (ReferenceEquals(value, null)) throw new ArgumentNullException("value");

            // Initialize serializer
            var serializer = new DataContractJsonSerializer(typeof(T));

            // Serializer to buffer
            using (var buffer = new MemoryStream())
            {
                serializer.WriteObject(buffer, value);

                // Return buffer as string
                return Encoding.UTF8.GetString(buffer.ToArray(), 0, (int)buffer.Length);
            }
        }

        /// <summary>
        /// De-serializes an object from a JSON string.
        /// </summary>
        public static T DeserializeJson<T>(this string value)
        {
            // Validate
            if (String.IsNullOrWhiteSpace("value")) throw new ArgumentNullException("value");

            // Initialize serializer
            var serializer = new DataContractJsonSerializer(typeof(T));

            // De-serialize and return
            using (var buffer = new MemoryStream(Encoding.UTF8.GetBytes(value)))
                return (T)serializer.ReadObject(buffer);
        }
    }
}