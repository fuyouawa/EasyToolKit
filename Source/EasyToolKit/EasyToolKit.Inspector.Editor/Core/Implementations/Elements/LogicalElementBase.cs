using System;
using System.Linq;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    public abstract class LogicalElementBase : ElementBase, ILogicalElement
    {
        [CanBeNull] private readonly ILogicalElement _logicalParent;
        [CanBeNull] private IReadOnlyElementList<ILogicalElement> _logicalChildren;
        private string _path;

        private IStructureResolver _structureResolver;

        protected LogicalElementBase(
            [NotNull] IElementDefinition definition,
            [NotNull] IElementSharedContext sharedContext,
            [CanBeNull] ILogicalElement logicalParent)
            : base(definition, sharedContext)
        {
            _logicalParent = logicalParent;
            Parent = logicalParent;
        }

        /// <summary>
        /// Gets the logical parent element that owns this element in the code structure.
        /// </summary>
        public ILogicalElement LogicalParent => _logicalParent;

        public override string Path
        {
            get
            {
                if (_path == null)
                {
                    if (LogicalParent != null)
                    {
                        var children = LogicalParent.LogicalChildren!;
                        var index = children.IndexOf(this);
                        if (index >= 0)
                        {
                            _path = children.GetPath(index);
                        }
                    }
                    else
                    {
                        _path = $"$CUSTOM$.{Definition.Name}";
                    }
                }

                return _path ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets the child elements defined by the code structure.
        /// These are immutable and determined solely by the definition.
        /// </summary>
        public IReadOnlyElementList<ILogicalElement> LogicalChildren
        {
            get
            {
                ValidateDisposed();
                return _logicalChildren;
            }
        }

        protected override bool CanHaveChildren()
        {
            return _structureResolver != null;
        }

        public override bool PostProcessIfNeeded()
        {
            // Begin batch mode to queue all events during post-processing
            // This significantly improves performance for deeply nested element hierarchies
            SharedContext.BeginBatchMode();
            try
            {
                var needed = base.PostProcessIfNeeded();
                if (_logicalChildren != null)
                {
                    foreach (var child in _logicalChildren)
                    {
                        if (child.PostProcessIfNeeded())
                        {
                            needed = true;
                        }
                    }
                }

                return needed;
            }
            finally
            {
                // End batch mode and flush all queued events at once
                // This triggers all ElementMovedEventArgs events in a single batch
                SharedContext.EndBatchMode();
            }
        }

        protected override void OnUpdate(bool forceUpdate)
        {
            if (_logicalChildren != null)
            {
                foreach (var child in _logicalChildren)
                {
                    child.Update(forceUpdate);
                }
            }

            base.OnUpdate(forceUpdate);
        }

        protected override void Dispose()
        {
            if (_logicalChildren != null)
            {
                foreach (var logicalChild in _logicalChildren)
                {
                    logicalChild.Destroy();
                }
            }

            (_logicalChildren as IDisposable)?.Dispose();

            // Release structure resolver back to pool
            if (_structureResolver != null)
            {
                ResolverUtility.ReleaseResolver(_structureResolver);
                _structureResolver = null;
            }

            base.Dispose();
        }

        /// <summary>
        /// Creates the logical children list based on the structure resolver.
        /// </summary>
        /// <returns>A read-only list of child elements defined by the structure.</returns>
        [NotNull]
        protected virtual IReadOnlyElementList<ILogicalElement> CreateLogicalChildren()
        {
            var childrenDefinitions = _structureResolver?.GetChildrenDefinitions();

            var children = new ElementList<ILogicalElement>(this,
                childrenDefinitions?.Select(definition => (ILogicalElement)SharedContext.Tree.ElementFactory.CreateElement(definition, this)));

            return children;
        }

        public override bool Request(Action action, bool forceDelay = false)
        {
            if (_logicalParent != null &&
                (_logicalParent.Phases.IsRefreshing() || _logicalParent.Phases.IsPendingRefresh()))
            {
                return false;
            }

            return base.Request(action, forceDelay);
        }

        protected override void Refresh()
        {
            {
                // Release old structure resolver before creating new one
                if (_structureResolver != null)
                {
                    ResolverUtility.ReleaseResolver(_structureResolver);
                    _structureResolver = null;
                }

                // Initialize structure resolver (before children)
                var factory = SharedContext.GetResolverFactory<IStructureResolver>();
                _structureResolver = factory.CreateResolver(this);
                if (_structureResolver != null)
                {
                    _structureResolver.Element = this;
                }
            }

            if (_logicalChildren != null)
            {
                Assert.IsFalse(Phases.IsDrawing(), "Element is drawing when refreshing children.");
                foreach (var logicalChild in _logicalChildren)
                {
                    logicalChild.Destroy();
                }

#if UNITY_ASSERTIONS
                foreach (var logicalChild in _logicalChildren)
                {
                    Assert.IsFalse(Children.Contains(logicalChild),
                        () => $"Disposed logical child '{logicalChild}' is still in children list.");
                }
#endif
            }

            (_logicalChildren as IDisposable)?.Dispose();

            // Recreate children if needed
            if (CanHaveChildren())
            {
                // Recreate logical children
                _logicalChildren = CreateLogicalChildren();
            }
            else
            {
                _logicalChildren = null;
            }

            base.Refresh();
        }
    }
}
