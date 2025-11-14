using System;
using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    public class EasyMethodAttributeDrawer<TAttribute> : EasyAttributeDrawer<TAttribute>
        where TAttribute : MethodAttribute
    {
        private MethodInfo _methodInfo;

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
