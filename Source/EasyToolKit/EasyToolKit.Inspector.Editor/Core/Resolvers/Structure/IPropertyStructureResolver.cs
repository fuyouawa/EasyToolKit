using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for resolving property structure information in the inspector system.
    /// Focuses purely on property structure without collection operations or change management.
    /// </summary>
    public interface IPropertyStructureResolver : IInitializableResolver
    {
        /// <summary>
        /// Gets the number of child properties
        /// </summary>
        int ChildCount { get; }

        bool CanResolver(InspectorProperty property);

        /// <summary>
        /// Gets information about a child property at the specified index
        /// </summary>
        /// <param name="childIndex">The index of the child property</param>
        /// <returns>Information about the child property</returns>
        InspectorPropertyInfo GetChildInfo(int childIndex);

        /// <summary>
        /// Converts a child property name to its index
        /// </summary>
        /// <param name="name">The name of the child property</param>
        /// <returns>The index of the child property, or -1 if not found</returns>
        int ChildNameToIndex(string name);
    }
}
