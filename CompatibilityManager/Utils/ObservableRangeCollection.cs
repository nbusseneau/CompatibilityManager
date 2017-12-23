using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public class ObservableRangeCollection<T> : ObservableCollection<T>
    {
        private const string CountName = nameof(Count);
        private const string ItemArrayName = "Item[]";

        /// <summary>
        /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection<T> class.
        /// </summary>
        public ObservableRangeCollection() : base() { }

        /// <summary>
        /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection<T> class that contains elements copied from the specified list.
        /// </summary>
        /// <param name="list">The list from which the elements are copied.</param>
        /// <exception cref="System.ArgumentNullException">The list parameter cannot be null.</exception>
        public ObservableRangeCollection(List<T> list) : base(list) { }

        /// <summary>
        /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection<T> class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param>
        /// <exception cref="System.ArgumentNullException">The collection parameter cannot be null.</exception>
        public ObservableRangeCollection(IEnumerable<T> collection) : base(collection) { }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the ObservableCollection<T>.
        /// </summary>
        public void AddRange(IEnumerable<T> collection)
        {
            // Safeguards
            if (collection == null) { throw new ArgumentNullException(nameof(collection)); }
            if (!collection.Any()) { return; }

            this.CheckReentrancy();

            foreach (var item in collection) { Items.Add(item); }

            this.OnPropertyChanged(new PropertyChangedEventArgs(CountName));
            this.OnPropertyChanged(new PropertyChangedEventArgs(ItemArrayName));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Removes all elements from the ObservableRangeCollection<T>.
        /// </summary>
        public new void ClearItems()
        {
            base.ClearItems();
        }
    }
}