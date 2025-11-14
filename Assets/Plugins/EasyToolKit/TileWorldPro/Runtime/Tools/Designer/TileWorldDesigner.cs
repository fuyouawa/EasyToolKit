using System;
using EasyToolKit.Core;
using EasyToolKit.Inspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace EasyToolKit.TileWorldPro
{
    public class TileWorldDesigner : MonoBehaviour
    {
        [Serializable]
        public class PreviewBlockInfo
        {
            public GameObject Instance;
            public TransformRecorder TransformRecorder;
            public Guid TerrainGuid;
            public TerrainTileRuleType RuleType;
        }

        [Required]
        [LabelText("起始点")]
        [SerializeField] private TileWorldStartPoint _startPoint;

        [FoldoutBoxGroup("设置")]
        [HideLabel]
        [SerializeField] private TileWorldDesignerSettings _settings;

        [EndFoldoutBoxGroup]
        [Required]
        [LabelText("资产")]
        [SerializeField, InlineEditor] private TileWorldAsset _tileWorldAsset;

        [SerializeField, HideInInspector] private PreviewBlockInfo _previewBlockInfo;

        public TileWorldStartPoint StartPoint => _startPoint;

        public TileWorldDesignerSettings Settings => _settings;

        public TileWorldAsset TileWorldAsset => _tileWorldAsset;

        public void DestroyPreviewBlock()
        {
            if (_previewBlockInfo != null)
            {
                if (_previewBlockInfo.Instance != null)
                {
                    DestroyImmediate(_previewBlockInfo.Instance);
                }
                _previewBlockInfo = null;
            }
        }

        public GameObject GetOrCreatePreviewBlock(Guid terrainGuid, TerrainTileRuleType ruleType)
        {
            if (_previewBlockInfo == null ||
                _previewBlockInfo.Instance == null ||
                _previewBlockInfo.TerrainGuid != terrainGuid ||
                _previewBlockInfo.RuleType != ruleType)
            {
                _previewBlockInfo ??= new PreviewBlockInfo();
                if (_previewBlockInfo.Instance != null)
                {
                    DestroyImmediate(_previewBlockInfo.Instance);
                }

                var terrainDefinition = _tileWorldAsset.TerrainDefinitionSet.GetByGuid(terrainGuid);
                _previewBlockInfo.Instance = terrainDefinition.RuleSetAsset.GetTileInstanceByRuleType(ruleType);
                _previewBlockInfo.Instance.transform.SetParent(transform);
                _previewBlockInfo.TransformRecorder = _previewBlockInfo.Instance.transform.GetRecorder();
                _previewBlockInfo.TerrainGuid = terrainGuid;
                _previewBlockInfo.RuleType = ruleType;
            }

            _previewBlockInfo.TransformRecorder.Resume(_previewBlockInfo.Instance.transform);
            return _previewBlockInfo.Instance;
        }
    }
}
