using System;

namespace EasyToolKit.Inspector
{
    [AttributePropertyPriority(AttributePropertyPriorityLevel.Topest + 1)]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnInspectorInitAttribute : MethodAttribute
    {
    }
}
