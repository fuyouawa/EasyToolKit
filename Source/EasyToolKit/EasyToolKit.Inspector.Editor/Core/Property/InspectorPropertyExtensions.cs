using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor.Internal;
using EasyToolKit.ThirdParty.OdinSerializer;
using System;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Provides extension methods for <see cref="InspectorProperty"/> to simplify common operations.
    /// </summary>
    public static class InspectorPropertyExtensions
    {
        /// <summary>
        /// Draws the property in the inspector with a custom label and optional tooltip.
        /// </summary>
        /// <param name="property">The <see cref="InspectorProperty"/> to draw.</param>
        /// <param name="label">The custom label to display.</param>
        /// <param name="tooltip">Optional tooltip text for the property.</param>
        public static void Draw(this InspectorProperty property, string label, string tooltip = null)
        {
            property.Draw(EditorHelper.TempContent(label, tooltip));
        }

        /// <summary>
        /// Gets a persistent context value for the property that persists across Unity sessions.
        /// </summary>
        /// <typeparam name="T">The type of the persistent value.</typeparam>
        /// <param name="property">The <see cref="InspectorProperty"/> to get persistent context for.</param>
        /// <param name="key">The key used to identify the persistent value.</param>
        /// <param name="defaultValue">The default value to use if no persistent value exists.</param>
        /// <returns>A <see cref="LocalPersistentContext{T}"/> instance for the specified key.</returns>
        public static LocalPersistentContext<T> GetPersistentContext<T>(this InspectorProperty property, string key, T defaultValue = default)
        {
            var key1 = TwoWaySerializationBinder.Default.BindToName(property.Tree.TargetType);
            var key2 = property.Path;

            return PersistentContext.GetLocal(string.Join("+", key1, key2, key), defaultValue);
        }
    }
}
