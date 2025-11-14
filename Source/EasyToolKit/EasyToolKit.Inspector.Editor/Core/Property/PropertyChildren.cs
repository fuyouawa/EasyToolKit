using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    public class PropertyChildren : IDisposable
    {
        private InspectorProperty _property;
        private readonly Dictionary<int, InspectorProperty> _childByIndex = new Dictionary<int, InspectorProperty>();
        private readonly Dictionary<int, string> _childPathByIndex = new Dictionary<int, string>();

        public int Count => _property.ChildrenResolver.ChildCount;

        public InspectorProperty this[int index] => Get(index);
        public InspectorProperty this[string name] => Get(name);

        internal PropertyChildren(InspectorProperty property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            _property = property;
        }

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

        internal void Update()
        {
        }

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

        public void Clear()
        {
            foreach (var child in _childByIndex)
            {
                child.Value.Dispose();
            }
            _childByIndex.Clear();
            _childPathByIndex.Clear();
        }

        public void Dispose()
        {
            Clear();
            _property = null;
        }
    }
}
