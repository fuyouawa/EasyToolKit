using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [PostProcessorPriority(PostProcessorPriorityLevel.Super - 1)]
    public class GroupElementPostProcessor : PostProcessor
    {
        protected override bool CanProcess(IElement element)
        {
            return !element.Definition.Roles.IsGroup();
        }

        protected override void Process()
        {
            if (Element.Children == null)
            {
                CallNextProcessor();
                return;
            }

            int elementIndex = 0;
            for (; elementIndex < Element.Children.Count; elementIndex++)
            {
                var child = Element.Children[elementIndex];
                if (child.GetAttribute<BeginGroupAttribute>(includeDerived: true) != null)
                {
                    break;
                }
            }

            if (elementIndex == Element.Children.Count)
            {
                CallNextProcessor();
                return;
            }

            var elementChild = Element.Children[elementIndex];
            var beginGroupAttributeInfo = elementChild.GetAttributeInfos()
                .First(info => info.Attribute is BeginGroupAttribute);

            var beginGroupAttribute = (BeginGroupAttribute)beginGroupAttributeInfo.Attribute;
            var beginGroupAttributeType = beginGroupAttribute.GetType();
            var endGroupAttributeType = InspectorAttributeUtility.GetCorrespondGroupAttributeType(beginGroupAttributeType);

            var groupDefinition = InspectorElements.Configurator.Group()
                .WithGroupAttributes(beginGroupAttributeType, endGroupAttributeType)
                .WithName(beginGroupAttribute.GroupName)
                .CreateDefinition();
            var groupElement = Element.SharedContext.Tree.ElementFactory.CreateGroupElement(groupDefinition);

            Element.Children.Insert(elementIndex, groupElement);

            var childrenToMove = new List<IElement>();

            if (beginGroupAttributeInfo.Source == ElementAttributeSource.Type)
            {
                var valueElement = (IValueElement)elementChild;
                if (valueElement.Children == null)
                {
                    Debug.LogWarning($"Can not use BeginGroupAttribute on a value element '{elementChild}' that can not have children.");
                    return;
                }

                childrenToMove.AddRange(valueElement.Children);
            }
            else
            {
                childrenToMove.Add(elementChild);
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
                    }
                }
            }

            Element.Request(() =>
            {
                groupElement.Update();
                foreach (var child in childrenToMove)
                {
                    groupElement.Children.Add(child);
                }
            });
            CallNextProcessor();
        }
    }
}
