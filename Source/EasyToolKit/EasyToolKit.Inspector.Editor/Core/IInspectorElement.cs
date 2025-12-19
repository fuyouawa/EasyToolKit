using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Base interface for all EasyToolKit inspector elements that manage properties and initialization state.
    /// This interface provides common functionality for drawers, resolvers, and other inspector components.
    /// </summary>
    public interface IInspectorElement
    {
        /// <summary>
        /// Gets or sets the <see cref="InspectorProperty"/> that this element is associated with.
        /// </summary>
        InspectorProperty Property { get; set; }

        Type MatchedType { get; }

        /// <summary>
        /// Gets a value indicating whether this element has been initialized.
        /// </summary>
        bool IsInitialized { get; }

        bool CanHandle(InspectorProperty property);

        /// <summary>
        /// Initializes this element with its current property and any required resources.
        /// This method should be called before using the element for drawing or resolving.
        /// </summary>
        void Initialize();
    }
}
