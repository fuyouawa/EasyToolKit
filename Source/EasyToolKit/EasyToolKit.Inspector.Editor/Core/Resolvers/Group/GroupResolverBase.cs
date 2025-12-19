using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for group property resolver in the <see cref="InspectorProperty"/> system
    /// </summary>
    public abstract class GroupResolverBase : InspectorElementBase, IGroupResolver
    {
        /// <summary>
        /// Gets all properties that belong to the same group as the current property
        /// </summary>
        /// <param name="beginGroupAttributeType">The type of the begin group attribute</param>
        /// <returns>Array of properties in the group</returns>
        public abstract InspectorProperty[] GetGroupProperties(Type beginGroupAttributeType);
    }
}
