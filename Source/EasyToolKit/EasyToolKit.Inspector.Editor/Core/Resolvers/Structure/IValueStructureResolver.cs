using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for resolving value structure information in the inspector system.
    /// Focuses purely on value structure without collection operations or change management.
    /// </summary>
    public interface IValueStructureResolver : IHandler
    {
        /// <summary>
        /// Gets the number of child properties
        /// </summary>
        int ChildCount { get; }

        /// <summary>
        /// Gets the definition of a child at the specified index
        /// </summary>
        /// <param name="childIndex">The index of the child</param>
        /// <returns>Definition of the child</returns>
        IElementDefinition GetChildDefinition(int childIndex);

        /// <summary>
        /// Converts a child name to its index
        /// </summary>
        /// <param name="name">The name of the child</param>
        /// <returns>The index of the child, or -1 if not found</returns>
        int ChildNameToIndex(string name);
    }
}
