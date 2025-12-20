using System;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents the priority of a drawer in the inspector drawer chain.
    /// Higher priority drawers are executed before lower priority drawers.
    /// This class implements comparison and equality operations for proper drawer ordering.
    /// </summary>
    public class InspectorPriority : IEquatable<InspectorPriority>, IComparable<InspectorPriority>
    {
        public static readonly InspectorPriority Default = new InspectorPriority(0.0);

        /// <summary>
        /// Gets the numeric value of this drawer priority.
        /// </summary>
        public readonly double Value;

        /// <summary>
        /// Initializes a new instance of the DrawerPriority class with the specified value.
        /// </summary>
        /// <param name="value">The numeric priority value.</param>
        public InspectorPriority(double value)
        {
            Value = value;
        }

        /// <summary>
        /// Determines whether two DrawerPriority instances are equal.
        /// Uses approximate equality comparison to account for floating-point precision.
        /// </summary>
        /// <param name="left">The first DrawerPriority to compare.</param>
        /// <param name="right">The second DrawerPriority to compare.</param>
        /// <returns>true if the two DrawerPriority instances are equal; otherwise, false.</returns>
        public static bool operator ==(InspectorPriority left, InspectorPriority right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;

            return left.Value.IsApproximatelyOf(right.Value);
        }

        /// <summary>
        /// Determines whether two DrawerPriority instances are not equal.
        /// </summary>
        /// <param name="left">The first DrawerPriority to compare.</param>
        /// <param name="right">The second DrawerPriority to compare.</param>
        /// <returns>true if the two DrawerPriority instances are not equal; otherwise, false.</returns>
        public static bool operator !=(InspectorPriority left, InspectorPriority right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines whether one DrawerPriority is greater than another.
        /// </summary>
        /// <param name="left">The first DrawerPriority to compare.</param>
        /// <param name="right">The second DrawerPriority to compare.</param>
        /// <returns>true if left is greater than right; otherwise, false.</returns>
        public static bool operator >(InspectorPriority left, InspectorPriority right)
        {
            if (left == right) return false;
            if (left.Value > right.Value) return true;
            return false;
        }

        /// <summary>
        /// Determines whether one DrawerPriority is less than another.
        /// </summary>
        /// <param name="left">The first DrawerPriority to compare.</param>
        /// <param name="right">The second DrawerPriority to compare.</param>
        /// <returns>true if left is less than right; otherwise, false.</returns>
        public static bool operator <(InspectorPriority left, InspectorPriority right)
        {
            if (left == right) return false;
            if (left.Value < right.Value) return true;
            return false;
        }

        /// <summary>
        /// Determines whether one DrawerPriority is greater than or equal to another.
        /// </summary>
        /// <param name="left">The first DrawerPriority to compare.</param>
        /// <param name="right">The second DrawerPriority to compare.</param>
        /// <returns>true if left is greater than or equal to right; otherwise, false.</returns>
        public static bool operator >=(InspectorPriority left, InspectorPriority right)
        {
            if (left.Value.IsApproximatelyOf(right.Value) || left.Value > right.Value) return true;
            return false;
        }

        /// <summary>
        /// Determines whether one DrawerPriority is less than or equal to another.
        /// </summary>
        /// <param name="left">The first DrawerPriority to compare.</param>
        /// <param name="right">The second DrawerPriority to compare.</param>
        /// <returns>true if left is less than or equal to right; otherwise, false.</returns>
        public static bool operator <=(InspectorPriority left, InspectorPriority right)
        {
            if (left.Value.IsApproximatelyOf(right.Value) || left.Value < right.Value) return true;
            return false;
        }

        /// <summary>
        /// Returns the hash code for this DrawerPriority.
        /// </summary>
        /// <returns>A hash code for the current DrawerPriority.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current DrawerPriority.
        /// </summary>
        /// <param name="obj">The object to compare with the current DrawerPriority.</param>
        /// <returns>true if the specified object is equal to the current DrawerPriority; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is InspectorPriority priority)
            {
                return this == priority;
            }
            return false;
        }

        /// <summary>
        /// Indicates whether the current DrawerPriority is equal to another DrawerPriority.
        /// </summary>
        /// <param name="other">A DrawerPriority to compare with this DrawerPriority.</param>
        /// <returns>true if the current DrawerPriority is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(InspectorPriority other)
        {
            return this == other;
        }

        /// <summary>
        /// Compares this DrawerPriority with another DrawerPriority and returns an integer
        /// that indicates whether this instance precedes, follows, or occurs in the same position
        /// in the sort order as the other DrawerPriority.
        /// </summary>
        /// <param name="other">A DrawerPriority to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(InspectorPriority other)
        {
            if (this > other)
            {
                return 1;
            }

            if (this < other)
            {
                return -1;
            }

            return 0;
        }
    }
}
