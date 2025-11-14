using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public class BeforeBuildTileEvent
    {
        public TileWorldBuilder Builder { get; }
        public ChunkObject ChunkObject { get; }
        public Guid TerrainGuid { get; set; }
        public ChunkTileCoordinate ChunkTileCoordinate { get; set; }
        public TerrainTileRuleType RuleType { get; set; }

        public BeforeBuildTileEvent(TileWorldBuilder builder, ChunkObject chunkObject, Guid terrainGuid, ChunkTileCoordinate chunkTileCoordinate, TerrainTileRuleType ruleType)
        {
            Builder = builder;
            ChunkObject = chunkObject;
            TerrainGuid = terrainGuid;
            ChunkTileCoordinate = chunkTileCoordinate;
            RuleType = ruleType;
        }
    }

    public class AfterBuildTileEvent
    {
        public TileWorldBuilder Builder { get; }
        public ChunkTerrainObject ChunkTerrainObject { get; }
        public ChunkTerrainTileInfo TileInfo { get; }

        public AfterBuildTileEvent(TileWorldBuilder builder, ChunkTerrainObject chunkTerrainObject, ChunkTerrainTileInfo tileInfo)
        {
            Builder = builder;
            ChunkTerrainObject = chunkTerrainObject;
            TileInfo = tileInfo;
        }
    }

    public interface ITileBuildProcessor
    {
        void OnBeforeBuildTile(BeforeBuildTileEvent e);
        void OnAfterBuildTile(AfterBuildTileEvent e);
    }
}
