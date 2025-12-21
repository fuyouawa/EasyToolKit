namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Specifies the state of a value entry.
    /// </summary>
    public enum ValueEntryState
    {
        /// <summary>
        /// The value is consistent across all targets.
        /// </summary>
        Normal,

        /// <summary>
        /// The value differs between targets (multi‑object editing conflict).
        /// </summary>
        Conflicted
    }
}
