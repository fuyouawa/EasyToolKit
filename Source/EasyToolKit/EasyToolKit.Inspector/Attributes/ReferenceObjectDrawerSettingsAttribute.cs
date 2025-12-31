using System;
using System.Diagnostics;

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class ReferenceObjectDrawerSettingsAttribute : InspectorAttribute
    {
        public bool HideFoldout { get; set; }
        public bool HideIfNull { get; set; }
        public bool? InstantiateIfNull { get; set; }

        public ReferenceObjectDrawerSettingsAttribute()
        {
        }
    }
}
