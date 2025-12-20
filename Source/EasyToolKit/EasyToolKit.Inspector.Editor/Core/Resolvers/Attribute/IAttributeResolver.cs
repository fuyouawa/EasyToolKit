using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for resolving attributes associated with an <see cref="InspectorProperty"/>
    /// </summary>
    public interface IAttributeResolver : IInspectorHandler
    {
        /// <summary>
        /// Gets all attributes associated with the property
        /// </summary>
        /// <returns>Array of attributes</returns>
        Attribute[] GetAttributes();

        /// <summary>
        /// Gets the source of an attribute to determine where it was originally defined
        /// </summary>
        /// <param name="attribute">The attribute to check</param>
        /// <returns>The source of the attribute indicating whether it was defined on a member, type, or passed from a list</returns>
        AttributeSource GetAttributeSource(Attribute attribute);
    }
}
