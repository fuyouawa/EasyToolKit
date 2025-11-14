using System;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Specifies the source of an attribute in the InspectorProperty system
    /// </summary>
    public enum AttributeSource
    {
        Member,
        Type,
        ListPassToElement
    }

    public interface IAttributeResolver : IInitializableResolver
    {
        Attribute[] GetAttributes();

        /// <summary>
        /// Gets the source of an attribute to determine if it's a class-level attribute
        /// </summary>
        AttributeSource GetAttributeSource(Attribute attribute);
    }

    public abstract class AttributeResolver : IAttributeResolver
    {
        public InspectorProperty Property { get; private set; }
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

        protected virtual void Initialize() { }
        protected virtual void Deinitialize() { }

        public abstract Attribute[] GetAttributes();
        public abstract AttributeSource GetAttributeSource(Attribute attribute);
    }
}
