using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Abstract base class for output archives. Tracks Unity object references
    /// and provides common functionality for concrete archive implementations.
    /// </summary>
    internal abstract class OutputArchive : IArchive
    {
        private readonly List<UnityEngine.Object> _referencedUnityObjects = new List<UnityEngine.Object>();

        /// <summary>Gets the I/O type (Output) for this archive.</summary>
        public ArchiveIoType ArchiveIoType => ArchiveIoType.Output;

        /// <summary>Gets the archive format type.</summary>
        public abstract ArchiveTypes ArchiveType { get; }

        /// <summary>Gets the list of Unity objects referenced during serialization.</summary>
        public List<UnityEngine.Object> GetReferencedUnityObjects()
        {
            return _referencedUnityObjects;
        }

        /// <summary>Adds a Unity object to the references list and returns its index.</summary>
        /// <param name="unityObject">The Unity object to reference.</param>
        /// <returns>1-based index of the object (0 for null).</returns>
        protected uint AddUnityObjectReference(UnityEngine.Object unityObject)
        {
            if (unityObject == null)
            {
                return 0;
            }

            var idx = _referencedUnityObjects.Count + 1;
            _referencedUnityObjects.Add(unityObject);
            return (uint)idx;
        }

        /// <summary>Sets the name of the next field to be processed.</summary>
        public abstract void SetNextName(string name);

        /// <summary>Begins a nested object node.</summary>
        public abstract void StartNode();

        /// <summary>Ends a nested object node.</summary>
        public abstract void FinishNode();

        /// <summary>Processes an integer value.</summary>
        public abstract bool Process(ref int value);

        /// <summary>Processes a variable-length integer value.</summary>
        public abstract bool Process(ref Varint32 value);

        /// <summary>Processes a size tag value.</summary>
        public abstract bool Process(ref SizeTag sizeTag);

        /// <summary>Processes a boolean value.</summary>
        public abstract bool Process(ref bool value);

        /// <summary>Processes a float value.</summary>
        public abstract bool Process(ref float value);

        /// <summary>Processes a double value.</summary>
        public abstract bool Process(ref double value);

        /// <summary>Processes a string value.</summary>
        public abstract bool Process(ref string str);

        /// <summary>Processes a byte array value.</summary>
        public abstract bool Process(ref byte[] data);

        /// <summary>Processes a Unity object reference using index-based tracking.</summary>
        public abstract bool Process(ref UnityEngine.Object unityObject);

        /// <summary>Releases resources used by the archive.</summary>
        public abstract void Dispose();
    }
}
