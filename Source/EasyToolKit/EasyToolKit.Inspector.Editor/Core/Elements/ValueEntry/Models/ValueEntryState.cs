namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents the state of values when multiple targets are selected in the inspector.
    /// </summary>
    public enum ValueEntryState
    {
        /// <summary>
        /// No state or default state.
        /// </summary>
        None,

        /// <summary>
        /// All values are equal. When consistent, reference types also have equal types (no derived classes).
        /// </summary>
        Consistent,

        /// <summary>
        /// Reference types have equal types across targets, but children values may differ.
        /// </summary>
        TypeConsistent,

        /// <summary>
        /// Values differ among targets.
        /// </summary>
        Mixed,
    }
}
