using System;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for attribute resolver in the <see cref="InspectorProperty"/> system
    /// </summary>
    public abstract class AttributeResolverBase : ResolverBase, IAttributeResolver
    {
        private bool _isInitialized;

        /// <summary>
        /// Override this method to perform initialization logic
        /// </summary>
        protected virtual void Initialize()
        {
        }

        protected abstract ElementAttributeInfo[] GetAttributeInfos();

        private void EnsureInitialize()
        {
            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }
        }

        ElementAttributeInfo[] IAttributeResolver.GetAttributeInfos()
        {
            EnsureInitialize();
            return GetAttributeInfos();
        }
    }
}
