using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Inspector;
using EasyToolKit.TileWorldPro;
using UnityEngine;

[assembly: RegisterTileBuildProcessor(typeof(TBP_MaterialMap), "材质映射")]

namespace EasyToolKit.TileWorldPro
{
    [Serializable]
    public class MaterialMatrix : List<Material>
    {
        private int _size;

        public Material this[int x, int y]
        {
            get
            {
                var index = y * Size + x;
                if (index >= Count || index < 0)
                {
                    throw new IndexOutOfRangeException($"Index out of range: {index}");
                }

                return this[index];
            }
            set
            {
                var index = y * Size + x;
                if (index >= Count || index < 0)
                {
                    throw new IndexOutOfRangeException($"Index out of range: {index}");
                }

                this[index] = value;
            }
        }

        public int Size
        {
            get
            {
                if (Count * Count != _size)
                {
                    _size = Mathf.RoundToInt(Mathf.Sqrt(Count));
                }
                return _size;
            }
        }

        public MaterialMatrix()
        {
        }

        public MaterialMatrix(int size)
        {
            Resize(size);
        }

        public MaterialMatrix(MaterialMatrix other)
        {
            AddRange(other);
        }

        public void Resize(int size)
        {
            int newArea = size * size;

            if (Count == 0)
            {
                AddRange(new Material[newArea]);
                return;
            }

            int oldSize = Size;
            if (oldSize == size)
                return;

            var newMaterials = new List<Material>(new Material[newArea]);

            for (int oldIndex = 0; oldIndex < Count; oldIndex++)
            {
                int oldRow = oldIndex / oldSize;
                int oldCol = oldIndex % oldSize;

                if (oldRow < size && oldCol < size)
                {
                    int newIndex = oldRow * size + oldCol;
                    newMaterials[newIndex] = this[oldIndex];
                }
            }

            Clear();
            AddRange(newMaterials);
        }

        public void FlipHorizontal()
        {
            if (Count == 0)
                return;

            int size = Size;
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size / 2; col++)
                {
                    // 交换 (row, col) 和 (row, size - 1 - col)
                    int leftIndex = row * size + col;
                    int rightIndex = row * size + (size - 1 - col);

                    (this[leftIndex], this[rightIndex]) = (this[rightIndex], this[leftIndex]);
                }
            }
        }

        public void FlipVertical()
        {
            if (Count == 0)
                return;

            int size = Size;
            for (int row = 0; row < size / 2; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    // 交换 (row, col) 和 (size - 1 - row, col)
                    int topIndex = row * size + col;
                    int bottomIndex = (size - 1 - row) * size + col;

                    (this[topIndex], this[bottomIndex]) = (this[bottomIndex], this[topIndex]);
                }
            }
        }
    }

    public class TBP_MaterialMap : AbstractTileBuildProcessor
    {
        public enum Modes
        {
            [LabelText("根据瓦片坐标")] ByTilePosition
        }

        public enum MatrixSizes
        {
            [LabelText("2x2")] _2X2,
            [LabelText("3x3")] _3X3,
            [LabelText("4x4")] _4X4,
            [LabelText("5x5")] _5X5,
            [LabelText("6x6")] _6X6,
        }

        public enum FlipModes
        {
            [LabelText("水平翻转")] Horizontal,
            [LabelText("垂直翻转")] Vertical,
            [LabelText("水平或垂直翻转")] HorizontalOrVertical,
        }

        [FoldoutBoxGroup("材质映射")]
        [LabelText("映射模式")]
        public Modes Mode;

        [LabelText("材质索引")]
        public int MaterialIndex;

        [LabelText("材质矩阵大小")]
#if UNITY_EDITOR
        [OnValueChanged(nameof(OnMatrixSizeChanged))]
#endif
        public MatrixSizes MatrixSize;

        [Required(PassToListElements = true)]
        [LabelText("材质矩阵")]
#if UNITY_EDITOR
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, Draggable = false, ShowIndexLabel = true,
            CustomIndexLabelFunction = nameof(GetMaterialMatrixIndexLabel))]
