using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace EasyToolKit.Core
{
    public static class EasySerializerUtility
    {
        private static readonly ITypeMatcher SerializerTypeMatcher = TypeMatcherFactory.CreateDefault();

        private static readonly Dictionary<Type, IEasySerializer> SerializerByValueType =
            new Dictionary<Type, IEasySerializer>();

        static EasySerializerUtility()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract &&
                            t.IsInheritsFrom<IEasySerializer>())
                .ToArray();

            SerializerTypeMatcher.SetTypeMatchIndices(types.Select(type =>
            {
                var config = type.GetCustomAttribute<SerializerConfigurationAttribute>();
                config ??= SerializerConfigurationAttribute.Default;

                var argType = type.GetArgumentsOfInheritedOpenGenericType(typeof(EasySerializer<>));
                return new TypeMatchIndex(type, config.Priority, argType);
            }));
        }

        internal static IEasySerializer GetSerializer(Type valueType)
        {
            if (SerializerByValueType.TryGetValue(valueType, out var serializer))
            {
                return serializer;
            }

            var results = SerializerTypeMatcher.GetMatches(valueType);
            foreach (var result in results)
            {
                if (CanSerializeType(result.MatchedType, valueType))
                {
                    serializer = result.MatchedType.CreateInstance<IEasySerializer>();
                    break;
                }
            }

            SerializerByValueType[valueType] = serializer;
            return serializer;
        }

        public static EasySerializer<T> GetSerializer<T>()
        {
            var serializer = GetSerializer(typeof(T));
            return (EasySerializer<T>)serializer;
        }

        public static void ClearCache()
        {
            SerializerByValueType.Clear();
        }

        private static bool CanSerializeType(Type serializerType, Type valueType)
        {
            var serializer = (IEasySerializer)FormatterServices.GetUninitializedObject(serializerType);
            return serializer.CanSerialize(valueType);
        }
    }
}
