using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public class BrushTool : DraggableDrawingTool, IEasyEventTrigger
    {
        private static readonly float Epsilon = 0.0001f;

        protected override Vector3 AdjustTilePosition(
            DrawingToolContext context,
            IReadOnlyList<TileCoordinate> dragTileCoordinatePath)
        {
            var tileSize = context.Designer.TileWorldAsset.TileSize;
            var tilePosition = context.Designer.StartPoint.PositionToTileCoordinate(context.HitTilePosition, tileSize);

            if (!context.Designer.TileWorldAsset.TryGetChunkAt(tilePosition, out var chunk))
            {
                return context.HitTilePosition;
            }
            if (!chunk.TryGetTerrainGuidsAt(tilePosition, out var terrainGuids))
            {
                return context.HitTilePosition;
            }

            var front = new Rect(
                context.HitTilePosition.x, context.HitTilePosition.y,
                tileSize, tileSize);

            if (context.HitPoint.z.IsApproximatelyOf(context.HitTilePosition.z, Epsilon) &&
                front.Contains(new Vector2(context.HitPoint.x, context.HitPoint.y)))
            {
                context.HitTilePosition.z -= tileSize;
                return context.HitTilePosition;
            }

            if (context.HitPoint.z.IsApproximatelyOf(context.HitTilePosition.z + 1, Epsilon) &&
                front.Contains(new Vector2(context.HitPoint.x, context.HitPoint.y)))
            {
                context.HitTilePosition.z += tileSize;
                return context.HitTilePosition;
            }

            var side = new Rect(
                context.HitTilePosition.z, context.HitTilePosition.y,
                tileSize, tileSize);

            if (context.HitPoint.x.IsApproximatelyOf(context.HitTilePosition.x, Epsilon) &&
                side.Contains(new Vector2(context.HitPoint.z, context.HitPoint.y)))
            {
                context.HitTilePosition.x -= tileSize;
                return context.HitTilePosition;
            }

            if (context.HitPoint.x.IsApproximatelyOf(context.HitTilePosition.x + 1, Epsilon) &&
                side.Contains(new Vector2(context.HitPoint.z, context.HitPoint.y)))
            {
                context.HitTilePosition.x += tileSize;
                return context.HitTilePosition;
            }

            var top = new Rect(
                context.HitTilePosition.x, context.HitTilePosition.z,
                tileSize, tileSize);

            if (context.HitPoint.y.IsApproximatelyOf(context.HitTilePosition.y, Epsilon) &&
                top.Contains(new Vector2(context.HitPoint.x, context.HitPoint.z)))
            {
                context.HitTilePosition.y -= tileSize;
                return context.HitTilePosition;
            }

            if (context.HitPoint.y.IsApproximatelyOf(context.HitTilePosition.y + 1, Epsilon) &&
                top.Contains(new Vector2(context.HitPoint.x, context.HitPoint.z)))
            {
                context.HitTilePosition.y += tileSize;
                return context.HitTilePosition;
            }

            return context.HitTilePosition;
        }

        protected override void DoTiles(DrawingToolContext context, IReadOnlyList<TileCoordinate> tileCoordinates)
        {
            context.Designer.TileWorldAsset.SetTilesAt(tileCoordinates, context.TerrainDefinition.Guid);
            this.TriggerEvent(new SetTilesEvent(context.TerrainDefinition.Guid, tileCoordinates.ToArray()));
        }

        protected override Color GetHitColor(DrawingToolContext context)
        {
            return context.TerrainDefinition.DebugCubeColor;
        }

        private TileCoordinate _previousTileCoordinate;

        protected override void DrawCursorCube(DrawingToolContext context, TileCoordinate tileCoordinate, Color hitColor)
        {
            base.DrawCursorCube(context, tileCoordinate, hitColor);

            if (_previousTileCoordinate == tileCoordinate)
            {
                return;
            }
            _previousTileCoordinate = tileCoordinate;

            var settings = context.Designer.Settings;
            if (!settings.DisplayPreviewBlock || settings.TerrainConfigAssetForPreview == null)
            {
                return;
            }

            var ruleType = context.Designer.TileWorldAsset.CalculateRuleTypeOf(
                context.TerrainDefinition.Guid,
                settings.TerrainConfigAssetForPreview,
                tileCoordinate);

            if (!context.TerrainDefinition.IsEmpty)
            {
                var previewBlock = context.Designer.GetOrCreatePreviewBlock(context.TerrainDefinition.Guid, ruleType);
                previewBlock.transform.position += context.HitTilePosition;
            }
        }

        protected override void Clear(TileWorldDesigner designer)
        {
            base.Clear(designer);
            designer.DestroyPreviewBlock();
        }
    }
}
