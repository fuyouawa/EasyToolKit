using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Manages the state of an <see cref="IElement"/>, including persistent state like expansion state
    /// and transient state like visibility and enabled state.
    /// </summary>
    public class ElementState : IElementState
    {
        /// <summary>
        /// Gets the <see cref="IElement"/> associated with this state.
        /// </summary>
        public IElement Element { get; }

        private LocalPersistentContext<bool> _expanded;
        private LocalPersistentContext<bool> _visible;
        private LocalPersistentContext<bool> _enabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementState"/> class.
        /// </summary>
        /// <param name="element">The <see cref="IElement"/> to manage state for.</param>
        public ElementState([NotNull] IElement element)
        {
            Element = element ?? throw new ArgumentNullException(nameof(element));
        }

        /// <summary>
        /// Gets or sets the default expanded state for the element.
        /// </summary>
        public bool DefaultExpanded { get; set; }

        /// <summary>
        /// Gets or sets the default visible state for the element.
        /// </summary>
        public bool DefaultVisible { get; set; } = true;

        /// <summary>
        /// Gets or sets the default enabled state for the element.
        /// </summary>
        public bool DefaultEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the element is expanded in the inspector.
        /// This state is persisted across Unity sessions.
        /// </summary>
        public bool Expanded
        {
            get
            {
                _expanded ??= Element.GetPersistentContext(nameof(_expanded), DefaultExpanded);
                return _expanded.Value;
            }
            set
            {
                _expanded ??= Element.GetPersistentContext(nameof(_expanded), DefaultExpanded);
                _expanded.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the element is visible in the inspector.
        /// This state is persisted across Unity sessions.
        /// </summary>
        public bool Visible
        {
            get
            {
                _visible ??= Element.GetPersistentContext(nameof(_visible), DefaultVisible);
                return _visible.Value;
            }
            set
            {
                _visible ??= Element.GetPersistentContext(nameof(_visible), DefaultVisible);
                _visible.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the element is enabled (interactable) in the inspector.
        /// This state is persisted across Unity sessions.
        /// </summary>
        public bool Enabled
        {
            get
            {
                _enabled ??= Element.GetPersistentContext(nameof(_enabled), DefaultEnabled);
                return _enabled.Value;
            }
            set
            {
                _enabled ??= Element.GetPersistentContext(nameof(_enabled), DefaultEnabled);
                _enabled.Value = value;
            }
        }

        /// <summary>
        /// Updates the state based on the current element configuration.
        /// </summary>
        public void Update()
        {
        }
    }
}
