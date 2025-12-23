using System;
using System.Collections.Generic;
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
        public IValueElement LogicalParent { get; }

        /// <summary>
        /// Gets the current parent element in the element tree hierarchy.
        /// </summary>
        public IElement Parent { get; protected set; }

        /// <summary>
        /// Gets the runtime state of this element.
        /// </summary>
        public IElementState State { get; }

        private int? _lastUpdateId;
        private string _path;
        private GUIContent _label;
        private IAttributeResolver _attributeResolver;
        private IDrawerChainResolver _drawerChainResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementBase"/> class.
        /// </summary>
        /// <param name="definition">The definition that describes this element.</param>
        /// <param name="sharedContext">The shared context providing access to tree-level services.</param>
        /// <param name="logicalParent">The logical parent element in the code structure.</param>
        protected ElementBase(
            [NotNull] IElementDefinition definition,
            [NotNull] IElementSharedContext sharedContext,
            [CanBeNull] IValueElement logicalParent)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            SharedContext = sharedContext ?? throw new ArgumentNullException(nameof(sharedContext));
            LogicalParent = logicalParent;
            Parent = logicalParent;

            State = new ElementState(this);
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

        public virtual void OnChangeParent(IElement newParent)
        {
            Parent = newParent;
        }

        /// <summary>
        /// Gets all custom attribute infos applied to this element.
        /// </summary>
        /// <returns>An array of element attribute infos.</returns>
        public IReadOnlyList<ElementAttributeInfo> GetAttributeInfos()
        {
            if (_attributeResolver == null)
            {
                var factory = SharedContext.GetResolverFactory<IAttributeResolver>();
                _attributeResolver = factory.CreateResolver(this);
            }

            return _attributeResolver.GetAttributeInfos();
        }

        /// <summary>
        /// Gets the drawer chain for rendering this element.
        /// </summary>
        /// <returns>The drawer chain containing all applicable drawers.</returns>
        public DrawerChain GetDrawerChain()
        {
            if (_drawerChainResolver == null)
            {
                var factory = SharedContext.GetResolverFactory<IDrawerChainResolver>();
                _drawerChainResolver = factory.CreateResolver(this);
            }

            return _drawerChainResolver.GetDrawerChain();
        }

        protected virtual void Update(bool force)
        {
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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        void IElement.Update(bool force)
        {
            if (_lastUpdateId == SharedContext.UpdateId && !force)
            {
                return;
            }

            _lastUpdateId = SharedContext.UpdateId;
            Update(force);
        }
    }
}
