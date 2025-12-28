using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Extension methods for <see cref="ElementRoles"/> to provide convenient role checking operations.
    /// </summary>
    public static class ElementRolesExtensions
    {
        /// <summary>
        /// Determines if the element is a root element.
        /// </summary>
        /// <param name="roles">The element roles to check.</param>
        /// <returns>True if the element has the Root role set.</returns>
        public static bool IsRoot(this ElementRoles roles)
        {
            return (roles & ElementRoles.Root) == ElementRoles.Root;
        }

        /// <summary>
        /// Determines if the element contains data that can be displayed or edited.
        /// </summary>
        /// <param name="roles">The element roles to check.</param>
        /// <returns>True if the element has the Value role set.</returns>
        public static bool IsValue(this ElementRoles roles)
        {
            return (roles & ElementRoles.Value) == ElementRoles.Value;
        }

        public static bool IsCustomValue(this ElementRoles roles)
        {
            return roles.IsValue() && !roles.IsMember();
        }

        /// <summary>
        /// Determines if the element represents a class field.
        /// </summary>
        /// <param name="roles">The element roles to check.</param>
        /// <returns>True if the element has the Field role set.</returns>
        public static bool IsField(this ElementRoles roles)
        {
            return (roles & ElementRoles.Field) == ElementRoles.Field;
        }

        /// <summary>
        /// Determines if the element represents a class property.
        /// </summary>
        /// <param name="roles">The element roles to check.</param>
        /// <returns>True if the element has the Property role set.</returns>
        public static bool IsProperty(this ElementRoles roles)
        {
            return (roles & ElementRoles.Property) == ElementRoles.Property;
        }

        /// <summary>
        /// Determines if the element represents a collection (Array, List, or Dictionary).
        /// </summary>
        /// <param name="roles">The element roles to check.</param>
        /// <returns>True if the element has the Collection role set.</returns>
        public static bool IsCollection(this ElementRoles roles)
        {
            return (roles & ElementRoles.Collection) == ElementRoles.Collection;
        }

        /// <summary>
        /// Determines if the element represents an individual item within a collection.
        /// </summary>
        /// <param name="roles">The element roles to check.</param>
        /// <returns>True if the element has the CollectionItem role set.</returns>
        public static bool IsCollectionItem(this ElementRoles roles)
        {
            return (roles & ElementRoles.CollectionItem) == ElementRoles.CollectionItem;
        }

        /// <summary>
        /// Determines if the element represents a method that can be invoked in the inspector.
        /// </summary>
        /// <param name="roles">The element roles to check.</param>
        /// <returns>True if the element has the Method role set.</returns>
        public static bool IsMethod(this ElementRoles roles)
        {
            return (roles & ElementRoles.Method) == ElementRoles.Method;
        }

        /// <summary>
        /// Determines if the element represents a method parameter for method invocation.
        /// </summary>
        /// <param name="roles">The element roles to check.</param>
        /// <returns>True if the element has the MethodParameter role set.</returns>
        public static bool IsMethodParameter(this ElementRoles roles)
        {
            return (roles & ElementRoles.MethodParameter) == ElementRoles.MethodParameter;
        }

        /// <summary>
        /// Determines if the element is a group for organizing related elements in the UI.
        /// </summary>
        /// <param name="roles">The element roles to check.</param>
        /// <returns>True if the element has the Group role set.</returns>
        public static bool IsGroup(this ElementRoles roles)
        {
            return (roles & ElementRoles.Group) == ElementRoles.Group;
        }

        /// <summary>
        /// Determines if the element is a member (Field or Property or Method).
        /// </summary>
        /// <param name="roles">The element roles to check.</param>
        /// <returns>True if the element represents a class member.</returns>
        public static bool IsMember(this ElementRoles roles)
        {
            return roles.IsField() || roles.IsProperty() || roles.IsMethod();
        }

        /// <summary>
        /// Determines if the element has no roles set.
        /// </summary>
        /// <param name="roles">The element roles to check.</param>
        /// <returns>True if no roles are set.</returns>
        public static bool IsNone(this ElementRoles roles)
        {
            return roles == ElementRoles.None;
        }

        /// <summary>
        /// Checks if the element has any of the specified roles set.
        /// </summary>
        /// <param name="roles">The element roles to check.</param>
        /// <param name="rolesToCheck">The roles to check against.</param>
        /// <returns>True if any of the specified roles are set.</returns>
        public static bool HasAny(this ElementRoles roles, ElementRoles rolesToCheck)
        {
            return (roles & rolesToCheck) != ElementRoles.None;
        }

        /// <summary>
        /// Checks if the element has all of the specified roles set.
        /// </summary>
        /// <param name="roles">The element roles to check.</param>
        /// <param name="rolesToCheck">The roles that must all be set.</param>
        /// <returns>True if all specified roles are set.</returns>
        public static bool Has(this ElementRoles roles, ElementRoles rolesToCheck)
        {
            return (roles & rolesToCheck) == rolesToCheck;
        }

        /// <summary>
        /// Sets the specified role(s) on the element.
        /// </summary>
        /// <param name="roles">The original element roles.</param>
        /// <param name="rolesToSet">The role(s) to set.</param>
        /// <returns>New element roles with the specified roles set.</returns>
        public static ElementRoles Add(this ElementRoles roles, ElementRoles rolesToSet)
        {
            return roles | rolesToSet;
        }

        /// <summary>
        /// Removes the specified role(s) from the element.
        /// </summary>
        /// <param name="roles">The original element roles.</param>
        /// <param name="rolesToRemove">The role(s) to remove.</param>
        /// <returns>New element roles with the specified roles removed.</returns>
        public static ElementRoles Remove(this ElementRoles roles, ElementRoles rolesToRemove)
        {
            return roles & ~rolesToRemove;
        }

        /// <summary>
        /// Toggles the specified role(s) on the element.
        /// </summary>
        /// <param name="roles">The original element roles.</param>
        /// <param name="rolesToToggle">The role(s) to toggle.</param>
        /// <returns>New element roles with the specified roles toggled.</returns>
        public static ElementRoles Toggle(this ElementRoles roles, ElementRoles rolesToToggle)
        {
            return roles ^ rolesToToggle;
        }
    }
}
