namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Manages the state of an <see cref="InspectorProperty"/>, including persistent state like expansion state.
    /// </summary>
    public class PropertyState
    {
        /// <summary>
        /// Gets the <see cref="InspectorProperty"/> associated with this state.
        /// </summary>
        public InspectorProperty Property { get; }

        private LocalPersistentContext<bool> _expanded;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyState"/> class.
        /// </summary>
        /// <param name="property">The <see cref="InspectorProperty"/> to manage state for.</param>
        public PropertyState(InspectorProperty property)
        {
            Property = property;
        }

        /// <summary>
        /// Gets or sets the default expanded state for the property.
        /// </summary>
        public bool DefaultExpanded { get; set; } = false;

        /// <summary>
        /// Gets or sets whether the property is expanded in the inspector.
        /// This state is persisted across Unity sessions.
        /// </summary>
        public bool Expanded
        {
            get
            {
                _expanded ??= Property.GetPersistentContext("_expanded", DefaultExpanded);

                return _expanded.Value;
            }
            set
            {
                _expanded ??= Property.GetPersistentContext("_expanded", DefaultExpanded);

                _expanded.Value = value;
            }
        }
    }
}