#endif
        public MaterialMatrix MaterialMatrix = new MaterialMatrix(2);

        [FoldoutBoxGroup("随机翻转（伪随机）")]

        [LabelText("启用随机翻转")]
        public bool EnableRandomFlip;

        [LabelText("翻转模式")]
        public FlipModes FlipMode;

        [LabelText("翻转概率")]
        [Range(0, 1)]
        public float Probability = 0.5f;

        [LabelText("水平或垂直倾向")]
        [Range(0, 1)]
        [ShowIf(nameof(FlipMode), FlipModes.HorizontalOrVertical)]
        public float HorizontalOrVerticalBias = 0.5f;

        /// <summary>
        /// The flipped material matrices.
        /// </summary>
        /// <remarks>
        /// 0: Horizontal, 1: Vertical
        /// </remarks>
        private MaterialMatrix[] _flippedMaterialMatrices;

        protected override string Help => "将材质矩阵中定义的材质，按照指定的映射模式，映射到指定类型的对应瓦片上（根据材质索引，修改材质列表中对应的元素）。";

        public override void OnAfterBuildTile(AfterBuildTileEvent e)
        {
            var meshRenderer = e.TileInfo.Instance.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                return;

            var tileCoordinate = e.TileInfo.ChunkTileCoordinate.ToTileCoordinate(e.ChunkTerrainObject.ChunkObject.Area);

            switch (Mode)
            {
                case Modes.ByTilePosition:
                    MapMaterialByTileCoordinate(meshRenderer, tileCoordinate);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MapMaterialByTileCoordinate(MeshRenderer meshRenderer, TileCoordinate tileCoordinate)
        {
            if (MaterialMatrix == null)
                return;

            var size = MaterialMatrix.Size;
            int matrixX = tileCoordinate.X % size;
            int matrixY = size - 1 - tileCoordinate.Z % size;
            var materials = meshRenderer.sharedMaterials;

            var materialMatrix = MaterialMatrix;
            if (EnableRandomFlip)
            {
                var key = new Vector3Int(tileCoordinate.X / size, tileCoordinate.Y, tileCoordinate.Z / size).ToString();
                var doFlip = PseudoRandom.Range(key, 0, 100) < (int)(Probability * 100);
                if (doFlip)
                {
                    materialMatrix = GetFlippedMatrix(key, out var isHorizontal);
                }
            }

            if (MaterialIndex >= 0 && MaterialIndex < materials.Length)
            {
                materials[MaterialIndex] = materialMatrix[matrixX, matrixY];
                meshRenderer.sharedMaterials = materials;
            }
        }

        private MaterialMatrix GetFlippedMatrix(string key, out bool isHorizontal)
        {
            if (_flippedMaterialMatrices == null)
            {
                _flippedMaterialMatrices = new MaterialMatrix[2];
                _flippedMaterialMatrices[0] = new MaterialMatrix(MaterialMatrix);
                _flippedMaterialMatrices[1] = new MaterialMatrix(MaterialMatrix);
                _flippedMaterialMatrices[0].FlipHorizontal();
                _flippedMaterialMatrices[1].FlipVertical();
            }

            switch (FlipMode)
            {
                case FlipModes.Horizontal:
                    isHorizontal = true;
                    return _flippedMaterialMatrices[0];
                case FlipModes.Vertical:
                    isHorizontal = false;
                    return _flippedMaterialMatrices[1];
                case FlipModes.HorizontalOrVertical:
                    isHorizontal = PseudoRandom.Range(key, 0, 100, seed: 1) < (int)(HorizontalOrVerticalBias * 100);
                    return _flippedMaterialMatrices[isHorizontal ? 0 : 1];
                default:
                    throw new ArgumentOutOfRangeException(nameof(FlipMode), FlipMode, null);
            }
        }

#if UNITY_EDITOR
        [DirtyTrigger] private Action<string> _dirtyTrigger;
        private void OnMatrixSizeChanged(MatrixSizes matrixSize)
        {
            int originSize = MaterialMatrix.Size;
            switch (matrixSize)
            {
                case MatrixSizes._2X2:
                    MaterialMatrix.Resize(2);
                    break;
                case MatrixSizes._3X3:
                    MaterialMatrix.Resize(3);
                    break;
                case MatrixSizes._4X4:
                    MaterialMatrix.Resize(4);
                    break;
                case MatrixSizes._5X5:
                    MaterialMatrix.Resize(5);
                    break;
                case MatrixSizes._6X6:
                    MaterialMatrix.Resize(6);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(matrixSize), matrixSize, null);
            }
            _flippedMaterialMatrices = null;

            if (originSize != MaterialMatrix.Size)
            {
                _dirtyTrigger?.Invoke(nameof(MaterialMatrix));
            }
        }

        private string GetMaterialMatrixIndexLabel(int index)
        {
            switch (MatrixSize)
            {
                case MatrixSizes._2X2:
                    return GetIndex(2);
                case MatrixSizes._3X3:
                    return GetIndex(3);
                case MatrixSizes._4X4:
                    return GetIndex(4);
                case MatrixSizes._5X5:
                    return GetIndex(5);
                case MatrixSizes._6X6:
                    return GetIndex(6);
                default:
                    throw new IndexOutOfRangeException();
            }

            string GetIndex(int size)
            {
                return $"({index / size}, {index % size})";
            }
        }
#endif
    }
}
