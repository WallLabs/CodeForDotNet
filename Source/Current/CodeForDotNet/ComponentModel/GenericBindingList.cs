using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CodeForDotNet.ComponentModel
{
    /// <summary>
    /// Generic bindable collection of items.
    /// </summary>
    /// <remarks>
    /// To create a binding list of any type, inherit this class passing a key comparing function to the constructor.
    /// The <see cref="DataObjectAttribute"/> and <see cref="SerializableAttribute"/> must still be defined on your class.
    /// </remarks>
    [Serializable]
    public abstract class GenericBindingList<T> : BindingList<T>
    {
        #region Lifetime

        /// <summary>
        /// Key comparer function used to lookup items in the list, e.g. (x, y) => x.Id == y.Id
        /// </summary>
        private readonly Func<T, T, bool> _keyComparer;

        /// <summary>
        /// Creates an instance based on an existing list.
        /// </summary>
        /// <param name="list">Optional items to add to the list.</param>
        /// <param name="keyComparer">Key comparer used to lookup items in the list, e.g. (x, y) => x.Id == y.Id.</param>
        protected GenericBindingList(IEnumerable<T> list, Func<T, T, bool> keyComparer)
        {
            // Initialize members
            _keyComparer = keyComparer;

            // Add existing items when specified
            if (list != null)
                AddRange(list);
        }

        #endregion

        #region ObjectDataSource methods

        /// <summary>
        /// Inserts an item.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert)]
        public void Insert(T item)
        {
            Add(item);
        }

        /// <summary>
        /// Selects all items.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<T> Select()
        {
            var result = new List<T>();
            result.AddRange(this);
            return result;
        }

        /// <summary>
        /// Updates an item.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update)]
        public void Update(T item)
        {
            var index = IndexOfEntity(item);
            if (index < 0) return;
            RemoveAt(index);
            Insert(index, item);
        }

        /// <summary>
        /// Deletes an item.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Delete)]
        public void Delete(T item)
        {
            var index = IndexOfEntity(item);
            if (index >= 0)
                RemoveAt(index);
        }

        #endregion

        #region BindingList Methods

        /// <summary>
        /// Adds a range of items.
        /// </summary>
        private void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

        /// <summary>
        /// Gets the index of an item.
        /// </summary>
        private int IndexOfEntity(T match)
        {
            for (var i = 0; i < Count; i++)
            {
                var item = this[i];
                if (_keyComparer(item, match))
                    return i;
            }
            return -1;
        }

        #endregion
    }
}
