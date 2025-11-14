using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public class EraseTool : DraggableDrawingTool, IEasyEventTrigger
    {
        protected override void DoTiles(DrawingToolContext context, IReadOnlyList<TileCoordinate> tileCoordinates)
        {
            context.Designer.TileWorldAsset.RemoveTilesAt(tileCoordinates, context.TerrainDefinition.Guid);
            this.TriggerEvent(new RemoveTilesEvent(context.TerrainDefinition.Guid, tileCoordinates.ToArray()));
        }

        protected override Color GetHitColor(DrawingToolContext context)
        {
            return Color.red;
        }

        protected override bool FilterHitTile(DrawingToolContext context, TileCoordinate tileCoordinate)
        {
            if (context.Designer.TileWorldAsset.TryGetChunkAt(tileCoordinate, out Chunk chunk))
            {
                if (chunk.TryGetTerrainGuidsAt(tileCoordinate, out var terrainGuids))
                {
                    if (Array.FindIndex(terrainGuids, guid => guid == context.TerrainDefinition.Guid) != -1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
