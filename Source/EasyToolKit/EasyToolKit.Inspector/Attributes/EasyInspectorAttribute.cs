using System;
using System.Diagnostics;

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EasyInspectorAttribute : Attribute
    {
    }
}
