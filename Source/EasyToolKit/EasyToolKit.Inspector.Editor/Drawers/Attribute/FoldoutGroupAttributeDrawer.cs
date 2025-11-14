using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Attribute + 100)]
    public class FoldoutGroupAttributeDrawer : EasyGroupAttributeDrawer<FoldoutGroupAttribute>
    {
        private ICodeValueResolver<string> _labelResolver;

        protected override void Initialize()
        {
            var targetType = this.GetTargetTypeForResolver();
            _labelResolver = CodeValueResolver.Create<string>(Attribute.Label, targetType, true);
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (_labelResolver.HasError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            base.DrawProperty(label);
        }

        protected override void BeginDrawProperty(GUIContent label, ref bool foldout)
        {
            var labelText = _labelResolver.Resolve(this.GetTargetForResolver());
            Property.State.Expanded = EasyEditorGUI.Foldout(Property.State.Expanded, EditorHelper.TempContent(labelText));

            foldout = Property.State.Expanded;
            EditorGUI.indentLevel++;
        }

        protected override void EndDrawProperty()
        {
            EditorGUI.indentLevel--;
        }
    }
}
