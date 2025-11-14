using EasyToolKit.Inspector;
using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [MetroFoldoutGroup("边缘规则集", IconTextureGetter = "-t:EasyToolKit.TileWorldPro.Editor.TileWorldIcons -p:Instance.TerrainEdgeTypeIcon")]
    [HideLabel]
    [Serializable]
    public class TerrainEdgeRule : TerrainRuleBase
    {
        /// <summary>
        /// 使用完整定义。
        /// 如果为 false，则使用<see cref="_topTileDefinition"/>作为基准规则，其他方向的规则将基于此规则进行调整。
        /// </summary>
        [LabelText("使用完整定义")]
        [SerializeField] private bool _useFullDefinition;

        [Space(3)]
        [TerrainTileRuleType(TerrainTileRuleType.TopEdge)]
        [SerializeField] private TerrainTileDefinition _topTileDefinition;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainTileRuleType.BottomEdge)]
        [SerializeField] private TerrainTileDefinition _bottomTileDefinition;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainTileRuleType.LeftEdge)]
        [SerializeField] private TerrainTileDefinition _leftTileDefinition;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainTileRuleType.RightEdge)]
        [SerializeField] private TerrainTileDefinition _rightTileDefinition;

        public override TerrainRuleType RuleType => TerrainRuleType.Edge;

        public override GameObject GetTileInstanceByRuleType(TerrainTileRuleType ruleType)
        {
            switch (ruleType)
            {
                case TerrainTileRuleType.TopEdge:
                    return _topTileDefinition.TryInstantiate();
                case TerrainTileRuleType.RightEdge:
                    {
                        if (_useFullDefinition)
                        {
                            return _rightTileDefinition.TryInstantiate();
                        }

                        var instance = _topTileDefinition.TryInstantiate();
                        if (instance == null)
                            return null;

                        instance.transform.rotation *= Quaternion.Euler(0, 90, 0);
                        return instance;
                    }
                case TerrainTileRuleType.BottomEdge:
                    {
                        if (_useFullDefinition)
                        {
                            return _bottomTileDefinition.TryInstantiate();
                        }

                        var instance = _topTileDefinition.TryInstantiate();
                        if (instance == null)
                            return null;

                        instance.transform.rotation *= Quaternion.Euler(0, 180, 0);
                        return instance;
                    }
                case TerrainTileRuleType.LeftEdge:
                    {
                        if (_useFullDefinition)
                        {
                            return _leftTileDefinition.TryInstantiate();
                        }

                        var instance = _topTileDefinition.TryInstantiate();
                        if (instance == null)
                            return null;

                        instance.transform.rotation *= Quaternion.Euler(0, 270, 0);
                        return instance;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(ruleType), ruleType, null);
            }
        }

#if UNITY_EDITOR
        [DirtyTrigger] private Action<string> _dirtyTrigger;

        [Button("自动定义变换")]
        private void AutoDefineTransforms()
        {
            _rightTileDefinition.CopyTransform(_topTileDefinition);
            _bottomTileDefinition.CopyTransform(_topTileDefinition);
            _leftTileDefinition.CopyTransform(_topTileDefinition);

            _dirtyTrigger?.Invoke(nameof(_rightTileDefinition));
            _dirtyTrigger?.Invoke(nameof(_bottomTileDefinition));
            _dirtyTrigger?.Invoke(nameof(_leftTileDefinition));
        }
#endif
    }
}
