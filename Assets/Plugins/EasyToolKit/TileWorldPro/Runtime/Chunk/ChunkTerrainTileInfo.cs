using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [Serializable]
    public class ChunkTerrainTileInfo
    {
        [SerializeField] private GameObject _instance;
        [SerializeField] private ChunkTileCoordinate chunkTileCoordinate;
        [SerializeField] private TerrainTileRuleType _ruleType;

        public ChunkTileCoordinate ChunkTileCoordinate => chunkTileCoordinate;

        public GameObject Instance
        {
            get => _instance;
            set => _instance = value;
        }

        public TerrainTileRuleType RuleType
        {
            get => _ruleType;
            set => _ruleType = value;
        }

        public ChunkTerrainTileInfo(GameObject tileInstance, ChunkTileCoordinate chunkTileCoordinate, TerrainTileRuleType ruleType)
        {
            _instance = tileInstance;
            this.chunkTileCoordinate = chunkTileCoordinate;
            _ruleType = ruleType;
        }
    }
}