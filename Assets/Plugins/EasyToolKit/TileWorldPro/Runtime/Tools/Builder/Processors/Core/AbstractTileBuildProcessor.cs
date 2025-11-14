using EasyToolKit.Inspector;
using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [Serializable]
#if UNITY_EDITOR
    [MetroFoldoutGroup("@_label",
        RightLabel = "@RightLabel",
        RightLabelColorGetter = nameof(RightLabelColor))]
#endif
    [MessageBox("@Help")]
    public abstract class AbstractTileBuildProcessor : ITileBuildProcessor
    {
        [LabelText("名称")]
        [SerializeField] private string _label;

        [LabelText("启用")]
        [SerializeField] private bool _enabled = true;

        [LabelText("目标地形")]
#if UNITY_EDITOR
        [ValueDropdown(nameof(GetTerrainDropdownList))]
#endif
        [OdinSerialize, ShowInInspector] private Guid _targetTerrainGuid;

        [LabelText("目标瓦片地形规则")]
        [SerializeField] private TerrainTileRuleType _targetTileRuleType;

        protected abstract string Help { get; }

        void ITileBuildProcessor.OnBeforeBuildTile(BeforeBuildTileEvent e)
        {
            if (!_enabled)
                return;
            if (e.RuleType != _targetTileRuleType || e.TerrainGuid != _targetTerrainGuid)
                return;
            OnBeforeBuildTile(e);
        }

        void ITileBuildProcessor.OnAfterBuildTile(AfterBuildTileEvent e)
        {
            if (!_enabled)
                return;
            if (e.TileInfo.RuleType != _targetTileRuleType || e.ChunkTerrainObject.TerrainDefinition.Guid != _targetTerrainGuid)
                return;
            OnAfterBuildTile(e);
        }

        public virtual void OnBeforeBuildTile(BeforeBuildTileEvent e)
        {
        }

        public virtual void OnAfterBuildTile(AfterBuildTileEvent e)
        {
        }

#if UNITY_EDITOR
        private TileWorldBuilder _builder;
        [OnInspectorInit]
        private void OnInspectorInit(TileWorldBuilder builder)
        {
            _builder = builder;
        }

        private ValueDropdownList<Guid> GetTerrainDropdownList()
        {
            var list = new ValueDropdownList<Guid>();
            if (_builder?.TileWorldAsset == null)
                return list;
            var definitionSet = _builder.TileWorldAsset.TerrainDefinitionSet;
            foreach (var pathInfo in definitionSet.EnumeratePathInfo())
            {
                list.Add(string.Join('/', pathInfo.Path), pathInfo.Definition.Guid);
            }
            return list;
        }

        private string _rightLabel;
        private string RightLabel
        {
            get
            {
                if (_rightLabel == null)
                {
                    _rightLabel = TileBuildProcessorUtility.GetProcessorNameByType(GetType());
                    _rightLabel = $"（{_rightLabel}）";
                }
                return _rightLabel;
            }
        }

        private static readonly Color RightLabelColor = Color.green;
#endif
    }
}
