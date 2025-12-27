using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [PostProcessorPriority(PostProcessorPriorityLevel.Super)]
    public class HandleGroupPostProcessor : PostProcessor
    {
        protected override bool CanProcess(IElement element)
        {
            return element.GetAttribute<BeginGroupAttribute>(includeDerived: true) != null &&
                   !element.Definition.Flags.IsGroup();
        }

        protected override void Process()
        {
            var beginGroupAttributeInfo = Element.GetAttributeInfos()
                .First(info => info.Attribute is BeginGroupAttribute);

            var beginGroupAttribute = (BeginGroupAttribute)beginGroupAttributeInfo.Attribute;
            var beginGroupAttributeType = beginGroupAttribute.GetType();
            var endGroupAttributeType = InspectorAttributeUtility.GetCorrespondGroupAttributeType(beginGroupAttributeType);

            var groupDefinition = InspectorElements.Configurator.Group()
                .WithGroupAttributes(beginGroupAttributeType, endGroupAttributeType)
                .WithName(beginGroupAttribute.GroupName)
                .CreateDefinition();
            var groupElement = Element.SharedContext.Tree.ElementFactory.CreateGroupElement(groupDefinition, null);

            var parentCollection = ElementUtility.GetParentCollection(Element);
            var elementIndex = parentCollection.IndexOf(Element);
            parentCollection.Insert(elementIndex, groupElement);

            var childrenToMove = new List<IElement>();

            if (beginGroupAttributeInfo.Source == ElementAttributeSource.Type)
            {
                var valueElement = (IValueElement)Element;
                if (valueElement.Children == null)
                {
                    Debug.LogWarning($"Can not use BeginGroupAttribute on a value element '{Element}' that can not have children.");
                    return;
                }

                childrenToMove.AddRange(valueElement.Children);
            }
            else
            {
                childrenToMove.Add(Element);
                if (!beginGroupAttribute.EndAfterThisProperty)
                {
                    var groupCatalogue = beginGroupAttribute.GroupCatalogue;
                    var subGroupStack = new Stack<IElement>();
                    for (int i = elementIndex + 1; i < parentCollection.Count; i++)
                    {
                        var child = parentCollection[i];

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

            parentCollection.OwnerElement.Request(() =>
            {
                groupElement.InitializeChildren(childrenToMove);
                groupElement.Update();
            });
        }
    }
}
