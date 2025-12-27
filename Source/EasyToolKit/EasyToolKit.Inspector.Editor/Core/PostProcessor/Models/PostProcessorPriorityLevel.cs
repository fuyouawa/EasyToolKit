namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Provides standard priority levels for post processors.
    /// These constants define the standard priority ranges used by different types of post processors.
    /// Higher values indicate higher priority (executed earlier in the post processor chain).
    /// </summary>
    public static class PostProcessorPriorityLevel
    {
        /// <summary>
        /// The lowest priority level for post processors that should execute last.
        /// </summary>
        public const double Lowest = -100000.0;

        /// <summary>
        /// The standard priority level for validation post processors that handle property validation.
        /// </summary>
        public const double Validation = 100000.0;

        /// <summary>
        /// The priority level for cleanup post processors that handle resource cleanup.
        /// </summary>
        public const double Cleanup = 200000.0;

        /// <summary>
        /// The highest standard priority level for post processors that should execute first.
        /// </summary>
        public const double Super = 300000.0;
    }
}
