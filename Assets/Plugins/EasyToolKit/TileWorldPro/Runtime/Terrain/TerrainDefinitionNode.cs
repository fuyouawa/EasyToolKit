using System;
using EasyToolKit.Core;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [ShowOdinSerializedPropertiesInInspector]
#if UNITY_EDITOR
    [MetroFoldoutGroup("@Name", "@Tooltip",
        SideLineColorGetter = nameof(SideLineColor),
        IconTextureGetter = nameof(Icon),
        RightLabel = "@RightLabel",
        RightLabelColorGetter = nameof(RightLabelColor))]
#endif
    [HideLabel]
    public abstract class TerrainDefinitionNode
    {
        [OdinSerialize, HideInInspector] private Guid _guid;

        [FoldoutGroup("设置")]
        [LabelText("名称")]
        [OdinSerialize] private string _name;

#if UNITY_EDITOR
        [Title("调试")]
        [LabelText("颜色")]
        [OdinSerialize] private Color _color = Color.white;
#endif

        public string Name => _name;
        public Guid Guid
        {
            get
            {
                if (_guid == Guid.Empty)
                {
                    _guid = Guid.NewGuid();
                }
                return _guid;
            }
        }

        public abstract TerrainDefinitionNodeType NodeType { get; }

#if UNITY_EDITOR
        public Color Color => _color;
        public Color SideLineColor => _color.SetA(1f);
        public string Tooltip => Guid.ToString("D");

        private static Func<TerrainDefinitionNodeType, Texture2D> s_iconGetter;

        public string RightLabel
        {
            get
            {
                switch (NodeType)
                {
                    case TerrainDefinitionNodeType.Group:
                        return "（分组）";
                    case TerrainDefinitionNodeType.Terrain:
                        return "（地形）";
                    case TerrainDefinitionNodeType.CompositeTerrain:
                        return "（复合地形）";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Color RightLabelColor
        {
            get
            {
                switch (NodeType)
                {
                    case TerrainDefinitionNodeType.Group:
                        return Color.cyan;
                    case TerrainDefinitionNodeType.Terrain:
                        return Color.green;
                    case TerrainDefinitionNodeType.CompositeTerrain:
                        return Color.yellow;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private Texture2D _icon;
        public Texture2D Icon
        {
            get
            {
                if (s_iconGetter == null)
                {
                    var iconsType = TwoWaySerializationBinder.Default.BindToType("EasyToolKit.TileWorldPro.Editor.TileWorldIcons");
                    var iconsInstance = iconsType.GetProperty("Instance", BindingFlagsHelper.AllStatic).GetValue(null);
                    var getIconMethod = iconsType.GetMethod("GetTerrainDefinitionItemTypeIcon");
                    s_iconGetter = type => (Texture2D)getIconMethod.Invoke(iconsInstance, new object[] { type });
                }

                if (_icon == null)
                {
                    _icon = s_iconGetter(NodeType);
                }
                return _icon;
            }
        }
#endif
    }
}
