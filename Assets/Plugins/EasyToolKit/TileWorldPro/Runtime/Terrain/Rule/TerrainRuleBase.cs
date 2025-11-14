using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public abstract class TerrainRuleBase
    {
        public abstract TerrainRuleType RuleType { get; }

        public abstract GameObject GetTileInstanceByRuleType(TerrainTileRuleType ruleType);
    }
}
