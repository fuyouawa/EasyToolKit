using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for resolving group properties in the <see cref="InspectorProperty"/> system
    /// </summary>
    public interface IGroupResolver : IInspectorHandler
    {
        /// <summary>
        /// Gets all properties that belong to the same group as the current property
        /// </summary>
        /// <param name="beginGroupAttributeType">The type of the begin group attribute</param>
        /// <returns>Array of properties in the group</returns>
        InspectorProperty[] GetGroupProperties(Type beginGroupAttributeType);
    }
}
