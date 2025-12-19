using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Attribute used to specify the priority of an inspector drawer.
    /// This attribute can be applied to drawer classes to control their execution order
    /// in the drawer chain. Higher priority drawers are executed before lower priority drawers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DrawerPriorityAttribute : Attribute, IInspectorPriorityGetter
    {
        /// <summary>
        /// Represents the lowest possible drawer priority.
        /// </summary>
        public static readonly InspectorPriority LowestPriority = new InspectorPriority(DrawerPriorityLevel.Lowest);

        /// <summary>
        /// Represents the standard priority for value drawers.
        /// </summary>
        public static readonly InspectorPriority ValuePriority = new InspectorPriority(DrawerPriorityLevel.Value);

        /// <summary>
        /// Represents the priority for attribute-based drawers.
        /// </summary>
        public static readonly InspectorPriority AttributePriority = new InspectorPriority(DrawerPriorityLevel.Attribute);

        /// <summary>
        /// Represents the highest standard drawer priority.
        /// </summary>
        public static readonly InspectorPriority SuperPriority = new InspectorPriority(DrawerPriorityLevel.Super);

        /// <summary>
        /// Gets the priority value for the drawer.
        /// </summary>
        public InspectorPriority Priority { get; }

        /// <summary>
        /// Initializes a new instance of the DrawerPriorityAttribute class.
        /// </summary>
        /// <param name="value">The priority value for the drawer. Defaults to <see cref="DrawerPriorityLevel.Lowest"/>.</param>
        public DrawerPriorityAttribute(double value = DrawerPriorityLevel.Lowest)
        {
            Priority = new InspectorPriority(value);
        }
    }
}
