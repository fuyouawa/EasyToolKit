using System;
using System.Linq.Expressions;
using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Encapsulates method metadata and compiled delegate for fast invocation.
    /// </summary>
    internal sealed class HandlerInfo
    {
        /// <summary>
        /// Gets the method information.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Gets the priority of this handler.
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// Gets the metadata token for stable sorting.
        /// </summary>
        public int MethodMetadataToken { get; }

        /// <summary>
        /// Gets the compiled invoker delegate.
        /// </summary>
        private Func<object, object, object> Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerInfo"/> class.
        /// </summary>
        /// <param name="method">The method to invoke.</param>
        /// <param name="priority">The priority of this handler.</param>
        public HandlerInfo(MethodInfo method, int priority)
        {
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Priority = priority;
            MethodMetadataToken = method.MetadataToken;
            Invoker = CompileInvoker(method);
        }

        /// <summary>
        /// Invokes the handler method with the specified target and arguments.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <param name="args">The arguments to pass.</param>
        /// <returns>The return value of the method invocation.</returns>
        public object Invoke(object target, object args)
        {
            return Invoker(target, args);
        }

        /// <summary>
        /// Compiles a fast invoker delegate using expression trees.
        /// </summary>
        /// <param name="method">The method to compile.</param>
        /// <returns>A compiled delegate for fast invocation.</returns>
        private static Func<object, object, object> CompileInvoker(MethodInfo method)
        {
            // Parameters: (object target, object args)
            var targetParam = Expression.Parameter(typeof(object), "target");
            var argsParam = Expression.Parameter(typeof(object), "args");

            // Cast target to actual type
            var targetCast = Expression.Convert(targetParam, method.DeclaringType);

            // Get parameters of the method
            var parameters = method.GetParameters();
            Expression[] callArgs;

            if (parameters.Length == 0)
            {
                // Method takes no parameters
                callArgs = Array.Empty<Expression>();
            }
            else if (parameters.Length == 1)
            {
                // Method takes one parameter - cast args to parameter type
                var paramType = parameters[0].ParameterType;
                var argsCast = Expression.Convert(argsParam, paramType);
                callArgs = new[] { argsCast };
            }
            else
            {
                // Method takes multiple parameters - not supported by this dispatcher
                throw new ArgumentException($"Handler method {method.Name} must have 0 or 1 parameters.");
            }

            // Call the method
            var methodCall = Expression.Call(targetCast, method, callArgs);

            // Convert return value to object if needed
            Expression returnExpr;
            if (method.ReturnType == typeof(void))
            {
                returnExpr = Expression.Block(methodCall, Expression.Default(typeof(object)));
            }
            else
            {
                returnExpr = Expression.Convert(methodCall, typeof(object));
            }

            // Compile lambda: (object target, object args) => (object)method((TTarget)target, (TArgs)args)
            var lambda = Expression.Lambda<Func<object, object, object>>(
                returnExpr,
                targetParam,
                argsParam
            );

            return lambda.Compile();
        }
    }
}
