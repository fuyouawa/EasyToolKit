using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Attribute used to specify the priority of a post processor.
    /// This attribute can be applied to post processor classes to control their execution order
    /// in the post processor chain. Higher priority post processors are executed before lower priority post processors.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ElementPostProcessorPriorityAttribute : Attribute, IPriorityAccessor
    {
        /// <summary>
        /// Represents the lowest possible post processor priority.
        /// </summary>
        public static readonly Priority LowestPriority = new Priority(ElementPostProcessorPriorityLevel.Lowest);

        /// <summary>
        /// Represents the standard priority for validation post processors.
        /// </summary>
        public static readonly Priority ValidationPriority = new Priority(ElementPostProcessorPriorityLevel.Validation);

        /// <summary>
        /// Represents the priority for cleanup post processors.
        /// </summary>
        public static readonly Priority CleanupPriority = new Priority(ElementPostProcessorPriorityLevel.Cleanup);

        /// <summary>
        /// Represents the highest standard post processor priority.
        /// </summary>
        public static readonly Priority SuperPriority = new Priority(ElementPostProcessorPriorityLevel.Super);

        /// <summary>
        /// Gets the priority value for the post processor.
        /// </summary>
        public Priority Priority { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementPostProcessorPriorityAttribute"/> class.
        /// </summary>
        /// <param name="value">The priority value for the post processor..</param>
        public ElementPostProcessorPriorityAttribute(double value = 0.0)
        {
            Priority = new Priority(value);
        }
    }
}
