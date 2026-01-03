using System;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Internal
{
    /// <summary>
    /// Configuration settings for EasyEditorWindow rendering and behavior.
    /// </summary>
    [Serializable]
    internal sealed class WindowConfiguration
    {
        public const float DefaultLabelWidthRatio = 0.33f;
        public const int DefaultWrappedAreaMaxHeight = 1000;
        public const int DefaultEditorPreviewHeight = 170;
        public static readonly Vector4 DefaultWindowPadding = new Vector4(4, 4, 4, 4);

        [SerializeField] private float _labelWidth = DefaultLabelWidthRatio;
        [SerializeField] private Vector4 _windowPadding = DefaultWindowPadding;
        [SerializeField] private bool _useScrollView = true;
        [SerializeField] private bool _drawUnityEditorPreview;
        [SerializeField] private int _wrappedAreaMaxHeight = DefaultWrappedAreaMaxHeight;
        [SerializeField] private bool _preventContentFromExpanding;
        [SerializeField] private float _editorPreviewHeight = DefaultEditorPreviewHeight;

        /// <summary>
        /// Gets or sets the label width ratio for inspector fields.
        /// </summary>
        public float LabelWidth
        {
            get => _labelWidth;
            set => _labelWidth = value;
        }

        /// <summary>
        /// Gets or sets the window padding for the editor window.
        /// </summary>
        public Vector4 WindowPadding
        {
            get => _windowPadding;
            set => _windowPadding = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether scroll view is enabled.
        /// </summary>
        public bool UseScrollView
        {
            get => _useScrollView;
            set => _useScrollView = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to draw Unity editor preview.
        /// </summary>
        public bool DrawUnityEditorPreview
        {
            get => _drawUnityEditorPreview;
            set => _drawUnityEditorPreview = value;
        }

        /// <summary>
        /// Gets or sets the maximum height for wrapped areas.
        /// </summary>
        public int WrappedAreaMaxHeight
        {
            get => _wrappedAreaMaxHeight;
            set => _wrappedAreaMaxHeight = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether content expansion is prevented.
        /// </summary>
        public bool PreventContentFromExpanding
        {
            get => _preventContentFromExpanding;
            set => _preventContentFromExpanding = value;
        }

        /// <summary>
        /// Gets or sets the editor preview height.
        /// </summary>
        public float EditorPreviewHeight
        {
            get => _editorPreviewHeight;
            set => _editorPreviewHeight = value;
        }
    }
}
