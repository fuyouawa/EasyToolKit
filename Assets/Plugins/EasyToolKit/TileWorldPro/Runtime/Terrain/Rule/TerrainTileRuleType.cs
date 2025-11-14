using EasyToolKit.Inspector;

namespace EasyToolKit.TileWorldPro
{
    public enum TerrainTileRuleType
    {
        /// <summary>
        /// 填充地形
        /// </summary>
        [LabelText("填充地形")] Fill,

        /// <summary>
        /// 上边缘地形
        /// </summary>
        [LabelText("上边缘地形")] TopEdge,

        /// <summary>
        /// 右边缘地形
        /// </summary>
        [LabelText("右边缘地形")] RightEdge,

        /// <summary>
        /// 下边缘地形
        /// </summary>
        [LabelText("下边缘地形")] BottomEdge,

        /// <summary>
        /// 左边缘地形
        /// </summary>
        [LabelText("左边缘地形")] LeftEdge,

        /// <summary>
        /// 左上部分的外转角地形
        /// </summary>
        [LabelText("左上部分的外转角地形")] TopLeftExteriorCorner,

        /// <summary>
        /// 右上部分的外转角地形
        /// </summary>
        [LabelText("右上部分的外转角地形")] TopRightExteriorCorner,

        /// <summary>
        /// 右下部分的外转角地形
        /// </summary>
        [LabelText("右下部分的外转角地形")] BottomRightExteriorCorner,

        /// <summary>
        /// 左下部分的外转角地形
        /// </summary>
        [LabelText("左下部分的外转角地形")] BottomLeftExteriorCorner,

        /// <summary>
        /// 左上部分的内转角地形
        /// </summary>
        [LabelText("左上部分的内转角地形")] TopLeftInteriorCorner,

        /// <summary>
        /// 右上部分的内转角地形
        /// </summary>
        [LabelText("右上部分的内转角地形")] TopRightInteriorCorner,

        /// <summary>
        /// 右下部分的内转角地形
        /// </summary>
        [LabelText("右下部分的内转角地形")] BottomRightInteriorCorner,

        /// <summary>
        /// 左下部分的内转角地形
        /// </summary>
        [LabelText("左下部分的内转角地形")] BottomLeftInteriorCorner
    }
}
