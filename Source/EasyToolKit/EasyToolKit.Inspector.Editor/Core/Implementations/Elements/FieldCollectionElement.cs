using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Field collection element implementation representing collection fields on an object.
    /// Handles display and editing of collection-based <see cref="System.Reflection.FieldInfo"/>.
    /// </summary>
    public class FieldCollectionElement : CollectionElement, IFieldCollectionElement
    {
        private const string RootPrefix = "$ROOT$.";

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldCollectionElement"/> class.
        /// </summary>
        /// <param name="definition">The field collection definition that describes this element.</param>
        /// <param name="sharedContext">The shared context providing access to tree-level services.</param>
        /// <param name="logicalParent">The logical parent element in the code structure.</param>
        public FieldCollectionElement(
            [NotNull] IFieldCollectionDefinition definition,
            [NotNull] IElementSharedContext sharedContext,
            [CanBeNull] IValueElement logicalParent)
            : base(definition, sharedContext, logicalParent)
        {
        }

        /// <summary>
        /// Gets the field collection definition that describes this field collection.
        /// </summary>
        public new IFieldCollectionDefinition Definition => (IFieldCollectionDefinition)base.Definition;

        IFieldDefinition IFieldElement.Definition => Definition;

        public new IValueElement LogicalParent => (IValueElement)base.LogicalParent;

        /// <summary>
        /// Gets the Unity‑style property path, equivalent to <see cref="IElement.Path"/> but formatted for Unity's <see cref="UnityEditor.SerializedProperty"/>.
        /// </summary>
        public string UnityPath
        {
            get
            {
                var path = Path;
                if (path.StartsWith(RootPrefix))
                {
                    return path[RootPrefix.Length..];
                }

                return path;
            }
        }
    }
}
