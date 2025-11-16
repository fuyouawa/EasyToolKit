namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Provides standard priority levels for inspector drawers.
    /// These constants define the standard priority ranges used by different types of drawers.
    /// Higher values indicate higher priority (executed earlier in the drawer chain).
    /// </summary>
    public static class DrawerPriorityLevel
    {
        /// <summary>
        /// The lowest priority level for drawers that should execute last.
        /// </summary>
        public const double Lowest = -100000.0;

        /// <summary>
        /// The standard priority level for value drawers that handle basic property types.
        /// </summary>
        public const double Value = 100000.0;

        /// <summary>
        /// The priority level for attribute-based drawers that handle custom attributes.
        /// </summary>
        public const double Attribute = 200000.0;

        /// <summary>
        /// The highest standard priority level for drawers that should execute first.
        /// </summary>
        public const double Super = 300000.0;
    }
}