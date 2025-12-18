using System;

namespace EasyToolKit.Inspector.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ResolverPriorityAttribute : Attribute
    {
        public double Priority { get; }

        public ResolverPriorityAttribute(double priority)
        {
            Priority = priority;
        }
    }
}
