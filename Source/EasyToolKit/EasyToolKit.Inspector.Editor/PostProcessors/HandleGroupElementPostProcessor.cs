using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [ElementPostProcessorPriority(ElementPostProcessorPriorityLevel.Super)]
    public class HandleGroupElementPostProcessor : ElementPostProcessor
    {
        protected override bool CanProcess(IElement element)
        {
            return element.GetAttribute<BeginGroupAttribute>(includeDerived: true) != null;
        }

        protected override void Process(IElement element)
        {
            var beginGroupAttributeInfo = element.GetAttributeInfos()
                .First(info => info.Attribute is BeginGroupAttribute);

            var beginGroupAttribute = (BeginGroupAttribute)beginGroupAttributeInfo.Attribute;
            var beginGroupAttributeType = beginGroupAttribute.GetType();
            var endGroupAttributeType =
                InspectorAttributeUtility.GetCorrespondGroupAttributeType(beginGroupAttributeType);

            var groupName = beginGroupAttribute.GroupName;
            var groupDefinition = InspectorElements.Configurator.Group()
                .WithGroupAttributes(beginGroupAttributeType, endGroupAttributeType)
                .WithName(groupName)
                .CreateDefinition();
            var groupElement = element.SharedContext.Tree.ElementFactory.CreateGroupElement(groupDefinition, null);

            var parentCollection = ElementUtility.GetParentCollection(element);
            var elementIndex = parentCollection.IndexOf(element);
            parentCollection.Insert(elementIndex, groupElement);

            var childrenToMove = new List<IElement>();

            if (beginGroupAttributeInfo.Source == ElementAttributeSource.Type)
            {
                var valueElement = (IValueElement)element;
                if (valueElement.Children == null)
                {
                    Debug.LogWarning($"Can not use BeginGroupAttribute on a value element '{element}' that can not have children.");
                    return;
                }

                childrenToMove.AddRange(valueElement.Children);
            }
            else
            {
                var subGroupStack = new Stack<IElement>();
                for (int i = elementIndex + 1; i < parentCollection.Count; i++)
                {
                    var child = parentCollection[i];

                    var childBeginGroupAttribute = (BeginGroupAttribute)child.GetAttribute(beginGroupAttributeType);
                    if (childBeginGroupAttribute != null)
                    {
                        var childGroupName = childBeginGroupAttribute.GroupName;
                        bool isSubGroup = groupName.IsNotNullOrEmpty() &&
                                          childGroupName.IsNotNullOrEmpty() &&
                                          childGroupName.StartsWith(groupName) &&
                                          childGroupName[groupName.Length] == '/';

                        if (isSubGroup)
                        {
                            subGroupStack.Push(child);
                        }
                        else
                        {
                            break;
                        }
                    }

                    var childEndGroupAttribute = child.GetAttribute(endGroupAttributeType);
                    if (childEndGroupAttribute != null)
                    {
                        if (subGroupStack.Count > 0)
                        {
                            subGroupStack.Pop();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            foreach (var child in childrenToMove)
            {
                groupElement.Children.Add(child);
            }
        }
    }
}
