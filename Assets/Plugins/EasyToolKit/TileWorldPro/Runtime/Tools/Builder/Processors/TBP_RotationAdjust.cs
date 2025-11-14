using System;
using UnityEngine;
using EasyToolKit.Inspector;
using EasyToolKit.TileWorldPro;

[assembly: RegisterTileBuildProcessor(typeof(TBP_RotationAdjust), "旋转调整")]
namespace EasyToolKit.TileWorldPro
{
    public class TBP_RotationAdjust : AbstractTileBuildProcessor
    {
        public enum Modes
        {
            [LabelText("相对模式")] Relative,
            [LabelText("绝对模式")] Absolute
        }

        [FoldoutBoxGroup("旋转调整")]
        [LabelText("模式")]
        public Modes Mode;

        [LabelText("旋转")]
        public Vector3 Rotation;

        protected override string Help => "调整指定类型瓦片的旋转角度。";

        public override void OnAfterBuildTile(AfterBuildTileEvent e)
        {
            // 获取瓦片实例的Transform组件
            var tileTransform = e.TileInfo.Instance.transform;
            if (tileTransform == null)
                return;

            // 根据模式应用旋转
            switch (Mode)
            {
                case Modes.Relative:
                    // 相对模式：在当前旋转基础上添加指定的旋转
                    tileTransform.rotation *= Quaternion.Euler(Rotation);
                    break;
                case Modes.Absolute:
                    // 绝对模式：设置为指定的旋转值
                    tileTransform.rotation = Quaternion.Euler(Rotation);
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }
    }
}
