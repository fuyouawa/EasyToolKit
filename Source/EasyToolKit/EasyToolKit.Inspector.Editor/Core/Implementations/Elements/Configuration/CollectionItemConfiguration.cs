namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Configuration interface for creating collection item element definitions.
    /// Collection items represent individual elements within a collection.
    /// </summary>
    public class CollectionItemConfiguration : ValueConfiguration, ICollectionItemConfiguration
    {
        /// <summary>
        /// Gets or sets the index of this item within its parent collection.
        /// </summary>
        public int ItemIndex { get; set; }

        protected void ProcessDefinition(CollectionItemDefinition definition)
        {
            definition.Roles = definition.Roles.Add(ElementRoles.CollectionItem);
            definition.ItemIndex = ItemIndex;
            base.ProcessDefinition(definition);
        }

        /// <summary>
        /// Creates a new <see cref="ICollectionItemDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new collection item definition instance.</returns>
        public ICollectionItemDefinition CreateDefinition()
        {
            var definition = new CollectionItemDefinition();
            ProcessDefinition(definition);
            return definition;
        }
    }
}
