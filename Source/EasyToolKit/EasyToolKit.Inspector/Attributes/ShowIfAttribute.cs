using System;
using System.Diagnostics;

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class ShowIfAttribute : InspectorAttribute
    {
        public string Condition { get; set; }
        public object Value { get; set; }

        public ShowIfAttribute(string condition)
        {
            Condition = condition;
            Value = true;
        }

        public ShowIfAttribute(string condition, object value)
        {
            Condition = condition;
            Value = value;
        }
    }
}
