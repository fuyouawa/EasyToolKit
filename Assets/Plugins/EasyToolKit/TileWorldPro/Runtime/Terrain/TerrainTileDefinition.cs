using EasyToolKit.Inspector;
using JetBrains.Annotations;
using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [Serializable]
    [HideLabel]
    public class TerrainTileDefinition
    {
        [Required]
        [LabelText("预制体")]
        [SerializeField] private GameObject _prefab;

        [LabelText("位置偏移")]
        [SerializeField] private Vector3 _positionOffset;

        [LabelText("旋转偏移")]
        [SerializeField] private Vector3 _rotationOffset;

        [LabelText("缩放偏移")]
        [SerializeField] private Vector3 _scaleOffset;

        public GameObject Prefab => _prefab;
        public Vector3 PositionOffset => _positionOffset;
        public Vector3 RotationOffset => _rotationOffset;
        public Vector3 ScaleOffset => _scaleOffset;

        [CanBeNull]
        public GameObject TryInstantiate()
        {
            if (_prefab == null)
            {
                return null;
            }

            var instance = GameObject.Instantiate(Prefab);
            ApplyTransform(instance);
            return instance;
        }

        public void ApplyTransform(GameObject instance)
        {
            instance.transform.position = PositionOffset;
            instance.transform.rotation *= Quaternion.Euler(RotationOffset);
            instance.transform.localScale += ScaleOffset;
            instance.name = Prefab.name;
        }

        public void CopyTransform(TerrainTileDefinition source)
        {
            _positionOffset = source.PositionOffset;
            _rotationOffset = source.RotationOffset;
            _scaleOffset = source.ScaleOffset;
        }
    }
}
