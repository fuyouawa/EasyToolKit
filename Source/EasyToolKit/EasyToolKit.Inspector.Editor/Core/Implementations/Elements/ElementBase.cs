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
        [CanBeNull] private IElementList<IElement> _children;
        private int? _lastUpdateId;
        private GUIContent _label;
        private bool _disposed;
        private ElementPhases _phases;

        [CanBeNull] private IAttributeResolver _attributeResolver;
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
        protected ElementBase(
            [NotNull] IElementDefinition definition,
            [NotNull] IElementSharedContext sharedContext)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            SharedContext = sharedContext ?? throw new ArgumentNullException(nameof(sharedContext));

            State = new ElementState(this);

            SharedContext.RegisterEventHandler<ElementMovedEventArgs>(OnElementMoved);
        }

        /// <summary>
        /// Gets the hierarchical path of this element.
        /// </summary>
        public abstract string Path { get; }

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

        /// <summary>
        /// Gets the current phase of this element.
        /// </summary>
        public ElementPhases Phases => _phases;

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

        public virtual bool Request(Action action, bool forceDelay = false)
        {
            if (_phases.IsDrawing())
            {
                SharedContext.Tree.QueueCallback(action);
            }
            else
            {
                action();
            }

            return true;
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
                _phases = _phases.Add(ElementPhases.Drawing);
                chain.Current.Draw(label);
                _phases = _phases.Remove(ElementPhases.Drawing);
            }
        }

        /// <summary>
        /// Refreshes all element state, forcing recreation of resolvers and children.
        /// </summary>
        public bool RequestRefresh()
        {
            ValidateDisposed();

            _phases = _phases.Add(ElementPhases.PendingRefresh);
            if (Request(Refresh))
            {
                return true;
            }
            _phases = _phases.Remove(ElementPhases.PendingRefresh);
            return false;
        }

        protected virtual void OnUpdate(bool forceUpdate)
        {
            _children?.Update();
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

            if (_children != null)
            {
                _children.BeforeElementMoved -= OnChildrenElementMoved;
                _children.AfterElementMoved -= OnChildrenElementMoved;
            }

            SharedContext.UnregisterEventHandler<ElementMovedEventArgs>(OnElementMoved);

            (_children as IDisposable)?.Dispose();
        }

        [NotNull]
        protected virtual IElementList<IElement> CreateChildren()
        {
            var children = new RequestedElementList<IElement>(this);
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
            if (args.Timing == ElementMovedTiming.After)
            {
                // NOTE: Checking args.NewParent != null is necessary for the following reason:
                // NewParent being null typically occurs when an element is removed from its original Children collection.
                // Moving an element from one Children to another is a two-step process:
                //   1) Remove from the original Children (triggers TryRemove)
                //   2) Add to the new Children
                // The execution order of these two steps is uncertain. It's possible that the element is added to
                // the new Children first, and then removed from the original Children.
                // If removal from the original Children occurs after being added to the new Children, it will
                // trigger ElementMovedEventArgs again with NewParent as null. Without checking NewParent != null,
                // this would incorrectly cause the element's Parent to be set to null.
                if (args.Element == this && args.NewParent != null)
                {
                    Request(() => Parent = args.NewParent);
                    return;
                }

                if (args.ChangeType == ElementListChangeType.Insert && args.NewParent != this)
                {
                    // Element was moved to another parent but still exists in our children list
                    // This indicates the element needs to be updated/removed
                    _children?.TryRemove(args.Element);
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


        protected virtual bool CanHaveChildren()
        {
            return false;
        }

        protected virtual void Refresh()
        {
            _phases = _phases.Remove(ElementPhases.PendingRefresh);
            _phases = _phases.Add(ElementPhases.Refreshing);

            (_children as IDisposable)?.Dispose();
            if (CanHaveChildren())
            {
                // Recreate children list
                _children = CreateChildren();
            }
            else
            {
                _children = null;
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

            _phases = _phases.Remove(ElementPhases.Refreshing);
            _phases = _phases.Add(ElementPhases.JustRefreshed);
        }

        public void PostProcess()
        {
            var chain = GetPostProcessorChain();
            chain.Reset();
            while (chain.MoveNext() && chain.Current != null)
            {
                _phases = _phases.Add(ElementPhases.PostProcessing);
                chain.Current.Process();
                _phases = _phases.Remove(ElementPhases.PostProcessing);
            }
        }

        void IElement.Update(bool forceUpdate)
        {
            if (_lastUpdateId == SharedContext.UpdateId && !forceUpdate)
            {
                return;
            }

            if (_phases.IsJustRefreshed())
            {
                _phases = _phases.Remove(ElementPhases.JustRefreshed);
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
