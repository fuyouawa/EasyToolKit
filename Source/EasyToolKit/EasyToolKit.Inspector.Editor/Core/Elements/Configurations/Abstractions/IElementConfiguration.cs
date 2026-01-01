using System;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Base configuration interface for all element configurations in the inspector system.
    /// Provides common properties required for creating element definitions.
    /// </summary>
    public interface IElementConfiguration
    {
        /// <summary>
        /// Gets or sets the display name of the element.
        /// This name is shown in the inspector interface and used for identification.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the additional attributes for customizing the element behavior.
        /// </summary>
        IReadOnlyList<Attribute> AdditionalAttributes { get; set; }
    }
}
