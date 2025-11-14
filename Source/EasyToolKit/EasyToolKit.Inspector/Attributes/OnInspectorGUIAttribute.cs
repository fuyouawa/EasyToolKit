using System;

namespace EasyToolKit.Inspector
{
    [AttributePropertyPriority(AttributePropertyPriorityLevel.Topest)]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnInspectorGUIAttribute : MethodAttribute
    {
    }
}
