using System;

namespace EasyToolKit.TileWorldPro.Editor
{
    public abstract class CompositeDraggableDrawingTool : DraggableDrawingTool
    {
        protected override bool FilterHitTile(DrawingToolContext context, TileCoordinate tileCoordinate)
        {
            if (context.Designer.TileWorldAsset.TryGetChunkAt(tileCoordinate, out Chunk chunk))
            {
                if (chunk.TryGetTerrainGuidsAt(tileCoordinate, out _))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
