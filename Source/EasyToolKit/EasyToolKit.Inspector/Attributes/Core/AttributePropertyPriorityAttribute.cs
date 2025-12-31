using System;

namespace EasyToolKit.Inspector
{
    public static class AttributePropertyPriorityLevel
    {
        public const int Default = 0;
        public const int Method = 1000;
        public const int Property = 2000;
        public const int Field = 3000;
        public const int Topest = 10000;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AttributePropertyPriorityAttribute : InspectorAttribute
    {
        public int Priority { get; private set; }

        public AttributePropertyPriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }
}
