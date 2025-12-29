using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [PostProcessorPriority(PostProcessorPriorityLevel.Super - 1)]
    public class GroupElementPostProcessor : PostProcessor
    {
        protected override void Process()
        {
            if (Element.Children == null)
            {
                CallNextProcessor();
                return;
            }

            int elementIndex = 0;
            do
            {
                ProcessImpl(ref elementIndex);
            } while (elementIndex < Element.Children.Count);

            CallNextProcessor();
        }

        private void ProcessImpl(ref int elementIndex)
        {
            if (!TryFindNextContainedGroupElement(ref elementIndex, out var beginGroupAttributeInfo))
            {
                return;
            }

            var elementChild = Element.Children[elementIndex];

            var beginGroupAttribute = (BeginGroupAttribute)beginGroupAttributeInfo.Attribute;
            var beginGroupAttributeType = beginGroupAttribute.GetType();
            var endGroupAttributeType = InspectorAttributeUtility.GetCorrespondGroupAttributeType(beginGroupAttributeType);

            var newGroupDefinition = InspectorElements.Configurator.Group()
                .WithGroupAttributes(beginGroupAttributeType, endGroupAttributeType)
                .WithAdditionalAttributes(beginGroupAttributeInfo.Attribute)
                .WithName(beginGroupAttribute.GroupName)
                .CreateDefinition();
            var newGroupElement = Element.SharedContext.Tree.ElementFactory.CreateGroupElement(newGroupDefinition);

            if (elementChild is ILogicalElement logicalElement)
            {
                newGroupElement.AssociatedElement = logicalElement;
            }

            var childrenToMove = new List<IElement> { elementChild };

            if (beginGroupAttributeInfo.Source != ElementAttributeSource.Type)
            {
                if (!beginGroupAttribute.EndAfterThisProperty)
                {
                    var groupCatalogue = beginGroupAttribute.GroupCatalogue;
                    var subGroupStack = new Stack<IElement>();
                    for (int i = elementIndex + 1; i < Element.Children.Count; i++)
                    {
                        var child = Element.Children[i];

                        var childBeginGroupAttribute = (BeginGroupAttribute)child.GetAttribute(beginGroupAttributeType);
                        if (childBeginGroupAttribute != null)
                        {
                            var childGroupName = childBeginGroupAttribute.GroupCatalogue;
                            bool isSubGroup = groupCatalogue.IsNotNullOrEmpty() &&
                                              childGroupName.IsNotNullOrEmpty() &&
                                              childGroupName.Length > groupCatalogue.Length &&
                                              childGroupName.StartsWith(groupCatalogue) &&
                                              childGroupName[groupCatalogue.Length] == '/';

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

                        childrenToMove.Add(child);
                    }
                }
            }

            Element.Children.Insert(elementIndex, newGroupElement);
            elementIndex++;

            Element.Request(() =>
            {
                newGroupElement.Update();
                foreach (var child in childrenToMove)
                {
                    newGroupElement.Children.Add(child);
                }
            });
        }

        private bool TryFindNextContainedGroupElement(ref int elementIndex, out ElementAttributeInfo beginGroupAttributeInfo)
        {
            beginGroupAttributeInfo = null;
            for (; elementIndex < Element.Children.Count; elementIndex++)
            {
                var child = Element.Children[elementIndex];
                foreach (var attributeInfo in child.GetAttributeInfos())
                {
                    var attributeType = attributeInfo.Attribute.GetType();
                    if (!attributeType.IsInheritsFrom<BeginGroupAttribute>())
                    {
                        continue;
                    }
                    if (Element is IGroupElement groupElement)
                    {
                        if (groupElement.Definition.BeginGroupAttributeType == attributeType)
                        {
                            continue;
                        }
                    }

                    beginGroupAttributeInfo = attributeInfo;
                    return true;
                }
            }

            return false;
        }
    }
}
