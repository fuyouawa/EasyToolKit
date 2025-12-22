// using System;
// using System.Collections.Generic;
// using System.Linq;
// using EasyToolKit.Core;
//
// namespace EasyToolKit.Inspector.Editor
// {
//     /// <summary>
//     /// Default implementation of group property resolver for <see cref="InspectorProperty"/>
//     /// </summary>
//     public class DefaultGroupResolver : GroupResolverBase
//     {
//         private readonly Dictionary<Type, InspectorProperty[]> _groupPropertiesCache = new Dictionary<Type, InspectorProperty[]>();
//
//         /// <summary>
//         /// Gets all properties that belong to the same group as the current property
//         /// </summary>
//         /// <param name="beginGroupAttributeType">The type of the begin group attribute</param>
//         /// <returns>Array of properties in the group</returns>
//         protected override InspectorProperty[] GetGroupProperties(Type beginGroupAttributeType)
//         {
//             // Return cached properties if available
//             if (_groupPropertiesCache.TryGetValue(beginGroupAttributeType, out var properties))
//             {
//                 // Update all properties in the group before returning
//                 foreach (var property in properties)
//                 {
//                     property.Update();
//                 }
//                 return properties;
//             }
//
//             var beginGroupAttribute = (BeginGroupAttribute)Property.GetAttributes().FirstOrDefault(attr => attr.GetType() == beginGroupAttributeType);
//             if (beginGroupAttribute == null)
//             {
//                 properties = new InspectorProperty[] { };
//                 _groupPropertiesCache[beginGroupAttributeType] = properties;
//                 return properties;
//             }
//
//             // Check if this is a class attribute, and if so, automatically set EndAfterThisProperty
//             bool isClassAttribute = Property.GetAttributeSource(beginGroupAttribute) == AttributeSource.Type;
//             if (isClassAttribute || beginGroupAttribute.EndAfterThisProperty)
//             {
//                 properties = new InspectorProperty[] { };
//                 _groupPropertiesCache[beginGroupAttributeType] = properties;
//                 return properties;
//             }
//
//             var parent = Property.Parent;
//
//             int beginIndex = 0;
//             for (; beginIndex < parent.Children.Count; beginIndex++)
//             {
//                 if (parent.Children[beginIndex] == Property)
//                 {
//                     break;
//                 }
//             }
//
//             if (beginIndex >= parent.Children.Count - 1)
//             {
//                 properties = new InspectorProperty[] { };
//                 _groupPropertiesCache[beginGroupAttributeType] = properties;
//                 return properties;
//             }
//
//
//             var groupName = beginGroupAttribute.GroupName;
//             var endGroupAttributeType =
//                 InspectorAttributeUtility.GetCorrespondGroupAttributeType(beginGroupAttribute.GetType());
//
//             var groupPropertyList = new List<InspectorProperty>();
//             var subGroupPropertyStack = new Stack<InspectorProperty>();
//             for (int i = beginIndex + 1; i < parent.Children.Count; i++)
//             {
//                 var child = parent.Children[i];
//
//                 var childBeginGroupAttribute = (BeginGroupAttribute)child.GetAttributes().FirstOrDefault(attr => attr.GetType() == beginGroupAttributeType);
//                 if (childBeginGroupAttribute != null)
//                 {
//                     var childGroupName = childBeginGroupAttribute.GroupName;
//                     bool isSubGroup = groupName.IsNotNullOrEmpty() &&
//                                       childGroupName.IsNotNullOrEmpty() &&
//                                       childGroupName.StartsWith(groupName) &&
//                                       childGroupName[groupName.Length] == '/';
//
//                     if (isSubGroup)
//                     {
//                         subGroupPropertyStack.Push(child);
//                     }
//                     else
//                     {
//                         break;
//                     }
//                 }
//
//                 var childEndGroupAttribute = (EndGroupAttribute)child.GetAttributes().FirstOrDefault(attr => attr.GetType() == endGroupAttributeType);
//                 if (childEndGroupAttribute != null)
//                 {
//                     if (subGroupPropertyStack.Count > 0)
//                     {
//                         subGroupPropertyStack.Pop();
//                     }
//                     else
//                     {
//                         break;
//                     }
//                 }
//
//                 groupPropertyList.Add(child);
//             }
//
//             properties = groupPropertyList.ToArray();
//             _groupPropertiesCache[beginGroupAttributeType] = properties;
//             return properties;
//         }
//     }
// }
