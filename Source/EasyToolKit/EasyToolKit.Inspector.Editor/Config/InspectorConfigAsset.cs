using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Core.Editor.Internal;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace EasyToolKit.Inspector.Editor
{
    [ModuleEditorConfigsPath("Inspector")]
    public class InspectorConfigAsset : ScriptableObjectSingleton<InspectorConfigAsset>, ISerializationCallbackReceiver
    {
        [SerializeField] private bool _drawMonoScriptInEditor = true;
        [SerializeField] private bool _instantiateReferenceObjectIfNull = true;

        public bool DrawMonoScriptInEditor => _drawMonoScriptInEditor;
        public bool TryInstantiateReferenceObjectIfNull => _instantiateReferenceObjectIfNull;

        private bool _hasUpdatedEditorsOnce;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnityEditorEventUtility.DelayAction(UpdateEditors);
        }

        public void UpdateEditors()
        {
            var drawnTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsDefined<EasyInspectorAttribute>(inherit: true, includeInterface: true))
                .Where(type => type.IsSubclassOf(typeof(Component)) ||
                               type.IsSubclassOf(typeof(ScriptableObject)));

            foreach (var drawnType in drawnTypes)
            {
                CustomEditorUtility.SetCustomEditor(drawnType, typeof(EasyEditor), false, false);
            }

            EditorApplication.delayCall += () =>
            {
                Type inspectorWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
                Type activeEditorTrackerType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ActiveEditorTracker");

                if (inspectorWindowType != null && activeEditorTrackerType != null)
                {
                    var createTrackerMethod =
                        inspectorWindowType.GetMethod("CreateTracker", BindingFlagsHelper.AllInstance);
                    var trackerField = inspectorWindowType.GetField("m_Tracker", BindingFlagsHelper.AllInstance);
                    var forceRebuild =
                        activeEditorTrackerType.GetMethod("ForceRebuild", BindingFlagsHelper.AllInstance);

                    if (createTrackerMethod != null && trackerField != null && forceRebuild != null)
                    {
                        // 获取所有检查器窗口并强制重建
                        var windows = Resources.FindObjectsOfTypeAll(inspectorWindowType);

                        foreach (var window in windows)
                        {
                            createTrackerMethod.Invoke(window, null);
                            object tracker = trackerField.GetValue(window);
                            forceRebuild.Invoke(tracker, null);
                        }
                    }
                }
            };
            _hasUpdatedEditorsOnce = true;
        }

        public void EnsureEditorsHaveBeenUpdated()
        {
            if (!_hasUpdatedEditorsOnce)
            {
                UpdateEditors();
                _hasUpdatedEditorsOnce = true;
            }
        }
    }
}
