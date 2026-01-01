using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Default implementation of post processor chain resolver for <see cref="IElement"/>
    /// </summary>
    public class DefaultPostProcessorChainResolver : PostProcessorChainResolverBase
    {
        private PostProcessorChain _chain;

        protected override void Initialize()
        {
            // Get default post processor types for the element
            var postProcessorTypes = PostProcessorUtility.GetPostProcessorTypes(Element);
            var postProcessors = new List<IPostProcessor>();

            // Create and initialize post processor instances
            foreach (var postProcessorType in postProcessorTypes)
            {
                var postProcessor = postProcessorType.CreateInstance<IPostProcessor>();
                postProcessors.Add(postProcessor);
            }

            // Create and cache the post processor chain
            _chain = new PostProcessorChain(Element, postProcessors);
        }

        /// <summary>
        /// Gets the post processor chain for the element by discovering and initializing appropriate post processors
        /// </summary>
        /// <returns>The post processor chain</returns>
        protected override PostProcessorChain GetPostProcessorChain()
        {
            return _chain;
        }

        /// <summary>
        /// Clears the cached post processor chain when the resolver is returned to the pool.
        /// </summary>
        protected override void OnRelease()
        {
            base.OnRelease();
            _chain = null;
        }
    }
}
