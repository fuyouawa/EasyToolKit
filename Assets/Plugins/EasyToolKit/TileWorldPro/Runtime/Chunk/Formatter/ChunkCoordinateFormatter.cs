using EasyToolKit.ThirdParty.OdinSerializer;
using EasyToolKit.TileWorldPro;

[assembly: RegisterFormatter(typeof(ChunkCoordinateFormatter))]
namespace EasyToolKit.TileWorldPro
{
    public class ChunkCoordinateFormatter : MinimalBaseFormatter<ChunkCoordinate>
    {
        private static readonly Serializer<ushort> UShortSerializer = Serializer.Get<ushort>();

        protected override void Read(ref ChunkCoordinate value, IDataReader reader)
        {
            var x = UShortSerializer.ReadValue(reader);
            var y = UShortSerializer.ReadValue(reader);
            value = new ChunkCoordinate(x, y);
        }

        protected override void Write(ref ChunkCoordinate value, IDataWriter writer)
        {
            UShortSerializer.WriteValue(value.X, writer);
            UShortSerializer.WriteValue(value.Y, writer);
        }
    }
}
