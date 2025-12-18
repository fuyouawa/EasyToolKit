using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class EasyEditorWindow : EditorWindow
    {
        private const int DEFAULT_INSPECT_OBJECT_WINDOW_COUNT = 3;
        private const int DEFAULT_WRAPPED_AREA_MAX_HEIGHT = 1000;
        private const int DEFAULT_EDITOR_PREVIEW_HEIGHT = 170;
        private const int DRAW_WARMUP_COUNT = 10;
        private const float DEFAULT_LABEL_WIDTH_RATIO = 0.33f;
        private const int AUTOMATIC_HEIGHT_MAX_HEIGHT = 600;
        private const int AUTOMATIC_HEIGHT_SCREEN_MARGIN = 40;
        private const int AUTOMATIC_HEIGHT_FRAME_COUNT = 10;
        private const int COORDINATE_VALIDATION_FRAMES = 2;

        private static readonly Vector4 DefaultWindowPadding = new Vector4(4, 4, 4, 4);

        private static readonly object LockObject = new object();
        private static bool s_hasUpdatedEasyEditors;
        private static int s_inspectObjectWindowCounter;
        private static readonly System.Reflection.PropertyInfo MaterialForceVisibleProperty;

        static EasyEditorWindow()
        {
            MaterialForceVisibleProperty = typeof(MaterialEditor).GetProperty("forceVisible", BindingFlagsHelper.All);
        }

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
        private float _labelWidth = DEFAULT_LABEL_WIDTH_RATIO;

        [SerializeField, HideInInspector]
        private Vector4 _windowPadding = DefaultWindowPadding;

        [SerializeField, HideInInspector]
        private bool _useScrollView = true;

        [SerializeField, HideInInspector]
        private bool _drawUnityEditorPreview;

        [SerializeField, HideInInspector]
        private int _wrappedAreaMaxHeight = DEFAULT_WRAPPED_AREA_MAX_HEIGHT;

        private object _targetObject;
        private int _drawCountWarmup;
        private bool _isInitialized;
        private GUIStyle _marginStyle;
        private object[] _currentTargets = new object[0];
        private UnityEditor.Editor[] _editors = new UnityEditor.Editor[0];
        private PropertyTree[] _propertyTrees = new PropertyTree[0];
        private Vector2 _scrollPosition;
        private int _mouseDownControlId;
        private EditorWindow _mouseDownWindow;
        private int _mouseDownKeyboardControl;
        private Vector2 _contentSize;
        private float _defaultEditorPreviewHeight = DEFAULT_EDITOR_PREVIEW_HEIGHT;
        private bool _preventContentFromExpanding;

        /// <summary>
        /// Gets the label width to be used. Values between 0 and 1 are treated as percentages, and values above as pixels.
        /// </summary>
        public virtual float DefaultLabelWidth
        {
            get { return _labelWidth; }
            set { _labelWidth = value; }
        }

        /// <summary>
        /// Gets or sets the window padding. x = left, y = right, z = top, w = bottom.
        /// </summary>
        public virtual Vector4 WindowPadding
        {
            get { return _windowPadding; }
            set { _windowPadding = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the window should draw a scroll view.
        /// </summary>
        public virtual bool UseScrollView
        {
            get { return _useScrollView; }
            set { _useScrollView = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the window should draw a Unity editor preview, if possible.
        /// </summary>
        public virtual bool DrawUnityEditorPreview
        {
            get { return _drawUnityEditorPreview; }
            set { _drawUnityEditorPreview = value; }
        }

        /// <summary>
        /// Gets the default preview height for Unity editors.
        /// </summary>
        public virtual float DefaultEditorPreviewHeight
        {
            get { return _defaultEditorPreviewHeight; }
            set { _defaultEditorPreviewHeight = value; }
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
        protected IReadOnlyList<object> CurrentDrawingTargets => _currentTargets;

        /// <summary>
        /// <para>
        /// Pops up an editor window for the given object in a drop-down window which closes when it loses its focus.
        /// This particular overload uses a few frames to calculate the height of the content before showing the window with a height that matches its content.
        /// </para>
        /// <para>Protip: You can subscribe to OnClose if you want to know when that occurs.</para>
        /// </summary>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, Rect btnRect, float windowWidth)
        {
            return InspectObjectInDropDown(obj, btnRect, new Vector2(windowWidth, 0));
        }

        private void SetupAutomaticHeightAdjustment(int maxHeight)
        {
            _preventContentFromExpanding = true;
            _wrappedAreaMaxHeight = maxHeight;

            // TODO: The AUTOMATIC_HEIGHT_SCREEN_MARGIN pixels right now represents the bottom task bar on Windows.
            // We need a good way of getting screen a "real estate" rect.
            var screenHeight = Screen.currentResolution.height - AUTOMATIC_HEIGHT_SCREEN_MARGIN;
            var originalPosition = position;
            originalPosition.x = (int)originalPosition.x;
            originalPosition.y = (int)originalPosition.y;
            originalPosition.width = (int)originalPosition.width;
            originalPosition.height = (int)originalPosition.height;
            var currentPosition = originalPosition;
            var windowReference = this;
            var getGoodOriginalPointer = 0;
            var tempFrameCount = 0;

            EditorApplication.CallbackFunction callback = null;
            callback = () =>
            {
                EditorApplication.update -= callback;
                EditorApplication.update -= callback;

                if (windowReference == null)
                {
                    return;
                }

                if (tempFrameCount++ < AUTOMATIC_HEIGHT_FRAME_COUNT)
                {
                    windowReference.Repaint();
                }

                // In the first frame the x and y coordinates are zero, so we must wait a frame, unless it's not zero.
                if (getGoodOriginalPointer <= 1 && originalPosition.y < 1)
                {
                    getGoodOriginalPointer++;
                    originalPosition = position;
                }
                else
                {
                    var currentContentHeight = (int)_contentSize.y;
                    if (currentContentHeight != currentPosition.height)
                    {
                        tempFrameCount = 0;
                        currentPosition = originalPosition;
                        currentPosition.height = (int)Math.Min(currentContentHeight, maxHeight);
                        windowReference.minSize = new Vector2(windowReference.minSize.x, currentPosition.height);
                        windowReference.maxSize = new Vector2(windowReference.maxSize.x, currentPosition.height);
                        if (currentPosition.yMax >= screenHeight)
                        {
                            var delta = currentPosition.yMax - screenHeight;
                            currentPosition.y -= delta;
                        }
                        windowReference.position = currentPosition;
                    }
                }
                EditorApplication.update += callback;
            };
            EditorApplication.update += callback;
        }

        /// <summary>
        /// <para>
        /// Pops up an editor window for the given object in a drop-down window which closes when it loses its focus.
        /// </para>
        /// <para>Protip: You can subscribe to OnClose if you want to know when that occurs.</para>
        /// </summary>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, Rect btnRect, Vector2 windowSize)
        {
            var window = CreateEasyEditorWindowInstanceForObject(obj);

            if (windowSize.x <= 1) windowSize.x = btnRect.width;
            if (windowSize.x <= 1) windowSize.x = 400;

            // Having floating point values, can cause Unity's editor window to be transparent.
            btnRect.x = (int)btnRect.x;
            btnRect.width = (int)btnRect.width;
            btnRect.height = (int)btnRect.height;
            btnRect.y = (int)btnRect.y;
            windowSize.x = (int)windowSize.x;
            windowSize.y = (int)windowSize.y;

            try
            {
                // Also repaint parent window, when the drop down repaints.
                var currentWindow = EasyGUIHelper.CurrentWindow;
                if (currentWindow != null)
                {
                    window.GuiBegun += () => currentWindow.Repaint();
                }
            }
            catch
            {
            }

            // Draw lighter bg.
            if (!EditorGUIUtility.isProSkin)
            {
                window.GuiBegun += () => EasyEditorGUI.DrawSolidRect(new Rect(0, 0, window.position.width, window.position.height), EasyGUIStyles.MenuBackgroundColor);
            }

            // Draw borders.
            window.GuiEnded += () => EasyEditorGUI.DrawBorders(new Rect(0, 0, window.position.width, window.position.height), 1);
            window.DefaultLabelWidth = DEFAULT_LABEL_WIDTH_RATIO;
            window.DrawUnityEditorPreview = true;
            btnRect.position = GUIUtility.GUIToScreenPoint(btnRect.position);

            if ((int)windowSize.y == 0)
            {
                window.ShowAsDropDown(btnRect, new Vector2(windowSize.x, 10));
                window.SetupAutomaticHeightAdjustment(AUTOMATIC_HEIGHT_MAX_HEIGHT);
            }
            else
            {
                window.ShowAsDropDown(btnRect, windowSize);
            }

            return window;
        }

        /// <summary>
        /// <para>
        /// Pops up an editor window for the given object in a drop-down window which closes when it loses its focus.
        /// </para>
        /// <para>Protip: You can subscribe to OnClose if you want to know when that occurs.</para>
        /// </summary>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, Vector2 position)
        {
            var btnRect = new Rect(position.x, position.y, 1, 1);
            return InspectObjectInDropDown(obj, btnRect, 350);
        }

        /// <summary>
        /// <para>
        /// Pops up an editor window for the given object in a drop-down window which closes when it loses its focus.
        /// </para>
        /// <para>Protip: You can subscribe to OnClose if you want to know when that occurs.</para>
        /// </summary>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, float windowWidth)
        {
            var position = Event.current.mousePosition;
            var btnRect = new Rect(position.x, position.y, 1, 1);
            return InspectObjectInDropDown(obj, btnRect, windowWidth);
        }

        /// <summary>
        /// <para>
        /// Pops up an editor window for the given object in a drop-down window which closes when it loses its focus.
        /// </para>
        /// <para>Protip: You can subscribe to OnClose if you want to know when that occurs.</para>
        /// </summary>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, Vector2 position, float windowWidth)
        {
            var btnRect = new Rect(position.x, position.y, 1, 1);
            return InspectObjectInDropDown(obj, btnRect, windowWidth);
        }

        /// <summary>
        /// <para>
        /// Pops up an editor window for the given object in a drop-down window which closes when it loses its focus.
        /// </para>
        /// <para>Protip: You can subscribe to OnClose if you want to know when that occurs.</para>
        /// </summary>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, float width, float height)
        {
            var r = new Rect(Event.current.mousePosition, Vector2.one);
            return InspectObjectInDropDown(obj, r, new Vector2(width, height));
        }

        /// <summary>
        /// <para>
        /// Pops up an editor window for the given object in a drop-down window which closes when it loses its focus.
        /// </para>
        /// <para>Protip: You can subscribe to OnClose if you want to know when that occurs.</para>
        /// </summary>
        public static EasyEditorWindow InspectObjectInDropDown(object obj)
        {
            return InspectObjectInDropDown(obj, Event.current.mousePosition);
        }

        /// <summary>
        /// Pops up an editor window for the given object.
        /// </summary>
        public static EasyEditorWindow InspectObject(object obj)
        {
            var window = CreateEasyEditorWindowInstanceForObject(obj);
            window.Show();

            lock (LockObject)
            {
                var offset = new Vector2(30, 30) * ((s_inspectObjectWindowCounter++ % 6) - 3);
                window.position = EasyGUIHelper.GetEditorWindowRect()
                    .AlignCenter(400, 300)
                    .AddPosition(offset);
            }

            return window;
        }

        /// <summary>
        /// Inspects the object using an existing EasyEditorWindow.
        /// </summary>
        public static EasyEditorWindow InspectObject(EasyEditorWindow window, object obj)
        {
            var unityObject = obj as UnityEngine.Object;
            if (unityObject)
            {
                // If it's a Unity object, then it's likely the reference can survive a recompile.
                window._targetObject = null;
                window._serializedTarget = unityObject;
            }
            else
            {
                // Otherwise, it can't. In which case we don't want want to serialize it - hence targetObject and not serializedTarget.
                // If we did the user would be inspecting a different reference than provided.
                window._serializedTarget = null;
                window._targetObject = obj;
            }

            if (unityObject as Component)
            {
                window.titleContent = new GUIContent((unityObject as Component).gameObject.name);
            }
            else if (unityObject)
            {
                window.titleContent = new GUIContent(unityObject.name);
            }
            else
            {
                window.titleContent = new GUIContent(obj.ToString());
            }

            EditorUtility.SetDirty(window);
            return window;
        }

        /// <summary>
        /// Creates an editor window instance for the specified object, without opening the window.
        /// </summary>
        public static EasyEditorWindow CreateEasyEditorWindowInstanceForObject(object obj)
        {
            var window = CreateInstance<EasyEditorWindow>();

            // In Unity version 2017.3+ the new window doesn't recive focus on the first click if something from another window has focus.
            GUIUtility.hotControl = 0;
            GUIUtility.keyboardControl = 0;

            var unityObject = obj as UnityEngine.Object;
            if (unityObject)
            {
                // If it's a Unity object, then it's likely the reference can survive a recompile.
                window._serializedTarget = unityObject;
            }
            else
            {
                // Otherwise, it can't. In which case we don't want want to serialize it - targetObject and not serializedTarget.
                // If we did the user would be inspecting a different reference than provided.
                window._targetObject = obj;
            }

            if (unityObject as Component)
            {
                window.titleContent = new GUIContent((unityObject as Component).gameObject.name);
            }
            else if (unityObject)
            {
                window.titleContent = new GUIContent(unityObject.name);
            }
            else
            {
                window.titleContent = new GUIContent(obj.ToString());
            }

            window.position = EasyGUIHelper.GetEditorWindowRect().AlignCenter(600, 600);

            EditorUtility.SetDirty(window);
            return window;
        }

        /// <summary>
        /// Draws the Easy Editor Window.
        /// </summary>
        protected virtual void OnGUI()
        {
            bool measureArea = _preventContentFromExpanding;
            if (measureArea)
            {
                GUILayout.BeginArea(new Rect(0, 0, position.width, _wrappedAreaMaxHeight));
            }

            GuiBegun?.Invoke();

            // Editor windows, can be created before Easy assigns EasyEditors to all relevent types via reflection.
            // This ensures that that happens before we render anything.
            if (!s_hasUpdatedEasyEditors)
            {
                InspectorConfigAsset.Instance.EnsureEditorsHaveBeenUpdated();
                s_hasUpdatedEasyEditors = true;
            }

            InitializeIfNeeded();

            _marginStyle = _marginStyle ?? new GUIStyle() { padding = new RectOffset() };

            if (Event.current.type == EventType.Layout)
            {
                _marginStyle.padding.left = (int)WindowPadding.x;
                _marginStyle.padding.right = (int)WindowPadding.y;
                _marginStyle.padding.top = (int)WindowPadding.z;
                _marginStyle.padding.bottom = (int)WindowPadding.w;

                // Creates the editors.
                UpdateEditors();
            }

            // Removes focus from text-fields when clicking on an empty area.
            var previousEventType = Event.current.type;
            if (Event.current.type == EventType.MouseDown)
            {
                _mouseDownControlId = GUIUtility.hotControl;
                _mouseDownKeyboardControl = GUIUtility.keyboardControl;
                _mouseDownWindow = focusedWindow;
            }

            // Draws the editors.
            bool useScrollView = UseScrollView;
            if (useScrollView)
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            }

            // Draw the GUI
            Vector2 size;
            if (_preventContentFromExpanding)
            {
                size = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false)).size;
            }
            else
            {
                size = EditorGUILayout.BeginVertical().size;
            }

            if (_contentSize == Vector2.zero || Event.current.type == EventType.Repaint)
            {
                _contentSize = size;
            }

            EasyGUIHelper.PushHierarchyMode(false);
            float calculatedLabelWidth;
            if (DefaultLabelWidth < 1)
            {
                calculatedLabelWidth = _contentSize.x * DefaultLabelWidth;
            }
            else
            {
                calculatedLabelWidth = DefaultLabelWidth;
            }

            EasyGUIHelper.PushLabelWidth(calculatedLabelWidth);
            OnBeginDrawEditors();
            GUILayout.BeginVertical(_marginStyle);

            DrawEditors();

            GUILayout.EndVertical();
            OnEndDrawEditors();
            EasyGUIHelper.PopLabelWidth();
            EasyGUIHelper.PopHierarchyMode();

            EditorGUILayout.EndVertical();

            if (useScrollView)
            {
                EditorGUILayout.EndScrollView();
            }

            GuiEnded?.Invoke();

            // This removes focus from text-fields when clicking on an empty area.
            if (Event.current.type != previousEventType) _mouseDownControlId = -2;
            if (Event.current.type == EventType.MouseUp && GUIUtility.hotControl == _mouseDownControlId && focusedWindow == _mouseDownWindow && GUIUtility.keyboardControl == _mouseDownKeyboardControl)
            {
                EasyGUIHelper.RemoveFocusControl();
                GUI.FocusControl(null);
            }

            if (_drawCountWarmup < DRAW_WARMUP_COUNT)
            {
                Repaint();

                if (Event.current.type == EventType.Repaint)
                {
                    _drawCountWarmup++;
                }
            }

            // TODO: Find out why the window doesn't repaint properly when this is 0. And then remove this if statement.
            // Try navigating a menu tree with the keyboard filled menu items with nothing to inspect.
            // It only updates when you start moving the mouse.
            if (Event.current.isMouse || Event.current.type == EventType.Used || _currentTargets == null || _currentTargets.Length == 0 || EasyGUIHelper.CurrentWindowHasFocus)
            {
                Repaint();
            }

            //TODO Not yet realized: RepaintIfRequested();

            if (measureArea)
            {
                GUILayout.EndArea();
            }
        }

        /// <summary>
        /// Calls DrawEditor(index) for each of the currently drawing targets.
        /// </summary>
        protected virtual void DrawEditors()
        {
            for (int i = 0; i < _currentTargets.Length; i++)
            {
                DrawEditor(i);
            }
        }

        private void UpdateEditors()
        {
            _currentTargets = _currentTargets ?? new object[] { };
            _editors = _editors ?? new UnityEditor.Editor[] { };
            _propertyTrees = _propertyTrees ?? new PropertyTree[] { };

            IList<object> newTargets = GetTargets().ToArray() ?? new object[0];

            if (_currentTargets.Length != newTargets.Count)
            {
                if (_editors.Length > newTargets.Count)
                {
                    var toDestroy = _editors.Length - newTargets.Count;
                    for (int i = 0; i < toDestroy; i++)
                    {
                        var e = _editors[_editors.Length - i - 1];
                        if (e) DestroyImmediate(e);
                    }
                }

                if (_propertyTrees.Length > newTargets.Count)
                {
                    var toDestroy = _propertyTrees.Length - newTargets.Count;
                    for (int i = 0; i < toDestroy; i++)
                    {
                        var e = _propertyTrees[_propertyTrees.Length - i - 1];
                        if (e != null) e.Dispose();
                    }
                }

                Array.Resize(ref _currentTargets, newTargets.Count);
                Array.Resize(ref _editors, newTargets.Count);
                Array.Resize(ref _propertyTrees, newTargets.Count);
                Repaint();
            }

            for (int i = 0; i < newTargets.Count; i++)
            {
                var newTarget = newTargets[i];
                var curTarget = _currentTargets[i];
                if (!object.ReferenceEquals(newTarget, curTarget))
                {
                    EasyGUIHelper.RequestRepaint();
                    _currentTargets[i] = newTarget;

                    if (newTarget == null)
                    {
                        if (_propertyTrees[i] != null) _propertyTrees[i].Dispose();
                        _propertyTrees[i] = null;
                        if (_editors[i]) DestroyImmediate(_editors[i]);
                        _editors[i] = null;
                    }
                    else
                    {
                        var editorWindow = newTarget as EditorWindow;
                        if (newTarget.GetType().IsInheritsFrom<UnityEngine.Object>() && !editorWindow)
                        {
                            var unityObject = newTarget as UnityEngine.Object;
                            if (unityObject)
                            {
                                if (_propertyTrees[i] != null) _propertyTrees[i].Dispose();
                                _propertyTrees[i] = null;
                                if (_editors[i]) DestroyImmediate(_editors[i]);
                                _editors[i] = UnityEditor.Editor.CreateEditor(unityObject);
                                var materialEditor = _editors[i] as MaterialEditor;
                                if (materialEditor != null && MaterialForceVisibleProperty != null)
                                {
                                    MaterialForceVisibleProperty.SetValue(materialEditor, true, null);
                                }
                            }
                            else
                            {
                                if (_propertyTrees[i] != null) _propertyTrees[i].Dispose();
                                _propertyTrees[i] = null;
                                if (_editors[i]) DestroyImmediate(_editors[i]);
                                _editors[i] = null;
                            }
                        }
                        else
                        {
                            if (_propertyTrees[i] != null) _propertyTrees[i].Dispose();
                            if (_editors[i]) DestroyImmediate(_editors[i]);
                            _editors[i] = null;

                            if (newTarget is System.Collections.IList)
                            {
                                _propertyTrees[i] = PropertyTree.Create((newTarget as System.Collections.IList).Cast<object>().ToArray(), null);
                            } else
                            {
                                _propertyTrees[i] = PropertyTree.Create(new []{ newTarget }, null);
                            }
                        }
                    }
                }
            }
        }

        private void InitializeIfNeeded()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;

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
            var tmpPropertyTree = _propertyTrees[index];
            var tmpEditor = _editors[index];

            if (tmpPropertyTree != null || (tmpEditor != null && tmpEditor.target != null))
            {
                if (tmpPropertyTree != null)
                {
                    //TODO Not yet realized: withUndo argument for PropertyTree.Draw
                    // bool withUndo = tmpPropertyTree.Targets.FirstOrDefault() as UnityEngine.Object;
                    // tmpPropertyTree.Draw(withUndo);
                    tmpPropertyTree.Draw();
                }
                else
                {
                    if (tmpEditor is EasyEditor easyEditor)
                    {
                        easyEditor.IsInlineEditor = true;
                    }
                    tmpEditor.OnInspectorGUI();
                }
            }

            if (DrawUnityEditorPreview)
            {
                DrawEditorPreview(index, _defaultEditorPreviewHeight);
            }
        }

        /// <summary>
        /// Uses the <see cref="UnityEditor.Editor.DrawPreview(Rect)"/> method to draw a preview for the CurrentDrawingTargets[index].
        /// </summary>
        protected virtual void DrawEditorPreview(int index, float height)
        {
            UnityEditor.Editor editor = _editors[index];

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
            if (_editors != null)
            {
                for (int i = 0; i < _editors.Length; i++)
                {
                    if (_editors[i])
                    {
                        DestroyImmediate(_editors[i]);
                        _editors[i] = null;
                    }
                }
            }

            if (_propertyTrees != null)
            {
                for (int i = 0; i < _propertyTrees.Length; i++)
                {
                    if (_propertyTrees[i] != null)
                    {
                        _propertyTrees[i].Dispose();
                        _propertyTrees[i] = null;
                    }
                }
            }

            Selection.selectionChanged -= SelectionChanged;

            WindowClosed?.Invoke();
        }

        /// <summary>
        /// Called before starting to draw all editors for the <see cref="CurrentDrawingTargets"/>.
        /// </summary>
        protected virtual void OnEndDrawEditors()
        {
        }

        /// <summary>
        /// Called after all editors for the <see cref="CurrentDrawingTargets"/> has been drawn.
        /// </summary>
        protected virtual void OnBeginDrawEditors()
        {
        }
    }
}
