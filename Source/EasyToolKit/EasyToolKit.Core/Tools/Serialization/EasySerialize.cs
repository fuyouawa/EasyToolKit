using System;
using System.IO;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Main entry point for serialization and deserialization operations.
    /// </summary>
    public static class EasySerialize
    {
        /// <summary>Gets or sets the default serialization settings.</summary>
        public static EasySerializeSettings DefaultSettings { get; set; } = new EasySerializeSettings();

        /// <summary>Gets the current serialization settings.</summary>
        internal static EasySerializeSettings CurrentSettings { get; private set; }

        /// <summary>Serializes a value to the specified serialization data.</summary>
        /// <typeparam name="T">The type of value to serialize.</typeparam>
        /// <param name="value">The value to serialize.</param>
        /// <param name="serializationData">The serialization data to populate.</param>
        /// <param name="settings">Optional serialization settings.</param>
        public static void To<T>(T value, ref EasySerializationData serializationData,
            EasySerializeSettings settings = null)
        {
            To(value, typeof(T), ref serializationData, settings);
        }

        /// <summary>Deserializes a value from the specified serialization data.</summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="serializationData">The serialization data to read from.</param>
        /// <param name="settings">Optional serialization settings.</param>
        /// <returns>The deserialized value.</returns>
        public static T From<T>(ref EasySerializationData serializationData, EasySerializeSettings settings = null)
        {
            return (T)From(typeof(T), ref serializationData, settings);
        }

        /// <summary>Serializes a value to the specified serialization data.</summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="valueType">The type of value to serialize.</param>
        /// <param name="serializationData">The serialization data to populate.</param>
        /// <param name="settings">Optional serialization settings.</param>
        public static void To(object value, Type valueType, ref EasySerializationData serializationData,
            EasySerializeSettings settings = null)
        {
            if (value == null)
            {
                // Debug.LogWarning("Serialize null value!");
                serializationData.SetData(new byte[] { });
                return;
            }

            ChangeSettings(settings);

            using (var stream = new MemoryStream())
            {
                using (var arch = GetOutputArchive(serializationData.Format, stream))
                {
                    var serializer = GetSerializerWithThrow(valueType);
                    serializer.IsRoot = true;

                    serializer.Process(ref value, arch);

                    serializationData.ReferencedUnityObjects = arch.GetReferencedUnityObjects();
                }

                serializationData.SetData(stream.ToArray());
            }
        }

        /// <summary>Deserializes a value from the specified serialization data.</summary>
        /// <param name="type">The type of value to deserialize.</param>
        /// <param name="serializationData">The serialization data to read from.</param>
        /// <param name="settings">Optional serialization settings.</param>
        /// <returns>The deserialized value.</returns>
        public static object From(Type type, ref EasySerializationData serializationData,
            EasySerializeSettings settings = null)
        {
            ChangeSettings(settings);

            object res = null;
            var buf = serializationData.GetData();
            if (buf.Length == 0)
                return null;

            using (var stream = new MemoryStream(buf))
            {
                using (var arch = GetInputArchive(serializationData.Format, stream))
                {
                    arch.SetupReferencedUnityObjects(serializationData.ReferencedUnityObjects);

                    var serializer = GetSerializerWithThrow(type);
                    serializer.IsRoot = true;

                    serializer.Process(ref res, arch);
                }
            }

            return res;
        }

        /// <summary>Gets a serializer for the specified type or throws an exception.</summary>
        private static IEasySerializer GetSerializerWithThrow(Type type)
        {
            var serializer = EasySerializerUtility.GetSerializer(type);
            if (serializer == null)
            {
                throw new ArgumentException(
                    $"There is no serializer for type '{type.FullName}'." +
                    "You need to implement a 'IEasySerializer' for type.");
            }

            return serializer;
        }

        /// <summary>Changes the current settings if different from the specified settings.</summary>
        private static void ChangeSettings(EasySerializeSettings settings)
        {
            settings ??= DefaultSettings;
            if (settings != CurrentSettings)
            {
                CurrentSettings = settings;
                EasySerializerUtility.ClearCache();
            }
        }

        /// <summary>Creates an output archive for the specified format and stream.</summary>
        private static OutputArchive GetOutputArchive(EasyDataFormat format, Stream stream)
        {
            return format switch
            {
                EasyDataFormat.Binary => new BinaryOutputArchive(stream),
                EasyDataFormat.Json => new JsonOutputArchive(stream),
                EasyDataFormat.Xml => throw new NotImplementedException("XML serialization is not yet implemented."),
                EasyDataFormat.Yaml => throw new NotImplementedException("YAML serialization is not yet implemented."),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }

        /// <summary>Creates an input archive for the specified format and stream.</summary>
        private static InputArchive GetInputArchive(EasyDataFormat format, Stream stream)
        {
            return format switch
            {
                EasyDataFormat.Binary => new BinaryInputArchive(stream),
                EasyDataFormat.Json => new JsonInputArchive(stream),
                EasyDataFormat.Xml => throw new NotImplementedException("XML deserialization is not yet implemented."),
                EasyDataFormat.Yaml => throw new NotImplementedException("YAML deserialization is not yet implemented."),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }
    }
}
