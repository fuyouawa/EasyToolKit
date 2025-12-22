namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Specifies the state of a value entry.
    /// </summary>
    public enum ValueEntryState
    {
        None,

        Consistent,

        /// <summary>
        /// The value differs between targets (multi‑object editing conflict).
        /// </summary>
        Mixed
    }
}
