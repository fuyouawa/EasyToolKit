using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Provides extension methods for collections and enumerables to enhance functionality
    /// with common operations like validation, transformation, and iteration.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Determines whether all elements in the sequence are equal using the default equality comparer.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence</typeparam>
        /// <param name="enumerator">The sequence to check</param>
        /// <returns>true if all elements are equal; otherwise, false</returns>
        public static bool IsAllSame<T>(this IEnumerable<T> enumerator)
        {
            return enumerator.IsAllSame((a, b) => EqualityComparer<T>.Default.Equals(a, b));
        }

        /// <summary>
        /// Determines whether all elements in the sequence are equal using a custom comparison function.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence</typeparam>
        /// <param name="enumerator">The sequence to check</param>
        /// <param name="comparison">The custom comparison function</param>
        /// <returns>true if all elements are equal according to the comparison function; otherwise, false</returns>
        /// <exception cref="ArgumentNullException">Thrown when enumerator is null</exception>
        public static bool IsAllSame<T>(this IEnumerable<T> enumerator, Func<T, T, bool> comparison)
        {
            if (enumerator == null)
                throw new ArgumentNullException(nameof(enumerator));

            T fst = default;
            bool hasFst = false;
            foreach (var value in enumerator)
            {
                if (!hasFst)
                {
                    fst = value;
                    hasFst = true;
                }
                else
                {
                    if (!comparison(fst, value))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Performs the specified action on each element of the sequence.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence</typeparam>
        /// <param name="enumerator">The sequence to iterate</param>
        /// <param name="callback">The action to perform on each element</param>
        public static void ForEach<T>(this IEnumerable<T> enumerator, Action<T> callback)
        {
            foreach (var elem in enumerator)
            {
                callback(elem);
            }
        }

        /// <summary>
        /// Determines whether the sequence contains any duplicate elements based on a selector function.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence</typeparam>
        /// <typeparam name="E">The type of the selected value to check for duplicates</typeparam>
        /// <param name="enumerator">The sequence to check</param>
        /// <param name="selector">The function to select the value to check for duplicates</param>
        /// <returns>true if any duplicate elements are found; otherwise, false</returns>
        public static bool HasDuplicate<T, E>(this IEnumerable<T> enumerator, Func<T, E> selector)
        {
            var set = new HashSet<E>();

            foreach (var e in enumerator)
            {
                if (!set.Add(selector(e)))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the sequence contains any duplicate elements using the default equality comparer.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence</typeparam>
        /// <param name="enumerator">The sequence to check</param>
        /// <returns>true if any duplicate elements are found; otherwise, false</returns>
        public static bool HasDuplicate<T>(this IEnumerable<T> enumerator)
        {
            return enumerator.HasDuplicate(e => e);
        }

        /// <summary>
        /// Determines whether the sequence is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence</typeparam>
        /// <param name="source">The sequence to check</param>
        /// <returns>true if the sequence is null or empty; otherwise, false</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// Determines whether the sequence is not null and contains at least one element.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence</typeparam>
        /// <param name="source">The sequence to check</param>
        /// <returns>true if the sequence is not null and contains elements; otherwise, false</returns>
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return !source.IsNullOrEmpty();
        }

        /// <summary>
        /// Converts a one-dimensional list to a two-dimensional array with specified dimensions.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list</typeparam>
        /// <param name="source">The source list to convert</param>
        /// <param name="rows">The number of rows in the resulting 2D array</param>
        /// <param name="columns">The number of columns in the resulting 2D array</param>
        /// <returns>A two-dimensional array with the specified dimensions</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when source length doesn't match rows * columns</exception>
        public static T[,] To2dArray<T>(this IList<T> source, int rows, int columns)
        {
            if (source.Count != rows * columns)
            {
                throw new ArgumentOutOfRangeException("Source length does not match the specified rows and columns.");
            }

            var result = new T[rows, columns];

            // Fill the 2D array
            for (int i = 0; i < source.Count; i++)
            {
                int row = i / columns;
                int col = i % columns;
                result[row, col] = source[i];
            }

            return result;
        }

        /// <summary>
        /// Converts a one-dimensional list to a jagged array with specified dimensions.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list</typeparam>
        /// <param name="source">The source list to convert</param>
        /// <param name="rows">The number of rows in the resulting jagged array</param>
        /// <param name="columns">The number of columns in the resulting jagged array</param>
        /// <returns>A jagged array with the specified dimensions</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when source length doesn't match rows * columns</exception>
        public static T[][] ToJaggedArray<T>(this IList<T> source, int rows, int columns)
        {
            if (source.Count != rows * columns)
            {
                throw new ArgumentOutOfRangeException("Source length does not match the specified rows and columns.");
            }

            var result = new T[rows][];

            for (int row = 0; row < rows; row++)
            {
                result[row] = new T[columns];
            }

            // Fill the jagged array
            for (int i = 0; i < source.Count; i++)
            {
                int row = i / columns;
                int col = i % columns;
                result[row][col] = source[i];
            }

            return result;
        }

        /// <summary>
        /// Creates a new array with the element at the specified index removed.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array</typeparam>
        /// <param name="array">The source array</param>
        /// <param name="index">The zero-based index of the element to remove</param>
        /// <returns>A new array with the specified element removed</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when index is out of range</exception>
        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            if (index < 0 || index >= array.Length)
            {
                throw new IndexOutOfRangeException();
            }

            T[] newArray = new T[array.Length - 1];

            Array.Copy(array, 0, newArray, 0, index);
            Array.Copy(array, index + 1, newArray, index, array.Length - index - 1);

            return newArray;
        }

        /// <summary>
        /// Resizes the list to the specified length by adding default values or removing elements.
        /// Note: This method cannot be used on arrays - use Array.Resize instead.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list</typeparam>
        /// <param name="list">The list to resize</param>
        /// <param name="length">The target length of the list</param>
        /// <exception cref="InvalidOperationException">Thrown when attempting to resize an array</exception>
        public static void Resize<T>(this IList<T> list, int length)
        {
            if (list.GetType().IsArray)
            {
                throw new InvalidOperationException("Use Array.Resize instead.");
            }

            while (list.Count < length)
            {
                list.Add(default);
            }

            while (list.Count > length)
            {
                list.RemoveAt(list.Count - 1);
            }
        }
    }
}
