using System;
using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Property definition interface that handles <see cref="System.Reflection.PropertyInfo"/>.
    /// Provides consistent access to property reflection information.
    /// </summary>
    public interface IPropertyDefinition : IValueDefinition, IMemberDefinition
    {
        /// <summary>
        /// Gets the <see cref="System.Reflection.PropertyInfo"/> that represents this property.
        /// Provides access to reflection information about the underlying property.
        /// </summary>
        PropertyInfo PropertyInfo { get; }
    }
}
