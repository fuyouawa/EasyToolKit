using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a collection of child properties for an InspectorProperty.
    /// Provides access to child properties by index or name and supports recursive traversal.
    /// </summary>
    public class PropertyChildren : IDisposable
    {
        private InspectorProperty _property;
        private readonly Dictionary<int, InspectorProperty> _childByIndex = new Dictionary<int, InspectorProperty>();
        private readonly Dictionary<int, string> _childPathByIndex = new Dictionary<int, string>();

        /// <summary>
        /// Gets the number of child properties.
        /// </summary>
        public int Count => _property.ChildrenResolver.ChildCount;

        /// <summary>
        /// Gets the child property at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the child property to get.</param>
        /// <returns>The child property at the specified index.</returns>
        public InspectorProperty this[int index] => Get(index);

        /// <summary>
        /// Gets the child property with the specified name.
        /// </summary>
        /// <param name="name">The name of the child property to get.</param>
        /// <returns>The child property with the specified name.</returns>
        public InspectorProperty this[string name] => Get(name);

        /// <summary>
        /// Initializes a new instance of the PropertyChildren class.
        /// </summary>
        /// <param name="property">The parent property whose children are being managed.</param>
        internal PropertyChildren(InspectorProperty property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            _property = property;
        }

        /// <summary>
        /// Gets the child property at the specified index.
        /// </summary>
        /// <param name="childIndex">The zero-based index of the child property.</param>
        /// <returns>The child property at the specified index.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when the index is out of range.</exception>
        public InspectorProperty Get(int childIndex)
        {
            if (childIndex < 0 || childIndex > Count)
            {
                throw new IndexOutOfRangeException();
            }

            if (_childByIndex.TryGetValue(childIndex, out var child))
            {
                child.Update();
                return child;
            }

            child = new InspectorProperty(_property.Tree, _property, _property.ChildrenResolver.GetChildInfo(childIndex), childIndex);
            _childByIndex[childIndex] = child;
            child.Update();
            return child;
        }

        /// <summary>
        /// Gets the child property with the specified name.
        /// </summary>
        /// <param name="name">The name of the child property.</param>
        /// <returns>The child property with the specified name, or null if no child exists with that name.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the name is null.</exception>
        /// <exception cref="ArgumentException">Thrown when no child property exists with the specified name.</exception>
        public InspectorProperty Get([NotNull] string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (Count == 0)
                return null;

            var index = _property.ChildrenResolver.ChildNameToIndex(name);
            if (index < 0)
            {
                throw new ArgumentException($"The property '{_property.Path}' has no child with name '{name}'!");
            }
            return Get(index);
        }

        /// <summary>
        /// Gets the full path of the child property at the specified index.
        /// </summary>
        /// <param name="childIndex">The zero-based index of the child property.</param>
        /// <returns>The full path of the child property.</returns>
        public string GetPath(int childIndex)
        {
            if (_childPathByIndex.TryGetValue(childIndex, out var path))
            {
                return path;
            }

            var info = _property.ChildrenResolver.GetChildInfo(childIndex);
            path = _property.Path + "." + info.PropertyName;
            _childPathByIndex[childIndex] = path;
            return path;
        }

        /// <summary>
        /// Updates the children collection. Currently does nothing but reserved for future functionality.
        /// </summary>
        internal void Update()
        {
        }

        /// <summary>
        /// Recursively enumerates all child properties and their descendants.
        /// </summary>
        /// <returns>An enumerable collection of all descendant properties.</returns>
        public IEnumerable<InspectorProperty> Recurse()
        {
            for (var i = 0; i < Count; i++)
            {
                var child = this[i];
                yield return child;

                if (child.Children != null)
                {
                    foreach (var subChild in child.Children.Recurse())
                    {
                        yield return subChild;
                    }
                }
            }
        }

        /// <summary>
        /// Clears all cached child properties and their paths.
        /// </summary>
        public void Clear()
        {
            foreach (var child in _childByIndex)
            {
                child.Value.Dispose();
            }
            _childByIndex.Clear();
            _childPathByIndex.Clear();
        }

        /// <summary>
        /// Releases all resources used by the PropertyChildren instance.
        /// </summary>
        public void Dispose()
        {
            Clear();
            _property = null;
        }
    }
}
