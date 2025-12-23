using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Extension methods for <see cref="ElementFlags"/> to provide convenient flag checking operations.
    /// </summary>
    public static class ElementFlagsExtensions
    {
        /// <summary>
        /// Determines if the element is a root element.
        /// </summary>
        /// <param name="flags">The element flags to check.</param>
        /// <returns>True if the element has the Root flag set.</returns>
        public static bool IsRoot(this ElementFlags flags)
        {
            return (flags & ElementFlags.Root) == ElementFlags.Root;
        }

        /// <summary>
        /// Determines if the element contains data that can be displayed or edited.
        /// </summary>
        /// <param name="flags">The element flags to check.</param>
        /// <returns>True if the element has the Value flag set.</returns>
        public static bool IsValue(this ElementFlags flags)
        {
            return (flags & ElementFlags.Value) == ElementFlags.Value;
        }

        public static bool IsCustomValue(this ElementFlags flags)
        {
            return flags.IsValue() && !flags.IsMember();
        }

        /// <summary>
        /// Determines if the element represents a class field.
        /// </summary>
        /// <param name="flags">The element flags to check.</param>
        /// <returns>True if the element has the Field flag set.</returns>
        public static bool IsField(this ElementFlags flags)
        {
            return (flags & ElementFlags.Field) == ElementFlags.Field;
        }

        /// <summary>
        /// Determines if the element represents a class property.
        /// </summary>
        /// <param name="flags">The element flags to check.</param>
        /// <returns>True if the element has the Property flag set.</returns>
        public static bool IsProperty(this ElementFlags flags)
        {
            return (flags & ElementFlags.Property) == ElementFlags.Property;
        }

        /// <summary>
        /// Determines if the element represents a collection (Array, List, or Dictionary).
        /// </summary>
        /// <param name="flags">The element flags to check.</param>
        /// <returns>True if the element has the Collection flag set.</returns>
        public static bool IsCollection(this ElementFlags flags)
        {
            return (flags & ElementFlags.Collection) == ElementFlags.Collection;
        }

        /// <summary>
        /// Determines if the element represents an individual item within a collection.
        /// </summary>
        /// <param name="flags">The element flags to check.</param>
        /// <returns>True if the element has the CollectionItem flag set.</returns>
        public static bool IsCollectionItem(this ElementFlags flags)
        {
            return (flags & ElementFlags.CollectionItem) == ElementFlags.CollectionItem;
        }

        /// <summary>
        /// Determines if the element represents a method that can be invoked in the inspector.
        /// </summary>
        /// <param name="flags">The element flags to check.</param>
        /// <returns>True if the element has the Method flag set.</returns>
        public static bool IsMethod(this ElementFlags flags)
        {
            return (flags & ElementFlags.Method) == ElementFlags.Method;
        }

        /// <summary>
        /// Determines if the element represents a method parameter for method invocation.
        /// </summary>
        /// <param name="flags">The element flags to check.</param>
        /// <returns>True if the element has the MethodParameter flag set.</returns>
        public static bool IsMethodParameter(this ElementFlags flags)
        {
            return (flags & ElementFlags.MethodParameter) == ElementFlags.MethodParameter;
        }

        /// <summary>
        /// Determines if the element is a group for organizing related elements in the UI.
        /// </summary>
        /// <param name="flags">The element flags to check.</param>
        /// <returns>True if the element has the Group flag set.</returns>
        public static bool IsGroup(this ElementFlags flags)
        {
            return (flags & ElementFlags.Group) == ElementFlags.Group;
        }

        /// <summary>
        /// Determines if the element is a member (Field or Property or Method).
        /// </summary>
        /// <param name="flags">The element flags to check.</param>
        /// <returns>True if the element represents a class member.</returns>
        public static bool IsMember(this ElementFlags flags)
        {
            return flags.IsField() || flags.IsProperty() || flags.IsMethod();
        }

        /// <summary>
        /// Determines if the element has no flags set.
        /// </summary>
        /// <param name="flags">The element flags to check.</param>
        /// <returns>True if no flags are set.</returns>
        public static bool IsNone(this ElementFlags flags)
        {
            return flags == ElementFlags.None;
        }

        /// <summary>
        /// Checks if the element has any of the specified flags set.
        /// </summary>
        /// <param name="flags">The element flags to check.</param>
        /// <param name="flagsToCheck">The flags to check against.</param>
        /// <returns>True if any of the specified flags are set.</returns>
        public static bool HasAny(this ElementFlags flags, ElementFlags flagsToCheck)
        {
            return (flags & flagsToCheck) != ElementFlags.None;
        }

        /// <summary>
        /// Checks if the element has all of the specified flags set.
        /// </summary>
        /// <param name="flags">The element flags to check.</param>
        /// <param name="flagsToCheck">The flags that must all be set.</param>
        /// <returns>True if all specified flags are set.</returns>
        public static bool Has(this ElementFlags flags, ElementFlags flagsToCheck)
        {
            return (flags & flagsToCheck) == flagsToCheck;
        }

        /// <summary>
        /// Sets the specified flag(s) on the element.
        /// </summary>
        /// <param name="flags">The original element flags.</param>
        /// <param name="flagsToSet">The flag(s) to set.</param>
        /// <returns>New element flags with the specified flags set.</returns>
        public static ElementFlags Set(this ElementFlags flags, ElementFlags flagsToSet)
        {
            return flags | flagsToSet;
        }

        /// <summary>
        /// Removes the specified flag(s) from the element.
        /// </summary>
        /// <param name="flags">The original element flags.</param>
        /// <param name="flagsToRemove">The flag(s) to remove.</param>
        /// <returns>New element flags with the specified flags removed.</returns>
        public static ElementFlags Remove(this ElementFlags flags, ElementFlags flagsToRemove)
        {
            return flags & ~flagsToRemove;
        }

        /// <summary>
        /// Toggles the specified flag(s) on the element.
        /// </summary>
        /// <param name="flags">The original element flags.</param>
        /// <param name="flagsToToggle">The flag(s) to toggle.</param>
        /// <returns>New element flags with the specified flags toggled.</returns>
        public static ElementFlags Toggle(this ElementFlags flags, ElementFlags flagsToToggle)
        {
            return flags ^ flagsToToggle;
        }
    }
}
