using System;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [Serializable]
    public class TileWorldDesignerSettings
    {
        [Title("预览")]
        [LabelText("显示预览方块")]
        [SerializeField] private bool _displayPreviewBlock;

        [Required]
        [LabelText("地形配置")]
        [ShowIf(nameof(_displayPreviewBlock))]
        [SerializeField] private TerrainConfigAsset _terrainConfigAssetForPreview;

        [Title("调试")]
        [LabelText("显示调试数据")]
        [SerializeField] private bool _drawDebugData = false;

        [LabelText("绘制调试地基")]
        [SerializeField] private bool _drawDebugBase = true;

        [LabelText("地基调试颜色")]
        [SerializeField] private Color _baseDebugColor = Color.white;

        [LabelText("绘制指针调试块")]
        [SerializeField] private bool _drawCursorDebugCube = true;

        [LabelText("填充指针调试块")]
        [SerializeField] private bool _fillCursorDebugCube = true;

        [LabelText("绘制地图调试块")]
        [SerializeField] private bool _drawMapDebugCube = false;

        [LabelText("为地图调试块启用深度测试")]
        [SerializeField] private bool _enableZTestForMapDebugCube = true;


        public bool DisplayPreviewBlock => _displayPreviewBlock;

        public TerrainConfigAsset TerrainConfigAssetForPreview => _terrainConfigAssetForPreview;

        public bool DrawDebugBase => _drawDebugBase;

        public Color BaseDebugColor => _baseDebugColor;

        public bool DrawDebugData => _drawDebugData;

        public bool DrawCursorDebugCube => _drawCursorDebugCube;
        public bool FillCursorDebugCube => _fillCursorDebugCube;

        public bool DrawMapDebugCube => _drawMapDebugCube;
        public bool EnableZTestForMapDebugCube => _enableZTestForMapDebugCube;
    }
}
