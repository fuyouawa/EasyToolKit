using System;
using System.Collections.Generic;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Configuration interface for member filtering during serialization.
    /// Defines which members (fields, properties) should be serialized.
    /// </summary>
    public interface IMemberFilterConfiguration : IValidatableConfiguration
    {
        /// <summary>
        /// Gets the flags determining which members to include based on access modifiers and types.
        /// </summary>
        MemberFilterFlags FilterFlags { get; }

        /// <summary>
        /// Gets the collection of types to exclude from serialization.
        /// </summary>
        IReadOnlyList<Type> ExcludedTypes { get; }

        /// <summary>
        /// Creates the member filter delegate from this configuration.
        /// </summary>
        /// <returns>A member filter delegate based on this configuration.</returns>
        MemberFilter CreateFilter();
    }
}
