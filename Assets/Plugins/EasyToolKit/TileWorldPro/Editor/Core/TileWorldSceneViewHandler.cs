using UnityEditor;

namespace EasyToolKit.TileWorldPro.Editor
{
    public static class TileWorldSceneViewHandler
    {
        private static TileWorldDesigner _target;

        static TileWorldSceneViewHandler()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (_target == null || Selection.activeGameObject == _target.gameObject)
                return;

            DrawSceneGUI(_target);
        }

        public static void DrawSceneGUI(TileWorldDesigner target)
        {
            _target = target;

            if (target.TileWorldAsset == null || target.StartPoint == null)
                return;

            if (target.Settings.DrawDebugBase)
            {
                TileWorldHandles.DrawBase(target);
            }

            if (target.Settings.DrawMapDebugCube)
            {
                TileWorldHandles.DrawTileCubes(target);
            }

            // if (context.IsMarkingRuleType)
            // {
            //     foreach (var kvp in context.RuleTypeMapCache)
            //     {
            //         var tilePosition = kvp.Key;
            //         var ruleType = kvp.Value;
            //         var tileWorldPosition = TilemapUtility.TilePositionToWorldPosition(context.Target.transform.position, tilePosition, tileSize);
            //         TilemapHandles.DrawDebugRuleTypeGUI(tileWorldPosition, ruleType);
            //     }
            // }
        }
    }
}
