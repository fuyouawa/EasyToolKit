// using EasyToolKit.Core.Editor;
// using System;
// using System.Collections.Generic;
// using System.Diagnostics.CodeAnalysis;
// using UnityEditor;
//
// namespace EasyToolKit.Inspector.Editor
// {
//     public class UnityCollectionOperation<TItem> : OrderedCollectionOperationBase<IList<TItem>, TItem>
//     {
//         private readonly InspectorProperty _property;
//         private SerializedProperty _serializedProperty;
//
//         private static readonly Func<SerializedProperty, TItem> ElementValueGetter =
//             SerializedPropertyUtility.GetValueGetter<TItem>();
//
//         private static readonly Action<SerializedProperty, TItem> ElementValueSetter =
//             SerializedPropertyUtility.GetValueSetter<TItem>();
//
//         public UnityCollectionOperation(Type ownerType, [NotNull] InspectorProperty property)
//             : base(ownerType)
//         {
//             _property = property ?? throw new ArgumentNullException(nameof(property));
//             InitializeSerializedProperty();
//         }
//
//         public override bool IsReadOnly => _serializedProperty?.hasMultipleDifferentValues ?? false;
//
//         private void InitializeSerializedProperty()
//         {
//             if (_property.Info.IsLogicRoot)
//             {
//                 _serializedProperty = _property.Tree.SerializedObject.GetIterator();
//             }
//             else
//             {
//                 _serializedProperty = _property.Tree.GetUnityPropertyByPath(_property.UnityPath);
//                 if (_serializedProperty == null)
//                 {
//                     throw new InvalidOperationException($"SerializedProperty is null for path: {_property.UnityPath}");
//                 }
//             }
//         }
//
//         public override void AddItem(ref IList<TItem> collection, TItem value)
//         {
//             if (_serializedProperty == null)
//                 return;
//
//             _serializedProperty.InsertArrayElementAtIndex(_serializedProperty.arraySize);
//             var newElement = _serializedProperty.GetArrayElementAtIndex(_serializedProperty.arraySize - 1);
//             ElementValueSetter(newElement, value);
//         }
//
//         public override void RemoveItem(ref IList<TItem> collection, TItem value)
//         {
//             if (_serializedProperty == null)
//                 return;
//
//             // Find and remove the element matching the value
//             for (int i = 0; i < _serializedProperty.arraySize; i++)
//             {
//                 var element = _serializedProperty.GetArrayElementAtIndex(i);
//                 var elementValue = ElementValueGetter(element);
//
//                 if (EqualityComparer<TItem>.Default.Equals(elementValue, value))
//                 {
//                     _serializedProperty.DeleteArrayElementAtIndex(i);
//                     break;
//                 }
//             }
//         }
//
//         public override void InsertItemAt(ref IList<TItem> collection, int insertIndex, TItem value)
//         {
//             if (_serializedProperty == null)
//                 return;
//
//             _serializedProperty.InsertArrayElementAtIndex(insertIndex);
//             var newElement = _serializedProperty.GetArrayElementAtIndex(insertIndex);
//             ElementValueSetter(newElement, value);
//         }
//
//         public override void RemoveItemAt(ref IList<TItem> collection, int removeIndex)
//         {
//             if (_serializedProperty == null || removeIndex < 0 || removeIndex >= _serializedProperty.arraySize)
//                 return;
//
//             _serializedProperty.DeleteArrayElementAtIndex(removeIndex);
//         }
//     }
// }
