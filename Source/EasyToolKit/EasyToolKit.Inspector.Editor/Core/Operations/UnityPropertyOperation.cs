using System;
using EasyToolKit.Core.Editor;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Property operation implementation for Unity serialized properties
    /// </summary>
    /// <typeparam name="TOwner">Owner type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public class UnityPropertyOperation<TOwner, TValue> : PropertyOperation<TOwner, TValue>
    {
        private static readonly Func<SerializedProperty, TValue> ValueGetter = SerializedPropertyUtility.GetValueGetter<TValue>();
        private static readonly Action<SerializedProperty, TValue> ValueSetter = SerializedPropertyUtility.GetValueSetter<TValue>();

        private readonly SerializedProperty _serializedProperty;

        /// <summary>
        /// Initializes a new instance of UnityPropertyOperation
        /// </summary>
        /// <param name="serializedProperty">Unity serialized property</param>
        public UnityPropertyOperation(SerializedProperty serializedProperty)
        {
            _serializedProperty = serializedProperty.Copy();
        }

        /// <summary>
        /// Whether the property is read-only
        /// </summary>
        public override bool IsReadOnly => ValueSetter == null;

        /// <summary>
        /// Gets the value with type safety
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <returns>Property value</returns>
        public override TValue GetValue(ref TOwner owner)
        {
            return ValueGetter(_serializedProperty);
        }

        /// <summary>
        /// Sets the value with type safety
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <param name="value">Value to set</param>
        public override void SetValue(ref TOwner owner, TValue value)
        {
            if (ValueSetter == null)
                throw new NotSupportedException("Unity serialized property is read-only");

            ValueSetter(_serializedProperty, value);
        }
    }
}
