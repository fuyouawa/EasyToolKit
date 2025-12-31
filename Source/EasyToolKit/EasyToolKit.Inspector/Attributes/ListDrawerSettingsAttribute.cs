using System.Diagnostics;
using System;

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class ListDrawerSettingsAttribute : InspectorAttribute
    {
        private bool? _expanded;
        private bool? _draggable;
        private bool? _showIndexLabel;
        private bool? _showFoldoutHeader;

        public bool Expanded
        {
            get => _expanded ?? throw new InvalidOperationException();
            set => _expanded = value;
        }

        public bool Draggable
        {
            get => _draggable ?? throw new InvalidOperationException();
            set => _draggable = value;
        }

        public bool ShowIndexLabel
        {
            get => _showIndexLabel ?? throw new InvalidOperationException();
            set => _showIndexLabel = value;
        }

        public bool ShowFoldoutHeader
        {
            get => _showFoldoutHeader ?? throw new InvalidOperationException();
            set => _showFoldoutHeader = value;
        }

        public bool IsDefinedExpanded => _expanded.HasValue;
        public bool IsDefinedDraggable => _draggable.HasValue;
        public bool IsDefinedShowIndexLabel => _showIndexLabel.HasValue;
        public bool IsDefinedShowFoldoutHeader => _showFoldoutHeader.HasValue;

        public string CustomIndexLabelFunction { get; set; }

        public bool HideAddButton { get; set; }
        public bool HideRemoveButton { get; set; }

        public string OnAddedItemCallback { get; set; }
        public string OnRemovedItemCallback { get; set; }

        public string CustomCreateItemFunction { get; set; }
        public string CustomRemoveIndexFunction { get; set; }
        public string CustomRemoveItemFunction { get; set; }

        public bool IsReadOnly { get; set; }

        public ListDrawerSettingsAttribute()
        {
        }
    }
}
