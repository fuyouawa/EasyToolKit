using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor.Modules;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class EasyEditorWindow : EditorWindow
    {
        /// <summary>
        /// Occurs when the window is closed.
        /// </summary>
        public event Action WindowClosed;

        /// <summary>
        /// Occurs at the beginning the OnGUI method.
        /// </summary>
        public event Action GuiBegun;

        /// <summary>
        /// Occurs at the end the OnGUI method.
        /// </summary>
        public event Action GuiEnded;

        [SerializeField, HideInInspector]
        private UnityEngine.Object _serializedTarget;

        [SerializeField, HideInInspector]
        private float _labelWidth = 0.33f;

        [SerializeField, HideInInspector]
        private Vector4 _windowPadding = new Vector4(4, 4, 4, 4);

        [SerializeField, HideInInspector]
        private bool _useScrollView = true;

        [SerializeField, HideInInspector]
        private bool _drawUnityEditorPreview;

        [SerializeField, HideInInspector]
        private int _wrappedAreaMaxHeight = 1000;

        [SerializeField, HideInInspector]
        private float _defaultEditorPreviewHeight = 170f;

        private object _targetObject;
        private bool _isInitialized;
        private Dictionary<Type, IEditorWindowModule> _modules;

        /// <summary>
        /// Gets the label width to be used. Values between 0 and 1 are treated as percentages, and values above as pixels.
        /// </summary>
        public virtual float DefaultLabelWidth
        {
            get { return _labelWidth; }
            set
            {
                _labelWidth = value;
                var layoutModule = GetModule<GUILayoutModule>();
                if (layoutModule != null) layoutModule.DefaultLabelWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the window padding. x = left, y = right, z = top, w = bottom.
        /// </summary>
        public virtual Vector4 WindowPadding
        {
            get { return _windowPadding; }
            set
            {
                _windowPadding = value;
                var layoutModule = GetModule<GUILayoutModule>();
                if (layoutModule != null) layoutModule.WindowPadding = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the window should draw a scroll view.
        /// </summary>
        public virtual bool UseScrollView
        {
            get { return _useScrollView; }
            set
            {
                _useScrollView = value;
                var layoutModule = GetModule<GUILayoutModule>();
                if (layoutModule != null) layoutModule.UseScrollView = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the window should draw a Unity editor preview, if possible.
        /// </summary>
        public virtual bool DrawUnityEditorPreview
        {
            get { return _drawUnityEditorPreview; }
            set
            {
                _drawUnityEditorPreview = value;
                var previewModule = GetModule<PreviewRenderingModule>();
                if (previewModule != null) previewModule.DrawUnityEditorPreview = value;
            }
        }

        /// <summary>
        /// Gets the default preview height for Unity editors.
        /// </summary>
        public virtual float DefaultEditorPreviewHeight
        {
            get { return _defaultEditorPreviewHeight; }
            set
            {
                _defaultEditorPreviewHeight = value;
                var previewModule = GetModule<PreviewRenderingModule>();
                if (previewModule != null) previewModule.DefaultEditorPreviewHeight = value;
            }
        }

        /// <summary>
        /// Gets a module of the specified type
        /// </summary>
        /// <typeparam name="T">The module type</typeparam>
        /// <returns>The module instance or null if not found</returns>
        internal T GetModule<T>() where T : class, IEditorWindowModule
        {
            if (_modules != null && _modules.TryGetValue(typeof(T), out var module))
            {
                return module as T;
            }
            return null;
        }

        /// <summary>
        /// Internal method to set the serialized target
        /// </summary>
        internal void SetSerializedTarget(UnityEngine.Object target)
        {
            _serializedTarget = target;
        }

        /// <summary>
        /// Internal method to set the target object
        /// </summary>
        internal void SetTargetObject(object target)
        {
            _targetObject = target;
        }

        /// <summary>
        /// Gets the target which the window is supposed to draw.
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
        /// Gets the targets to be drawn by the editor window.
        /// </summary>
        protected internal virtual IEnumerable<object> GetTargets()
        {
            yield return GetTarget();
        }

        /// <summary>
        /// At the start of each OnGUI event when in the Layout event, the GetTargets() method is called and cached into a list.
        /// </summary>
        protected IReadOnlyList<object> CurrentDrawingTargets
        {
            get
            {
                var editorModule = GetModule<EditorManagementModule>();
                return editorModule?.CurrentTargets ?? new List<object>();
            }
        }

        /// <summary>
        /// Pops up an editor window for the given object in a drop-down window.
        /// </summary>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, Rect btnRect, float windowWidth)
        {
            return WindowManagementModule.InspectObjectInDropDown(obj, btnRect, windowWidth);
        }

        /// <summary>
        /// Pops up an editor window for the given object in a drop-down window.
        /// </summary>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, Rect btnRect, Vector2 windowSize)
        {
            return WindowManagementModule.InspectObjectInDropDown(obj, btnRect, windowSize);
        }

        /// <summary>
        /// Pops up an editor window for the given object in a drop-down window.
        /// </summary>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, Vector2 position)
        {
            return WindowManagementModule.InspectObjectInDropDown(obj, position);
        }

        /// <summary>
        /// Pops up an editor window for the given object in a drop-down window.
        /// </summary>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, float windowWidth)
        {
            return WindowManagementModule.InspectObjectInDropDown(obj, windowWidth);
        }

        /// <summary>
        /// Pops up an editor window for the given object in a drop-down window.
        /// </summary>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, Vector2 position, float windowWidth)
        {
            return WindowManagementModule.InspectObjectInDropDown(obj, position, windowWidth);
        }

        /// <summary>
        /// Pops up an editor window for the given object in a drop-down window.
        /// </summary>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, float width, float height)
        {
            return WindowManagementModule.InspectObjectInDropDown(obj, width, height);
        }

        /// <summary>
        /// Pops up an editor window for the given object in a drop-down window.
        /// </summary>
        public static EasyEditorWindow InspectObjectInDropDown(object obj)
        {
            return WindowManagementModule.InspectObjectInDropDown(obj);
        }

        /// <summary>
        /// Pops up an editor window for the given object.
        /// </summary>
        public static EasyEditorWindow InspectObject(object obj)
        {
            return WindowManagementModule.InspectObject(obj);
        }

        /// <summary>
        /// Inspects the object using an existing EasyEditorWindow.
        /// </summary>
        public static EasyEditorWindow InspectObject(EasyEditorWindow window, object obj)
        {
            var unityObject = obj as UnityEngine.Object;
            if (unityObject != null)
            {
                // If it's a Unity object, then it's likely the reference can survive a recompile.
                window._targetObject = null;
                window._serializedTarget = unityObject;
            }
            else
            {
                // Otherwise, it can't. In which case we don't want want to serialize it - hence targetObject and not serializedTarget.
                window._serializedTarget = null;
                window._targetObject = obj;
            }

            SetWindowTitle(window, obj, unityObject);
            EditorUtility.SetDirty(window);
            return window;
        }

        /// <summary>
        /// Creates an editor window instance for the specified object, without opening the window.
        /// </summary>
        public static EasyEditorWindow CreateEasyEditorWindowInstanceForObject(object obj)
        {
            return WindowManagementModule.CreateEasyEditorWindowInstanceForObject(obj);
        }

        /// <summary>
        /// Sets the window title based on the object type
        /// </summary>
        private static void SetWindowTitle(EasyEditorWindow window, object obj, UnityEngine.Object unityObject)
        {
            if (unityObject is Component component)
            {
                window.titleContent = new GUIContent(component.gameObject.name);
            }
            else if (unityObject != null)
            {
                window.titleContent = new GUIContent(unityObject.name);
            }
            else
            {
                window.titleContent = new GUIContent(obj.ToString());
            }
        }

        /// <summary>
        /// Draws the Easy Editor Window.
        /// </summary>
        protected virtual void OnGUI()
        {
            GuiBegun?.Invoke();

            // Initialize modules if needed
            InitializeIfNeeded();

            // Update module properties with serialized values
            UpdateModuleProperties();

            // Let all modules handle their OnGUI logic
            foreach (var module in _modules.Values)
            {
                module.OnGUI();
            }

            GuiEnded?.Invoke();
        }

        /// <summary>
        /// Called when the window is enabled.
        /// </summary>
        protected virtual void OnEnable()
        {
            InitializeIfNeeded();
            foreach (var module in _modules.Values)
            {
                module.OnEnable();
            }
        }

        /// <summary>
        /// Called when the window is disabled/destroyed.
        /// </summary>
        protected virtual void OnDisable()
        {
            foreach (var module in _modules?.Values ?? Enumerable.Empty<IEditorWindowModule>())
            {
                module.OnDisable();
            }
        }

        /// <summary>
        /// Called when the window is destroyed.
        /// </summary>
        protected virtual void OnDestroy()
        {
            foreach (var module in _modules?.Values ?? Enumerable.Empty<IEditorWindowModule>())
            {
                module.Dispose();
            }
            _modules?.Clear();

            WindowClosed?.Invoke();
        }

        /// <summary>
        /// Initializes the window and its modules.
        /// </summary>
        private void InitializeIfNeeded()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;

                // Initialize modules
                InitializeModules();

                // Give it a better default name
                if (titleContent != null && titleContent.text == GetType().FullName)
                {
                    titleContent.text = GetType().Name;
                }

                Initialize();
            }
        }

        /// <summary>
        /// Initializes all modules
        /// </summary>
        private void InitializeModules()
        {
            _modules = new Dictionary<Type, IEditorWindowModule>();

            // Create and initialize modules
            var modules = new IEditorWindowModule[]
            {
                new EditorManagementModule(),
                new GUILayoutModule(),
                new InputHandlingModule(),
                new PreviewRenderingModule(),
                new WindowManagementModule()
            };

            foreach (var module in modules)
            {
                module.Initialize(this);
                _modules[module.GetType()] = module;
            }
        }

        /// <summary>
        /// Updates module properties with current serialized values
        /// </summary>
        private void UpdateModuleProperties()
        {
            var layoutModule = GetModule<GUILayoutModule>();
            if (layoutModule != null)
            {
                layoutModule.DefaultLabelWidth = _labelWidth;
                layoutModule.WindowPadding = _windowPadding;
                layoutModule.UseScrollView = _useScrollView;
                layoutModule.WrappedAreaMaxHeight = _wrappedAreaMaxHeight;
            }

            var previewModule = GetModule<PreviewRenderingModule>();
            if (previewModule != null)
            {
                previewModule.DrawUnityEditorPreview = _drawUnityEditorPreview;
                previewModule.DefaultEditorPreviewHeight = _defaultEditorPreviewHeight;
            }
        }

        /// <summary>
        /// Initialize get called by OnEnable and by OnGUI after assembly reloads.
        /// </summary>
        protected virtual void Initialize()
        {
            // Override in derived classes
        }

        /// <summary>
        /// Called before starting to draw all editors.
        /// </summary>
        protected internal virtual void OnBeginDrawEditors()
        {
            // Override in derived classes
        }

        /// <summary>
        /// Called after all editors have been drawn.
        /// </summary>
        protected internal virtual void OnEndDrawEditors()
        {
            // Override in derived classes
        }
    }
}
