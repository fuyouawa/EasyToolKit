using System;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for resolving drawer chains for <see cref="InspectorProperty"/>
    /// </summary>
    public interface IDrawerChainResolver : IInitializableResolver
    {
        /// <summary>
        /// Gets the drawer chain for the property
        /// </summary>
        /// <returns>The drawer chain</returns>
        DrawerChain GetDrawerChain();
    }

    /// <summary>
    /// Abstract base class for drawer chain resolution in the <see cref="InspectorProperty"/> system
    /// </summary>
    public abstract class DrawerChainResolver : IDrawerChainResolver
    {
        /// <summary>
        /// Gets the <see cref="InspectorProperty"/> associated with this resolver
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
        /// Gets the drawer chain for the property
        /// </summary>
        /// <returns>The drawer chain</returns>
        public abstract DrawerChain GetDrawerChain();
    }
}
