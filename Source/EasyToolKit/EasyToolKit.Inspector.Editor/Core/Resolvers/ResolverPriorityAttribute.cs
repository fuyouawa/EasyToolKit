using System;

namespace EasyToolKit.Inspector.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ResolverPriorityAttribute : Attribute, IPriorityAccessor
    {
        public Priority Priority { get; }

        public ResolverPriorityAttribute(double priority = 0.0)
        {
            Priority = new Priority(priority);
        }
    }
}
