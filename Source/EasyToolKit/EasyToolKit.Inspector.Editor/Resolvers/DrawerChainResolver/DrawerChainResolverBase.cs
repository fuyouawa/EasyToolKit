using System;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for drawer chain resolution in the <see cref="InspectorProperty"/> system
    /// </summary>
    public abstract class DrawerChainResolverBase : ResolverBase, IDrawerChainResolver
    {
        private bool _isInitialized;

        /// <summary>
        /// Override this method to perform initialization logic
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Gets the drawer chain for the property
        /// </summary>
        /// <returns>The drawer chain</returns>
        protected abstract DrawerChain GetDrawerChain();

        private void EnsureInitialize()
        {
            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }
        }

        DrawerChain IDrawerChainResolver.GetDrawerChain()
        {
            EnsureInitialize();
            return GetDrawerChain();
        }

        /// <summary>
        /// Resets the initialization flag when the resolver is returned to the pool.
        /// </summary>
        protected override void OnRelease()
        {
            base.OnRelease();
            _isInitialized = false;
        }
    }
}
