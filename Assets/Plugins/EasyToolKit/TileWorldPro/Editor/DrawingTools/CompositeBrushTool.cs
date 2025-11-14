using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public class CompositeBrushTool : CompositeDraggableDrawingTool, IEasyEventTrigger
    {
        protected override void DoTiles(DrawingToolContext context, IReadOnlyList<TileCoordinate> tileCoordinates)
        {
            context.Designer.TileWorldAsset.SetTilesAt(tileCoordinates, context.TerrainDefinition.Guid);
            this.TriggerEvent(new SetTilesEvent(context.TerrainDefinition.Guid, tileCoordinates.ToArray()));
        }

        protected override Color GetHitColor(DrawingToolContext context)
        {
            return context.TerrainDefinition.DebugCubeColor;
        }
    }
}
