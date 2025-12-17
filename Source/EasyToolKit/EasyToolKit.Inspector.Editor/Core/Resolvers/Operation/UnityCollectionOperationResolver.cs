using EasyToolKit.Core.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Operation resolver for Unity serialized collections in the inspector system.
    /// Handles Unity-specific collection operations using SerializedProperty system.
    /// </summary>
    /// <typeparam name="TElement">The type of elements in the collection</typeparam>
    public class UnityCollectionOperationResolver<TElement> : OrderedCollectionOperationResolver<IList<TElement>, TElement>
    {
        private readonly InspectorProperty _property;
        private SerializedProperty _serializedProperty;

        private static readonly Func<SerializedProperty, TElement> ElementValueGetter =
            SerializedPropertyUtility.GetValueGetter<TElement>();

        private static readonly Action<SerializedProperty, TElement> ElementValueSetter =
            SerializedPropertyUtility.GetValueSetter<TElement>();

        /// <summary>
        /// Initializes a new instance of the UnityCollectionOperationResolver
        /// </summary>
        /// <param name="property">The inspector property associated with this resolver</param>
        public UnityCollectionOperationResolver(InspectorProperty property)
        {
            _property = property ?? throw new ArgumentNullException(nameof(property));
            InitializeSerializedProperty();
        }

        /// <summary>
        /// Gets whether the collection is read-only based on SerializedProperty
        /// </summary>
        public override bool IsReadOnly => _serializedProperty?.hasMultipleDifferentValues ?? false;

        /// <summary>
        /// Gets the type of the collection (System.Collections.Generic.List{T})
        /// </summary>
        public override Type CollectionType => typeof(System.Collections.Generic.List<TElement>);

        /// <summary>
        /// Initializes the SerializedProperty for Unity collection access
        /// </summary>
        private void InitializeSerializedProperty()
        {
            if (_property.Info.IsLogicRoot)
            {
                _serializedProperty = _property.Tree.SerializedObject.GetIterator();
            }
            else
            {
                _serializedProperty = _property.Tree.GetUnityPropertyByPath(_property.UnityPath);
                if (_serializedProperty == null)
                {
                    throw new InvalidOperationException($"SerializedProperty is null for path: {_property.UnityPath}");
                }
            }
        }

        /// <summary>
        /// Actually adds an element to the Unity collection
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context</param>
        /// <param name="value">The value to add to the collection</param>
        protected override void AddElement(int targetIndex, TElement value)
        {
            if (_serializedProperty == null)
                return;

            _serializedProperty.InsertArrayElementAtIndex(_serializedProperty.arraySize);
            var newElement = _serializedProperty.GetArrayElementAtIndex(_serializedProperty.arraySize - 1);
            ElementValueSetter(newElement, value);
        }

        /// <summary>
        /// Actually removes an element from the Unity collection
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context</param>
        /// <param name="value">The value to remove from the collection</param>
        protected override void RemoveElement(int targetIndex, TElement value)
        {
            if (_serializedProperty == null)
                return;

            // Find and remove the element matching the value
            for (int i = 0; i < _serializedProperty.arraySize; i++)
            {
                var element = _serializedProperty.GetArrayElementAtIndex(i);
                var elementValue = ElementValueGetter(element);

                if (EqualityComparer<TElement>.Default.Equals(elementValue, value))
                {
                    _serializedProperty.DeleteArrayElementAtIndex(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Inserts an element at a specific index in the Unity collection
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context</param>
        /// <param name="insertIndex">The position where the element should be inserted</param>
        /// <param name="value">The value to insert into the collection</param>
        protected override void InsertElementAt(int targetIndex, int insertIndex, TElement value)
        {
            if (_serializedProperty == null)
                return;

            _serializedProperty.InsertArrayElementAtIndex(insertIndex);
            var newElement = _serializedProperty.GetArrayElementAtIndex(insertIndex);
            ElementValueSetter(newElement, value);
        }

        /// <summary>
        /// Removes an element at a specific index from the Unity collection
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context</param>
        /// <param name="removeIndex">The index of the element to remove</param>
        protected override void RemoveElementAt(int targetIndex, int removeIndex)
        {
            if (_serializedProperty == null || removeIndex < 0 || removeIndex >= _serializedProperty.arraySize)
                return;

            _serializedProperty.DeleteArrayElementAtIndex(removeIndex);
        }

        /// <summary>
        /// Gets the element count from the SerializedProperty
        /// </summary>
        /// <returns>The number of elements in the Unity collection</returns>
        public virtual int GetElementCount()
        {
            return _serializedProperty?.arraySize ?? 0;
        }

        /// <summary>
        /// Gets an element at the specified index from a collection
        /// </summary>
        /// <param name="collection">The collection to get the element from</param>
        /// <param name="elementIndex">The index of the element</param>
        /// <returns>The element at the specified index</returns>
        protected override TElement GetElementAt(IList<TElement> collection, int elementIndex)
        {
            // For Unity collections, we ignore the passed collection and use SerializedProperty
            // This assumes the collection parameter is not used for Unity serialized collections
            if (_serializedProperty == null || elementIndex < 0 || elementIndex >= _serializedProperty.arraySize)
                throw new ArgumentOutOfRangeException(nameof(elementIndex));

            var element = _serializedProperty.GetArrayElementAtIndex(elementIndex);
            return ElementValueGetter(element);
        }

        /// <summary>
        /// Sets an element at the specified index in a collection
        /// </summary>
        /// <param name="collection">The collection to set the element in</param>
        /// <param name="elementIndex">The index of the element to set</param>
        /// <param name="value">The value to set</param>
        protected override void SetElementAt(IList<TElement> collection, int elementIndex, TElement value)
        {
            // For Unity collections, we ignore the passed collection and use SerializedProperty
            // This assumes the collection parameter is not used for Unity serialized collections
            if (_serializedProperty == null || elementIndex < 0 || elementIndex >= _serializedProperty.arraySize)
                throw new ArgumentOutOfRangeException(nameof(elementIndex));

            var element = _serializedProperty.GetArrayElementAtIndex(elementIndex);
            ElementValueSetter(element, value);
        }
    }
}
