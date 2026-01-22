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

    public class DualParameterListHandler<T1, T2> : IHandler<T1> where T1 : IList<T2>
    {
    }

    public class DualParameterEnumerableHandler<T1, T2> : IHandler<T1> where T1 : IEnumerable<T2>
    {
    }

    /// <summary>
    /// Handler that expects a Dictionary&lt;TKey, TValue&gt; type as target.
    /// </summary>
    /// <typeparam name="TKey">The key type of the dictionary.</typeparam>
    /// <typeparam name="TValue">The value type of the dictionary.</typeparam>
    public class DictionaryHandler<TKey, TValue> : IHandler<Dictionary<TKey, TValue>>
    {
    }

    /// <summary>
    /// Handler that expects an IDictionary&lt;TKey, TValue&gt; type as target.
    /// </summary>
    /// <typeparam name="TKey">The key type of the dictionary.</typeparam>
    /// <typeparam name="TValue">The value type of the dictionary.</typeparam>
    public class IDictionaryHandler<TKey, TValue> : IHandler<IDictionary<TKey, TValue>>
    {
    }

    /// <summary>
    /// Triple parameter handler where T1 is constrained to IDictionary&lt;TKey, TValue&gt;.
    /// </summary>
    /// <typeparam name="T1">The dictionary type (constrained to IDictionary&lt;TKey, TValue&gt;).</typeparam>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class TripleParameterDictionaryHandler<T1, TKey, TValue> : IHandler<T1>
        where T1 : IDictionary<TKey, TValue>
    {
    }
}
