using System.IO;
using System.Text;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Binary input archive implementation. Deserializes data from a binary format
    /// using length-prefixed field names and varint encoding.
    /// </summary>
    internal class BinaryInputArchive : InputArchive
    {
        private readonly BinaryReader _reader;

        /// <summary>Creates a new binary input archive.</summary>
        /// <param name="stream">The stream to read from.</param>
        public BinaryInputArchive(Stream stream)
        {
            _reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
        }

        /// <summary>Gets the archive format type (Binary).</summary>
        public override ArchiveTypes ArchiveType => ArchiveTypes.Binary;

        /// <summary>Sets the name of the next field by reading and verifying the length-prefixed string.</summary>
        public override void SetNextName(string name)
        {
            var length = ReadVarint32();
            _ = Encoding.UTF8.GetString(_reader.ReadBytes((int)length));
        }

        /// <summary>Begins a nested object node (no-op for binary format).</summary>
        public override void StartNode()
        {
        }

        /// <summary>Ends a nested object node (no-op for binary format).</summary>
        public override void FinishNode()
        {
        }

        /// <summary>Processes an integer value (4 bytes, little-endian).</summary>
        public override bool Process(ref int value)
        {
            value = _reader.ReadInt32();
            return true;
        }

        /// <summary>Processes a variable-length integer value using 7-bit decoding.</summary>
        public override bool Process(ref Varint32 value)
        {
            value = new Varint32(ReadVarint32());
            return true;
        }

        /// <summary>Processes a size tag value using varint decoding.</summary>
        public override bool Process(ref SizeTag sizeTag)
        {
            sizeTag = new SizeTag(ReadVarint32());
            return true;
        }

        /// <summary>Processes a boolean value from a single byte.</summary>
        public override bool Process(ref bool value)
        {
            var byteValue = _reader.ReadByte();
            value = byteValue != 0;
            return true;
        }

        /// <summary>Processes a float value (4 bytes, IEEE 754).</summary>
        public override bool Process(ref float value)
        {
            value = _reader.ReadSingle();
            return true;
        }

        /// <summary>Processes a double value (8 bytes, IEEE 754).</summary>
        public override bool Process(ref double value)
        {
            value = _reader.ReadDouble();
            return true;
        }

        /// <summary>Processes a string value from length-prefixed UTF-8 bytes.</summary>
        public override bool Process(ref string str)
        {
            var length = ReadVarint32();
            if (length == 0)
            {
                str = null;
                return true;
            }

            var bytes = _reader.ReadBytes((int)length);
            str = Encoding.UTF8.GetString(bytes);
            return true;
        }

        /// <summary>Processes a byte array value from length-prefixed bytes.</summary>
        public override bool Process(ref byte[] data)
        {
            var length = ReadVarint32();
            if (length == 0)
            {
                data = null;
                return true;
            }

            data = _reader.ReadBytes((int)length);
            return true;
        }

        /// <summary>Processes a Unity object reference by resolving its index.</summary>
        public override bool Process(ref UnityEngine.Object unityObject)
        {
            var index = ReadVarint32();
            unityObject = ResolveUnityObjectReference(index);
            return true;
        }

        /// <summary>Releases resources used by the archive.</summary>
        public override void Dispose()
        {
            _reader?.Dispose();
        }

        /// <summary>Reads a 32-bit unsigned integer using variable-length decoding.</summary>
        /// <returns>The decoded value.</returns>
        private uint ReadVarint32()
        {
            uint value = 0;
            int shift = 0;
            byte b;
            do
            {
                b = _reader.ReadByte();
                value |= (uint)(b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            return value;
        }
    }
}
