using EasyToolKit.Inspector;
using System;
using System.Diagnostics;

[assembly: RegisterGroupAttributeScope(typeof(MetroBoxGroupAttribute), typeof(EndMetroBoxGroupAttribute))]

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class MetroBoxGroupAttribute : BeginGroupAttribute
    {
        public string Label { get; set; }
        public string IconTextureGetter { get; set; }

        public override string GroupName => GroupCatalogue + "/" + Label;

        public MetroBoxGroupAttribute(string label)
        {
            Label = label;
        }
    }

    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class EndMetroBoxGroupAttribute : EndGroupAttribute
    {
    }
}
