using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Represents a type matching index that defines a type and its target types for matching.
    /// </summary>
    /// <remarks>
    /// Type match indices are used to register candidate types (e.g., handlers or serializers)
    /// with the type matcher. Each index specifies the type to be matched, the target types
    /// it should match against, and a priority value for ordering when multiple matches exist.
    /// </remarks>
    public class TypeMatchIndex
    {
        /// <summary>
        /// Gets or sets the type to be matched.
        /// </summary>
        /// <value>
        /// The candidate type (e.g., a handler or serializer type) that may match
        /// against the specified target types.
        /// </value>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the priority of this match index (higher values have higher priority).
        /// </summary>
        /// <value>
        /// The priority value used to order match results. When multiple matches are found,
        /// results are sorted by priority in descending order.
        /// </value>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the target types that this index should match against.
        /// </summary>
        /// <value>
        /// An array of types that define the matching criteria. For example, a generic
        /// handler might specify generic parameter types as targets.
        /// </value>
        public Type[] Targets { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeMatchIndex"/> class.
        /// </summary>
        /// <param name="type">The type to be matched.</param>
        /// <param name="priority">The priority of this match index.</param>
        /// <param name="targets">The target types to match against.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="targets"/> is null.
        /// </exception>
        public TypeMatchIndex(Type type, int priority, Type[] targets)
        {
            Type = type;
            Priority = priority;
            Targets = targets ?? Type.EmptyTypes;
        }
    }
}
