using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IElement"/> interface,
    /// serving as the foundation for all inspector element implementations.
    /// </summary>
    public abstract class ElementBase : IElement
    {
        private IReadOnlyElementList<IElement> _logicalChildren;
        private IElementList<IElement> _children;
        private int? _lastUpdateId;
        private string _path;
        private GUIContent _label;
        private bool _disposed;

        private IStructureResolver _structureResolver;
        private IAttributeResolver _attributeResolver;
        private IDrawerChainResolver _drawerChainResolver;

        /// <summary>
        /// Gets the definition that describes this element.
        /// </summary>
        public IElementDefinition Definition { get; }

        /// <summary>
        /// Gets the element shared context that provides access to tree-level services and resolver factories.
        /// </summary>
        public IElementSharedContext SharedContext { get; }

        /// <summary>
        /// Gets the logical parent element that owns this element in the code structure.
        /// </summary>
        public IElement LogicalParent { get; }

        /// <summary>
        /// Gets the current parent element in the element tree hierarchy.
        /// </summary>
        public IElement Parent { get; protected set; }

        /// <summary>
        /// Gets the runtime state of this element.
        /// </summary>
        public IElementState State { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementBase"/> class.
        /// </summary>
        /// <param name="definition">The definition that describes this element.</param>
        /// <param name="sharedContext">The shared context providing access to tree-level services.</param>
        /// <param name="logicalParent">The logical parent element in the code structure.</param>
        protected ElementBase(
            [NotNull] IElementDefinition definition,
            [NotNull] IElementSharedContext sharedContext,
            [CanBeNull] IElement logicalParent)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            SharedContext = sharedContext ?? throw new ArgumentNullException(nameof(sharedContext));
            LogicalParent = logicalParent;
            Parent = logicalParent;

            State = new ElementState(this);

            SharedContext.RegisterEventHandler<ElementMovedEventArgs>(OnElementMoved);
        }

        /// <summary>
        /// Gets the hierarchical path of this element.
        /// </summary>
        public string Path
        {
            get
            {
                if (_path == null && LogicalParent?.LogicalChildren != null)
                {
                    var children = LogicalParent.LogicalChildren;
                    var index = children.IndexOf(this);
                    if (index >= 0)
                    {
                        _path = children.GetPath(index);
                    }
                }

                return _path ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the label displayed in the inspector.
        /// </summary>
        public GUIContent Label
        {
            get
            {
                if (_label == null)
                {
                    var displayName = ObjectNames.NicifyVariableName(Definition.Name);
                    _label = new GUIContent(displayName);
                }

                return _label;
            }
            set => _label = value;
        }

        /// <summary>
        /// Gets the child elements defined by the code structure.
        /// These are immutable and determined solely by the definition.
        /// </summary>
        public IReadOnlyElementList<IElement> LogicalChildren => _logicalChildren;

        /// <summary>
        /// Gets child elements that were added or removed at runtime.
        /// This collection is mutable and reflects runtime modifications.
        /// </summary>
        public IElementList<IElement> Children => _children;

        /// <summary>
        /// Gets all custom attribute infos applied to this element.
        /// </summary>
        /// <returns>An array of element attribute infos.</returns>
        public IReadOnlyList<ElementAttributeInfo> GetAttributeInfos()
        {
            return _attributeResolver.GetAttributeInfos();
        }

        /// <summary>
        /// Gets the drawer chain for rendering this element.
        /// </summary>
        /// <returns>The drawer chain containing all applicable drawers.</returns>
        public DrawerChain GetDrawerChain()
        {
            return _drawerChainResolver.GetDrawerChain();
        }

        /// <summary>
        /// Draws this element in the inspector with the specified label.
        /// </summary>
        /// <param name="label">The label to display. If null, uses the element's default label.</param>
        public virtual void Draw(GUIContent label)
        {
            ((IElement)this).Update();

            var chain = GetDrawerChain();
            chain.Reset();

            if (chain.MoveNext() && chain.Current != null)
            {
                chain.Current.Draw(label);
            }
        }

        /// <summary>
        /// Refreshes all element state, forcing recreation of resolvers and children.
        /// </summary>
        protected virtual void Refresh()
        {
            // Initialize structure resolver (before children)
            var factory = SharedContext.GetResolverFactory<IStructureResolver>();
            _structureResolver = factory.CreateResolver(this);

            // Recreate children if needed
            if (CanHaveChildren())
            {
                if (_logicalChildren != null)
                {
                    foreach (var logicalChild in _logicalChildren)
                    {
                        logicalChild.Dispose();
                    }

#if UNITY_ASSERTIONS
                    foreach (var logicalChild in _logicalChildren)
                    {
                        Assert.IsFalse(_children.Contains(logicalChild));
                    }
#endif
                }

                // Recreate logical children
                _logicalChildren = CreateLogicalChildren();

                // Recreate children list
                if (_children == null)
                {
                    _children = new ElementList<IElement>(this);
                    _children.BeforeElementMoved += OnChildrenElementMoved;
                    _children.AfterElementMoved += OnChildrenElementMoved;
                }

                // Initialize with logical children
                if (_logicalChildren != null)
                {
                    foreach (var child in _logicalChildren)
                    {
                        _children.Add(child);
                    }
                }
                OnCreatedChildren();
            }

            // Initialize attribute resolver (after children)
            var attributeFactory = SharedContext.GetResolverFactory<IAttributeResolver>();
            _attributeResolver = attributeFactory.CreateResolver(this);

            // Initialize drawer chain resolver (after children)
            var drawerFactory = SharedContext.GetResolverFactory<IDrawerChainResolver>();
            _drawerChainResolver = drawerFactory.CreateResolver(this);
        }

        protected virtual bool CanHaveChildren()
        {
            return _structureResolver != null;
        }

        protected virtual void Update(bool forceUpdate)
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected virtual void Dispose()
        {
            if (Parent != null)
            {
                Parent.Children.TryRemove(this);
            }

            _children.BeforeElementMoved -= OnChildrenElementMoved;
            _children.AfterElementMoved -= OnChildrenElementMoved;

            SharedContext.UnregisterEventHandler<ElementMovedEventArgs>(OnElementMoved);
        }

        protected virtual void OnCreatedChildren()
        {
        }

        /// <summary>
        /// Creates the logical children list based on the structure resolver.
        /// </summary>
        /// <returns>A read-only list of child elements defined by the structure.</returns>
        private IReadOnlyElementList<IElement> CreateLogicalChildren()
        {
            var childrenDefinitions = _structureResolver.GetChildrenDefinitions();

            var childrenList = new ElementList<IElement>(this);

            foreach (var childDefinition in childrenDefinitions)
            {
                var childElement = SharedContext.Tree.ElementFactory.CreateElement(childDefinition, this);
                childrenList.Insert(childrenList.Count, childElement);
            }

            return childrenList;
        }

        void IElement.Update(bool forceUpdate)
        {
            if (_lastUpdateId == SharedContext.UpdateId && !forceUpdate)
            {
                return;
            }

            if (_lastUpdateId == null)
            {
                Refresh();
            }

            _lastUpdateId = SharedContext.UpdateId;
            Update(forceUpdate);
        }

        void IDisposable.Dispose()
        {
            if (_disposed)
                return;

            Dispose();

            _disposed = true;
        }

        /// <summary>
        /// Handles the <see cref="ElementList{TElement}.AfterElementMoved"/> event from children collection.
        /// Triggers the event through the shared context to notify the entire element tree.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event arguments containing element move details.</param>
        private void OnChildrenElementMoved(object sender, ElementMovedEventArgs args)
        {
            SharedContext.TriggerEvent(this, args);
        }

        /// <summary>
        /// Handles the <see cref="ElementMovedEventArgs"/> event from the shared context.
        /// Updates parent-child relationships when elements are moved in the tree.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event arguments containing element move details.</param>
        private void OnElementMoved(object sender, ElementMovedEventArgs args)
        {
            var element = sender as IElement;
            if (args.Timing == ElementMovedTiming.After)
            {
                if (element == this)
                {
                    Parent = args.NewParent;
                    return;
                }

                if (args.ChangeType == ElementListChangeType.Insert && args.NewParent != this)
                {
                    // Element was moved to another parent but still exists in our children list
                    // This indicates the element needs to be updated/removed
                    _children?.TryRemove(element);
                }
            }
        }
    }
}
