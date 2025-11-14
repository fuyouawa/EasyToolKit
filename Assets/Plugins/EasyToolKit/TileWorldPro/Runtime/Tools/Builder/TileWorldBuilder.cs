using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EasyToolKit.TileWorldPro
{
    [ExecuteAlways]
    public class TileWorldBuilder : SerializedMonoBehaviour, IEasyEventListener<SetTilesEvent>, IEasyEventListener<RemoveTilesEvent>
    {
        [Required]
        [LabelText("起始点")]
        [SerializeField] private TileWorldStartPoint _startPoint;

        [FoldoutBoxGroup("设置")]
        [HideLabel]
        [SerializeField] private TileWorldBuilderSettings _settings;

        [Required]
        [EndFoldoutBoxGroup]
        [LabelText("资产")]
        [InlineEditor(Style = InlineEditorStyle.FoldoutBox)]
        [SerializeField] private TileWorldAsset _tileWorldAsset;

        [Required]
        [LabelText("地形配置")]
        [InlineEditor(Style = InlineEditorStyle.FoldoutBox)]
        [SerializeField] private TerrainConfigAsset _terrainConfigAsset;

        [HideLabel]
        [OdinSerialize, ShowInInspector] private TileBuildPipline _tileBuildPipline;

        private Dictionary<ChunkCoordinate, ChunkObject> _chunkObjects;

        public TileWorldStartPoint StartPoint => _startPoint;

        public TileWorldBuilderSettings Settings => _settings;

        public TileWorldAsset TileWorldAsset => _tileWorldAsset;

        public IReadOnlyDictionary<ChunkCoordinate, ChunkObject> ChunkObjects
        {
            get
            {
                if (_chunkObjects == null)
                {
                    _chunkObjects = transform.GetComponentsInChildren<ChunkObject>(true).ToDictionary(chunkObject => chunkObject.Area.Coordinate);
                }
                return _chunkObjects;
            }
        }

        public void RebuildAll()
        {
            ClearAll();
            BuildAll();
        }

        private void BuildAll()
        {
            foreach (var terrainDefinition in _tileWorldAsset.TerrainDefinitionSet)
            {
                BuildTerrain(terrainDefinition.Guid);
            }
        }

        public void RebuildTerrain(Guid terrainGuid)
        {
            ClearTerrain(terrainGuid);
            BuildTerrain(terrainGuid);
        }

        private void BuildTerrain(Guid terrainGuid)
        {
            var tileCoordinates = _tileWorldAsset
                    .EnumerateChunks()
                    .SelectMany(chunk => chunk.EnumerateTerrainTiles(terrainGuid))
                    .Select(tile => tile.TileCoordinate)
                    .ToArray();
            BuildTiles(terrainGuid, tileCoordinates, false);
        }

        public ChunkObject GetChunkObjectOf(ChunkCoordinate chunkCoordinate)
        {
            if (ChunkObjects.TryGetValue(chunkCoordinate, out var chunkObject) && chunkObject != null)
            {
                return chunkObject;
            }

            chunkObject = new GameObject($"Chunk_{chunkCoordinate}").AddComponent<ChunkObject>();
            chunkObject.transform.SetParent(transform);
            chunkObject.Initialize(this, new ChunkArea(chunkCoordinate, _tileWorldAsset.ChunkSize));
            _chunkObjects[chunkCoordinate] = chunkObject;
            return chunkObject;
        }

        public ChunkTerrainTileInfo TryGetTileInfoOf(Guid terrainGuid, TileCoordinate tileCoordinate)
        {
            var chunkObject = GetChunkObjectOf(tileCoordinate.ToChunkCoordinate(_tileWorldAsset.ChunkSize));
            var chunkTilePosition = chunkObject.Area.TileCoordinateToChunkTileCoordinate(tileCoordinate);
            return chunkObject.GetTerrainObject(terrainGuid).TryGetTileInfoOf(chunkTilePosition);
        }

        public void ClearAll(bool destroyChunkObject = true)
        {
            foreach (var chunkObject in ChunkObjects.Values)
            {
                if (destroyChunkObject)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(chunkObject.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(chunkObject.gameObject);
                    }
                }
                else
                {
                    chunkObject.ClearAll(false);
                }
            }

            if (destroyChunkObject)
            {
                _chunkObjects.Clear();
            }
        }

        public void ClearTerrain(Guid terrainGuid, bool destroyTerrainObject = true)
        {
            foreach (var chunkObject in ChunkObjects.Values)
            {
                chunkObject.ClearTerrain(terrainGuid, destroyTerrainObject);
            }
        }

        public void RebuildTiles(Guid terrainGuid, IReadOnlyList<TileCoordinate> tileCoordinates, bool rebuildAffectedTilesIfNeeded = true)
        {
            DestroyTiles(terrainGuid, tileCoordinates, false);
            BuildTiles(terrainGuid, tileCoordinates, rebuildAffectedTilesIfNeeded);
        }

        private void BuildTiles(Guid terrainGuid, IReadOnlyList<TileCoordinate> tileCoordinates, bool rebuildAffectedTilesIfNeeded = true)
        {
            var terrainDefinition = _tileWorldAsset.TerrainDefinitionSet.GetByGuid(terrainGuid);
            if (terrainDefinition.IsEmpty)
            {
                return;
            }

            var builtTileCoordinates = new List<TileCoordinate>(tileCoordinates);
            BuildTilesImpl(terrainGuid, tileCoordinates, ref builtTileCoordinates, rebuildAffectedTilesIfNeeded);

            ForEachTiles(terrainGuid, builtTileCoordinates, (tileCoordinate, chunkObject, terrainObject) =>
            {
                var chunkTilePosition = chunkObject.Area.TileCoordinateToChunkTileCoordinate(tileCoordinate);
                var tileInfo = terrainObject.TryGetTileInfoOf(chunkTilePosition);
                Assert.IsNotNull(tileInfo);
                var afterBuildTileEvent = new AfterBuildTileEvent(this, terrainObject, tileInfo);
                _tileBuildPipline.AfterInstantiateTile(afterBuildTileEvent);
            });
        }

        private void BuildTilesImpl(Guid terrainGuid, IReadOnlyList<TileCoordinate> tileCoordinates, ref List<TileCoordinate> affectedTileCoordinates, bool rebuildAffectedTilesIfNeeded = true)
        {
            ForEachTiles(terrainGuid, tileCoordinates, (tileCoordinate, chunkObject, terrainObject) =>
            {
                InstantiateTileAndAddToChunk(chunkObject, terrainGuid, tileCoordinate);
            });

            if (rebuildAffectedTilesIfNeeded)
            {
                List<TileCoordinate> allAffectedTileCoordinates = null;
                if (affectedTileCoordinates != null)
                {
                    allAffectedTileCoordinates = new List<TileCoordinate>();
                }
                RebuildAffectedTilesIfNeeded(terrainGuid, tileCoordinates, allAffectedTileCoordinates);

                if (affectedTileCoordinates != null)
                {
                    Assert.IsFalse(allAffectedTileCoordinates.HasDuplicate());
                    affectedTileCoordinates.AddRange(allAffectedTileCoordinates);
                }
            }
        }

        public void DestroyTiles(Guid terrainGuid, IReadOnlyList<TileCoordinate> tileCoordinates, bool rebuildAffectedTilesIfNeeded = true)
        {
            ForEachTiles(terrainGuid, tileCoordinates, (tileCoordinate, chunkObject, terrainObject) =>
            {
                var chunkTilePosition = chunkObject.Area.TileCoordinateToChunkTileCoordinate(tileCoordinate);
                terrainObject.DestroyTileAt(chunkTilePosition);
            });

            if (rebuildAffectedTilesIfNeeded)
            {
                RebuildAffectedTilesIfNeeded(terrainGuid, tileCoordinates);
            }
        }

        private void RebuildAffectedTilesIfNeeded(
            Guid terrainGuid,
            IReadOnlyList<TileCoordinate> tileCoordinates,
            List<TileCoordinate> allAffectedTileCoordinates = null,
            List<TileCoordinate> ignoredAffectedTileCoordinates = null)
        {
            var affectedTileCoordinates = new HashSet<TileCoordinate>();
            foreach (var tileCoordinate in tileCoordinates)
            {
                var coordinate = tileCoordinate + TileCoordinate.ForwardLeft;
                if (IsValidTileCoordinate(coordinate)) affectedTileCoordinates.Add(coordinate);
                coordinate = tileCoordinate + TileCoordinate.Forward;
                if (IsValidTileCoordinate(coordinate)) affectedTileCoordinates.Add(coordinate);
                coordinate = tileCoordinate + TileCoordinate.ForwardRight;
                if (IsValidTileCoordinate(coordinate)) affectedTileCoordinates.Add(coordinate);

                coordinate = tileCoordinate + TileCoordinate.Left;
                if (IsValidTileCoordinate(coordinate)) affectedTileCoordinates.Add(coordinate);
                coordinate = tileCoordinate + TileCoordinate.Right;
                if (IsValidTileCoordinate(coordinate)) affectedTileCoordinates.Add(coordinate);

                coordinate = tileCoordinate + TileCoordinate.BackLeft;
                if (IsValidTileCoordinate(coordinate)) affectedTileCoordinates.Add(coordinate);
                coordinate = tileCoordinate + TileCoordinate.Back;
                if (IsValidTileCoordinate(coordinate)) affectedTileCoordinates.Add(coordinate);
                coordinate = tileCoordinate + TileCoordinate.BackRight;
                if (IsValidTileCoordinate(coordinate)) affectedTileCoordinates.Add(coordinate);
            }

            foreach (var tileCoordinate in tileCoordinates)
            {
                affectedTileCoordinates.Remove(tileCoordinate);
            }

            if (ignoredAffectedTileCoordinates != null)
            {
                foreach (var tileCoordinate in ignoredAffectedTileCoordinates)
                {
                    affectedTileCoordinates.Remove(tileCoordinate);
                }
            }

            var changedTileCoordinates = new List<TileCoordinate>();

            ForEachTiles(terrainGuid, affectedTileCoordinates, (tileCoordinate, chunkObject, terrainObject) =>
            {
                var chunkTileCoordinate = chunkObject.Area.TileCoordinateToChunkTileCoordinate(tileCoordinate);
                var tileInfo = terrainObject.TryGetTileInfoOf(chunkTileCoordinate);
                if (tileInfo == null)
                {
                    return;
                }

                var ruleType = _tileWorldAsset.CalculateRuleTypeOf(terrainGuid, _terrainConfigAsset, tileCoordinate);
                if (tileInfo.RuleType == ruleType)
                {
                    return;
                }

                if (allAffectedTileCoordinates != null)
                {
                    allAffectedTileCoordinates.Add(tileCoordinate);
                }
                terrainObject.DestroyTileAt(chunkTileCoordinate);
                InstantiateTileAndAddToChunk(chunkObject, terrainGuid, chunkTileCoordinate, ruleType);
                changedTileCoordinates.Add(tileCoordinate);
            });

            if (changedTileCoordinates.Count > 0)
            {
                if (ignoredAffectedTileCoordinates == null)
                {
                    ignoredAffectedTileCoordinates = new List<TileCoordinate>();
                }
                ignoredAffectedTileCoordinates.AddRange(tileCoordinates);
                RebuildAffectedTilesIfNeeded(terrainGuid, changedTileCoordinates, ignoredAffectedTileCoordinates);
            }
        }

        private void InstantiateTileAndAddToChunk(ChunkObject chunkObject, Guid terrainGuid, TileCoordinate tileCoordinate)
        {
            var ruleType = _tileWorldAsset.CalculateRuleTypeOf(terrainGuid, _terrainConfigAsset, tileCoordinate);
            var chunkTilePosition = chunkObject.Area.TileCoordinateToChunkTileCoordinate(tileCoordinate);
            InstantiateTileAndAddToChunk(chunkObject, terrainGuid, chunkTilePosition, ruleType);
        }

        private void InstantiateTileAndAddToChunk(ChunkObject chunkObject, Guid terrainGuid, ChunkTileCoordinate chunkTileCoordinate, TerrainTileRuleType ruleType)
        {
            var terrainDefinition = _tileWorldAsset.TerrainDefinitionSet.GetByGuid(terrainGuid);
            if (terrainDefinition.IsEmpty)
            {
                return;
            }

            var beforeBuildTileEvent = new BeforeBuildTileEvent(this, chunkObject, terrainGuid, chunkTileCoordinate, ruleType);
            _tileBuildPipline.BeforeInstantiateTile(beforeBuildTileEvent);
            terrainGuid = beforeBuildTileEvent.TerrainGuid;
            chunkTileCoordinate = beforeBuildTileEvent.ChunkTileCoordinate;
            ruleType = beforeBuildTileEvent.RuleType;
            var tileInstance = terrainDefinition.RuleSetAsset.GetTileInstanceByRuleType(ruleType);
            if (tileInstance == null)
            {
                Debug.LogError($"The Rule Type '{ruleType}' of tile instance is null for tile position '{chunkTileCoordinate}'");
                return;
            }

            var tileInfo = new ChunkTerrainTileInfo(tileInstance, chunkTileCoordinate, ruleType);
            // var afterTileInstantiateParameters = new AfterBuildTileParameters(this, chunkObject, tileInfo);
            // _tileBuildPipline.AfterInstantiateTile(afterTileInstantiateParameters);

            chunkObject.AddTile(terrainGuid, tileInfo);
        }

        public void Bake()
        {
            _tileWorldAsset.DataStore.ClearAllBakedChunks();
            foreach (var chunk in _tileWorldAsset.EnumerateChunks())
            {
                var bakedTerrainSections = new List<BakedChunk.TerrainSection>();
                foreach (var terrainSection in chunk.TerrainSections)
                {
                    // var terrainDefinition = _tileWorldAsset.TerrainDefinitionSet.TryGetByGuid(terrainSection.TerrainGuid);
                    var bakedTiles = new List<BakedChunk.TerrainSection.Tile>();
                    foreach (var chunkTileCoordinate in terrainSection.Tiles)
                    {
                        var tileCoordinate = chunkTileCoordinate.ToTileCoordinate(chunk.Area);
                        var ruleType = _tileWorldAsset.CalculateRuleTypeOf(terrainSection.TerrainGuid, _terrainConfigAsset, tileCoordinate);
                        //TODO hide detection
                        var bakedTile = new BakedChunk.TerrainSection.Tile(chunkTileCoordinate, ruleType, false);
                        bakedTiles.Add(bakedTile);
                    }
                    var bakedTerrainSection = new BakedChunk.TerrainSection(terrainSection.TerrainGuid, bakedTiles);
                    bakedTerrainSections.Add(bakedTerrainSection);
                }
                var bakedChunk = new BakedChunk(chunk.Area, bakedTerrainSections);
                _tileWorldAsset.DataStore.UpdateBakedChunk(bakedChunk);
            }
        }


        private bool IsValidTileCoordinate(TileCoordinate tileCoordinate)
        {
            if (tileCoordinate.X < 0 ||
                tileCoordinate.Y < 0 ||
                tileCoordinate.Z < 0)
            {
                return false;
            }
            return true;
        }

        private void ForEachTiles(Guid terrainGuid, IEnumerable<TileCoordinate> tileCoordinates, Action<TileCoordinate, ChunkObject, ChunkTerrainObject> callback)
        {
            ChunkObject currentChunkObject = null;
            ChunkTerrainObject currentTerrainObject = null;
            foreach (var tileCoordinate in tileCoordinates)
            {
                if (currentChunkObject == null || !currentChunkObject.Area.Contains(tileCoordinate))
                {
                    currentChunkObject = GetChunkObjectOf(tileCoordinate.ToChunkCoordinate(_tileWorldAsset.ChunkSize));
                    currentTerrainObject = currentChunkObject.GetTerrainObject(terrainGuid);
                }
                callback(tileCoordinate, currentChunkObject, currentTerrainObject);
            }
        }

        private void OnEnable()
        {
            this.RegisterListener<SetTilesEvent>();
            this.RegisterListener<RemoveTilesEvent>();
        }

        private void OnDisable()
        {
            this.UnregisterListener<SetTilesEvent>();
            this.UnregisterListener<RemoveTilesEvent>();
        }

        void IEasyEventListener<SetTilesEvent>.OnEvent(object sender, SetTilesEvent eventArg)
        {
            if (Settings.RealTimeIncrementalBuild)
            {
                RebuildTiles(eventArg.TerrainGuid, eventArg.TilePositions);
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
#endif
            }
        }

        void IEasyEventListener<RemoveTilesEvent>.OnEvent(object sender, RemoveTilesEvent eventArg)
        {
            if (Settings.RealTimeIncrementalBuild)
            {
                DestroyTiles(eventArg.TerrainGuid, eventArg.TilePositions);
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
#endif
            }
        }
    }
}
