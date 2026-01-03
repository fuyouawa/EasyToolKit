using System;
using System.Collections.Generic;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class EasyEditorWindow : EditorWindow
    {
        private static bool s_hasUpdatedEasyEditors;

        [SerializeField, HideInInspector]
        private WindowConfiguration _configuration;

        private object _targetObject;
        private IEditorLifecycleManager _editorLifecycleManager;
        private IWindowRenderer _windowRenderer;
        private IFocusManager _focusManager;

        [NonSerialized]
        private bool _isInitialized;

        private event Action _windowBeginGUI;
        private event Action _windowEndGUI;

        /// <summary>
        /// Gets the configuration for this window.
        /// </summary>
        internal WindowConfiguration Configuration => _configuration;

        /// <summary>
        /// Occurs when the window is closed.
        /// </summary>
        public event Action ClosedWindow;

        /// <summary>
        /// Occurs at the beginning the OnGUI method.
        /// </summary>
        public event Action BeginGUI
        {
            add => _windowBeginGUI += value;
            remove => _windowBeginGUI -= value;
        }

        /// <summary>
        /// Occurs at the end the OnGUI method.
        /// </summary>
        public event Action EndGUI
        {
            add => _windowEndGUI += value;
            remove => _windowEndGUI -= value;
        }

        /// <summary>
        /// Gets or sets the target object for the inspector.
        /// </summary>
        internal object TargetObject
        {
            get => _targetObject;
            set => _targetObject = value;
        }

        /// <summary>
        /// Gets or sets the serialized target for the inspector.
        /// </summary>
        internal UnityEngine.Object SerializedTarget
        {
            get => _serializedTarget;
            set => _serializedTarget = value;
        }

        [SerializeField, HideInInspector]
        private UnityEngine.Object _serializedTarget;

        /// <summary>
        /// Gets or sets the maximum height for the wrapped area.
        /// </summary>
        internal int WrappedAreaMaxHeight
        {
            get => _configuration?.WrappedAreaMaxHeight ?? WindowConfiguration.DefaultWrappedAreaMaxHeight;
            set
            {
                if (_configuration != null) _configuration.WrappedAreaMaxHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets whether content expansion should be prevented.
        /// </summary>
        internal bool PreventContentFromExpanding
        {
            get => _configuration?.PreventContentFromExpanding ?? false;
            set
            {
                if (_configuration != null) _configuration.PreventContentFromExpanding = value;
            }
        }

        /// <summary>
        /// Gets the current content size.
        /// </summary>
        internal Vector2 ContentSize => _windowRenderer?.ContentSize ?? Vector2.zero;

        /// <summary>
        /// Gets the label width to be used. Values between 0 and 1 are treated as percentages, and values above as pixels.
        /// </summary>
        public virtual float DefaultLabelWidth
        {
            get => _configuration?.LabelWidth ?? WindowConfiguration.DefaultLabelWidthRatio;
            set
            {
                if (_configuration != null) _configuration.LabelWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the window padding. x = left, y = right, z = top, w = bottom.
        /// </summary>
        public virtual Vector4 WindowPadding
        {
            get => _configuration?.WindowPadding ?? WindowConfiguration.DefaultWindowPadding;
            set
            {
                if (_configuration != null) _configuration.WindowPadding = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the window should draw a scroll view.
        /// </summary>
        public virtual bool UseScrollView
        {
            get => _configuration?.UseScrollView ?? true;
            set
            {
                if (_configuration != null) _configuration.UseScrollView = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the window should draw a Unity editor preview, if possible.
        /// </summary>
        public virtual bool DrawUnityEditorPreview
        {
            get => _configuration?.DrawUnityEditorPreview ?? false;
            set
            {
                if (_configuration != null) _configuration.DrawUnityEditorPreview = value;
            }
        }

        /// <summary>
        /// Gets the default preview height for Unity editors.
        /// </summary>
        public virtual float DefaultEditorPreviewHeight
        {
            get => _configuration?.EditorPreviewHeight ?? WindowConfiguration.DefaultEditorPreviewHeight;
            set
            {
                if (_configuration != null) _configuration.EditorPreviewHeight = value;
            }
        }

        /// <summary>
        /// Gets the target which which the window is supposed to draw. By default it simply returns the editor window instance itself. By default, this method is called by <see cref="GetTargets"/>().
        /// </summary>
        protected virtual object GetTarget()
        {
            if (_targetObject != null)
            {
                return _targetObject;
            }

            if (_serializedTarget != null)
            {
                return _serializedTarget;
            }

            return this;
        }

        /// <summary>
        /// Gets the targets to be drawn by the editor window. By default this simply yield returns the <see cref="GetTarget"/> method.
        /// </summary>
        protected virtual IEnumerable<object> GetTargets()
        {
            yield return GetTarget();
        }

        /// <summary>
        /// At the start of each OnGUI event when in the Layout event, the GetTargets() method is called and cached into a list which you can access from here.
        /// </summary>
        protected internal IReadOnlyList<object> CurrentDrawingTargets => _editorLifecycleManager?.CurrentTargets;

        /// <summary>
        /// Draws the Easy Editor Window.
        /// </summary>
        protected virtual void OnGUI()
        {
            // Editor windows, can be created before Easy assigns EasyEditors to all relevent types via reflection.
            // This ensures that that happens before we render anything.
            if (!s_hasUpdatedEasyEditors)
            {
                InspectorConfigAsset.Instance.EnsureEditorsHaveBeenUpdated();
                s_hasUpdatedEasyEditors = true;
            }

            InitializeIfNeeded();

            _windowRenderer.BeginRendering(_configuration);
            _focusManager.ProcessFocusEvents();

            if (Event.current.type == EventType.Layout)
            {
                _editorLifecycleManager.UpdateEditors(GetTargets());
            }

            _windowRenderer.RenderEditors(() => DrawEditors());
            _windowRenderer.EndRendering();
        }

        /// <summary>
        /// Calls DrawEditor(index) for each of the currently drawing targets.
        /// </summary>
        protected virtual void DrawEditors()
        {
            var targets = _editorLifecycleManager?.CurrentTargets;
            if (targets == null) return;

            for (int i = 0; i < targets.Count; i++)
            {
                DrawEditor(i);
            }
        }

        private void InitializeIfNeeded()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;

                _configuration = _configuration ?? new WindowConfiguration();
                _editorLifecycleManager = CreateEditorLifecycleManager();
                _windowRenderer = CreateWindowRenderer();
                _focusManager = CreateFocusManager();

                InitializeComponents();

                // Lets give it a better default name.
                if (titleContent != null && titleContent.text == GetType().FullName)
                {
                    titleContent.text = GetType().Name;
                }

                // Mouse move please
                wantsMouseMove = true;
                Selection.selectionChanged -= SelectionChanged;
                Selection.selectionChanged += SelectionChanged;
                Initialize();
            }
        }

        /// <summary>
        /// Initializes the components. Override this method to customize component creation.
        /// </summary>
        protected virtual void InitializeComponents()
        {
            // Wire up event handlers
            if (_windowRenderer != null)
            {
                _windowRenderer.BeginGUI += () => _windowBeginGUI?.Invoke();
                _windowRenderer.EndGUI += () => _windowEndGUI?.Invoke();
            }
        }

        /// <summary>
        /// Creates the editor lifecycle manager. Override to provide a custom implementation.
        /// </summary>
        private IEditorLifecycleManager CreateEditorLifecycleManager()
            => new EditorLifecycleManager();

        /// <summary>
        /// Creates the window renderer. Override to provide a custom implementation.
        /// </summary>
        private IWindowRenderer CreateWindowRenderer()
            => new WindowRenderer(this);

        /// <summary>
        /// Creates the focus manager. Override to provide a custom implementation.
        /// </summary>
        private IFocusManager CreateFocusManager()
            => new FocusManager();

        /// <summary>
        /// Initialize get called by OnEnable and by OnGUI after assembly reloads
        /// which often happens when you recompile or enter and exit play mode.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        private void SelectionChanged()
        {
            Repaint();
        }

        /// <summary>
        /// Called when the window is enabled. Remember to call base.OnEnable();
        /// </summary>
        protected virtual void OnEnable()
        {
            InitializeIfNeeded();
        }

        /// <summary>
        /// Draws the editor for the CurrentDrawingTargets[index].
        /// </summary>
        protected virtual void DrawEditor(int index)
        {
            var trees = _editorLifecycleManager?.ElementTrees;
            var editors = _editorLifecycleManager?.Editors;

            if (trees == null || editors == null) return;
            if (index < 0 || index >= trees.Count || index >= editors.Count) return;

            var tree = trees[index];
            var editor = editors[index];

            if (tree != null)
            {
                tree.Draw();
            }
            else if (editor != null && editor.target != null)
            {
                if (editor is EasyEditor easyEditor)
                {
                    easyEditor.IsInlineEditor = true;
                }

                editor.OnInspectorGUI();
            }

            if (_configuration?.DrawUnityEditorPreview == true)
            {
                DrawEditorPreview(index, _configuration.EditorPreviewHeight);
            }
        }

        /// <summary>
        /// Uses the <see cref="UnityEditor.Editor.DrawPreview(Rect)"/> method to draw a preview for the CurrentDrawingTargets[index].
        /// </summary>
        protected virtual void DrawEditorPreview(int index, float height)
        {
            var editors = _editorLifecycleManager?.Editors;
            if (editors == null || index < 0 || index >= editors.Count) return;

            UnityEditor.Editor editor = editors[index];

            if (editor != null && editor.HasPreviewGUI())
            {
                Rect rect = EditorGUILayout.GetControlRect(false, height);
                editor.DrawPreview(rect);
            }
        }

        /// <summary>
        /// Called when the window is destroyed. Remember to call base.OnDestroy();
        /// </summary>
        protected virtual void OnDestroy()
        {
            _editorLifecycleManager?.DestroyAll();
            Selection.selectionChanged -= SelectionChanged;
            ClosedWindow?.Invoke();
        }

        /// <summary>
        /// Called before starting to draw all editors for the <see cref="CurrentDrawingTargets"/>.
        /// </summary>
        protected internal virtual void OnEndDrawEditors()
        {
        }

        /// <summary>
        /// Called after all editors for the <see cref="CurrentDrawingTargets"/> has been drawn.
        /// </summary>
        protected internal virtual void OnBeginDrawEditors()
        {
        }
    }
}
