using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public class Chunk
    {
        public class TerrainSection
        {
            private Guid _terrainGuid;
            private HashSet<ChunkTileCoordinate> _tiles;
            private TerrainDefinition _terrainDefinitionCache;

            public Guid TerrainGuid => _terrainGuid;
            public HashSet<ChunkTileCoordinate> Tiles => _tiles;

            public TerrainSection(Guid terrainGuid)
            {
                _terrainGuid = terrainGuid;
                _tiles = new HashSet<ChunkTileCoordinate>();
            }

            public TerrainSection(Guid terrainGuid, IEnumerable<ChunkTileCoordinate> initialTiles)
            {
                _terrainGuid = terrainGuid;
                _tiles = new HashSet<ChunkTileCoordinate>(initialTiles);
            }

            public TerrainDefinition GetTerrainDefinitionCached(TerrainDefinitionSet terrainDefinitionSet)
            {
                if (_terrainDefinitionCache == null)
                {
                    _terrainDefinitionCache = terrainDefinitionSet.GetByGuid(_terrainGuid);
                }
                return _terrainDefinitionCache;
            }
        }

        private readonly ChunkArea _area;
        private readonly List<TerrainSection> _terrainSections;

        public ChunkArea Area => _area;
        public IReadOnlyList<TerrainSection> TerrainSections => _terrainSections;

        public Chunk(ChunkArea area)
        {
            _area = area;
            _terrainSections = new List<TerrainSection>();
        }

        public Chunk(ChunkArea area, IEnumerable<TerrainSection> initialTerrainSections)
        {
            _area = area;
            _terrainSections = new List<TerrainSection>(initialTerrainSections);
        }

        public bool TryGetTerrainGuidsAt(TileCoordinate tileCoordinate, out Guid[] terrainGuids)
        {
            var total = new List<Guid>();
            var chunkTilePosition = _area.TileCoordinateToChunkTileCoordinate(tileCoordinate);
            foreach (var terrainSection in _terrainSections)
            {
                if (terrainSection.Tiles.Contains(chunkTilePosition))
                {
                    total.Add(terrainSection.TerrainGuid);
                }
            }
            terrainGuids = total.ToArray();
            return total.Count > 0;
        }

        public void SetTilesAt(IReadOnlyList<ChunkTileCoordinate> chunkTilePositions, Guid terrainGuid)
        {
            var terrainSection = _terrainSections.FirstOrDefault(section => section.TerrainGuid == terrainGuid);
            if (terrainSection == null)
            {
                terrainSection = new TerrainSection(terrainGuid);
                _terrainSections.Add(terrainSection);
            }

            foreach (var chunkTilePosition in chunkTilePositions)
            {
                terrainSection.Tiles.Add(chunkTilePosition);
            }
        }

        public void RemoveTilesAt(IReadOnlyList<ChunkTileCoordinate> chunkTilePositions, Guid terrainGuid)
        {
            var terrainSection = _terrainSections.FirstOrDefault(section => section.TerrainGuid == terrainGuid);
            if (terrainSection == null)
            {
                return;
            }

            foreach (var chunkTilePosition in chunkTilePositions)
            {
                terrainSection.Tiles.Remove(chunkTilePosition);
            }
        }

        public bool RemoveTerrain(Guid terrainGuid)
        {
            var index = _terrainSections.FindIndex(section => section.TerrainGuid == terrainGuid);
            if (index == -1)
            {
                return false;
            }
            _terrainSections.RemoveAt(index);
            return true;
        }

        public int CalculateTilesCount()
        {
            return _terrainSections.Sum(section => section.Tiles.Count);
        }

        public IEnumerable<TerrainTileCoordinate> EnumerateTiles()
        {
            foreach (var terrainSection in _terrainSections)
            {
                foreach (var tile in terrainSection.Tiles)
                {
                    yield return new TerrainTileCoordinate(tile.ToTileCoordinate(_area), terrainSection.TerrainGuid);
                }
            }
        }

        public IEnumerable<TerrainTileCoordinate> EnumerateTerrainTiles(Guid terrainGuid)
        {
            var terrainSection = _terrainSections.FirstOrDefault(section => section.TerrainGuid == terrainGuid);
            if (terrainSection == null)
            {
                yield break;
            }

            foreach (var tile in terrainSection.Tiles)
            {
                yield return new TerrainTileCoordinate(tile.ToTileCoordinate(_area), terrainGuid);
            }
        }
    }
}
