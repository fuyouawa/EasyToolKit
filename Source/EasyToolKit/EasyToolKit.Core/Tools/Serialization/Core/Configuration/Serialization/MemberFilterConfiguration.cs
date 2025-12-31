using System;
using System.Collections.Generic;
using System.Reflection;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Configuration for member filtering during serialization.
    /// Determines which class members should be included in serialization based on
    /// access modifiers, member types, and custom exclusion rules.
    /// </summary>
    public sealed class MemberFilterConfiguration : ValidatableConfigurationBase, IMemberFilterConfiguration
    {
        private MemberFilterFlags _filterFlags;
        private readonly List<Type> _excludedTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberFilterConfiguration"/> class with default values.
        /// </summary>
        internal MemberFilterConfiguration()
        {
            _filterFlags = MemberFilterFlags.None;
            _excludedTypes = new List<Type> { typeof(EasySerializationData) };
        }

        /// <inheritdoc/>
        public MemberFilterFlags FilterFlags => _filterFlags;

        /// <inheritdoc/>
        public IReadOnlyList<Type> ExcludedTypes => _excludedTypes.AsReadOnly();

        /// <inheritdoc/>
        protected override ConfigurationValidationResult ValidateCore()
        {
            var errors = new List<ConfigurationValidationError>();

            // Rule 1: At least one member type must be specified
            if ((_filterFlags & MemberFilterFlags.AllProperty) == 0 &&
                (_filterFlags & MemberFilterFlags.Field) == 0)
            {
                errors.Add(new ConfigurationValidationError(
                    nameof(FilterFlags),
                    "At least one member type (Field or Property) must be specified",
                    "ERR_NO_MEMBER_TYPE"));
            }

            // Rule 2: At least one access modifier must be specified
            if ((_filterFlags & MemberFilterFlags.Public) == 0 &&
                (_filterFlags & MemberFilterFlags.NonPublic) == 0 &&
                !_filterFlags.HasFlag(MemberFilterFlags.SerializeField))
            {
                errors.Add(new ConfigurationValidationError(
                    nameof(FilterFlags),
                    "At least one access modifier (Public, NonPublic, or SerializeField) must be specified",
                    "ERR_NO_ACCESS_MODIFIER"));
            }

            return errors.Count == 0
                ? ConfigurationValidationResult.Valid()
                : ConfigurationValidationResult.Invalid(errors.ToArray());
        }

        /// <summary>
        /// Sets the filter flags directly.
        /// </summary>
        internal void SetFilterFlags(MemberFilterFlags flags)
        {
            _filterFlags = flags;
            ResetValidation();
        }

        /// <summary>
        /// Adds a type to the exclusion list.
        /// </summary>
        internal void AddExcludedType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!_excludedTypes.Contains(type))
            {
                _excludedTypes.Add(type);
            }
            ResetValidation();
        }

        /// <summary>
        /// Adds multiple types to the exclusion list.
        /// </summary>
        internal void AddExcludedTypes(IEnumerable<Type> types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            foreach (var type in types)
            {
                if (type != null && !_excludedTypes.Contains(type))
                {
                    _excludedTypes.Add(type);
                }
            }
            ResetValidation();
        }

        /// <summary>
        /// Removes a type from the exclusion list.
        /// </summary>
        internal void RemoveExcludedType(Type type)
        {
            _excludedTypes.Remove(type);
            ResetValidation();
        }

        /// <summary>
        /// Clears all types from the exclusion list.
        /// </summary>
        internal void ClearExcludedTypes()
        {
            _excludedTypes.Clear();
            ResetValidation();
        }

        /// <inheritdoc/>
        public MemberFilter CreateFilter()
        {
            ValidateOrThrow();

            var flags = _filterFlags;
            var excludedTypes = _excludedTypes.ToArray();

            return member => MemberFilterImpl.Invoke(member, flags, excludedTypes);
        }
    }
}
