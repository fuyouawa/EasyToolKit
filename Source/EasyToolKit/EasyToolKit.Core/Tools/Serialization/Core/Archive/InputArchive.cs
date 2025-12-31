using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Abstract base class for input archives. Manages Unity object references
    /// and provides common functionality for concrete archive implementations.
    /// </summary>
    internal abstract class InputArchive : IArchive
    {
        private List<UnityEngine.Object> _referencedUnityObjects;

        /// <summary>Gets the I/O type (Input) for this archive.</summary>
        public ArchiveIoType ArchiveIoType => ArchiveIoType.Input;

        /// <summary>Gets the archive format type.</summary>
        public abstract ArchiveTypes ArchiveType { get; }

        /// <summary>Sets up the list of Unity objects for reference resolution during deserialization.</summary>
        /// <param name="referencedUnityObjects">The list of Unity objects referenced in the serialized data.</param>
        public void SetupReferencedUnityObjects(List<UnityEngine.Object> referencedUnityObjects)
        {
            _referencedUnityObjects = referencedUnityObjects;
        }

        /// <summary>Resolves a Unity object reference by index.</summary>
        /// <param name="index">1-based index of the object (0 for null).</param>
        /// <returns>The resolved Unity object, or null if index is 0.</returns>
        protected UnityEngine.Object ResolveUnityObjectReference(uint index)
        {
            if (index == 0)
            {
                return null;
            }

            if (_referencedUnityObjects == null || index > _referencedUnityObjects.Count)
            {
                return null;
            }

            return _referencedUnityObjects[(int)index - 1];
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

        /// <summary>Processes a Unity object reference using index-based resolution.</summary>
        public abstract bool Process(ref UnityEngine.Object unityObject);

        /// <summary>Releases resources used by the archive.</summary>
        public abstract void Dispose();
    }
}
