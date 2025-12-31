using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Defines the direction of archive operations.
    /// </summary>
    public enum ArchiveIoType
    {
        /// <summary>Reading from archive.</summary>
        Input,
        /// <summary>Writing to archive.</summary>
        Output
    }

    /// <summary>
    /// Defines the archive format type.
    /// </summary>
    public enum ArchiveTypes
    {
        /// <summary>Binary format.</summary>
        Binary,
        /// <summary>JSON format (not yet implemented).</summary>
        Json,
        /// <summary>XML format (not yet implemented).</summary>
        Xml,
        /// <summary>YAML format (not yet implemented).</summary>
        Yaml
    }

    /// <summary>
    /// Variable-length integer encoding for efficient storage of small integers.
    /// Uses 7-bit encoding where each byte contains 7 bits of data and a continuation bit.
    /// </summary>
    public struct Varint32
    {
        /// <summary>The decoded integer value.</summary>
        public uint Value { get; set; }

        /// <summary>Creates a new Varint32 with the specified value.</summary>
        public Varint32(uint value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Size prefix tag for collections and arrays.
    /// </summary>
    public struct SizeTag
    {
        /// <summary>The size of the collection.</summary>
        public uint Size { get; set; }

        /// <summary>Creates a new SizeTag with the specified size.</summary>
        public SizeTag(uint size)
        {
            Size = size;
        }
    }

    /// <summary>
    /// Interface for serialization archives. Provides methods for reading and writing
    /// structured data in various formats (binary, JSON, etc.).
    /// </summary>
    public interface IArchive : IDisposable
    {
        /// <summary>Gets the I/O type (input or output) of this archive.</summary>
        ArchiveIoType ArchiveIoType { get; }

        /// <summary>Gets the format type of this archive.</summary>
        ArchiveTypes ArchiveType { get; }

        /// <summary>Sets the name of the next field to be processed.</summary>
        /// <param name="name">The field name.</param>
        void SetNextName(string name);

        /// <summary>Begins a nested object node. Increases depth tracking.</summary>
        void StartNode();

        /// <summary>Ends a nested object node. Decreases depth tracking.</summary>
        void FinishNode();

        /// <summary>Processes an integer value.</summary>
        bool Process(ref int value);

        /// <summary>Processes a variable-length integer value.</summary>
        bool Process(ref Varint32 value);

        /// <summary>Processes a size tag value.</summary>
        bool Process(ref SizeTag sizeTag);

        /// <summary>Processes a boolean value.</summary>
        bool Process(ref bool value);

        /// <summary>Processes a float value.</summary>
        bool Process(ref float value);

        /// <summary>Processes a double value.</summary>
        bool Process(ref double value);

        /// <summary>Processes a string value.</summary>
        bool Process(ref string str);

        /// <summary>Processes a byte array value.</summary>
        bool Process(ref byte[] data);

        /// <summary>Processes a Unity object reference using index-based tracking.</summary>
        bool Process(ref UnityEngine.Object unityObject);
    }
}
