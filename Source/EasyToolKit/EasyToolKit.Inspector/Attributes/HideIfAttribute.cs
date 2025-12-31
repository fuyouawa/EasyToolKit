using System;

namespace EasyToolKit.Inspector
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class HideIfAttribute : InspectorAttribute
    {
        public string Condition { get; set; }
        public object Value { get; set; }

        public HideIfAttribute(string condition)
        {
            Condition = condition;
            Value = true;
        }

        public HideIfAttribute(string condition, object value)
        {
            Condition = condition;
            Value = value;
        }
    }
}
