using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Defines a handler for collection change events with before/after notifications.
    /// </summary>
    public interface ICollectionChangeHandler : IValueChangeHandler
    {
        /// <summary>
        /// Occurs before a collection is changed.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs> BeforeCollectionChanged;

        /// <summary>
        /// Occurs after a collection has been changed.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs> AfterCollectionChanged;
    }
}
