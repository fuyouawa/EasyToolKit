using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public struct ChunkCoordinate : IEquatable<ChunkCoordinate>
    {
        private readonly ushort _x;
        private readonly ushort _y;

        public ushort X => _x;
        public ushort Y => _y;

        public ChunkCoordinate(ushort x, ushort y)
        {
            _x = x;
            _y = y;
        }

        public override int GetHashCode()
        {
            return ((int)_x << 16) | (int)_y;
        }

        public bool Equals(ChunkCoordinate other)
        {
            return _x == other._x && _y == other._y;
        }

        public override bool Equals(object obj)
        {
            return obj is ChunkCoordinate other && Equals(other);
        }

        public static bool operator ==(ChunkCoordinate left, ChunkCoordinate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ChunkCoordinate left, ChunkCoordinate right)
        {
            return !left.Equals(right);
        }

        public static implicit operator Vector2Int(ChunkCoordinate coordinate)
        {
            return new Vector2Int(coordinate.X, coordinate.Y);
        }

        public static implicit operator ChunkCoordinate(Vector2Int position)
        {
            return new ChunkCoordinate((ushort)position.x, (ushort)position.y);
        }

        public override string ToString()
        {
            return $"({_x}, {_y})";
        }
    }
}
