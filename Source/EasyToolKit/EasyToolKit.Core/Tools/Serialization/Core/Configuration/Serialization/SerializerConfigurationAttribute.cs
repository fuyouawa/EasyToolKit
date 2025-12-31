using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Attribute for configuring serializer registration and priority.
    /// Applied to serializer classes to control their registration behavior.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class SerializerConfigurationAttribute : Attribute
    {
        private static readonly SerializerConfigurationAttribute DefaultImpl = new SerializerConfigurationAttribute();

        /// <summary>
        /// Gets the default configuration instance.
        /// </summary>
        public static SerializerConfigurationAttribute Default => DefaultImpl;

        /// <summary>
        /// Gets the priority level for this serializer.
        /// Higher priority serializers are evaluated first during type matching.
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// Gets a value indicating whether this serializer can handle inherited types.
        /// When true, the serializer matching algorithm considers base type relationships.
        /// </summary>
        public bool AllowInherit { get; }

        /// <summary>
        /// Initializes a new instance with Custom priority level.
        /// </summary>
        public SerializerConfigurationAttribute()
            : this(SerializerPriorityLevel.Custom)
        {
        }

        /// <summary>
        /// Initializes a new instance with the specified priority level.
        /// </summary>
        public SerializerConfigurationAttribute(SerializerPriorityLevel priority)
            : this((int)priority)
        {
        }

        /// <summary>
        /// Initializes a new instance with the specified priority value.
        /// </summary>
        public SerializerConfigurationAttribute(int priority)
            : this(priority, allowInherit: false)
        {
        }

        /// <summary>
        /// Initializes a new instance with the specified priority and inheritance behavior.
        /// </summary>
        public SerializerConfigurationAttribute(int priority, bool allowInherit)
        {
            Priority = priority;
            AllowInherit = allowInherit;
        }
    }
}
