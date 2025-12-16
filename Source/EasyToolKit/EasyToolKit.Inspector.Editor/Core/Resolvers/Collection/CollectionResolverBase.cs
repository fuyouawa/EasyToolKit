using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Base class for resolving collection properties in the inspector.
    /// Provides common functionality for handling collections with deferred change operations.
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection (must implement <see cref="IEnumerable{TElement}"/>).</typeparam>
    /// <typeparam name="TElement">The type of elements in the collection.</typeparam>
    public abstract class CollectionResolverBase<TCollection, TElement> : PropertyResolver, ICollectionResolver
        where TCollection : IEnumerable<TElement>
    {
        private Action _changeAction;
        private int _lastIsReadOnlyUpdateId;
        private bool _isReadOnly;
        private IPropertyValueEntry<TCollection> _valueEntry;

        /// <summary>
        /// Gets the property value entry for the collection, with lazy initialization.
        /// </summary>
        public IPropertyValueEntry<TCollection> ValueEntry
        {
            get
            {
                if (_valueEntry == null)
                {
                    _valueEntry = Property.ValueEntry as IPropertyValueEntry<TCollection>;
                    if (_valueEntry == null)
                    {
                        Property.Update(true);
                        _valueEntry = Property.ValueEntry as IPropertyValueEntry<TCollection>;
                    }
                }

                return _valueEntry;
            }
        }

        /// <summary>
        /// Gets whether the collection is read-only, with caching for performance.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                if (_lastIsReadOnlyUpdateId != Property.Tree.UpdateId)
                {
                    _lastIsReadOnlyUpdateId = Property.Tree.UpdateId;
                    _isReadOnly = false;

                    for (int i = 0; i < ValueEntry.Values.Count; i++)
                    {
                        var collection = ValueEntry.Values[i];
                        if (IsReadOnlyCollection(collection))
                        {
                            _isReadOnly = true;
                            break;
                        }
                    }
                }

                return _isReadOnly;
            }
        }

        /// <summary>
        /// Gets the type of the collection.
        /// </summary>
        public Type CollectionType => typeof(TCollection);

        /// <summary>
        /// Gets the type of elements in the collection.
        /// </summary>
        public Type ElementType => typeof(TElement);

        /// <summary>
        /// Queues an element to be added to the collection at the specified target index.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="value">The value to add to the collection.</param>
        public void QueueAddElement(int targetIndex, object value)
        {
            EnqueueChange(() => AddElement(targetIndex, (TElement)value));
        }

        /// <summary>
        /// Queues an element to be removed from the collection at the specified target index.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="value">The value to remove from the collection.</param>
        public void QueueRemoveElement(int targetIndex, object value)
        {
            EnqueueChange(() => RemoveElement(targetIndex, (TElement)value));
        }

        /// <summary>
        /// Abstract method to add an element to the collection at the specified target index.
        /// Must be implemented by derived classes.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="value">The value to add to the collection.</param>
        protected abstract void AddElement(int targetIndex, TElement value);

        /// <summary>
        /// Abstract method to remove an element from the collection at the specified target index.
        /// Must be implemented by derived classes.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="value">The value to remove from the collection.</param>
        protected abstract void RemoveElement(int targetIndex, TElement value);

        /// <summary>
        /// Enqueues a change action to be applied during the next repaint cycle.
        /// </summary>
        /// <param name="action">The action to execute when changes are applied.</param>
        protected void EnqueueChange(Action action)
        {
            _changeAction += action;
            Property.Tree.QueueCallbackUntilRepaint(() => { Property.Tree.SetPropertyDirty(Property); });
        }

        /// <summary>
        /// Applies all queued changes to the collection.
        /// </summary>
        /// <returns>True if changes were applied, false if there were no pending changes.</returns>
        protected override bool ApplyChanges()
        {
            if (_changeAction != null)
            {
                if (Property.Tree.Targets[0] is UnityEngine.Object)
                {
                    foreach (UnityEngine.Object target in Property.Tree.Targets)
                    {
                        Undo.RecordObject(target, $"Change {Property.Path} on {target.name}");
                    }
                }

                _changeAction();
                _changeAction = null;

                Property.Update();
                foreach (var child in Property.Children.Recurse())
                {
                    child.Update();
                }

                Property.Children.Clear();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if a specific collection instance is read-only.
        /// Can be overridden by derived classes to provide custom read-only detection.
        /// </summary>
        /// <param name="collection">The collection instance to check.</param>
        /// <returns>True if the collection is read-only, false otherwise.</returns>
        protected virtual bool IsReadOnlyCollection(TCollection collection) => false;
    }
}
