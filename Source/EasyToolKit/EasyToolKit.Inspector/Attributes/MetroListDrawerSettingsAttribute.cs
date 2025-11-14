using System;
using System.Diagnostics;
using UnityEngine;

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class MetroListDrawerSettingsAttribute : ListDrawerSettingsAttribute
    {
        public string IconTextureGetter { get; set; }

        public MetroListDrawerSettingsAttribute()
        {
        }
    }
}
