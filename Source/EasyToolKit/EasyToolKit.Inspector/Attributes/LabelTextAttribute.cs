using System;
using System.Diagnostics;

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class LabelTextAttribute : Attribute
    {
        public string Label { get; set; }

        public LabelTextAttribute(string label)
        {
            Label = label;
        }
    }
}
