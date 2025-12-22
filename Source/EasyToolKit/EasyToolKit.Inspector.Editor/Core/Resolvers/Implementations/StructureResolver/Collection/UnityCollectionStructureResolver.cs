// using System;
// using System.Collections.Generic;
// using UnityEditor;
//
// namespace EasyToolKit.Inspector.Editor
// {
//     [ResolverPriority(-10000.0)]
//     public class UnityCollectionStructureResolver<TCollection, TItem> : CollectionStructureResolverBase<TCollection>
//         where TCollection : IList<TItem>
//     {
//         private SerializedProperty _serializedProperty;
//
//         public override Type ItemType => typeof(TItem);
//
//         protected override bool CanResolveElement(IValueElement element)
//         {
//             if (element.Definition.Flags.IsRoot())
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
//             return _serializedProperty.isArray;
//         }
//
//         protected override void Initialize()
//         {
//             base.Initialize();
//             if (Element.Definition.Flags.IsRoot())
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
//         }
//
//         protected override int CalculateChildCount()
//         {
//             return _serializedProperty.arraySize;
//         }
//     }
// }
