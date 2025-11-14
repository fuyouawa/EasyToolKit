namespace EasyToolKit.Inspector.Editor
{
    public class PropertyState
    {
        public InspectorProperty Property { get; }

        private LocalPersistentContext<bool> _expanded;

        public PropertyState(InspectorProperty property)
        {
            Property = property;
        }

        public bool DefaultExpanded { get; set; } = false;

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
