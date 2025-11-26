using System;
using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Base class for drawers that handle method attributes.
    /// Method attributes are applied to methods and can be used to create buttons or method call UI in the Inspector.
    /// </summary>
    /// <typeparam name="TAttribute">The type of method attribute this drawer handles.</typeparam>
    public class EasyMethodAttributeDrawer<TAttribute> : EasyAttributeDrawer<TAttribute>
        where TAttribute : MethodAttribute
    {
        private MethodInfo _methodInfo;

        /// <summary>
        /// Gets the MethodInfo for the method that this drawer is handling.
        /// The MethodInfo is lazily loaded from the property when first accessed.
        /// </summary>
        public MethodInfo MethodInfo
        {
            get
            {
                if (_methodInfo == null)
                {
                    if (Property.Info.TryGetMemberInfo(out var memberInfo))
                    {
                        _methodInfo = memberInfo as MethodInfo;
                    }

                    if (_methodInfo == null)
                    {
                        throw new InvalidOperationException($"{memberInfo} is not a MethodInfo.");
                    }
                }
                return _methodInfo;
            }
        }

        /// <summary>
        /// Determines whether this drawer can draw the specified attribute property.
        /// This method ensures that the property represents a method.
        /// </summary>
        /// <param name="property">The InspectorProperty to check.</param>
        /// <returns>True if the property represents a method, false otherwise.</returns>
        protected override bool CanDrawAttributeProperty(InspectorProperty property)
        {
            if (property.Info.TryGetMemberInfo(out var memberInfo))
            {
                return memberInfo is MethodInfo;
            }

            return false;
        }
    }
}
