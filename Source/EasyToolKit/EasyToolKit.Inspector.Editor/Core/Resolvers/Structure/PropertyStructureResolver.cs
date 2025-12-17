using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for property structure resolvers in the inspector system.
    /// Provides common functionality for property structure resolution without collection operations.
    /// </summary>
    public abstract class PropertyStructureResolver : IPropertyStructureResolver
    {
        private int? _lastChildCountUpdateId;
        private int _childCount;

        /// <summary>
        /// Gets the InspectorProperty associated with this resolver
        /// </summary>
        public InspectorProperty Property { get; private set; }

        /// <summary>
        /// Gets whether this resolver has been initialized
        /// </summary>
        public bool IsInitialized { get; private set; }

        InspectorProperty IInitializableResolver.Property
        {
            get => Property;
            set => Property = value;
        }

        bool IInitializable.IsInitialized => IsInitialized;

        public int ChildCount
        {
            get
            {
                if (_lastChildCountUpdateId != Property.Tree.UpdateId)
                {
                    _lastChildCountUpdateId = Property.Tree.UpdateId;
                    _childCount = CalculateChildCount();
                }
                return _childCount;
            }
        }

        void IInitializable.Initialize()
        {
            if (IsInitialized) return;
            Initialize();
            IsInitialized = true;
        }

        void IInitializable.Deinitialize()
        {
            if (!IsInitialized) return;
            Deinitialize();
            IsInitialized = false;
        }

        /// <summary>
        /// Initializes the resolver (can be overridden by derived classes)
        /// </summary>
        protected virtual void Initialize() { }

        /// <summary>
        /// Deinitializes the resolver (can be overridden by derived classes)
        /// </summary>
        protected virtual void Deinitialize() { }

        /// <summary>
        /// Gets information about a child property at the specified index
        /// </summary>
        /// <param name="childIndex">The index of the child property</param>
        /// <returns>Information about the child property</returns>
        public abstract InspectorPropertyInfo GetChildInfo(int childIndex);

        /// <summary>
        /// Converts a child property name to its index
        /// </summary>
        /// <param name="name">The name of the child property</param>
        /// <returns>The index of the child property, or -1 if not found</returns>
        public abstract int ChildNameToIndex(string name);

        /// <summary>
        /// Calculates the number of child properties
        /// </summary>
        /// <returns>The number of child properties</returns>
        public abstract int CalculateChildCount();
    }
}