using System;

namespace EasyToolKit.Inspector
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnInspectorGUIAttribute : MethodAttribute
    {
    }
}
