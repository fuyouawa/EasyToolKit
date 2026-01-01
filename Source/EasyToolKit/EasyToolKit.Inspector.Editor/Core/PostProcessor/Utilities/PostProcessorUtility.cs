using System;
using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Utility class for post processor operations
    /// </summary>
    public static class PostProcessorUtility
    {
        /// <summary>
        /// Gets the post processor types that can handle the specified element
        /// </summary>
        /// <param name="element">The element to get post processor types for</param>
        /// <returns>A collection of post processor types</returns>
        public static IEnumerable<Type> GetPostProcessorTypes(IElement element)
        {
            return HandlerUtility.GetHandlerTypes(element, type => type.IsInheritsFrom<IPostProcessor>());
        }
    }
}
