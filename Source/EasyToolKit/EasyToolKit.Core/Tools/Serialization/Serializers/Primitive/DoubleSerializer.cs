namespace EasyToolKit.Core
{
    [SerializerConfiguration(SerializerPriorityLevel.Primitive)]
    public class DoubleSerializer : EasySerializer<double>
    {
        public override void Process(string name, ref double value, IArchive archive)
        {
            archive.SetNextName(name);
            archive.Process(ref value);
        }
    }
}
