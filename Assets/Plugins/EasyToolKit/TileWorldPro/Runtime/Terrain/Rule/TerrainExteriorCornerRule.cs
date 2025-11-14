using EasyToolKit.Inspector;
using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [MetroFoldoutGroup("外转角规则集", IconTextureGetter = "-t:EasyToolKit.TileWorldPro.Editor.TileWorldIcons -p:Instance.TerrainExteriorCornerTypeIcon")]
    [HideLabel]
    [Serializable]
    public class TerrainExteriorCornerRule : TerrainRuleBase
    {
        /// <summary>
        /// 使用完整定义。
        /// 如果为 false，则使用<see cref="_topLeftTileDefinition"/>作为基准规则，其他方向的规则将基于此规则进行调整。
        /// </summary>
        [LabelText("使用完整定义")]
        [SerializeField] private bool _useFullDefinition;

        [Space(3)]
        [TerrainTileRuleType(TerrainTileRuleType.TopLeftExteriorCorner)]
        [SerializeField] private TerrainTileDefinition _topLeftTileDefinition;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainTileRuleType.TopRightExteriorCorner)]
        [SerializeField] private TerrainTileDefinition _topRightTileDefinition;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainTileRuleType.BottomLeftExteriorCorner)]
        [SerializeField] private TerrainTileDefinition _bottomLeftTileDefinition;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainTileRuleType.BottomRightExteriorCorner)]
        [SerializeField] private TerrainTileDefinition _bottomRightTileDefinition;

        public override TerrainRuleType RuleType => TerrainRuleType.ExteriorCorner;

        public override GameObject GetTileInstanceByRuleType(TerrainTileRuleType ruleType)
        {
            switch (ruleType)
            {
                case TerrainTileRuleType.TopLeftExteriorCorner:
                    return _topLeftTileDefinition.TryInstantiate();
                case TerrainTileRuleType.TopRightExteriorCorner:
                    {
                        if (_useFullDefinition)
                        {
                            return _topRightTileDefinition.TryInstantiate();
                        }

                        var instance = _topLeftTileDefinition.TryInstantiate();
                        if (instance == null)
                            return null;

                        instance.transform.rotation *= Quaternion.Euler(0, 90, 0);
                        return instance;
                    }
                case TerrainTileRuleType.BottomRightExteriorCorner:
                    {
                        if (_useFullDefinition)
                        {
                            return _bottomRightTileDefinition.TryInstantiate();
                        }

                        var instance = _topLeftTileDefinition.TryInstantiate();
                        if (instance == null)
                            return null;

                        instance.transform.rotation *= Quaternion.Euler(0, 180, 0);
                        return instance;
                    }
                case TerrainTileRuleType.BottomLeftExteriorCorner:
                    {
                        if (_useFullDefinition)
                        {
                            return _bottomLeftTileDefinition.TryInstantiate();
                        }

                        var instance = _topLeftTileDefinition.TryInstantiate();
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
            _topRightTileDefinition.CopyTransform(_topLeftTileDefinition);
            _bottomRightTileDefinition.CopyTransform(_topLeftTileDefinition);
            _bottomLeftTileDefinition.CopyTransform(_topLeftTileDefinition);

            _dirtyTrigger?.Invoke(nameof(_topRightTileDefinition));
            _dirtyTrigger?.Invoke(nameof(_bottomRightTileDefinition));
            _dirtyTrigger?.Invoke(nameof(_bottomLeftTileDefinition));
        }
#endif
    }
}
