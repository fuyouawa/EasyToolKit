using System;

namespace EasyToolKit.Core
{
    [SerializerConfiguration(SerializerPriorityLevel.Generic)]
    public class GenericSerializer<T> : EasySerializer<T>
    {
        private SerializedMemberInfo[] _serializedMemberInfos;
        private static readonly bool IsNode = IsNodeImpl(typeof(T));
        private static readonly Func<T> Constructor = CreateConstructor();

        public override bool CanSerialize(Type valueType)
        {
            return !valueType.IsBasicValueType() && !valueType.IsSubclassOf(typeof(UnityEngine.Object));
        }

        public override void Process(string name, ref T value, IArchive archive)
        {
            _serializedMemberInfos ??= Settings.SerializedMemberInfoAccessor.Get(typeof(T));

            if (value == null)
            {
                if (Constructor == null)
                {
                    throw new ArgumentException($"Type '{typeof(T)}' does not have a default constructor.");
                }

                value = Constructor();
            }

            if (!IsRoot)
            {
                if (IsNode)
                {
                    archive.SetNextName(name);
                    archive.StartNode();
                }
            }

            var archiveIoType = archive.ArchiveIoType;
            foreach (var serializedMemberInfo in _serializedMemberInfos)
            {
                var memberType = serializedMemberInfo.MemberType;
                var member = serializedMemberInfo.Member;
                var memberName = serializedMemberInfo.MemberName;
                var serializer = serializedMemberInfo.Serializer;

                object obj = null;
                if (archiveIoType == ArchiveIoType.Output)
                {
                    var getter = serializedMemberInfo.ValueGetter;
                    if (getter == null)
                    {
                        throw new ArgumentException($"Member '{member}' is not readable!");
                    }

                    obj = getter(value);
                }

                serializer.Process(memberName, ref obj, archive);

                if (archiveIoType == ArchiveIoType.Input)
                {
                    var setter = serializedMemberInfo.ValueSetter;
                    if (setter == null)
                    {
                        throw new ArgumentException($"Member '{member}' is not writable!");
                    }

                    setter(value, obj);
                }
            }

            if (!IsRoot)
            {
                if (IsNode)
                {
                    archive.FinishNode();
                }
            }
        }

        private static bool IsNodeImpl(Type type)
        {
            return (type.IsClass && type != typeof(string)) ||
                   (type.IsValueType && !type.IsPrimitive && !type.IsEnum);
        }

        private static Func<T> CreateConstructor()
        {
            try
            {
                return () => (T)Activator.CreateInstance(typeof(T));
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
