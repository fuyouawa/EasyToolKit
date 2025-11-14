using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public class CompositeRectangleBrushTool : CompositeBrushTool
    {
        protected override Vector3 AdjustTilePosition(DrawingToolContext context, IReadOnlyList<TileCoordinate> dragTileCoordinatePath)
        {
            var newTileWorldPosition = base.AdjustTilePosition(context, dragTileCoordinatePath);
            if (dragTileCoordinatePath.Count < 2)
            {
                return newTileWorldPosition;
            }

            var startTilePosition = dragTileCoordinatePath.First();
            var startTileWorldPosition = context.Designer.StartPoint.TileCoordinateToPosition(startTilePosition, context.Designer.TileWorldAsset.TileSize);
            return newTileWorldPosition.SetY(startTileWorldPosition.y);
        }

        protected override IReadOnlyList<TileCoordinate> GetDrawingTileCoordinates(
            DrawingToolContext context,
            IReadOnlyList<TileCoordinate> dragTileCoordinatePath)
        {
            if (dragTileCoordinatePath.Count < 2)
            {
                return dragTileCoordinatePath;
            }

            var startTilePosition = dragTileCoordinatePath.First();
            var endTilePosition = context.Designer.StartPoint.PositionToTileCoordinate(context.HitTilePosition, context.Designer.TileWorldAsset.TileSize);

            var tilePositions = new List<TileCoordinate>();

            var startX = Mathf.Min(startTilePosition.X, endTilePosition.X);
            var endX = Mathf.Max(startTilePosition.X, endTilePosition.X);
            var startZ = Mathf.Min(startTilePosition.Z, endTilePosition.Z);
            var endZ = Mathf.Max(startTilePosition.Z, endTilePosition.Z);

            for (int x = startX; x <= endX; x++)
            {
                for (int z = startZ; z <= endZ; z++)
                {
                    tilePositions.Add(new TileCoordinate(x, startTilePosition.Y, z));
                }
            }
            return tilePositions;
        }
    }
}
