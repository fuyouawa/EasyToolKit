using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    [CustomEditor(typeof(TileWorldBuilder))]
    public class TileWorldBuilderEditor : EasyEditor
    {
        private TileWorldBuilder _target;

        protected override void OnEnable()
        {
            base.OnEnable();
            _target = (TileWorldBuilder)target;
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw();
            Tree.DrawProperties();
            EasyEditorGUI.Title("构建操作", textAlignment: TextAlignment.Center);
            if (GUILayout.Button("重新构建"))
            {
                _target.RebuildAll();
                EditorSceneManager.MarkSceneDirty(_target.gameObject.scene);
            }

            if (GUILayout.Button("清空构建瓦片"))
            {
                _target.ClearAll();
                EditorSceneManager.MarkSceneDirty(_target.gameObject.scene);
            }

            EasyEditorGUI.Title("烘培操作", textAlignment: TextAlignment.Center);

            if (GUILayout.Button("烘焙瓦片数据"))
            {
                _target.Bake();
                EditorUtility.SetDirty(_target);
            }

            Tree.EndDraw();
        }
    }
}
