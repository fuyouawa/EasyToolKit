using System;
using JetBrains.Annotations;

namespace EasyToolKit.Core
{
    public static class CodeValueResolver
    {
        public static ICodeValueResolver CreateWeak(
            string code,
            [CanBeNull] Type targetType,
            bool needStartFlag = false,
            Type resultType = null)
        {
            if (code.IsNullOrWhiteSpace())
            {
                return new PrimitiveValueResolver(code);
            }

            if (needStartFlag)
            {
                if (code.StartsWith("@"))
                {
                    return new ExpressionValueResolver(code[1..], resultType, targetType);
                }

                if (resultType != null && resultType != typeof(string))
                {
                    throw new ArgumentException($"When using primitive code, the result type '{resultType}' must be string.");
                }
                return new PrimitiveValueResolver(code);
            }

            return new ExpressionValueResolver(code, resultType, targetType);
        }

        public static ICodeValueResolver<T> Create<T>(string code, Type targetType = null, bool needStartFlag = false)
        {
            if (code.IsNullOrWhiteSpace())
            {
                return new PrimitiveValueResolver<T>(code);
            }

            if (needStartFlag)
            {
                if (code.StartsWith("@"))
                {
                    return new ExpressionValueResolver<T>(code[1..], targetType);
                }

                if (typeof(T) != typeof(string))
                {
                    throw new ArgumentException($"When using primitive code, the result type '{typeof(T)}' must be string.");
                }
                return new PrimitiveValueResolver<T>(code);
            }

            return new ExpressionValueResolver<T>(code, targetType);
        }
    }
}
