using System;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor.Internal
{
    /// <summary>
    /// Manages the lifecycle of Unity Editors and ElementTrees for an EasyEditorWindow.
    /// </summary>
    internal interface IEditorLifecycleManager
    {
        /// <summary>
        /// Gets the current target objects being edited.
        /// </summary>
        IReadOnlyList<object> CurrentTargets { get; }

        /// <summary>
        /// Gets the Unity Editors for the current targets.
        /// </summary>
        IReadOnlyList<UnityEditor.Editor> Editors { get; }

        /// <summary>
        /// Gets the ElementTrees for the current targets.
        /// </summary>
        IReadOnlyList<IElementTree> ElementTrees { get; }

        /// <summary>
        /// Updates the editors to match the provided targets.
        /// </summary>
        /// <param name="targets">The target objects to create editors for.</param>
        void UpdateEditors(IEnumerable<object> targets);

        /// <summary>
        /// Destroys all editors and element trees.
        /// </summary>
        void DestroyAll();

        /// <summary>
        /// Occurs when editors have been updated and a repaint is requested.
        /// </summary>
        event Action EditorsUpdated;
    }
}
