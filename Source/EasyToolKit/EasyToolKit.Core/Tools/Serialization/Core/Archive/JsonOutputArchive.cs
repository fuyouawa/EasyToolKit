using System;
using System.IO;
using UnityEngine;

namespace EasyToolKit.Core
{
    /// <summary>
    /// JSON output archive stub. JSON serialization is not yet implemented.
    /// </summary>
    internal class JsonOutputArchive : OutputArchive
    {
        /// <summary>Creates a new JSON output archive (not implemented).</summary>
        /// <param name="stream">The stream to write to.</param>
        /// <exception cref="NotImplementedException">Always thrown.</exception>
        public JsonOutputArchive(Stream stream)
        {
            throw new NotImplementedException("JSON serialization is not yet implemented.");
        }

        /// <summary>Gets the archive format type (Json).</summary>
        public override ArchiveTypes ArchiveType => ArchiveTypes.Json;

        public override void SetNextName(string name) =>
            throw new NotImplementedException("JSON serialization is not yet implemented.");

        public override void StartNode() =>
            throw new NotImplementedException("JSON serialization is not yet implemented.");

        public override void FinishNode() =>
            throw new NotImplementedException("JSON serialization is not yet implemented.");

        public override bool Process(ref int value) =>
            throw new NotImplementedException("JSON serialization is not yet implemented.");

        public override bool Process(ref Varint32 value) =>
            throw new NotImplementedException("JSON serialization is not yet implemented.");

        public override bool Process(ref SizeTag sizeTag) =>
            throw new NotImplementedException("JSON serialization is not yet implemented.");

        public override bool Process(ref bool value) =>
            throw new NotImplementedException("JSON serialization is not yet implemented.");

        public override bool Process(ref float value) =>
            throw new NotImplementedException("JSON serialization is not yet implemented.");

        public override bool Process(ref double value) =>
            throw new NotImplementedException("JSON serialization is not yet implemented.");

        public override bool Process(ref string str) =>
            throw new NotImplementedException("JSON serialization is not yet implemented.");

        public override bool Process(ref byte[] data) =>
            throw new NotImplementedException("JSON serialization is not yet implemented.");

        public override bool Process(ref UnityEngine.Object unityObject) =>
            throw new NotImplementedException("JSON serialization is not yet implemented.");

        public override void Dispose()
        {
        }
    }
}
