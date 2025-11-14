using System;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public struct ChunkArea
    {
        private readonly ChunkCoordinate _coordinate;
        private readonly Vector2Int _size;

        public ChunkCoordinate Coordinate => _coordinate;
        public Vector2Int Size => _size;

        public ChunkArea(ChunkCoordinate coordinate, Vector2Int size)
        {
            _coordinate = coordinate;
            _size = size;
        }

        public readonly TileCoordinate GetStartTileCoordinate()
        {
            return new TileCoordinate(
                _coordinate.X * _size.x,
                0,
                _coordinate.Y * _size.y);
        }

        public readonly TileCoordinate GetEndTileCoordinate()
        {
            return new TileCoordinate(
                (_coordinate.X + 1) * _size.x - 1,
                _size.y,
                (_coordinate.Y + 1) * _size.y - 1);
        }

        public readonly ChunkTileCoordinate TileCoordinateToChunkTileCoordinate(TileCoordinate tileCoordinate)
        {
            var x = tileCoordinate.X % _size.x;
            var y = tileCoordinate.Z % _size.y;
            Assert.IsTrue(x >= 0 && x < _size.x);
            Assert.IsTrue(y >= 0 && y < _size.y);

            return new ChunkTileCoordinate((ushort)x, (ushort)tileCoordinate.Y, (ushort)y);
        }

        public readonly bool Contains(TileCoordinate tileCoordinate)
        {
            var startTileCoordinate = GetStartTileCoordinate();
            var endTileCoordinate = GetEndTileCoordinate();
            return tileCoordinate.X >= startTileCoordinate.X && tileCoordinate.X <= endTileCoordinate.X &&
                   tileCoordinate.Y >= startTileCoordinate.Y && tileCoordinate.Y <= endTileCoordinate.Y &&
                   tileCoordinate.Z >= startTileCoordinate.Z && tileCoordinate.Z <= endTileCoordinate.Z;
        }
    }
}
