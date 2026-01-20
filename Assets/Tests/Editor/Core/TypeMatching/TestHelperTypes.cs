using System.Collections.Generic;

namespace Tests.Core.TypeMatching
{
    public class TestClass
    {
    }

    /// <summary>
    /// Test interface for handler types.
    /// </summary>
    /// <typeparam name="T">The type this handler handles.</typeparam>
    public interface IHandler<T>
    {
    }

    /// <summary>
    /// Generic handler that can handle any type T.
    /// </summary>
    /// <typeparam name="T">The type to handle.</typeparam>
    public class GenericHandler<T> : IHandler<T>
    {
    }

    /// <summary>
    /// Handler that expects an array type T[] as target.
    /// </summary>
    /// <typeparam name="T">The element type of the array.</typeparam>
    public class ArrayHandler<T> : IHandler<T[]>
    {
    }

    /// <summary>
    /// Handler that expects a List&lt;T&gt; type as target.
    /// </summary>
    /// <typeparam name="T">The element type of the list.</typeparam>
    public class ListHandler<T> : IHandler<List<T>>
    {
    }

    /// <summary>
    /// Handler that expects an IList&lt;T&gt; type as target.
    /// </summary>
    /// <typeparam name="T">The element type of the list.</typeparam>
    public class IListHandler<T> : IHandler<IList<T>>
    {
    }

    /// <summary>
    /// Handler specifically for int type.
    /// </summary>
    public class IntHandler : IHandler<int>
    {
    }

    /// <summary>
    /// Handler specifically for string type.
    /// </summary>
    public class StringHandler : IHandler<string>
    {
    }

    /// <summary>
    /// Handler specifically for int[] type.
    /// </summary>
    public class IntArrayHandler : IHandler<int[]>
    {
    }

    /// <summary>
    /// Handler specifically for string[] type.
    /// </summary>
    public class StringArrayHandler : IHandler<string[]>
    {
    }

    /// <summary>
    /// High priority handler for int type (priority 100).
    /// </summary>
    public class HighPriorityIntHandler : IHandler<int>
    {
    }

    /// <summary>
    /// Low priority handler for int type (priority 0).
    /// </summary>
    public class LowPriorityIntHandler : IHandler<int>
    {
    }

    /// <summary>
    /// Test class with struct constraint on generic parameter.
    /// </summary>
    public class StructConstraintContainer<T> where T : struct { }

    /// <summary>
    /// Test class with new() constraint on generic parameter.
    /// </summary>
    public class NewConstraintContainer<T> where T : new() { }

    /// <summary>
    /// Handler with dual type parameters where T2 is constrained to be IList&lt;T1&gt;.
    /// When matching against List&lt;int&gt;, T1 should be inferred as int and T2 as List&lt;int&gt;.
    /// </summary>
    /// <typeparam name="T1">The element type that T2's list contains.</typeparam>
    /// <typeparam name="T2">A list type containing elements of type T1.</typeparam>
    public class DualParameterListHandler<T1, T2> : IHandler<T1> where T2 : IList<T1>
    {
    }

    /// <summary>
    /// Handler with dual type parameters where T2 is constrained to be IEnumerable&lt;T1&gt;.
    /// When matching against List&lt;string&gt;, T1 should be inferred as string and T2 as List&lt;string&gt;.
    /// </summary>
    /// <typeparam name="T1">The element type that T2's enumerable contains.</typeparam>
    /// <typeparam name="T2">An enumerable type containing elements of type T1.</typeparam>
    public class DualParameterEnumerableHandler<T1, T2> : IHandler<IList<T1>> where T2 : IEnumerable<T1>
    {
    }
}
