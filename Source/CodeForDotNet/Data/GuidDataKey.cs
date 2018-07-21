using System;
using System.Runtime.Serialization;

namespace CodeForDotNet.Data
{
    /// <summary>
    /// Error data used to report failures to a service.
    /// </summary>
    [DataContract]
    public class GuidDataKey
    {
        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(GuidDataKey source, GuidDataKey target)
        {
            return !ReferenceEquals(source, null)
                       ? source.Equals(target)
                       : ReferenceEquals(target, null);
        }

        /// <summary>
        /// Tests two objects of this type for in-equality by value.
        /// </summary>
        public static bool operator !=(GuidDataKey source, GuidDataKey target)
        {
            return !ReferenceEquals(source, null)
                       ? !source.Equals(target)
                       : !ReferenceEquals(target, null);
        }

        /// <summary>
        /// Compares this object with another by value.
        /// </summary>
        public override bool Equals(object obj)
        {
            // Compare null and type
            var other = obj as GuidDataKey;
            if (ReferenceEquals(other, null))
                return false;

            return
                other.Id == Id;
        }

        /// <summary>
        /// Gets an XOR based hash code based on the contents of this object.
        /// </summary>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Unique identifier used to group reports from the same source.
        /// </summary>
        [DataMember(IsRequired = true)]
        public Guid Id { get; set; }

        #endregion
    }
}
