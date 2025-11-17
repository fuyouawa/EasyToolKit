using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Property resolver implementation for Unity's <see cref="SerializedProperty"/> system
    /// </summary>
    public class UnityPropertyResolver : PropertyResolver
    {
        private SerializedProperty _serializedProperty;
        private readonly List<InspectorPropertyInfo> _propertyInfos = new List<InspectorPropertyInfo>();

        /// <summary>
        /// Initializes the resolver by discovering Unity serialized properties
        /// </summary>
        protected override void Initialize()
        {
            // Get the appropriate SerializedProperty based on whether this is a logic root
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

            // Iterate through all child properties
            var iterator = _serializedProperty.Copy();
            if (!iterator.Next(true))
            {
                return;
            }

            do
            {
                // For non-root properties, only process children of the current property
                if (!Property.Info.IsLogicRoot)
                {
                    if (!iterator.propertyPath.StartsWith(_serializedProperty.propertyPath + "."))
                    {
                        break;
                    }
                }

                // Find the corresponding field in the target type
                var field = Property.Info.PropertyType.GetField(iterator.name, BindingFlagsHelper.AllInstance);
                if (field == null)
                {
                    continue;
                }

                // Filter non-public fields based on Unity serialization rules
                if (!field.IsPublic)
                {
                    if (field.IsDefined<HideInInspector>())
                    {
                        continue;
                    }

                    if (!field.IsDefined<SerializeField>() &&
                        !field.IsDefined<ShowInInspectorAttribute>())
                    {
                        continue;
                    }

                    if (field.IsDefined<NonSerializedAttribute>())
                    {
                        continue;
                    }
                }

                // Filter non-serializable types
                if (!field.FieldType.IsSubclassOf(typeof(UnityEngine.Object)) &&
                    !field.FieldType.IsValueType &&
                    !field.FieldType.IsDefined<SerializableAttribute>())
                {
                    continue;
                }

                // Add valid property info
                _propertyInfos.Add(InspectorPropertyInfo.CreateForUnityProperty(iterator, Property.Info.PropertyType, field.FieldType));
            } while (iterator.Next(false));
        }

        /// <summary>
        /// Deinitializes the resolver by clearing the property info cache
        /// </summary>
        protected override void Deinitialize()
        {
            _propertyInfos.Clear();
        }

        /// <summary>
        /// Gets information about a child property at the specified index
        /// </summary>
        /// <param name="childIndex">The index of the child property</param>
        /// <returns>Information about the child property</returns>
        public override InspectorPropertyInfo GetChildInfo(int childIndex)
        {
            return _propertyInfos[childIndex];
        }

        /// <summary>
        /// Converts a child property name to its index
        /// </summary>
        /// <param name="name">The name of the child property</param>
        /// <returns>The index of the child property, or -1 if not found</returns>
        public override int ChildNameToIndex(string name)
        {
            return _propertyInfos.FindIndex(info => info.PropertyName == name);
        }

        /// <summary>
        /// Calculates the number of child properties
        /// </summary>
        /// <returns>The number of child properties</returns>
        public override int CalculateChildCount()
        {
            return _propertyInfos.Count;
        }
    }
}
