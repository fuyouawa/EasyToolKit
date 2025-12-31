using System;

namespace EasyToolKit.Inspector
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class MethodAttribute : InspectorAttribute
    {
    }
}
