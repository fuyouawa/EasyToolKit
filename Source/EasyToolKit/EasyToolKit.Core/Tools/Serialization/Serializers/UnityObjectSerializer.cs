using System;

namespace EasyToolKit.Core
{
    [SerializerConfiguration(SerializerPriorityLevel.UnityObject)]
    public class UnityObjectSerializer<T> : EasySerializer<T>
        where T : UnityEngine.Object
    {
        public override void Process(string name, ref T value, IArchive archive)
        {
            if (IsRoot)
            {
                throw new NotImplementedException();
            }

            UnityEngine.Object unityObject = value;

            archive.SetNextName(name);
            archive.Process(ref unityObject);

            if (archive.ArchiveIoType == ArchiveIoType.Input)
            {
                value = unityObject as T;
            }
        }
    }
}
