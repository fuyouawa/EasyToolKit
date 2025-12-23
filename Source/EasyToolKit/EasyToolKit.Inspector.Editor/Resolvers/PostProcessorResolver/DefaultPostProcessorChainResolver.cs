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
        private ElementPostProcessorChain _chain;

        protected override void Initialize()
        {
            // Get default post processor types for the element
            var postProcessorTypes = ElementPostProcessorUtility.GetPostProcessorTypes(Element);
            var postProcessors = new List<IElementPostProcessor>();

            // Create and initialize post processor instances
            foreach (var postProcessorType in postProcessorTypes)
            {
                var postProcessor = postProcessorType.CreateInstance<IElementPostProcessor>();
                postProcessors.Add(postProcessor);
            }

            // Create and cache the post processor chain
            _chain = new ElementPostProcessorChain(Element, postProcessors);
        }

        /// <summary>
        /// Gets the post processor chain for the element by discovering and initializing appropriate post processors
        /// </summary>
        /// <returns>The post processor chain</returns>
        protected override ElementPostProcessorChain GetPostProcessorChain()
        {
            return _chain;
        }
    }
}
