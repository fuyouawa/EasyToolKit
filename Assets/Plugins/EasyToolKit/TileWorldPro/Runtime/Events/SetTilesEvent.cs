using System;

namespace EasyToolKit.TileWorldPro
{
    public class SetTilesEvent
    {
        public Guid TerrainGuid;
        public TileCoordinate[] TilePositions;

        public SetTilesEvent(Guid terrainGuid, TileCoordinate[] tilePositions)
        {
            TerrainGuid = terrainGuid;
            TilePositions = tilePositions;
        }
    }
}