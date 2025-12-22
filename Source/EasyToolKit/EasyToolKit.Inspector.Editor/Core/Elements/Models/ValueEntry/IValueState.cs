namespace EasyToolKit.Inspector.Editor
{
    public interface IValueState
    {
        bool IsDirty { get; }
        ValueEntryState State { get; }
        void MarkDirty();
        void ClearDirty();
    }
}
