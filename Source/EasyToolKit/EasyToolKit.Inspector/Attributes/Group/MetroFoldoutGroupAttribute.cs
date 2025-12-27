using System;
using EasyToolKit.Inspector;
using System.Diagnostics;
using UnityEngine;

[assembly: RegisterGroupAttributeScope(typeof(MetroFoldoutGroupAttribute), typeof(EndMetroFoldoutGroupAttribute))]

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class MetroFoldoutGroupAttribute : BeginGroupAttribute
    {
        public string Label { get; set; }
        public string RightLabel { get; set; }
        public string Tooltip { get; set; }

        /// <summary>
        /// The right label color getter.
        /// Default value is <see cref="Color.white"/>.
        /// </summary>
        public string RightLabelColorGetter { get; set; }

        /// <summary>
        /// The side line color getter.
        /// Default value is <see cref="Color.green"/>.
        /// </summary>
        public string SideLineColorGetter { get; set; }

        public string IconTextureGetter { get; set; }

        private bool? _expanded;

        public bool Expanded
        {
            get => _expanded ?? throw new InvalidOperationException();
            set => _expanded = value;
        }

        public bool IsDefinedExpanded => _expanded.HasValue;

        public override string GroupName => GroupCatalogue + "/" + Label;

        public MetroFoldoutGroupAttribute(string label, string tooltip = null)
        {
            Label = label;
            Tooltip = tooltip;
        }
    }

    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class EndMetroFoldoutGroupAttribute : EndGroupAttribute
    {
        public EndMetroFoldoutGroupAttribute()
        {
        }
    }
}
