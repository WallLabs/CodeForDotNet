using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace CodeForDotNet.Drawing
{
    /// <summary>
    /// Collection of <see cref="Color"/> entries
    /// </summary>
    public class ColorCollection : Collection<Color>
    {
        /// <summary>
        /// Creates an empty instance.
        /// </summary>
        public ColorCollection()
            : base()
        {
        }

        /// <summary>
        /// Creates an instance based on an existing list.
        /// </summary>
        /// <param name="list"></param>
        public ColorCollection(IList<Color> list)
            : base(list)
        {
        }
    }
}
