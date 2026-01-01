using System;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for post processor chain resolution in the <see cref="IElement"/> system
    /// </summary>
    public abstract class PostProcessorChainResolverBase : ResolverBase, IPostProcessorChainResolver
    {
        private bool _isInitialized;

        /// <summary>
        /// Override this method to perform initialization logic
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Gets the post processor chain for the element
        /// </summary>
        /// <returns>The post processor chain</returns>
        [MustUseReturnValue]
        protected abstract PostProcessorChain GetPostProcessorChain();

        private void EnsureInitialize()
        {
            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }
        }

        PostProcessorChain IPostProcessorChainResolver.GetPostProcessorChain()
        {
            EnsureInitialize();
            return GetPostProcessorChain();
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
