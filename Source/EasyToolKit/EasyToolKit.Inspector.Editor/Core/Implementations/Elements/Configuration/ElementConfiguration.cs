using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Base configuration class for all element configurations in the inspector system.
    /// Provides common properties required for creating element definitions.
    /// </summary>
    public abstract class ElementConfiguration : IElementConfiguration
    {
        /// <summary>
        /// Gets or sets the display name of the element.
        /// This name is shown in the inspector interface and used for identification.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the additional attributes for customizing the element behavior.
        /// </summary>
        public IReadOnlyList<Attribute> AdditionalAttributes { get; set; }

        protected void ProcessDefinition(ElementDefinition definition)
        {
            if (Name.IsNullOrWhiteSpace())
            {
                throw new InvalidOperationException("Name cannot be null or whitespace");
            }

            definition.Name = Name;
            definition.AdditionalAttributes = AdditionalAttributes?.ToArray();
        }
    }
}
