using System;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Specifies the source of an attribute in the inspector property system
    /// </summary>
    public enum AttributeSource
    {
        /// <summary>
        /// Attribute is defined on a member (field, property, method)
        /// </summary>
        Member,

        /// <summary>
        /// Attribute is defined on a type (class, struct, interface)
        /// </summary>
        Type,

        /// <summary>
        /// Attribute is defined on a list/collection type/field but is passed to each element
        /// </summary>
        ListPassToElement
    }

    /// <summary>
    /// Interface for resolving attributes associated with an <see cref="InspectorProperty"/>
    /// </summary>
    public interface IAttributeResolver : IInitializableResolver
    {
        /// <summary>
        /// Gets all attributes associated with the property
        /// </summary>
        /// <returns>Array of attributes</returns>
        Attribute[] GetAttributes();

        /// <summary>
        /// Gets the source of an attribute to determine where it was originally defined
        /// </summary>
        /// <param name="attribute">The attribute to check</param>
        /// <returns>The source of the attribute indicating whether it was defined on a member, type, or passed from a list</returns>
        AttributeSource GetAttributeSource(Attribute attribute);
    }

    /// <summary>
    /// Abstract base class for attribute resolver in the <see cref="InspectorProperty"/> system
    /// </summary>
    public abstract class AttributeResolver : IAttributeResolver
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
        /// Gets all attributes associated with the property
        /// </summary>
        /// <returns>Array of attributes</returns>
        public abstract Attribute[] GetAttributes();

        /// <summary>
        /// Gets the source of an attribute to determine where it was originally defined
        /// </summary>
        /// <param name="attribute">The attribute to check</param>
        /// <returns>The source of the attribute indicating whether it was defined on a member, type, or passed from a list</returns>
        public abstract AttributeSource GetAttributeSource(Attribute attribute);
    }
}
