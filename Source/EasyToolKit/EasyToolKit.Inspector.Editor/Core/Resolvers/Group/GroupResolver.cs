using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for resolving group properties in the <see cref="InspectorProperty"/> system
    /// </summary>
    public interface IGroupResolver : IInitializableResolver
    {
        /// <summary>
        /// Gets all properties that belong to the same group as the current property
        /// </summary>
        /// <param name="beginGroupAttributeType">The type of the begin group attribute</param>
        /// <returns>Array of properties in the group</returns>
        InspectorProperty[] GetGroupProperties(Type beginGroupAttributeType);
    }

    /// <summary>
    /// Abstract base class for group property resolver in the <see cref="InspectorProperty"/> system
    /// </summary>
    public abstract class GroupResolver : IGroupResolver
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
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Deinitializes the resolver (can be overridden by derived classes)
        /// </summary>
        protected virtual void Deinitialize()
        {
        }

        /// <summary>
        /// Gets all properties that belong to the same group as the current property
        /// </summary>
        /// <param name="beginGroupAttributeType">The type of the begin group attribute</param>
        /// <returns>Array of properties in the group</returns>
        public abstract InspectorProperty[] GetGroupProperties(Type beginGroupAttributeType);
    }
}
