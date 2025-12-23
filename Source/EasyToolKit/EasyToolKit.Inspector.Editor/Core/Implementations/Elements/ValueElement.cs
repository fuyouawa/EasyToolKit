using System;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Provides a value element implementation that represents data-containing elements
    /// such as fields, properties, or dynamically created custom values.
    /// </summary>
    public class ValueElement : ElementBase, IValueElement
    {
        private IReadOnlyElementList<IElement> _logicalChildren;
        private IElementList<IElement> _children;
        private IValueEntry _valueEntry;

        /// <summary>
        /// Gets the value definition that describes this value element.
        /// </summary>
        public new IValueDefinition Definition => (IValueDefinition)base.Definition;

        /// <summary>
        /// Gets the child elements defined by the code structure.
        /// These are immutable and determined solely by the definition.
        /// </summary>
        public IReadOnlyElementList<IElement> LogicalChildren
        {
            get
            {
                if (_logicalChildren == null && CanHaveChildren())
                {
                    _logicalChildren = CreateLogicalChildren();
                }

                return _logicalChildren;
            }
        }

        /// <summary>
        /// Gets child elements that were added or removed at runtime.
        /// This collection is mutable and reflects runtime modifications.
        /// </summary>
        public IElementList<IElement> Children
        {
            get
            {
                if (_children == null && CanHaveChildren())
                {
                    var childrenList = new ElementList<IElement>(this);
                    _children = childrenList;

                    // Initialize with logical children
                    if (_logicalChildren != null)
                    {
                        foreach (var child in _logicalChildren)
                        {
                            childrenList.Insert(childrenList.Count, child);
                        }
                    }
                }

                return _children;
            }
        }

        /// <summary>
        /// Gets the value entry that manages the underlying value storage and change notifications.
        /// </summary>
        public IValueEntry ValueEntry
        {
            get
            {
                if (_valueEntry == null)
                {
                    _valueEntry = CreateValueEntry();
                }

                return _valueEntry;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueElement"/> class.
        /// </summary>
        /// <param name="definition">The value definition that describes this element.</param>
        /// <param name="sharedContext">The shared context providing access to tree-level services.</param>
        /// <param name="logicalParent">The logical parent element in the code structure.</param>
        public ValueElement(
            [NotNull] IValueDefinition definition,
            [NotNull] IElementSharedContext sharedContext,
            [CanBeNull] IValueElement logicalParent)
            : base(definition, sharedContext, logicalParent)
        {
        }

        protected virtual bool CanHaveChildren()
        {
            var valueType = Definition.ValueType;
            // Check if it's a plain value type or UnityEngine.Object reference
            return !valueType.IsBasicValueType() && !typeof(UnityEngine.Object).IsAssignableFrom(valueType);
        }

        protected override void Update(bool force)
        {
            ValueEntry.Update();
        }

        /// <summary>
        /// Creates the logical children list based on the structure resolver.
        /// </summary>
        /// <returns>A read-only list of child elements defined by the structure.</returns>
        private IReadOnlyElementList<IElement> CreateLogicalChildren()
        {
            var factory = SharedContext.GetResolverFactory<IStructureResolver>();
            var resolver = factory.CreateResolver(this);
            var childrenDefinitions = resolver.GetChildrenDefinitions();

            var childrenList = new ElementList<IElement>(this);

            foreach (var childDefinition in childrenDefinitions)
            {
                var childElement = SharedContext.Tree.Factory.CreateElement(childDefinition, this);
                childrenList.Insert(childrenList.Count, childElement);
            }

            return childrenList;
        }

        /// <summary>
        /// Creates the value entry for managing the underlying value storage.
        /// </summary>
        /// <returns>A value entry instance for this element.</returns>
        protected virtual IValueEntry CreateValueEntry()
        {
            var valueType = Definition.ValueType;
            var valueEntryType = typeof(ValueEntry<>).MakeGenericType(valueType);

            return valueEntryType.CreateInstance<IValueEntry>(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            _logicalChildren?.ClearCache();
            _children?.ClearCache();

            base.Dispose();
        }
    }
}
