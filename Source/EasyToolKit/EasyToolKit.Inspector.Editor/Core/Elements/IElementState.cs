using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents the state of an element in the inspector.
    /// This interface manages persistent and transient state for elements, such as expansion state.
    /// </summary>
    public interface IElementState
    {
        /// <summary>
        /// Gets the element associated with this state.
        /// </summary>
        IElement Element { get; }

        /// <summary>
        /// Gets or sets the default expanded state for the element.
        /// </summary>
        bool DefaultExpanded { get; set; }

        /// <summary>
        /// Gets or sets whether the element is expanded in the inspector.
        /// This state is persisted across Unity sessions.
        /// </summary>
        bool Expanded { get; set; }

        /// <summary>
        /// Gets or sets whether the element is visible in the inspector.
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Gets or sets whether the element is enabled (interactable) in the inspector.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets the GUI content used for displaying the element's label.
        /// </summary>
        GUIContent LabelContent { get; }

        /// <summary>
        /// Updates the state based on the current element configuration.
        /// </summary>
        void Update();
    }
}
