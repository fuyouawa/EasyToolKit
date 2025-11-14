using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [ShowOdinSerializedPropertiesInInspector]
    [CreateAssetMenu(menuName = "EasyToolKit/TileWorldPro/TileWorld", fileName = "TileWorld")]
    public class TileWorldAsset : SerializedScriptableObject
    {
        [LabelText("地基范围")]
        [SerializeField] private Vector2Int _baseRange = new Vector2Int(20, 20);

        [LabelText("瓦片大小")]
        [SerializeField] private float _tileSize = 1f;

        [LabelText("世界区块大小")]
        [ReadOnly]
        [SerializeField] private Vector2Int _chunkSize;

        [HideLabel]
        [OdinSerialize] private TerrainDefinitionSet _terrainDefinitionSet;

        [Required]
        [OdinSerialize] private ITileWorldDataStore _dataStore;

        [SerializeField, HideInInspector] private bool _isInitialized = false;

        public Vector2Int BaseRange => _baseRange;
        public float TileSize => _tileSize;
        public Vector2Int ChunkSize
        {
            get
            {
                EnsureInitialized();
                return _chunkSize;
            }
        }
        public TerrainDefinitionSet TerrainDefinitionSet => _terrainDefinitionSet;
        public ITileWorldDataStore DataStore
        {
            get => _dataStore;
            set => _dataStore = value;
        }

        public IEnumerable<Chunk> EnumerateChunks()
        {
            if (_dataStore == null || !_dataStore.IsValid)
            {
                return Enumerable.Empty<Chunk>();
            }

            return _dataStore.EnumerateChunks();
        }

        public IEnumerable<BakedChunk> EnumerateBakedChunks()
        {
            if (_dataStore == null || !_dataStore.IsValid)
            {
                return Enumerable.Empty<BakedChunk>();
            }

            return _dataStore.EnumerateBakedChunks();
        }

        public bool TryGetChunkAt(TileCoordinate tileCoordinate, out Chunk chunk)
        {
            if (_dataStore == null || !_dataStore.IsValid)
            {
                throw new InvalidOperationException("The data store is not valid");
            }
            EnsureInitialized();

            var chunkIndex = tileCoordinate.ToChunkCoordinate(_chunkSize);
            if (_dataStore.TryGetChunk(chunkIndex, out chunk))
            {
                return true;
            }

            chunk = null;
            return false;
        }

        public Chunk GetOrCreateChunkAt(TileCoordinate tileCoordinate)
        {
            if (TryGetChunkAt(tileCoordinate, out var chunk))
            {
                return chunk;
            }

            var chunkIndex = tileCoordinate.ToChunkCoordinate(_chunkSize);
            var area = new ChunkArea(chunkIndex, _chunkSize);
            chunk = new Chunk(area);
            _dataStore.UpdateChunk(chunk);
            return chunk;
        }

        //TODO update chunk
        public void SetTilesAt(IReadOnlyList<TileCoordinate> tileCoordinates, Guid terrainGuid)
        {
            if (_dataStore == null || !_dataStore.IsValid)
            {
                throw new InvalidOperationException("The data store is not valid");
            }

            var affectedChunks = new List<Chunk>();
            Chunk currentChunk = null;
            var tiles = new List<ChunkTileCoordinate>();
            foreach (var tilePosition in tileCoordinates)
            {
                if (currentChunk == null || !currentChunk.Area.Contains(tilePosition))
                {
                    ApplyTiles();
                    currentChunk = GetOrCreateChunkAt(tilePosition);
                    affectedChunks.Add(currentChunk);
                }

                tiles.Add(currentChunk.Area.TileCoordinateToChunkTileCoordinate(tilePosition));
            }
            ApplyTiles();

            _dataStore.UpdateChunkRange(affectedChunks);

            void ApplyTiles()
            {
                if (currentChunk != null && tiles.Count > 0)
                {
                    currentChunk.SetTilesAt(tiles, terrainGuid);
                    tiles.Clear();
                }
            }
        }

        public void RemoveTilesAt(IReadOnlyList<TileCoordinate> tileCoordinates, Guid terrainGuid)
        {
            if (_dataStore == null || !_dataStore.IsValid)
            {
                throw new InvalidOperationException("The data store is not valid");
            }

            var affectedChunks = new List<Chunk>();
            Chunk currentChunk = null;
            var tiles = new List<ChunkTileCoordinate>();
            foreach (var tilePosition in tileCoordinates)
            {
                if (currentChunk == null || !currentChunk.Area.Contains(tilePosition))
                {
                    ApplyTiles();
                    currentChunk = GetOrCreateChunkAt(tilePosition);
                    affectedChunks.Add(currentChunk);
                }

                tiles.Add(currentChunk.Area.TileCoordinateToChunkTileCoordinate(tilePosition));
            }
            ApplyTiles();

            _dataStore.UpdateChunkRange(affectedChunks);
            void ApplyTiles()
            {
                if (currentChunk != null && tiles.Count > 0)
                {
                    currentChunk.RemoveTilesAt(tiles, terrainGuid);
                    tiles.Clear();
                }
            }
        }

        private bool[,] GetSudokuAt(Guid terrainGuid, TileCoordinate tileCoordinate)
        {
            var sudoku = new bool[3, 3];

            Chunk currentChunk = null;
            Do(0, 2, tileCoordinate + TileCoordinate.ForwardLeft);
            Do(1, 2, tileCoordinate + TileCoordinate.Forward);
            Do(2, 2, tileCoordinate + TileCoordinate.ForwardRight);

            Do(0, 1, tileCoordinate + TileCoordinate.Left);
            sudoku[1, 1] = true;
            Do(2, 1, tileCoordinate + TileCoordinate.Right);

            Do(0, 0, tileCoordinate + TileCoordinate.BackLeft);
            Do(1, 0, tileCoordinate + TileCoordinate.Back);
            Do(2, 0, tileCoordinate + TileCoordinate.BackRight);

            return sudoku;

            void Do(int x, int y, TileCoordinate coordinate)
            {
                if (!IsValidTilePosition(coordinate))
                    return;
                if (currentChunk == null || !currentChunk.Area.Contains(coordinate))
                {
                    if (!TryGetChunkAt(coordinate, out currentChunk))
                    {
                        sudoku[x, y] = false;
                        return;
                    }
                }

                if (currentChunk.TryGetTerrainGuidsAt(coordinate, out var terrainGuids))
                {
                    sudoku[x, y] = terrainGuids.Contains(terrainGuid);
                }
            }
        }

        public void ClearInvalidTerrains()
        {
            foreach (var chunk in EnumerateChunks().ToArray())
            {
                var invalidTerrainGuids = new List<Guid>();
                foreach (var terrainSection in chunk.TerrainSections)
                {
                    if (!_terrainDefinitionSet.TryGetByGuid(terrainSection.TerrainGuid, out _))
                    {
                        invalidTerrainGuids.Add(terrainSection.TerrainGuid);
                    }
                }

                if (invalidTerrainGuids.Count > 0)
                {
                    foreach (var invalidTerrainGuid in invalidTerrainGuids)
                    {
                        chunk.RemoveTerrain(invalidTerrainGuid);
                    }
                    _dataStore.UpdateChunk(chunk);
                }
            }
        }

        public TerrainTileRuleType CalculateRuleTypeOf(Guid terrainGuid, TerrainConfigAsset terrainConfigAsset, TileCoordinate tileCoordinate)
        {
            var sudoku = GetSudokuAt(terrainGuid, tileCoordinate);
            return terrainConfigAsset.GetRuleTypeBySudoku(sudoku);
        }

        private bool IsValidTilePosition(TileCoordinate tileCoordinate)
        {
            if (tileCoordinate.X < 0 ||
                tileCoordinate.Y < 0 ||
                tileCoordinate.Z < 0)
            {
                return false;
            }
            return true;
        }

        private void EnsureInitialized()
        {
            if (_isInitialized)
            {
                return;
            }

            _chunkSize = TileWorldConfigAsset.Instance.ChunkSize;
            _dataStore = TileWorldDataStoreUtility.GetDefaultDataStore();
            _isInitialized = true;
        }
    }
}
