using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [PostProcessorPriority(PostProcessorPriorityLevel.Super - 1)]
    public class GroupElementPostProcessor : PostProcessor
    {
        private Dictionary<Attribute, bool> _processedAttributeCache;
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
            // Clear the cache at the start of each ProcessImpl call
            _processedAttributeCache?.Clear();

            if (!TryFindNextElement(ref elementIndex, out var beginGroupAttributeInfo))
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

        private int i = 0;
        private bool TryFindNextElement(ref int elementIndex, out ElementAttributeInfo beginGroupAttributeInfo)
        {
            beginGroupAttributeInfo = null;

            for (; elementIndex < Element.Children.Count; elementIndex++)
            {
                var child = Element.Children[elementIndex];
                foreach (var attributeInfo in child.GetAttributeInfos())
                {
                    i++;
                    if (i > 100)
                    {
                        ;
                    }

                    var attributeType = attributeInfo.Attribute.GetType();
                    if (!attributeType.IsInheritsFrom<BeginGroupAttribute>())
                    {
                        continue;
                    }

                    // NOTE: We must check all parent group elements (not just the immediate Element) to avoid infinite recursion.
                    // This is necessary when a child element has multiple GroupAttributes (e.g., GroupAttributeA and GroupAttributeB).
                    // The processing flow in such cases is:
                    //   1. Process GroupAttributeA → Create GroupA → child becomes GroupA's child
                    //   2. Process GroupAttributeB → Create GroupB → child becomes GroupB's child, GroupB becomes GroupA's child
                    //   3. On next iteration, Element is GroupB and child is checked for GroupAttributes
                    // Without checking parent groups, child's GroupAttributeA would be detected as "unprocessed"
                    // (since GroupB only has GroupAttributeB), causing another GroupA to be created → infinite recursion.
                    if (IsAttributeProcessedInParentGroups(attributeInfo))
                    {
                        continue;
                    }

                    beginGroupAttributeInfo = attributeInfo;
                    return true;
                }
            }

            return false;
        }

        // NOTE: Checks if the given GroupAttribute has already been processed by any parent GroupElement.
        // This prevents infinite recursion when elements have multiple GroupAttributes by walking up the
        // element tree to check if any ancestor GroupElement already has this attribute.
        //
        // Example scenario that would cause infinite recursion without this check:
        //   - child has GroupAttributeA and GroupAttributeB
        //   - GroupA created (contains GroupAttributeA)
        //   - GroupB created (contains GroupAttributeB), child becomes GroupB's child, GroupB becomes GroupA's child
        //   - On next iteration with Element=GroupB: child still has both GroupAttributeA and GroupAttributeB
        //   - Without parent check: GroupAttributeA appears "unprocessed" → creates duplicate GroupA → recursion
        //   - With parent check: Detects GroupA (parent) already has GroupAttributeA → correctly skips it
        private bool IsAttributeProcessedInParentGroups(ElementAttributeInfo attributeInfo)
        {
            var attribute = attributeInfo.Attribute;

            // Check cache first to avoid redundant parent traversals
            if (_processedAttributeCache != null && _processedAttributeCache.TryGetValue(attribute, out var cachedResult))
            {
                return cachedResult;
            }

            // Initialize cache on first use
            _processedAttributeCache ??= new Dictionary<Attribute, bool>();

            // Walk up the parent chain to check if any parent group has this attribute
            var current = Element;
            bool found = false;
            while (current is IGroupElement groupElement)
            {
                if (groupElement.TryGetAttributeInfo(attribute, out _))
                {
                    found = true;
                    break;
                }
                current = groupElement.Parent;
            }

            // Cache the result for future lookups
            _processedAttributeCache[attribute] = found;
            return found;
        }
    }
}
