using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for resolving collection properties in the inspector.
    /// Provides basic collection operations and metadata for collection types.
    /// </summary>
    public interface ICollectionStructureResolver
    {
        /// <summary>
        /// Gets whether the collection is read-only.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Gets the type of the collection.
        /// </summary>
        Type CollectionType { get; }

        /// <summary>
        /// Gets the type of elements in the collection.
        /// </summary>
        Type ElementType { get; }

        IChangeManager ChangeManager { get; }
        ICollectionOperationResolver OperationResolver { get; }
    }
}
