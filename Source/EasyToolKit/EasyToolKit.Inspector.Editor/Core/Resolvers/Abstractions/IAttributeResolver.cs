using System;

namespace EasyToolKit.Inspector.Editor
{
    public interface IAttributeResolver : IResolver
    {
        /// <summary>
        /// Gets all attribute infos for this element.
        /// </summary>
        ElementAttributeInfo[] GetAttributeInfos();

        /// <summary>
        /// Tries to get the attribute info for the specified attribute instance.
        /// </summary>
        /// <param name="attribute">The attribute to find.</param>
        /// <param name="attributeInfo">When this method returns, contains the attribute info if found; otherwise, null.</param>
        /// <returns>true if the attribute info was found; otherwise, false.</returns>
        bool TryGetAttributeInfo(Attribute attribute, out ElementAttributeInfo attributeInfo);
    }
}
