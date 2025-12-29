using System;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Base configuration interface for creating collection element definitions.
    /// Collections represent data structures that contain multiple elements.
    /// </summary>
    public class CollectionConfiguration : ValueConfiguration, ICollectionConfiguration
    {
        /// <summary>
        /// Gets or sets the type of elements contained in this collection.
        /// For dictionaries, this represents the type of values.
        /// </summary>
        public Type ItemType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this collection is ordered (can be accessed by index).
        /// Ordered collections include arrays, lists, and other indexable sequences.
        /// Unordered collections include sets, dictionaries, and other non-indexable collections.
        /// </summary>
        public bool IsOrdered { get; set; }

        protected void ProcessDefinition(CollectionDefinition definition)
        {
            if (ItemType == null)
            {
                throw new InvalidOperationException("ItemType cannot be null");
            }

            definition.Roles = definition.Roles.Add(ElementRoles.Collection);
            definition.ItemType = ItemType;
            definition.IsOrdered = IsOrdered;
            base.ProcessDefinition(definition);
        }

        /// <summary>
        /// Creates a new <see cref="ICollectionDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new collection definition instance.</returns>
        public new ICollectionDefinition CreateDefinition()
        {
            var definition = new CollectionDefinition();
            ProcessDefinition(definition);
            return definition;
        }
    }
}
