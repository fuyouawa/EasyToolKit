using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolKit.TileWorldPro
{
    public class BakedChunk
    {
        public class TerrainSection
        {
            public class Tile
            {
                private readonly ChunkTileCoordinate _tileCoordinate;
                private readonly TerrainTileRuleType _ruleType;
                private readonly bool _canBeHidden;

                public ChunkTileCoordinate TileCoordinate => _tileCoordinate;
                public TerrainTileRuleType RuleType => _ruleType;
                public bool CanBeHidden => _canBeHidden;

                public Tile(ChunkTileCoordinate tileCoordinate, TerrainTileRuleType ruleType, bool canBeHidden)
                {
                    _tileCoordinate = tileCoordinate;
                    _ruleType = ruleType;
                    _canBeHidden = canBeHidden;
                }
            }

            private readonly Guid _terrainGuid;
            private readonly Dictionary<ChunkTileCoordinate, Tile> _tiles;

            public Guid TerrainGuid => _terrainGuid;
            public IReadOnlyDictionary<ChunkTileCoordinate, Tile> Tiles => _tiles;

            public TerrainSection(Guid terrainGuid, IEnumerable<Tile> tiles)
            {
                _terrainGuid = terrainGuid;
                _tiles = new Dictionary<ChunkTileCoordinate, Tile>(tiles.ToDictionary(tile => tile.TileCoordinate));
            }
        }

        private readonly List<TerrainSection> _terrainSections;
        private readonly ChunkArea _area;

        public IReadOnlyList<TerrainSection> TerrainSections => _terrainSections;
        public ChunkArea Area => _area;

        public BakedChunk(ChunkArea area, IEnumerable<TerrainSection> terrainSections)
        {
            _area = area;
            _terrainSections = new List<TerrainSection>(terrainSections);
        }

        public int CalculateTilesCount()
        {
            return _terrainSections.Sum(section => section.Tiles.Count);
        }
    }
}