using EasyToolKit.Core.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Resolver for Unity serialized collections in the inspector.
    /// Provides integration with Unity's <see cref="SerializedProperty"/> system for array/list operations.
    /// </summary>
    /// <remarks>
    /// TODO: UnityCollectionResolver has issues with multi-selection scenarios.
    /// </remarks>
    /// <typeparam name="TElement">The type of elements in the collection.</typeparam>
    public class UnityCollectionResolver<TElement> : OrderedCollectionResolverBase<IList<TElement>, TElement>
    {
        private SerializedProperty _serializedProperty;

        private readonly Dictionary<int, InspectorPropertyInfo> _propertyInfosByIndex =
            new Dictionary<int, InspectorPropertyInfo>();

        private static readonly Func<SerializedProperty, TElement> ElementValueGetter =
            SerializedPropertyUtility.GetValueGetter<TElement>();

        private static readonly Action<SerializedProperty, TElement> ElementValueSetter =
            SerializedPropertyUtility.GetValueSetter<TElement>();


        /// <summary>
        /// Initializes the resolver by obtaining the <see cref="SerializedProperty"/> for the collection.
        /// </summary>
        protected override void Initialize()
        {
            if (Property.Info.IsLogicRoot)
            {
                _serializedProperty = Property.Tree.SerializedObject.GetIterator();
            }
            else
            {
                _serializedProperty = Property.Tree.GetUnityPropertyByPath(Property.UnityPath);
                if (_serializedProperty == null)
                {
                    throw new InvalidOperationException($"SerializedProperty is null for path: {Property.UnityPath}");
                }
            }
        }

        /// <summary>
        /// Gets the property information for a child element at the specified index.
        /// Creates <see cref="InspectorPropertyInfo"/> based on Unity's <see cref="SerializedProperty"/> system.
        /// </summary>
        /// <param name="childIndex">The index of the child element.</param>
        /// <returns>The property information for the child element.</returns>
        public override InspectorPropertyInfo GetChildInfo(int childIndex)
        {
            if (_propertyInfosByIndex.TryGetValue(childIndex, out var info))
            {
                return info;
            }

            info = InspectorPropertyInfo.CreateForUnityProperty(
                _serializedProperty.GetArrayElementAtIndex(childIndex),
                Property.Info.PropertyType,
                ElementType);

            _propertyInfosByIndex[childIndex] = info;
            return info;
        }

        /// <summary>
        /// Converts a child name to an index. Not supported for Unity collection resolvers.
        /// </summary>
        /// <param name="name">The child name to convert.</param>
        /// <returns>Throws NotSupportedException.</returns>
        /// <exception cref="NotSupportedException">Always thrown as this operation is not supported.</exception>
        public override int ChildNameToIndex(string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Calculates the number of child properties (elements) in the collection.
        /// Uses Unity's <see cref="SerializedProperty"/> arraySize property.
        /// </summary>
        /// <returns>The number of elements in the serialized array.</returns>
        public override int CalculateChildCount()
        {
            return _serializedProperty.arraySize;
        }

        /// <summary>
        /// Adds an element to the collection at the specified target index.
        /// Implementation adds the element at the end of the array.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="value">The value to add to the collection.</param>
        protected override void AddElement(int targetIndex, TElement value)
        {
            InsertElementAt(targetIndex, _serializedProperty.arraySize, value);
        }

        /// <summary>
        /// Removes an element from the collection at the specified target index.
        /// Searches for the element by value and removes the first occurrence.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="value">The value to remove from the collection.</param>
        protected override void RemoveElement(int targetIndex, TElement value)
        {
            for (int i = 0; i < _serializedProperty.arraySize; i++)
            {
                var element = _serializedProperty.GetArrayElementAtIndex(i);
                var elementValue = ElementValueGetter(element);
                if (value.Equals(elementValue))
                {
                    RemoveElementAt(targetIndex, i);
                    return;
                }
            }
        }

        /// <summary>
        /// Inserts an element into the collection at a specific index.
        /// Uses Unity's <see cref="SerializedProperty"/>.InsertArrayElementAtIndex method.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="index">The position in the collection where the element should be inserted.</param>
        /// <param name="value">The value to insert into the collection.</param>
        protected override void InsertElementAt(int targetIndex, int index, TElement value)
        {
            _serializedProperty.InsertArrayElementAtIndex(index);
            var element = _serializedProperty.GetArrayElementAtIndex(index);
            ElementValueSetter(element, value);
        }

        /// <summary>
        /// Removes an element from the collection at a specific index.
        /// Uses Unity's <see cref="SerializedProperty"/>.DeleteArrayElementAtIndex method.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="index">The position in the collection from which to remove the element.</param>
        protected override void RemoveElementAt(int targetIndex, int index)
        {
            _serializedProperty.DeleteArrayElementAtIndex(index);
        }
    }
}
