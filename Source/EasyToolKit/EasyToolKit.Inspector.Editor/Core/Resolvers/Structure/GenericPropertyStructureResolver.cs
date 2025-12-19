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
    /// Generic property structure resolver implementation using reflection to discover properties.
    /// Focuses purely on property structure without collection operations.
    /// </summary>
    public class GenericPropertyStructureResolver : PropertyStructureResolverBase
    {
        private readonly List<InspectorPropertyInfo> _propertyInfos = new List<InspectorPropertyInfo>();

        protected override bool CanResolve(InspectorProperty property)
        {
            return !property.ValueEntry.ValueType.IsInheritsFrom<IEnumerable>();
        }

        /// <summary>
        /// Initializes the resolver by discovering properties, fields, and methods using reflection
        /// </summary>
        protected override void Initialize()
        {
            var targetType = Property.ValueEntry.ValueType;
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
                        _propertyInfos.Add(InspectorPropertyInfo.CreateForMember(memberInfo));
                        continue;
                    }

                    // Handle Odin serialized fields
                    if (showOdinSerializersInInspector && fieldInfo.IsDefined<OdinSerializeAttribute>())
                    {
                        _propertyInfos.Add(InspectorPropertyInfo.CreateForMember(memberInfo));
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
                }

                // Add valid member to property infos
                _propertyInfos.Add(InspectorPropertyInfo.CreateForMember(memberInfo));
            }
        }

        /// <summary>
        /// Gets information about a child property at the specified index
        /// </summary>
        /// <param name="childIndex">The index of the child property</param>
        /// <returns>Information about the child property</returns>
        public override InspectorPropertyInfo GetChildInfo(int childIndex)
        {
            if (childIndex < 0 || childIndex >= _propertyInfos.Count)
                throw new ArgumentOutOfRangeException(nameof(childIndex));

            return _propertyInfos[childIndex];
        }

        /// <summary>
        /// Converts a child property name to its index
        /// </summary>
        /// <param name="name">The name of the child property</param>
        /// <returns>The index of the child property, or -1 if not found</returns>
        public override int ChildNameToIndex(string name)
        {
            for (int i = 0; i < _propertyInfos.Count; i++)
            {
                if (_propertyInfos[i].PropertyName == name)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Calculates the number of child properties
        /// </summary>
        /// <returns>The number of child properties</returns>
        protected override int CalculateChildCount()
        {
            return _propertyInfos.Count;
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
            var targetType = Property.ValueEntry.ValueType;
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
