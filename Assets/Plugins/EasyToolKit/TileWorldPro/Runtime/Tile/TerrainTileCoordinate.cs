using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public struct TerrainTileCoordinate
    {
        public TileCoordinate TileCoordinate;
        public Guid TerrainGuid;

        public TerrainTileCoordinate(TileCoordinate tileCoordinate, Guid terrainGuid)
        {
            TileCoordinate = tileCoordinate;
            TerrainGuid = terrainGuid;
        }
    }
}
