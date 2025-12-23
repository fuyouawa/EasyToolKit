// using System;
// using System.Collections.Generic;
// using EasyToolKit.Core;
// using UnityEditor;
// using UnityEngine;
//
// namespace EasyToolKit.Inspector.Editor
// {
//     /// <summary>
//     /// Property structure resolver implementation for Unity's SerializedProperty system.
//     /// Focuses purely on property structure discovery without collection operations.
//     /// </summary>
//     [ResolverPriority(-10000.0)]
//     public class UnityPropertyStructureResolver : PropertyStructureResolverBase
//     {
//         private SerializedProperty _serializedProperty;
//         private readonly List<IElementDefinition> _definitions = new List<IElementDefinition>();
//
//         protected override bool CanResolve(IElement element)
//         {
//             if (element.Definition.Flags.HasFlag(ElementFlags.Root))
//             {
//                 _serializedProperty = element.SharedContext.Tree.SerializedObject?.GetIterator();
//             }
//             else
//             {
//                 if (element is IFieldElement fieldElement)
//                 {
//                     _serializedProperty = element.SharedContext.Tree.GetUnityPropertyByPath(fieldElement.UnityPath);
//                 }
//                 else
//                 {
//                     return false;
//                 }
//             }
//
//             if (_serializedProperty == null)
//             {
//                 return false;
//             }
//
//             return !_serializedProperty.isArray;
//         }
//
//         /// <summary>
//         /// Initializes the resolver by discovering Unity serialized properties
//         /// </summary>
//         protected override void Initialize()
//         {
//             if (Element.Definition.Flags.HasFlag(ElementFlags.Root))
//             {
//                 _serializedProperty = Element.SharedContext.Tree.SerializedObject.GetIterator();
//             }
//             else
//             {
//                 var fieldElement = (IFieldElement)Element;
//                 _serializedProperty = Element.SharedContext.Tree.GetUnityPropertyByPath(fieldElement.UnityPath);
//                 if (_serializedProperty == null)
//                 {
//                     throw new InvalidOperationException($"SerializedProperty is null for path: {fieldElement.UnityPath}");
//                 }
//             }
//
//             var iterator = _serializedProperty.Copy();
//             if (!iterator.Next(true))
//             {
//                 return;
//             }
//
//             do
//             {
//                 // For non-root properties, only process children of the current property
//                 if (!Element.Definition.Flags.HasFlag(ElementFlags.Root))
//                 {
//                     if (!iterator.propertyPath.StartsWith(_serializedProperty.propertyPath + "."))
//                     {
//                         break;
//                     }
//                 }
//
//                 // Find the corresponding field in the target type
//                 var field = Element.Definition.ValueType.GetField(iterator.name, BindingFlagsHelper.AllInstance);
//                 if (field == null)
//                 {
//                     continue;
//                 }
//
//                 // Filter non-public fields based on Unity serialization rules
//                 if (!field.IsPublic)
//                 {
//                     if (field.IsDefined<HideInInspector>())
//                     {
//                         continue;
//                     }
//
//                     if (!field.IsDefined<SerializeField>() &&
//                         !field.IsDefined<ShowInInspectorAttribute>())
//                     {
//                         continue;
//                     }
//
//                     if (field.IsDefined<NonSerializedAttribute>())
//                     {
//                         continue;
//                     }
//                 }
//
//                 // Filter non-serializable types
//                 if (!field.FieldType.IsSubclassOf(typeof(UnityEngine.Object)) &&
//                     !field.FieldType.IsValueType &&
//                     !field.FieldType.IsDefined<SerializableAttribute>())
//                 {
//                     continue;
//                 }
//
//                 // Add valid property definition
//                 _definitions.Add(
//                     InspectorElements.Configurator.Field()
//                         .AsUnityProperty()
//                         .WithFieldInfo(field)
//                         .WithValueType(field.FieldType)
//                         .CreateDefinition());
//             } while (iterator.Next(false));
//         }
//
//         /// <summary>
//         /// Gets the definition of a child property at the specified index
//         /// </summary>
//         /// <param name="childIndex">The index of the child property</param>
//         /// <returns>Definition of the child property</returns>
//         protected override IElementDefinition GetChildDefinition(int childIndex)
//         {
//             if (childIndex < 0 || childIndex >= _definitions.Count)
//                 throw new ArgumentOutOfRangeException(nameof(childIndex));
//
//             return _definitions[childIndex];
//         }
//
//         /// <summary>
//         /// Converts a child property name to its index
//         /// </summary>
//         /// <param name="name">The name of the child property</param>
//         /// <returns>The index of the child property, or -1 if not found</returns>
//         protected override int ChildNameToIndex(string name)
//         {
//             for (int i = 0; i < _definitions.Count; i++)
//             {
//                 if (_definitions[i].Name == name)
//                 {
//                     return i;
//                 }
//             }
//
//             return -1;
//         }
//
//         /// <summary>
//         /// Calculates the number of child properties
//         /// </summary>
//         /// <returns>The number of child properties</returns>
//         protected override int CalculateChildCount()
//         {
//             return _definitions.Count;
//         }
//     }
// }
