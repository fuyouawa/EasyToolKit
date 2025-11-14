using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor.Internal;
using EasyToolKit.ThirdParty.OdinSerializer;
using System;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public static class InspectorPropertyExtensions
    {
        public static void Draw(this InspectorProperty property, string label, string tooltip = null)
        {
            property.Draw(EditorHelper.TempContent(label, tooltip));
        }

        public static LocalPersistentContext<T> GetPersistentContext<T>(this InspectorProperty property, string key, T defaultValue = default)
        {
            var key1 = TwoWaySerializationBinder.Default.BindToName(property.Tree.TargetType);
            var key2 = property.Path;

            return PersistentContext.GetLocal(string.Join("+", key1, key2, key), defaultValue);
        }
    }
}
