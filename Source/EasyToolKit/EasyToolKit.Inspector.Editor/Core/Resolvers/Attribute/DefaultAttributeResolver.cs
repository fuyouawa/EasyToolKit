using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Default implementation of attribute resolution for <see cref="InspectorProperty"/>
    /// </summary>
    public class DefaultAttributeResolver : AttributeResolverBase
    {
        private Attribute[] _attributes;
        private Dictionary<Attribute, AttributeSource> _attributeSources;

        protected override void Initialize()
        {
            _attributeSources = new Dictionary<Attribute, AttributeSource>();

            if (!Property.Info.IsLogicRoot && Property.Parent.ChildrenResolver is ICollectionStructureResolver)
            {
                var passToListElementAttributes = Property.Parent.GetAttributes()
                    .Where(attr => attr is CanPassToListElementAttribute { PassToListElements: true });
                foreach (var attribute in passToListElementAttributes)
                {
                    _attributeSources[attribute] = AttributeSource.ListPassToElement;
                }
            }
            else if (Property.Info.MemberInfo != null)
            {
                var memberAttributes = Property.Info.MemberInfo.GetCustomAttributes();
                foreach (var attribute in memberAttributes)
                {
                    _attributeSources[attribute] = AttributeSource.Member;
                }
            }

            var typeAttributes = Property.ValueEntry.ValueType.GetCustomAttributes(true).Cast<Attribute>();
            foreach (var attribute in typeAttributes)
            {
                _attributeSources[attribute] = AttributeSource.Type;
            }

            _attributes = _attributeSources.Keys.ToArray();
        }

        /// <summary>
        /// Gets all attributes associated with the property from member, type, and list element sources
        /// </summary>
        /// <returns>Array of attributes</returns>
        protected override Attribute[] GetAttributes()
        {
            return _attributes;
        }

        /// <summary>
        /// Gets the source of an attribute to determine where it was originally defined
        /// </summary>
        /// <param name="attribute">The attribute to check</param>
        /// <returns>The source of the attribute indicating whether it was defined on a member, type, or passed from a list</returns>
        /// <exception cref="ArgumentException">Thrown when the attribute is not found</exception>
        protected override AttributeSource GetAttributeSource(Attribute attribute)
        {
            // Return the source if found
            if (_attributeSources.TryGetValue(attribute, out var source))
            {
                return source;
            }

            throw new ArgumentException($"Attribute '{attribute.GetType()}' not found in the property '{Property}'");
        }
    }
}
