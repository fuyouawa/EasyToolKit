using System;

namespace EasyToolKit.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ShowInInspectorAttribute : Attribute
    {
    }
}
