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
        private ElementPhases _phases;

        [CanBeNull] private IAttributeResolver _attributeResolver;
        [CanBeNull] private IDrawerChainResolver _drawerChainResolver;
        [CanBeNull] private IPostProcessorChainResolver _postProcessorChainResolver;
        [CanBeNull] private IMessageDispatcher _messageDispatcher;

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
        /// Tries to get the attribute info for the specified attribute instance.
        /// </summary>
        /// <param name="attribute">The attribute to find.</param>
        /// <param name="attributeInfo">When this method returns, contains the attribute info if found; otherwise, null.</param>
        /// <returns>true if the attribute info was found; otherwise, false.</returns>
        public bool TryGetAttributeInfo(Attribute attribute, out ElementAttributeInfo attributeInfo)
        {
            ValidateDisposed();
            if (_attributeResolver == null)
            {
                attributeInfo = null;
                return false;
            }

            return _attributeResolver.TryGetAttributeInfo(attribute, out attributeInfo);
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
        /// Gets the message dispatcher for this element (lazy initialization).
        /// </summary>
        [NotNull] private IMessageDispatcher MessageDispatcher
        {
            get
            {
                if (_messageDispatcher == null)
                {
                    _messageDispatcher = MessageDispatcherFactory.Create(this);
                }

                return _messageDispatcher;
            }
        }

        /// <inheritdoc/>
        public bool Send(string messageName, object args = null)
        {
            ValidateDisposed();
            return MessageDispatcher.Dispatch(messageName, args);
        }

        /// <inheritdoc/>
        public TResult Send<TResult>(string messageName, object args = null)
        {
            ValidateDisposed();

            if (_messageDispatcher is IMessageDispatcher<TResult> typedDispatcher)
            {
                return typedDispatcher.Dispatch(messageName, args);
            }

            return default;
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

        /// <summary>
        /// Destroys this element, disposing it and removing it from the factory's tracking container.
        /// If the element is not in an idle state, the destruction is queued and executed later.
        /// </summary>
        public void Destroy()
        {
            if (_phases.IsDestroyed() || _phases.IsPendingDestroy() || _phases.IsDestroying())
            {
                return;
            }

            _phases = _phases.Add(ElementPhases.PendingDestroy);
            SharedContext.Tree.ElementFactory.DestroyElement(this);
        }

        protected virtual void OnUpdate(bool forceUpdate)
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

            if (_children != null)
            {
                _children.BeforeElementMoved -= OnChildrenElementMoved;
                _children.AfterElementMoved -= OnChildrenElementMoved;
            }

            (_children as IDisposable)?.Dispose();

            // Release resolvers back to pool
            if (_attributeResolver != null)
            {
                ResolverUtility.ReleaseResolver(_attributeResolver);
                _attributeResolver = null;
            }

            if (_drawerChainResolver != null)
            {
                ResolverUtility.ReleaseResolver(_drawerChainResolver);
                _drawerChainResolver = null;
            }

            if (_postProcessorChainResolver != null)
            {
                ResolverUtility.ReleaseResolver(_postProcessorChainResolver);
                _postProcessorChainResolver = null;
            }
        }

        [NotNull]
        protected virtual IElementList<IElement> CreateChildren()
        {
            var children = new RequestedElementList<IElement>(this);
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
        /// Handles element moved events by updating parent-child relationships.
        /// This method is called directly by <see cref="ElementList{TElement}"/> when elements are inserted or removed,
        /// rather than through global event broadcasting to avoid O(n²) performance degradation.
        /// </summary>
        /// <param name="args">The event arguments containing element move details.</param>
        [MessageHandler(ElementMessageNames.ElementMoved)]
        private void OnElementMoved(ElementMovedEventArgs args)
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
            if (_phases.IsDestroyed())
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
                _children.BeforeElementMoved += OnChildrenElementMoved;
                _children.AfterElementMoved += OnChildrenElementMoved;
            }
            else
            {
                _children = null;
            }

            {
                // Release old attribute resolver before creating new one
                if (_attributeResolver != null)
                {
                    ResolverUtility.ReleaseResolver(_attributeResolver);
                    _attributeResolver = null;
                }

                // Initialize attribute resolver (after children)
                var factory = SharedContext.GetResolverFactory<IAttributeResolver>();
                _attributeResolver = factory.CreateResolver(this);
                if (_attributeResolver != null)
                {
                    _attributeResolver.Element = this;
                }
            }

            {
                // Release old drawer chain resolver before creating new one
                if (_drawerChainResolver != null)
                {
                    ResolverUtility.ReleaseResolver(_drawerChainResolver);
                    _drawerChainResolver = null;
                }

                // Initialize drawer chain resolver (after children)
                var factory = SharedContext.GetResolverFactory<IDrawerChainResolver>();
                _drawerChainResolver = factory.CreateResolver(this);
                if (_drawerChainResolver != null)
                {
                    _drawerChainResolver.Element = this;
                }
            }

            {
                // Release old post processor chain resolver before creating new one
                if (_postProcessorChainResolver != null)
                {
                    ResolverUtility.ReleaseResolver(_postProcessorChainResolver);
                    _postProcessorChainResolver = null;
                }

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
            _phases = _phases.Add(ElementPhases.PendingPostProcess);
        }

        public virtual bool PostProcessIfNeeded()
        {
            if (_phases.IsPendingPostProcess())
            {
                var chain = GetPostProcessorChain();
                chain.Reset();
                while (chain.MoveNext() && chain.Current != null)
                {
                    _phases = _phases.Add(ElementPhases.PostProcessing);
                    chain.Current.Process();
                    _phases = _phases.Remove(ElementPhases.PostProcessing);
                }

                _phases = _phases.Remove(ElementPhases.PendingPostProcess);
                return true;
            }

            return false;
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
            if (_phases.IsDestroyed() || _phases.IsDestroying())
            {
                return;
            }

            _phases = _phases.Remove(ElementPhases.PendingDestroy);
            _phases = _phases.Add(ElementPhases.Destroying);
            Dispose();
            _phases = _phases.Add(ElementPhases.Destroyed);
        }
    }
}
