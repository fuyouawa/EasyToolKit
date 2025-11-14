using EasyToolKit.ThirdParty.OdinSerializer;
using EasyToolKit.TileWorldPro;

[assembly: RegisterFormatter(typeof(ChunkTileCoordinateFormatter))]
namespace EasyToolKit.TileWorldPro
{
    public class ChunkTileCoordinateFormatter : MinimalBaseFormatter<ChunkTileCoordinate>
    {
        private static readonly Serializer<ushort> UShortSerializer = Serializer.Get<ushort>();

        protected override void Read(ref ChunkTileCoordinate value, IDataReader reader)
        {
            var x = UShortSerializer.ReadValue(reader);
            var y = UShortSerializer.ReadValue(reader);
            var z = UShortSerializer.ReadValue(reader);
            value = new ChunkTileCoordinate(x, y, z);
        }

        protected override void Write(ref ChunkTileCoordinate value, IDataWriter writer)
        {
            UShortSerializer.WriteValue(value.X, writer);
            UShortSerializer.WriteValue(value.Y, writer);
            UShortSerializer.WriteValue(value.Z, writer);
        }
    }
}
