using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using EasyToolKit.Core;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Generic value structure resolver implementation using reflection to discover elements.
    /// Focuses purely on value structure without collection operations.
    /// </summary>
    public class GenericValueStructureResolver : ValueStructureResolverBase
    {
        private readonly List<IElementDefinition> _definitions = new List<IElementDefinition>();

        protected override bool CanResolveElement(IValueElement element)
        {
            return !element.ValueEntry.ValueType.IsInheritsFrom(typeof(Delegate));
        }

        /// <summary>
        /// Initializes the resolver by discovering properties, fields, and methods using reflection
        /// </summary>
        protected override void Initialize()
        {
            var targetType = Element.ValueEntry.ValueType;

            var filteredMembers = targetType.GetAllMembers(BindingFlagsHelper.AllInstance)
                .Where(Filter)
                .OrderBy(Order)
                .ToList();

            var showOdinSerializersInInspector = targetType.IsDefined<ShowOdinSerializedPropertiesInInspector>(true);

            // Use for loop instead of foreach to avoid enumerator allocation
            for (int i = 0; i < filteredMembers.Count; i++)
            {
                var memberInfo = filteredMembers[i];
                var showInInspector = memberInfo.IsDefined<ShowInInspectorAttribute>();
                var hideInInspector = memberInfo.IsDefined<HideInInspector>();
                if (hideInInspector)
                {
                    continue;
                }

                if (memberInfo is FieldInfo fieldInfo)
                {
                    // Handle auxiliary attributes
                    if (fieldInfo.IsDefined<AuxiliaryAttribute>(true))
                    {
                        _definitions.Add(CreateDefinition(memberInfo));
                        continue;
                    }

                    // Handle Odin serialized fields
                    if (showOdinSerializersInInspector && fieldInfo.IsDefined<OdinSerializeAttribute>())
                    {
                        _definitions.Add(CreateDefinition(memberInfo));
                        continue;
                    }

                    // Filter non-serializable fields
                    if (!InspectorPropertyUtility.IsSerializableField(fieldInfo) && !showInInspector)
                    {
                        continue;
                    }

                    // Additional filtering for non-showInInspector fields
                    if (!showInInspector)
                    {
                        var memberType = memberInfo.GetMemberType();
                        if (!memberType.IsInheritsFrom<UnityEngine.Object>() &&
                            !memberType.IsValueType &&
                            !memberType.IsDefined<SerializableAttribute>())
                        {
                            continue;
                        }
                    }
                }
                else if (memberInfo is PropertyInfo propertyInfo)
                {
                    if (!showInInspector)
                    {
                        continue;
                    }
                }
                else if (memberInfo is MethodInfo methodInfo)
                {
                    // Only include methods with MethodAttribute
                    if (!methodInfo.IsDefined<MethodAttribute>(true))
                        continue;

                    _definitions.Add(CreateDefinition(memberInfo));
                    continue;
                }

                // Add valid member to property definitions
                _definitions.Add(CreateDefinition(memberInfo));
            }
        }

        private IElementDefinition CreateDefinition(MemberInfo memberInfo)
        {
            if (memberInfo is MethodInfo methodInfo)
            {
                return InspectorElements.Configurator.Method()
                    .WithMethodInfo(methodInfo)
                    .CreateDefinition();
            }

            if (memberInfo is FieldInfo fieldInfo)
            {
                if (TryCreateCollectionDefinition(fieldInfo, out var definition))
                {
                    return definition;
                }

                return InspectorElements.Configurator.Field()
                    .WithFieldInfo(fieldInfo)
                    .CreateDefinition();
            }

            if (memberInfo is PropertyInfo propertyInfo)
            {
                if (TryCreateCollectionDefinition(propertyInfo, out var definition))
                {
                    return definition;
                }

                return InspectorElements.Configurator.Property()
                    .WithPropertyInfo(propertyInfo)
                    .CreateDefinition();
            }

            throw new NotSupportedException($"Member '{memberInfo}' is not supported.");
        }

        private bool TryCreateCollectionDefinition(MemberInfo memberInfo, out ICollectionDefinition collectionDefinition)
        {
            collectionDefinition = null;
            var type = memberInfo.GetMemberType();

            if (!type.IsImplementsOpenGenericType(typeof(IReadOnlyCollection<>)) &&
                !type.IsImplementsOpenGenericType(typeof(ICollection<>)))
                return false;

            var isOrdered = type.IsImplementsOpenGenericType(typeof(IReadOnlyList<>)) ||
                            type.IsImplementsOpenGenericType(typeof(IList<>));

            var elementType = type.GetArgumentsOfInheritedOpenGenericType(typeof(IEnumerable<>))[0];
            if (memberInfo is FieldInfo fieldInfo)
            {
                collectionDefinition = InspectorElements.Configurator.FieldCollection()
                    .WithFieldInfo(fieldInfo)
                    .WithIsOrdered(isOrdered)
                    .WithItemType(elementType)
                    .CreateDefinition();
            }
            else if (memberInfo is PropertyInfo propertyInfo)
            {
                collectionDefinition = InspectorElements.Configurator.PropertyCollection()
                    .WithPropertyInfo(propertyInfo)
                    .WithIsOrdered(isOrdered)
                    .WithItemType(elementType)
                    .CreateDefinition();
            }
            else
            {
                return false;
            }

            return true;
        }

        protected override IElementDefinition[] GetChildrenDefinitions()
        {
            return _definitions.ToArray();
        }

        /// <summary>
        /// Clears the cached element definitions when the resolver is returned to the pool.
        /// </summary>
        protected override void OnRelease()
        {
            base.OnRelease();
            _definitions.Clear();
        }

        /// <summary>
        /// Orders members by priority for display in the inspector
        /// </summary>
        /// <param name="memberInfo">The member to order</param>
        /// <returns>The priority value (lower values appear first)</returns>
        private static int Order(MemberInfo memberInfo)
        {
            if (memberInfo.IsDefined<OnInspectorInitAttribute>())
            {
                return -2;
            }

            if (memberInfo.IsDefined<OnInspectorGUIAttribute>())
            {
                return -1;
            }

            if (memberInfo is FieldInfo) return 0;
            if (memberInfo is PropertyInfo) return 1;
            if (memberInfo is MethodInfo) return 2;
            return 3;
        }

        /// <summary>
        /// Filters members to determine which should be displayed in the inspector
        /// </summary>
        /// <param name="memberInfo">The member to filter</param>
        /// <returns>True if the member should be included in the inspector</returns>
        private bool Filter(MemberInfo memberInfo)
        {
            var targetType = Element.ValueEntry.ValueType;
            // Exclude object type members unless the target type is object
            if (memberInfo.DeclaringType == typeof(object) && targetType != typeof(object)) return false;
            // Only include fields, properties, and methods
            if (!(memberInfo is FieldInfo || memberInfo is PropertyInfo || memberInfo is MethodInfo)) return false;
            // Exclude special name members (compiler-generated)
            if (memberInfo is FieldInfo fieldInfo && fieldInfo.IsSpecialName) return false;
            if (memberInfo is MethodInfo methodInfo && methodInfo.IsSpecialName) return false;
            if (memberInfo is PropertyInfo propertyInfo && propertyInfo.IsSpecialName) return false;
            // Exclude compiler-generated members
            if (memberInfo.IsDefined<CompilerGeneratedAttribute>()) return false;

            return true;
        }
    }
}
