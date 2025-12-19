using System;

namespace EasyToolKit.Inspector.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ResolverPriorityAttribute : Attribute, IInspectorPriorityGetter
    {
        public InspectorPriority Priority { get; }

        public ResolverPriorityAttribute(double priority = 0.0)
        {
            Priority = new InspectorPriority(priority);
        }
    }
}
