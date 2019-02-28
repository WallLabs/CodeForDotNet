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
        #region Public Properties

        /// <summary>
        /// Unique identifier used to group reports from the same source.
        /// </summary>
        [DataMember(IsRequired = true)]
        public Guid Id { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Tests two objects of this type for in-equality by value.
        /// </summary>
        public static bool operator !=(GuidDataKey left, GuidDataKey right)
        {
            return !(left?.Equals(right) ?? right is null);
        }

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(GuidDataKey left, GuidDataKey right)
        {
            return left?.Equals(right) ?? right is null;
        }

        /// <summary>
        /// Compares this object with another by value.
        /// </summary>
        public override bool Equals(object unknown)
        {
            // Compare null and type
            if (!(unknown is GuidDataKey other) || other is null)
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

        #endregion Public Methods
    }
}
