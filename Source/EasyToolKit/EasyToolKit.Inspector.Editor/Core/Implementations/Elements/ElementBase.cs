using System;
using System.Collections.Generic;
using System.Linq;
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
    public abstract class ElementBase : IElement, IDisposable
    {
        [CanBeNull] private IReadOnlyElementList<IElement> _logicalChildren;
        [CanBeNull] private IElementList<IElement> _children;
        private int? _lastUpdateId;
        private string _path;
        private GUIContent _label;
        private bool _disposed;
        private bool _isDrawing;

        private IStructureResolver _structureResolver;
        [CanBeNull]  private IAttributeResolver _attributeResolver;
        [CanBeNull] private IDrawerChainResolver _drawerChainResolver;
        [CanBeNull] private IPostProcessorChainResolver _postProcessorChainResolver;

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
        public virtual string Path
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
        public IReadOnlyElementList<IElement> LogicalChildren
        {
            get
            {
                ValidateDisposed();
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
                ValidateDisposed();
                return _children;
            }
        }

        public bool IsDrawing => _isDrawing;

        /// <summary>
        /// Gets all custom attribute infos applied to this element.
        /// </summary>
        /// <returns>An array of element attribute infos.</returns>
        public IReadOnlyList<ElementAttributeInfo> GetAttributeInfos()
        {
            ValidateDisposed();
            if (_attributeResolver == null)
            {
                return Array.Empty<ElementAttributeInfo>();
            }

            return _attributeResolver.GetAttributeInfos();
        }

        /// <summary>
        /// Gets the drawer chain for rendering this element.
        /// </summary>
        /// <returns>The drawer chain containing all applicable drawers.</returns>
        public DrawerChain GetDrawerChain()
        {
            ValidateDisposed();
            if (_drawerChainResolver == null)
            {
                return new DrawerChain(this, Array.Empty<IEasyDrawer>());
            }

            return _drawerChainResolver.GetDrawerChain();
        }

        public PostProcessorChain GetPostProcessorChain()
        {
            ValidateDisposed();
            if (_postProcessorChainResolver == null)
            {
                return new PostProcessorChain(this, Array.Empty<IPostProcessor>());
            }

            return _postProcessorChainResolver.GetPostProcessorChain();
        }

        public void Request(Action action)
        {
            if (_isDrawing)
            {
                SharedContext.Tree.QueueCallbackUntilRepaint(action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Draws this element in the inspector with the specified label.
        /// </summary>
        /// <param name="label">The label to display. If null, uses the element's default label.</param>
        public virtual void Draw(GUIContent label)
        {
            ValidateDisposed();
            ((IElement)this).Update();

            var chain = GetDrawerChain();
            chain.Reset();

            if (chain.MoveNext() && chain.Current != null)
            {
                _isDrawing = true;
                chain.Current.Draw(label);
                _isDrawing = false;
            }
        }

        /// <summary>
        /// Refreshes all element state, forcing recreation of resolvers and children.
        /// </summary>
        public void RequestRefresh()
        {
            ValidateDisposed();
            Request(Refresh);
        }

        protected virtual bool CanHaveChildren()
        {
            return _structureResolver != null;
        }

        protected virtual void OnUpdate(bool forceUpdate)
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected virtual void Dispose()
        {
            if (_logicalChildren != null)
            {
                foreach (var child in _logicalChildren)
                {
                    (child as IDisposable)?.Dispose();
                }
            }

            if (Parent != null)
            {
                Parent.Children.TryRemove(this);
            }

            if (_children != null)
            {
                _children.BeforeElementMoved -= OnChildrenElementMoved;
                _children.AfterElementMoved -= OnChildrenElementMoved;
            }

            SharedContext.UnregisterEventHandler<ElementMovedEventArgs>(OnElementMoved);

            (_logicalChildren as IDisposable)?.Dispose();
            (_children as IDisposable)?.Dispose();
        }

        /// <summary>
        /// Creates the logical children list based on the structure resolver.
        /// </summary>
        /// <returns>A read-only list of child elements defined by the structure.</returns>
        [NotNull]
        protected virtual IReadOnlyElementList<IElement> CreateLogicalChildren()
        {
            var childrenDefinitions = _structureResolver.GetChildrenDefinitions();

            var children = new ElementList<IElement>(this,
                childrenDefinitions.Select(definition => SharedContext.Tree.ElementFactory.CreateElement(definition, this)));

            return children;
        }

        [NotNull]
        protected virtual IElementList<IElement> CreateChildren(IReadOnlyList<IElement> oldChildren)
        {
            var children = new RequestedElementList<IElement>(this, oldChildren.Concat(_logicalChildren!));
            children.BeforeElementMoved += OnChildrenElementMoved;
            children.AfterElementMoved += OnChildrenElementMoved;
            return children;
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
                    Request(() => Parent = args.NewParent);
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

        protected void ValidateDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public override string ToString()
        {
            var typeName = GetType().Name;
            var suffix = "Element";
            return $"{Path}:{typeName.Substring(0, typeName.Length - suffix.Length)}";
        }


        private void Refresh()
        {
            {
                // Initialize structure resolver (before children)
                var factory = SharedContext.GetResolverFactory<IStructureResolver>();
                _structureResolver = factory.CreateResolver(this);
                if (_structureResolver != null)
                {
                    _structureResolver.Element = this;
                }
            }

            // Recreate children if needed
            if (CanHaveChildren())
            {
                Assert.IsFalse(_isDrawing, "Element is drawing when refreshing children.");

                if (_logicalChildren != null)
                {
                    foreach (var logicalChild in _logicalChildren)
                    {
                        (logicalChild as IDisposable)?.Dispose();
                    }

#if UNITY_ASSERTIONS
                    foreach (var logicalChild in _logicalChildren)
                    {
                        Assert.IsFalse(_children.Contains(logicalChild),
                            () => $"Disposed logical child '{logicalChild}' is still in children list.");
                    }
#endif
                }

                (_logicalChildren as IDisposable)?.Dispose();
                // Recreate logical children
                _logicalChildren = CreateLogicalChildren();

                var oldChildren = new List<IElement>(_children ?? Enumerable.Empty<IElement>());
                (_children as IDisposable)?.Dispose();
                // Recreate children list
                _children = CreateChildren(oldChildren);
            }

            {
                // Initialize attribute resolver (after children)
                var factory = SharedContext.GetResolverFactory<IAttributeResolver>();
                _attributeResolver = factory.CreateResolver(this);
                if (_attributeResolver != null)
                {
                    _attributeResolver.Element = this;
                }
            }

            {
                // Initialize drawer chain resolver (after children)
                var factory = SharedContext.GetResolverFactory<IDrawerChainResolver>();
                _drawerChainResolver = factory.CreateResolver(this);
                if (_drawerChainResolver != null)
                {
                    _drawerChainResolver.Element = this;
                }
            }

            {
                // Initialize post processor chain resolver (after children)
                var factory = SharedContext.GetResolverFactory<IPostProcessorChainResolver>();
                _postProcessorChainResolver = factory.CreateResolver(this);
                if (_postProcessorChainResolver != null)
                {
                    _postProcessorChainResolver.Element = this;
                }
            }

            PostProcess();
        }

        private void PostProcess()
        {
            var chain = GetPostProcessorChain();
            chain.Reset();
            while (chain.MoveNext() && chain.Current != null)
            {
                chain.Current.Process();
            }
        }

        void IElement.Update(bool forceUpdate)
        {
            if (_lastUpdateId == SharedContext.UpdateId && !forceUpdate)
            {
                return;
            }

            if (_lastUpdateId == null)
            {
                RequestRefresh();
            }

            _lastUpdateId = SharedContext.UpdateId;
            OnUpdate(forceUpdate);
        }

        void IDisposable.Dispose()
        {
            if (_disposed)
                return;

            Dispose();

            _disposed = true;
        }
    }
}
