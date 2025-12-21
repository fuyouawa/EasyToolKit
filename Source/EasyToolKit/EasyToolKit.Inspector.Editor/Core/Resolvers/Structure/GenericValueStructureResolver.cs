using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using EasyToolKit.Core;
using EasyToolKit.ThirdParty.OdinSerializer;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Generic value structure resolver implementation using reflection to discover elements.
    /// Focuses purely on value structure without collection operations.
    /// </summary>
    public class GenericValueStructureResolver : ValueStructureResolverBase
    {
        private readonly List<IElementDefinition> _definitions = new List<IElementDefinition>();

        /// <summary>
        /// Initializes the resolver by discovering properties, fields, and methods using reflection
        /// </summary>
        protected override void Initialize()
        {
            var targetType = Element.ValueEntry.ValueType;
            // Get all members, filter them, and order by priority
            var memberInfos = targetType.GetAllMembers(BindingFlagsHelper.AllInstance).Where(Filter).OrderBy(Order);

            var showOdinSerializersInInspector = targetType.IsDefined<ShowOdinSerializedPropertiesInInspector>(true);
            foreach (var memberInfo in memberInfos)
            {
                var showInInspector = memberInfo.IsDefined<ShowInInspectorAttribute>();

                if (memberInfo is FieldInfo fieldInfo)
                {
                    // Handle auxiliary attributes
                    if (fieldInfo.GetCustomAttributes().Any(attr => attr is AuxiliaryAttribute))
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
                    if (!InspectorPropertyInfoUtility.IsSerializableField(fieldInfo) && !showInInspector)
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
                    if (!methodInfo.GetCustomAttributes().Any(attr => attr is MethodAttribute))
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
                    .WithName(methodInfo.Name)
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
                    .WithName(fieldInfo.Name)
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
                    .WithName(propertyInfo.Name)
                    .CreateDefinition();
            }

            throw new NotSupportedException($"Member '{memberInfo}' is not supported.");
        }

        private bool TryCreateCollectionDefinition(MemberInfo memberInfo, out ICollectionDefinition collectionDefinition)
        {
            collectionDefinition = null;
            var type = memberInfo.GetMemberType();
            if (!type.IsGenericType)
                return false;

            var genericType = type.GetGenericTypeDefinition();

            if (genericType != typeof(ICollection<>) || genericType != typeof(IReadOnlyCollection<>))
                return false;

            var elementType = type.GetArgumentsOfInheritedOpenGenericType(typeof(IEnumerable<>))[0];
            if (memberInfo is FieldInfo fieldInfo)
            {
                collectionDefinition = InspectorElements.Configurator.FieldCollection()
                    .WithFieldInfo(fieldInfo)
                    .WithName(fieldInfo.Name)
                    .WithItemType(elementType)
                    .CreateDefinition();
            }
            else if (memberInfo is PropertyInfo propertyInfo)
            {
                collectionDefinition = InspectorElements.Configurator.PropertyCollection()
                    .WithPropertyInfo(propertyInfo)
                    .WithName(propertyInfo.Name)
                    .WithItemType(elementType)
                    .CreateDefinition();
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the definition of a child at the specified index
        /// </summary>
        /// <param name="childIndex">The index of the child</param>
        /// <returns>Definition of the child</returns>
        protected override IElementDefinition GetChildDefinition(int childIndex)
        {
            if (childIndex < 0 || childIndex >= _definitions.Count)
                throw new ArgumentOutOfRangeException(nameof(childIndex));

            return _definitions[childIndex];
        }

        /// <summary>
        /// Converts a child name to its index
        /// </summary>
        /// <param name="name">The name of the child</param>
        /// <returns>The index of the child, or -1 if not found</returns>
        protected override int ChildNameToIndex(string name)
        {
            for (int i = 0; i < _definitions.Count; i++)
            {
                if (_definitions[i].Name == name)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Calculates the number of children
        /// </summary>
        /// <returns>The number of childreh</returns>
        protected override int CalculateChildCount()
        {
            return _definitions.Count;
        }

        /// <summary>
        /// Orders members by priority for display in the inspector
        /// </summary>
        /// <param name="memberInfo">The member to order</param>
        /// <returns>The priority value (lower values appear first)</returns>
        private static int Order(MemberInfo memberInfo)
        {
            if (InspectorAttributeUtility.TryGetMemberDefinedAttributePropertyPriority(memberInfo, out var priority))
            {
                return -priority.Priority;
            }

            if (memberInfo is FieldInfo) return -AttributePropertyPriorityLevel.Field;
            if (memberInfo is PropertyInfo) return -AttributePropertyPriorityLevel.Property;
            if (memberInfo is MethodInfo) return -AttributePropertyPriorityLevel.Method;
            return -AttributePropertyPriorityLevel.Default;
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
