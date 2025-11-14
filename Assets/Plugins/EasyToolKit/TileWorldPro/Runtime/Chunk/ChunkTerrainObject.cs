using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public class ChunkTerrainObject : SerializedMonoBehaviour
    {
        [OdinSerialize] private Guid _terrainGuid;
        [SerializeField, ReadOnly] private ChunkObject _chunkObject;

        private TerrainDefinition _terrainDefinition;

        private Dictionary<ChunkTileCoordinate, ChunkTerrainTileInfo> _tileInfos = new Dictionary<ChunkTileCoordinate, ChunkTerrainTileInfo>();
        private bool _isDirty = false;
        [SerializeField, HideInInspector] private byte[] _serializedTileInfos;
        [SerializeField, HideInInspector] private List<UnityEngine.Object> _serializedTileInstances;

        public TerrainDefinition TerrainDefinition
        {
            get
            {
                if (_terrainDefinition == null)
                {
                    _terrainDefinition = ChunkObject.Builder.TileWorldAsset.TerrainDefinitionSet.GetByGuid(_terrainGuid);
                }
                return _terrainDefinition;
            }
        }

        public ChunkObject ChunkObject => _chunkObject;

        public void Initialize(ChunkObject chunkObject, Guid terrainGuid)
        {
            _chunkObject = chunkObject;
            _terrainGuid = terrainGuid;
        }

        public void DestroyAll()
        {
            foreach (var tileInfo in _tileInfos.Values)
            {
                if (Application.isPlaying)
                {
                    Destroy(tileInfo.Instance);
                }
                else
                {
                    DestroyImmediate(tileInfo.Instance);
                }
            }
            _tileInfos.Clear();
        }

        public void AddTile(ChunkTerrainTileInfo tileInfo)
        {
            tileInfo.Instance.name += $"_{tileInfo.ChunkTileCoordinate}";
            tileInfo.Instance.transform.SetParent(transform);
            tileInfo.Instance.transform.position += ChunkObject.Builder.StartPoint.TileCoordinateToPosition(tileInfo.ChunkTileCoordinate.ToTileCoordinate(ChunkObject.Area), ChunkObject.Builder.TileWorldAsset.TileSize);

            _tileInfos[tileInfo.ChunkTileCoordinate] = tileInfo;
            _isDirty = true;
        }

        public ChunkTerrainTileInfo TryGetTileInfoOf(ChunkTileCoordinate chunkTileCoordinate)
        {
            return _tileInfos.GetValueOrDefault(chunkTileCoordinate);
        }

        public bool DestroyTileAt(ChunkTileCoordinate chunkTileCoordinate)
        {
            if (_tileInfos.TryGetValue(chunkTileCoordinate, out var tileInfo))
            {
                if (Application.isPlaying)
                {
                    Destroy(tileInfo.Instance);
                }
                else
                {
                    DestroyImmediate(tileInfo.Instance);
                }
                _tileInfos.Remove(chunkTileCoordinate);
                _isDirty = true;
                return true;
            }
            return false;
        }

        protected override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            if (_serializedTileInfos != null && _serializedTileInfos.Length > 0)
            {
                _tileInfos = SerializationUtility.DeserializeValue<Dictionary<ChunkTileCoordinate, ChunkTerrainTileInfo>>(_serializedTileInfos, DataFormat.Binary, _serializedTileInstances);
            }
        }

        protected override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
            if (_isDirty)
            {
                _serializedTileInfos = SerializationUtility.SerializeValue(_tileInfos, DataFormat.Binary, out _serializedTileInstances);
                _isDirty = false;
            }
        }
    }
}
