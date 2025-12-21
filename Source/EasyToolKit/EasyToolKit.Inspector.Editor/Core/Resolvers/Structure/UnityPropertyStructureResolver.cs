using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Property structure resolver implementation for Unity's SerializedProperty system.
    /// Focuses purely on property structure discovery without collection operations.
    /// </summary>
    [ResolverPriority(-10000.0)]
    public class UnityPropertyStructureResolver : PropertyStructureResolverBase
    {
        private SerializedProperty _serializedProperty;
        private readonly List<InspectorPropertyInfo> _propertyInfos = new List<InspectorPropertyInfo>();

        protected override bool CanResolve(InspectorProperty property)
        {
            if (property.Info.IsLogicRoot)
            {
                _serializedProperty = property.Tree.SerializedObject?.GetIterator();
            }
            else
            {
                _serializedProperty = property.Tree.GetUnityPropertyByPath(property.UnityPath);
            }

            if (_serializedProperty == null)
            {
                return false;
            }

            return !_serializedProperty.isArray;
        }

        /// <summary>
        /// Initializes the resolver by discovering Unity serialized properties
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
                _propertyInfos.Add(InspectorPropertyInfo.CreateForUnityProperty(iterator, Property.Info.PropertyType));
            } while (iterator.Next(false));
        }

        /// <summary>
        /// Gets information about a child property at the specified index
        /// </summary>
        /// <param name="childIndex">The index of the child property</param>
        /// <returns>Information about the child property</returns>
        protected override InspectorPropertyInfo GetChildInfo(int childIndex)
        {
            if (childIndex < 0 || childIndex >= _propertyInfos.Count)
                throw new ArgumentOutOfRangeException(nameof(childIndex));

            return _propertyInfos[childIndex];
        }

        /// <summary>
        /// Converts a child property name to its index
        /// </summary>
        /// <param name="name">The name of the child property</param>
        /// <returns>The index of the child property, or -1 if not found</returns>
        protected override int ChildNameToIndex(string name)
        {
            for (int i = 0; i < _propertyInfos.Count; i++)
            {
                if (_propertyInfos[i].PropertyName == name)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Calculates the number of child properties
        /// </summary>
        /// <returns>The number of child properties</returns>
        protected override int CalculateChildCount()
        {
            return _propertyInfos.Count;
        }
    }
}
