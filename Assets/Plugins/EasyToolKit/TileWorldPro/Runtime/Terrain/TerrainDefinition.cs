using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;
using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public class TerrainDefinition : TerrainDefinitionNode
    {
        [LabelText("隐藏地图调试块")]
        [OdinSerialize] private bool _hideMapDebugCube = false;

        [Title("地形集")]
        [LabelText("是否为空")]
        [OdinSerialize] private bool _isEmpty;

        [Required]
        [LabelText("地形规则集")]
        [HideIf(nameof(_isEmpty))]
        [InlineEditor(Style = InlineEditorStyle.Foldout)]
        [OdinSerialize] private TerrainRuleSetAsset _ruleSetAsset;

        [OdinSerialize, HideInInspector] private bool _isCompositeTerrain = false;

        public TerrainDefinition()
        {
        }

        public TerrainDefinition(bool isCompositeTerrain)
        {
            _isCompositeTerrain = isCompositeTerrain;
        }

        public override TerrainDefinitionNodeType NodeType => _isCompositeTerrain
            ? TerrainDefinitionNodeType.CompositeTerrain
            : TerrainDefinitionNodeType.Terrain;

        public bool IsEmpty => _isEmpty;
        public TerrainRuleSetAsset RuleSetAsset => _ruleSetAsset;
        public bool HideMapDebugCube => _hideMapDebugCube;

        public bool IsCompositeTerrain => _isCompositeTerrain;

#if UNITY_EDITOR
        public Color DebugCubeColor => Color;
#endif
    }
}
