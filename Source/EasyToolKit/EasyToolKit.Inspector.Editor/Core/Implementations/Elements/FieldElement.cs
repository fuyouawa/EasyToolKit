using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Field element implementation representing fields on an object.
    /// Handles display and editing of <see cref="System.Reflection.FieldInfo"/>.
    /// </summary>
    public class FieldElement : ValueElement, IFieldElement
    {
        private const string RootPrefix = "$ROOT$.";


        /// <summary>
        /// Initializes a new instance of the <see cref="FieldElement"/> class.
        /// </summary>
        /// <param name="definition">The field definition that describes this element.</param>
        /// <param name="sharedContext">The shared context providing access to tree-level services.</param>
        /// <param name="logicalParent">The logical parent element in the code structure.</param>
        public FieldElement(
            [NotNull] IFieldDefinition definition,
            [NotNull] IElementSharedContext sharedContext,
            [CanBeNull] IValueElement logicalParent)
            : base(definition, sharedContext, logicalParent)
        {
        }

        /// <summary>
        /// Gets the field definition that describes this field.
        /// </summary>
        public new IFieldDefinition Definition => (IFieldDefinition)base.Definition;

        public IValueElement LogicalParent => (IValueElement)base.LogicalParent;

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
