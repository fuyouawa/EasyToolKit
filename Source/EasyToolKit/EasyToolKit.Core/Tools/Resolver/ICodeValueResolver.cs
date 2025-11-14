namespace EasyToolKit.Core
{
    public interface ICodeValueResolver
    {
        bool HasError(out string error);
        object ResolveWeak(object context);
    }

    public interface ICodeValueResolver<T> : ICodeValueResolver
    {
        T Resolve(object context);
    }
}
