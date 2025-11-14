using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public class CompositeEraseTool : CompositeDraggableDrawingTool, IEasyEventTrigger
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
    }
}
