using EasyToolKit.Inspector;

namespace EasyToolKit.TileWorldPro
{
    public enum TerrainRuleType
    {
        [LabelText("填充地形")] Fill,
        [LabelText("边缘地形")] Edge,
        [LabelText("外转角地形")] ExteriorCorner,
        [LabelText("内转角地形")] InteriorCorner,
    }
}
