using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Inspector;
using EasyToolKit.TileWorldPro;
using UnityEngine;

[assembly: RegisterTileBuildProcessor(typeof(TBP_RandomMaterial), "随机材质")]

namespace EasyToolKit.TileWorldPro
{
    public class TBP_RandomMaterial : AbstractTileBuildProcessor
    {
        [HideLabel]
        public class MaterialItem
        {
            [Required]
            [LabelText("材质")]
            public Material Material;

            [Range(0f, 10f)]
            [LabelText("权重")]
            public float Weight = 5f;
        }

        [LabelText("材质索引")]
        public int MaterialIndex;

        [LabelText("材质列表")]
        [ListDrawerSettings(ShowIndexLabel = false)]
        public List<MaterialItem> MaterialItems;

        protected override string Help => "随机材质，使用伪随机算法（根据瓦片坐标生成固定的随机数）。\n可配置材质的权重，权重越大，被选中的概率越大。";

        public override void OnAfterBuildTile(AfterBuildTileEvent e)
        {
            var meshRenderer = e.TileInfo.Instance.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                return;

            if (MaterialItems == null || MaterialItems.Count == 0)
                return;
            var materials = meshRenderer.sharedMaterials;
            if (MaterialIndex >= materials.Length)
                return;

            var cumulativeWidgets = new float[MaterialItems.Count];
            cumulativeWidgets[0] = MaterialItems[0].Weight;
            for (int i = 1; i < MaterialItems.Count; i++)
            {
                cumulativeWidgets[i] = cumulativeWidgets[i - 1] + MaterialItems[i].Weight;
            }
            var totalWeight = cumulativeWidgets[MaterialItems.Count - 1];

            var tileCoordinate = e.TileInfo.ChunkTileCoordinate.ToTileCoordinate(e.ChunkTerrainObject.ChunkObject.Area);
            var key = tileCoordinate.ToString();
            var number = PseudoRandom.Range(key, 0, (int)(totalWeight * 100)) / 100f;
            for (int i = 0; i < MaterialItems.Count; i++)
            {
                if (number < cumulativeWidgets[i])
                {
                    materials[MaterialIndex] = MaterialItems[i].Material;
                    meshRenderer.sharedMaterials = materials;
                    break;
                }
            }
        }
    }
}
