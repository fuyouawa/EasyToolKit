using System;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [Serializable]
    public struct TileCoordinate : IEquatable<TileCoordinate>
    {
        /// <summary>
        /// TilePosition(0, 0, 0)
        /// </summary>
        public static readonly TileCoordinate Zero = new TileCoordinate(0, 0, 0);
        /// <summary>
        /// TilePosition(0, 0, 1)
        /// </summary>
        public static readonly TileCoordinate Forward = new TileCoordinate(0, 0, 1);
        /// <summary>
        /// TilePosition(0, 0, -1)
        /// </summary>
        public static readonly TileCoordinate Back = new TileCoordinate(0, 0, -1);
        /// <summary>
        /// TilePosition(-1, 0, 0)
        /// </summary>
        public static readonly TileCoordinate Left = new TileCoordinate(-1, 0, 0);
        /// <summary>
        /// TilePosition(1, 0, 0)
        /// </summary>
        public static readonly TileCoordinate Right = new TileCoordinate(1, 0, 0);

        /// <summary>
        /// TilePosition(0, 0, 1) + TilePosition(-1, 0, 0)
        /// </summary>
        public static readonly TileCoordinate ForwardLeft = new TileCoordinate(0, 0, 1) + new TileCoordinate(-1, 0, 0);
        /// <summary>
        /// TilePosition(0, 0, 1) + TilePosition(1, 0, 0)
        /// </summary>
        public static readonly TileCoordinate ForwardRight = new TileCoordinate(0, 0, 1) + new TileCoordinate(1, 0, 0);
        /// <summary>
        /// TilePosition(0, 0, -1) + TilePosition(-1, 0, 0)
        /// </summary>
        public static readonly TileCoordinate BackLeft = new TileCoordinate(0, 0, -1) + new TileCoordinate(-1, 0, 0);
        /// <summary>
        /// TilePosition(0, 0, -1) + TilePosition(1, 0, 0)
        /// </summary>
        public static readonly TileCoordinate BackRight = new TileCoordinate(0, 0, -1) + new TileCoordinate(1, 0, 0);

        [SerializeField] private Vector3Int _coordinate;

        public int X => _coordinate.x;
        public int Y => _coordinate.y;
        public int Z => _coordinate.z;

        public TileCoordinate(int x, int y, int z)
        {
            _coordinate = new Vector3Int(x, y, z);
        }

        public TileCoordinate(Vector3Int coordinate)
        {
            _coordinate = coordinate;
        }

        public ChunkCoordinate ToChunkCoordinate(Vector2Int chunkSize)
        {
            var chunkX = ((float)_coordinate.x / chunkSize.x).SafeFloorToInt();
            var chunkY = ((float)_coordinate.z / chunkSize.y).SafeFloorToInt();
            return new ChunkCoordinate((ushort)chunkX, (ushort)chunkY);
        }

        public bool Equals(TileCoordinate other)
        {
            return _coordinate == other._coordinate;
        }

        public override bool Equals(object obj)
        {
            return obj is TileCoordinate other && Equals(other);
        }

        public static bool operator ==(TileCoordinate left, TileCoordinate right)
        {
            return left.Equals(right);
        }

        public static TileCoordinate operator +(TileCoordinate left, TileCoordinate right)
        {
            return new TileCoordinate(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static TileCoordinate operator -(TileCoordinate left, TileCoordinate right)
        {
            return new TileCoordinate(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static TileCoordinate operator *(TileCoordinate coordinate, int scale)
        {
            return new TileCoordinate(coordinate.X * scale, coordinate.Y * scale, coordinate.Z * scale);
        }

        public static TileCoordinate operator *(int scale, TileCoordinate coordinate)
        {
            return new TileCoordinate(coordinate.X * scale, coordinate.Y * scale, coordinate.Z * scale);
        }

        public static TileCoordinate operator /(TileCoordinate coordinate, int scale)
        {
            return new TileCoordinate(coordinate.X / scale, coordinate.Y / scale, coordinate.Z / scale);
        }

        public static TileCoordinate operator /(int scale, TileCoordinate coordinate)
        {
            return new TileCoordinate(scale / coordinate.X, scale / coordinate.Y, scale / coordinate.Z);
        }

        public static bool operator !=(TileCoordinate left, TileCoordinate right)
        {
            return !left.Equals(right);
        }

        public static implicit operator Vector3Int(TileCoordinate coordinate)
        {
            return coordinate._coordinate;
        }

        public static implicit operator Vector3(TileCoordinate coordinate)
        {
            return coordinate._coordinate;
        }

        public override int GetHashCode()
        {
            return _coordinate.GetHashCode();
        }

        public override string ToString()
        {
            return _coordinate.ToString();
        }
    }
}
