using System;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for attribute resolver in the <see cref="InspectorProperty"/> system
    /// </summary>
    public abstract class AttributeResolverBase : InspectorResolverBase, IAttributeResolver
    {
        private bool _isInitialized;

        /// <summary>
        /// Override this method to perform initialization logic
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Gets all attributes associated with the property
        /// </summary>
        /// <returns>Array of attributes</returns>
        protected abstract Attribute[] GetAttributes();

        /// <summary>
        /// Gets the source of an attribute to determine where it was originally defined
        /// </summary>
        /// <param name="attribute">The attribute to check</param>
        /// <returns>The source of the attribute indicating whether it was defined on a member, type, or passed from a list</returns>
        protected abstract AttributeSource GetAttributeSource(Attribute attribute);

        private void EnsureInitialize()
        {
            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }
        }

        Attribute[] IAttributeResolver.GetAttributes()
        {
            EnsureInitialize();
            return GetAttributes();
        }

        AttributeSource IAttributeResolver.GetAttributeSource(Attribute attribute)
        {
            EnsureInitialize();
            return GetAttributeSource(attribute);
        }
    }
}
