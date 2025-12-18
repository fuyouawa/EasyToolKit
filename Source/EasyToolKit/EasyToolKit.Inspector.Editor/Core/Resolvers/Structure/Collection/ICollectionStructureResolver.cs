using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for resolving collection properties in the inspector.
    /// Provides basic collection structure metadata for collection types.
    /// </summary>
    public interface ICollectionStructureResolver : IPropertyStructureResolver
    {
        /// <summary>
        /// Gets the type of elements in the collection.
        /// </summary>
        Type ElementType { get; }
    }
}
