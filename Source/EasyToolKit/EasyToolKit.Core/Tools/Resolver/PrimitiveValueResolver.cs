using System;

namespace EasyToolKit.Core
{
    public class PrimitiveValueResolver : ICodeValueResolver
    {
        private readonly string _primitiveCode;

        public PrimitiveValueResolver(string primitiveCode)
        {
            _primitiveCode = primitiveCode;
        }

        public bool HasError(out string error)
        {
            error = null;
            return false;
        }

        public object ResolveWeak(object context)
        {
            return _primitiveCode;
        }
    }

    public class PrimitiveValueResolver<T> : PrimitiveValueResolver, ICodeValueResolver<T>
    {
        public PrimitiveValueResolver(string primitiveCode) : base(primitiveCode)
        {
        }

        public T Resolve(object context)
        {
            return (T)ResolveWeak(context);
        }
    }
}
