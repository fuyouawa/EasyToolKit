using System;
using System.Reflection;
using EasyToolKit.Core;
using EasyToolKit.ThirdParty.OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Enhanced Unity editor that provides property tree-based inspector functionality
    /// with support for audio filter GUI integration and custom inspector drawing.
    /// </summary>
    [InitializeOnLoad]
    [CanEditMultipleObjects]
    public class EasyEditor : UnityEditor.Editor
    {
        // Static delegates for Unity's internal audio filter functionality
        private static Func<MonoBehaviour, int> s_getCustomFilterChannelCount;
        private static Func<MonoBehaviour, bool> s_haveAudioCallback;
        private static Action<object, MonoBehaviour> s_drawAudioFilterGUI;

        // Reflection state tracking
        private static bool s_hasReflectedAudioFilter;
        private static bool s_initialized = false;
        private static Type s_audioFilterGUIType;

        // Instance fields for property tree and audio filter GUI
        private PropertyTree _tree;
        private object _audioFilterGUIInstance;

        /// <summary>
        /// Gets the property tree for the current serialized object.
        /// Creates the tree if it doesn't exist.
        /// </summary>
        public PropertyTree Tree
        {
            get
            {
                if (_tree == null)
                {
                    try
                    {
                        _tree = PropertyTree.Create(serializedObject);
                    }
                    catch (ArgumentException e)
                    {
                        Debug.LogException(e);
                    }
                }

                return _tree;
            }
        }

        /// <summary>
        /// Gets or sets whether this editor is being used as an inline editor.
        /// When true, the MonoScript field will not be drawn.
        /// </summary>
        public bool IsInlineEditor { get; set; }

        /// <summary>
        /// Called by Unity to draw the inspector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            DrawInspector();
        }

        /// <summary>
        /// Called when the editor is enabled.
        /// Ensures the static initialization is performed.
        /// </summary>
        protected virtual void OnEnable()
        {
            EnsureInitialized();
        }

        /// <summary>
        /// Called when the editor is disabled.
        /// </summary>
        protected virtual void OnDisable()
        {
        }


        /// <summary>
        /// Ensures the static initialization is performed once.
        /// Uses reflection to access Unity's internal audio filter functionality.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (!s_initialized)
            {
                s_initialized = true;

                try
                {
                    // Determine the correct method name based on Unity version
                    string haveAudioCallbackName =
                        UnityVersionUtility.IsVersionOrGreater(5, 6) ? "HasAudioCallback" : "HaveAudioCallback";

                    // Create delegates for Unity's internal audio utility methods using reflection
                    s_haveAudioCallback = (Func<MonoBehaviour, bool>)Delegate.CreateDelegate(
                        typeof(Func<MonoBehaviour, bool>),
                        typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.AudioUtil")
                            .GetMethod(haveAudioCallbackName, BindingFlags.Public | BindingFlags.Static));

                    s_getCustomFilterChannelCount = (Func<MonoBehaviour, int>)Delegate.CreateDelegate(
                        typeof(Func<MonoBehaviour, int>),
                        typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.AudioUtil")
                            .GetMethod("GetCustomFilterChannelCount", BindingFlags.Public | BindingFlags.Static));

                    // Get the internal AudioFilterGUI type and create a delegate for its DrawAudioFilterGUI method
                    s_audioFilterGUIType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.AudioFilterGUI");
                    s_drawAudioFilterGUI = EmitUtilities.CreateWeakInstanceMethodCaller<MonoBehaviour>(
                        s_audioFilterGUIType.GetMethod("DrawAudioFilterGUI",
                            BindingFlags.Public | BindingFlags.Instance));
                    s_hasReflectedAudioFilter = true;
                }
                catch (Exception)
                {
                    // Log a warning if reflection fails due to Unity internal changes
                    Debug.LogWarning(
                        "The internal Unity class AudioFilterGUI has been changed; cannot properly mock a generic Unity inspector. This probably won't be very noticeable.");
                }
            }
        }

        /// <summary>
        /// Main method that draws the inspector GUI.
        /// Handles property tree drawing and audio filter GUI integration.
        /// </summary>
        private void DrawInspector()
        {
            // Fall back to default inspector if property tree creation failed
            if (Tree == null)
            {
                base.OnInspectorGUI();
                return;
            }

            // Configure MonoScript field visibility during layout phase
            if (Event.current.type == EventType.Layout)
            {
                Tree.DrawMonoScriptObjectField = Tree.SerializedObject != null &&
                                                 Tree.TargetType != null &&
                                                 InspectorConfigAsset.Instance.DrawMonoScriptInEditor &&
                                                 !IsInlineEditor;
            }

            // Draw the inspector within localization context
            using (new LocalizationGroup(target))
            {
                // Draw the main property tree
                DrawTree();

                // Draw audio filter GUI if the target is a MonoBehaviour with audio callbacks
                if (s_hasReflectedAudioFilter && this.target is MonoBehaviour)
                {
                    if (s_haveAudioCallback(this.target as MonoBehaviour) &&
                        s_getCustomFilterChannelCount(this.target as MonoBehaviour) > 0)
                    {
                        // Create audio filter GUI instance if it doesn't exist
                        if (this._audioFilterGUIInstance == null)
                        {
                            this._audioFilterGUIInstance = Activator.CreateInstance(s_audioFilterGUIType);
                        }

                        // Draw the audio filter GUI using the reflected delegate
                        s_drawAudioFilterGUI(this._audioFilterGUIInstance, this.target as MonoBehaviour);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the property tree.
        /// Can be overridden by derived classes to customize tree drawing behavior.
        /// </summary>
        protected virtual void DrawTree()
        {
            Tree.Draw();
        }
    }
}
