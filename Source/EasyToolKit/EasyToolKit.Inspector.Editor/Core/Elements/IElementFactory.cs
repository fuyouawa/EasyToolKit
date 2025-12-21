using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Factory interface responsible for creating <see cref="IElement"/> instances based on their definitions.
    /// </summary>
    /// <remarks>
    /// The IElementFactory is owned by an <see cref="IElementTree"/> instance and is responsible for:
    /// </remarks>
    public interface IElementFactory
    {
        /// <summary>
        /// Creates a new <see cref="IElement"/> instance based on the provided definition.
        /// </summary>
        /// <param name="definition">The element definition that describes the element to be created.</param>
        /// <param name="parent">The parent element in the hierarchy. Can be <c>null</c> for root elements.</param>
        /// <returns>A new element instance with its context properly initialized to reference the tree's shared context.</returns>
        /// <remarks>
        /// This method automatically sets the created element's <see cref="IElement.SharedContext"/> property
        /// to the tree's shared <see cref="IElementSharedContext"/> instance, ensuring that all elements
        /// in the tree have access to the same resolver factories and update tracking mechanisms.
        /// </remarks>
        IElement CreateElement([NotNull] IElementDefinition definition, [CanBeNull] IElement parent);
    }
}
