using System;

namespace EasyToolKit.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class RequiredAttribute : CanPassToListElementAttribute
    {
    }
}
