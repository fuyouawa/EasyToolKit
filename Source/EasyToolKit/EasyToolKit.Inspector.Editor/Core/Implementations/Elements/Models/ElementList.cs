using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Provides a mutable collection of elements in the inspector tree that supports adding, removing, and reordering.
    /// Maintains cached elements and paths for efficient access, with change notifications via events.
    /// </summary>
    /// <typeparam name="TElement">The type of elements in this collection.</typeparam>
    public class ElementList<TElement> : IElementList<TElement>, IDisposable
        where TElement : IElement
    {
        private const string NamePathSeparator = ".";

        private bool _disposed;
        private readonly IElement _ownerElement;

        private readonly List<TElement> _elements;
        private readonly Dictionary<string, int> _nameToIndex;
        private readonly Dictionary<int, string> _pathByIndex;

        /// <summary>
        /// Occurs before the element list is changed.
        /// </summary>
        public event EventHandler<ElementMovedEventArgs> BeforeElementMoved;

        /// <summary>
        /// Occurs after the element list is changed.
        /// </summary>
        public event EventHandler<ElementMovedEventArgs> AfterElementMoved;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementList{TElement}"/> class.
        /// </summary>
        /// <param name="ownerElement">The element that owns this list.</param>
        public ElementList([NotNull] IElement ownerElement)
        {
            _ownerElement = ownerElement ?? throw new ArgumentNullException(nameof(ownerElement));
            _elements = new List<TElement>();
            _nameToIndex = new Dictionary<string, int>();
            _pathByIndex = new Dictionary<int, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementList{TElement}"/> class with optional initial elements.
        /// </summary>
        /// <param name="ownerElement">The element that owns this list.</param>
        /// <param name="initialElements">The optional initial elements to populate the list.</param>
        public ElementList([NotNull] IElement ownerElement, [CanBeNull] IEnumerable<TElement> initialElements)
            : this(ownerElement)
        {
            if (initialElements == null)
            {
                return;
            }

            var index = 0;
            foreach (var element in initialElements)
            {
                if (element == null)
                {
                    continue;
                }

                _elements.Add(element);

                var name = element.Definition?.Name;
                if (!string.IsNullOrEmpty(name))
                {
                    _nameToIndex[name] = index;
                }

                index++;
            }
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                ValidateDisposed();
                return _elements.Count;
            }
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        public TElement this[int index] => GetElement(index);

        /// <summary>
        /// Gets the element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to get.</param>
        /// <returns>The element with the specified name.</returns>
        public TElement this[string name] => GetElement(name);

        /// <summary>
        /// Gets the element that owns this list.
        /// </summary>
        public IElement OwnerElement => _ownerElement;

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when index is out of range.</exception>
        public TElement GetElement(int index)
        {
            ValidateDisposed();
            ValidateIndex(index);
            var element = _elements[index];
            element.Update();
            return element;
        }

        /// <summary>
        /// Gets the element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to get.</param>
        /// <returns>The element with the specified name.</returns>
        /// <exception cref="ArgumentNullException">Thrown when name is null.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when no element with the specified name exists.</exception>
        public TElement GetElement([NotNull] string name)
        {
            ValidateDisposed();
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var index = IndexOf(name);
            if (index < 0)
            {
                throw new KeyNotFoundException($"The element list does not contain an element with name '{name}'.");
            }

            return GetElement(index);
        }

        /// <summary>
        /// Inserts an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert the element.</param>
        /// <param name="element">The element to insert.</param>
        /// <exception cref="ArgumentNullException">Thrown when element is null.</exception>
        public virtual void Insert(int index, TElement element)
        {
            ValidateDisposed();
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            ValidateInsertIndex(index);

            using (var eventArgs = ElementMovedEventArgs.Create(ElementListChangeType.Insert, element, index, element.Parent, _ownerElement, ElementMovedTiming.Before))
            {
                OnBeforeElementChanged(eventArgs);
            }

            _elements.Insert(index, element);

            UpdateNameIndexAfterInsert(index, element);
            InvalidatePathCacheFrom(index);

            var postArgs = ElementMovedEventArgs.Create(ElementListChangeType.Insert, element, index, element.Parent, _ownerElement, ElementMovedTiming.After);
            try
            {
                OnAfterElementChanged(postArgs);

                // Directly call OnElementMoved for the inserted element and its old parent
                // This avoids O(n²) complexity where all elements process all move events
                DirectNotifyElementMoved(element, postArgs);
            }
            finally
            {
                postArgs.Dispose();
            }
        }

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when index is out of range.</exception>
        public virtual void RemoveAt(int index)
        {
            ValidateDisposed();
            ValidateIndex(index);

            var element = _elements[index];
            using (var eventArgs = ElementMovedEventArgs.Create(ElementListChangeType.Remove, element, index, _ownerElement, null, ElementMovedTiming.Before))
            {
                OnBeforeElementChanged(eventArgs);
            }

            RemoveNameIndex(element);
            _elements.RemoveAt(index);

            InvalidatePathCacheFrom(index);

            var postArgs = ElementMovedEventArgs.Create(ElementListChangeType.Remove, element, index, _ownerElement, null, ElementMovedTiming.After);
            try
            {
                OnAfterElementChanged(postArgs);

                // Directly call OnElementMoved for the removed element
                // This avoids O(n²) complexity where all elements process all move events
                DirectNotifyElementMoved(element, postArgs);
            }
            finally
            {
                postArgs.Dispose();
            }
        }

        public virtual void Clear()
        {
            ValidateDisposed();
            _elements.Clear();
            _nameToIndex.Clear();
            _pathByIndex.Clear();
        }

        /// <summary>
        /// Gets the zero-based index of the first occurrence of an element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to locate.</param>
        /// <returns>The zero-based index of the first occurrence of the element, or -1 if not found.</returns>
        public int IndexOf(string name)
        {
            ValidateDisposed();
            if (name == null)
            {
                return -1;
            }

            UpdateNameIndexIfNeeded();

            return _nameToIndex.TryGetValue(name, out var index) ? index : -1;
        }

        /// <summary>
        /// Gets the zero-based index of the specified element.
        /// </summary>
        /// <param name="element">The element to locate.</param>
        /// <returns>The zero-based index of the element, or -1 if not found.</returns>
        public int IndexOf(TElement element)
        {
            ValidateDisposed();
            return _elements.IndexOf(element);
        }

        /// <summary>
        /// Gets the full path of the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element.</param>
        /// <returns>The full path of the element.</returns>
        public string GetPath(int index)
        {
            ValidateDisposed();
            ValidateIndex(index);

            if (_pathByIndex.TryGetValue(index, out var path))
            {
                return path;
            }

            var element = _elements[index];
            path = ComputePath(index, element);
            _pathByIndex[index] = path;
            return path;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        public IEnumerator<TElement> GetEnumerator()
        {
            ValidateDisposed();
            return _elements.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            ValidateDisposed();
            return GetEnumerator();
        }

        /// <summary>
        /// Raises the <see cref="BeforeElementMoved"/> event.
        /// </summary>
        /// <param name="args">The event arguments containing change details.</param>
        protected virtual void OnBeforeElementChanged(ElementMovedEventArgs args)
        {
            BeforeElementMoved?.Invoke(_ownerElement, args);
        }

        /// <summary>
        /// Raises the <see cref="AfterElementMoved"/> event.
        /// </summary>
        /// <param name="args">The event arguments containing change details.</param>
        protected virtual void OnAfterElementChanged(ElementMovedEventArgs args)
        {
            AfterElementMoved?.Invoke(_ownerElement, args);
        }

        /// <summary>
        /// Directly notifies the moved element and its old parent about the move operation.
        /// This targeted notification avoids the O(n²) complexity of broadcasting to all elements.
        /// </summary>
        /// <param name="element">The element that was moved.</param>
        /// <param name="args">The event arguments containing move details.</param>
        private void DirectNotifyElementMoved(TElement element, ElementMovedEventArgs args)
        {
            // Notify the moved element itself (it will update its Parent property)
            element.Send(ElementMessageNames.ElementMoved, args);

            // Notify the old parent if different from current owner (it will remove from its Children)
            var oldParent = args.OldParent;
            if (oldParent != null && oldParent != _ownerElement)
            {
                oldParent.Send(ElementMessageNames.ElementMoved, args);
            }
        }

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range. Collection contains {Count} elements.");
            }
        }

        private void ValidateInsertIndex(int index)
        {
            if (index < 0 || index > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Insert index {index} is out of range. Collection contains {Count} elements.");
            }
        }

        private void UpdateNameIndexIfNeeded()
        {
            if (_nameToIndex.Count == Count)
            {
                return;
            }

            for (var i = 0; i < Count; i++)
            {
                var element = _elements[i];
                var name = element.Definition.Name;
                _nameToIndex.TryAdd(name, i);
            }
        }

        private void UpdateNameIndexAfterInsert(int index, TElement element)
        {
            var name = element.Definition.Name;
            _nameToIndex[name] = index;

            for (var i = index + 1; i < Count; i++)
            {
                var elemName = _elements[i].Definition.Name;
                if (_nameToIndex.TryGetValue(elemName, out var oldIndex) && oldIndex == i - 1)
                {
                    _nameToIndex[elemName] = i;
                }
            }
        }

        private void RemoveNameIndex(TElement element)
        {
            var name = element.Definition.Name;
            _nameToIndex.Remove(name);

            var index = _elements.IndexOf(element);
            for (var i = index + 1; i < Count; i++)
            {
                var elemName = _elements[i].Definition.Name;
                if (_nameToIndex.TryGetValue(elemName, out var oldIndex) && oldIndex == i)
                {
                    _nameToIndex[elemName] = i - 1;
                }
            }
        }

        private void InvalidatePathCacheFrom(int index)
        {
            var keysToRemove = new List<int>();

            foreach (var kvp in _pathByIndex)
            {
                if (kvp.Key >= index)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _pathByIndex.Remove(key);
            }
        }

        private string ComputePath(int index, IElement element)
        {
            return _ownerElement.Path + NamePathSeparator + element.Definition.Name;
        }

        private void ValidateDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        void IDisposable.Dispose()
        {
            if (_disposed)
                return;

            _elements.Clear();
            _nameToIndex.Clear();
            _pathByIndex.Clear();
            _disposed = true;
        }
    }
}
