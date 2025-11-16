using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Attribute used to specify the priority of an inspector drawer.
    /// This attribute can be applied to drawer classes to control their execution order
    /// in the drawer chain. Higher priority drawers are executed before lower priority drawers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DrawerPriorityAttribute : Attribute
    {
        /// <summary>
        /// Gets the priority value for the drawer.
        /// </summary>
        public DrawerPriority Priority { get; }

        /// <summary>
        /// Initializes a new instance of the DrawerPriorityAttribute class.
        /// </summary>
        /// <param name="value">The priority value for the drawer. Defaults to <see cref="DrawerPriorityLevel.Lowest"/>.</param>
        public DrawerPriorityAttribute(double value = DrawerPriorityLevel.Lowest)
        {
            Priority = new DrawerPriority(value);
        }
    }
}
