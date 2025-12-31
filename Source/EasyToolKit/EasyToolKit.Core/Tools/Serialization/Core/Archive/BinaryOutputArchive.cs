using System;
using System.IO;
using System.Text;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Binary output archive implementation. Serializes data to a binary format
    /// using length-prefixed field names and varint encoding.
    /// </summary>
    internal class BinaryOutputArchive : OutputArchive
    {
        private const int MaxVarintBytes = 5;

        private readonly BinaryWriter _writer;
        private int _nodeDepth;

        /// <summary>Creates a new binary output archive.</summary>
        /// <param name="stream">The stream to write to.</param>
        public BinaryOutputArchive(Stream stream)
        {
            _writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
            _nodeDepth = 0;
        }

        /// <summary>Gets the archive format type (Binary).</summary>
        public override ArchiveTypes ArchiveType => ArchiveTypes.Binary;

        /// <summary>Sets the name of the next field by writing a length-prefixed string.</summary>
        public override void SetNextName(string name)
        {
            var bytes = Encoding.UTF8.GetBytes(name);
            WriteVarint32((uint)bytes.Length);
            _writer.Write(bytes);
        }

        /// <summary>Begins a nested object node by increasing depth counter.</summary>
        public override void StartNode()
        {
            _nodeDepth++;
        }

        /// <summary>Ends a nested object node by decreasing depth counter.</summary>
        public override void FinishNode()
        {
            _nodeDepth--;
        }

        /// <summary>Processes an integer value (4 bytes, little-endian).</summary>
        public override bool Process(ref int value)
        {
            _writer.Write(value);
            return true;
        }

        /// <summary>Processes a variable-length integer value using 7-bit encoding.</summary>
        public override bool Process(ref Varint32 value)
        {
            WriteVarint32(value.Value);
            return true;
        }

        /// <summary>Processes a size tag value using varint encoding.</summary>
        public override bool Process(ref SizeTag sizeTag)
        {
            WriteVarint32(sizeTag.Size);
            return true;
        }

        /// <summary>Processes a boolean value as a single byte (0 or 1).</summary>
        public override bool Process(ref bool value)
        {
            _writer.Write(value ? (byte)1 : (byte)0);
            return true;
        }

        /// <summary>Processes a float value (4 bytes, IEEE 754).</summary>
        public override bool Process(ref float value)
        {
            _writer.Write(value);
            return true;
        }

        /// <summary>Processes a double value (8 bytes, IEEE 754).</summary>
        public override bool Process(ref double value)
        {
            _writer.Write(value);
            return true;
        }

        /// <summary>Processes a string value as length-prefixed UTF-8 bytes.</summary>
        public override bool Process(ref string str)
        {
            if (str == null)
            {
                WriteVarint32(0);
                return true;
            }

            var bytes = Encoding.UTF8.GetBytes(str);
            WriteVarint32((uint)bytes.Length);
            _writer.Write(bytes);
            return true;
        }

        /// <summary>Processes a byte array value as length-prefixed bytes.</summary>
        public override bool Process(ref byte[] data)
        {
            if (data == null)
            {
                WriteVarint32(0);
                return true;
            }

            WriteVarint32((uint)data.Length);
            _writer.Write(data);
            return true;
        }

        /// <summary>Processes a Unity object reference by writing its index as varint.</summary>
        public override bool Process(ref UnityEngine.Object unityObject)
        {
            var index = AddUnityObjectReference(unityObject);
            WriteVarint32(index);
            return true;
        }

        /// <summary>Releases resources used by the archive.</summary>
        public override void Dispose()
        {
            _writer?.Dispose();
        }

        /// <summary>Writes a 32-bit unsigned integer using variable-length encoding.</summary>
        /// <param name="value">The value to write.</param>
        private void WriteVarint32(uint value)
        {
            while (value > 0x7F)
            {
                _writer.Write((byte)((value & 0x7F) | 0x80));
                value >>= 7;
            }
            _writer.Write((byte)value);
        }
    }
}
