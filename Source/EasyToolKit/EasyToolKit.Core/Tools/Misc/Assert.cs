#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace EasyToolKit.Core
{
    public class AssertException : Exception
    {
        public AssertException(string message = null)
            : base(message.DefaultIfNullOrEmpty("Assertion failed"))
        {
        }
    }

    public static class Assert
    {
        private static void Condition([DoesNotReturnIf(false)] bool cond, string message)
        {
            if (!cond)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                throw new AssertException(message);
            }
        }

        private static void Condition([DoesNotReturnIf(false)] bool cond, Func<string> messageGetter)
        {
            if (!cond)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                throw new AssertException(messageGetter?.Invoke());
            }
        }

        /// <summary>
        /// Asserts that two values are equal.
        /// </summary>
        /// <typeparam name="T">Type of values to compare.</typeparam>
        /// <param name="x">First value.</param>
        /// <param name="y">Second value.</param>
        /// <param name="message">Error message.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsEqual<T>(T x, T y, string message = "Assert equal failed.")
        {
            Condition(EqualityComparer<T>.Default.Equals(x, y), message);
        }

        /// <summary>
        /// Asserts that two values are equal with lazy message evaluation.
        /// </summary>
        /// <typeparam name="T">Type of values to compare.</typeparam>
        /// <param name="x">First value.</param>
        /// <param name="y">Second value.</param>
        /// <param name="messageGetter">Function to generate error message only when assertion fails.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsEqual<T>(T x, T y, Func<string> messageGetter)
        {
            Condition(EqualityComparer<T>.Default.Equals(x, y), messageGetter);
        }

        /// <summary>
        /// Asserts that two values are not equal.
        /// </summary>
        /// <typeparam name="T">Type of values to compare.</typeparam>
        /// <param name="x">First value.</param>
        /// <param name="y">Second value.</param>
        /// <param name="message">Error message.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotEqual<T>(T x, T y, string message = "Assert not equal failed.")
        {
            Condition(!EqualityComparer<T>.Default.Equals(x, y), message);
        }

        /// <summary>
        /// Asserts that two values are not equal with lazy message evaluation.
        /// </summary>
        /// <typeparam name="T">Type of values to compare.</typeparam>
        /// <param name="x">First value.</param>
        /// <param name="y">Second value.</param>
        /// <param name="messageGetter">Function to generate error message only when assertion fails.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotEqual<T>(T x, T y, Func<string> messageGetter)
        {
            Condition(!EqualityComparer<T>.Default.Equals(x, y), messageGetter);
        }

        /// <summary>
        /// Asserts that an object is null.
        /// </summary>
        /// <param name="target">Object to check.</param>
        /// <param name="message">Error message.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull(object target, string message = "Assert null failed.")
        {
            Condition(target == null, message);
        }

        /// <summary>
        /// Asserts that an object is null with lazy message evaluation.
        /// </summary>
        /// <param name="target">Object to check.</param>
        /// <param name="messageGetter">Function to generate error message only when assertion fails.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull(object target, Func<string> messageGetter)
        {
            Condition(target == null, messageGetter);
        }

        /// <summary>
        /// Asserts that a value is null.
        /// </summary>
        /// <typeparam name="T">Type of value to check.</typeparam>
        /// <param name="target">Value to check.</param>
        /// <param name="message">Error message.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull<T>(T target, string message = "Assert null failed.")
        {
            Condition(target == null, message);
        }

        /// <summary>
        /// Asserts that a value is null with lazy message evaluation.
        /// </summary>
        /// <typeparam name="T">Type of value to check.</typeparam>
        /// <param name="target">Value to check.</param>
        /// <param name="messageGetter">Function to generate error message only when assertion fails.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull<T>(T target, Func<string> messageGetter)
        {
            Condition(target == null, messageGetter);
        }

        /// <summary>
        /// Asserts that an object is not null.
        /// </summary>
        /// <param name="target">Object to check.</param>
        /// <param name="message">Error message.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull(object target, string message = "Assert not null failed.")
        {
            Condition(target != null, message);
        }

        /// <summary>
        /// Asserts that an object is not null with lazy message evaluation.
        /// </summary>
        /// <param name="target">Object to check.</param>
        /// <param name="messageGetter">Function to generate error message only when assertion fails.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull(object target, Func<string> messageGetter)
        {
            Condition(target != null, messageGetter);
        }

        /// <summary>
        /// Asserts that a value is not null.
        /// </summary>
        /// <typeparam name="T">Type of value to check.</typeparam>
        /// <param name="target">Value to check.</param>
        /// <param name="message">Error message.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull<T>(T target, string message = "Assert not null failed.")
        {
            Condition(target != null, message);
        }

        /// <summary>
        /// Asserts that a value is not null with lazy message evaluation.
        /// </summary>
        /// <typeparam name="T">Type of value to check.</typeparam>
        /// <param name="target">Value to check.</param>
        /// <param name="messageGetter">Function to generate error message only when assertion fails.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull<T>(T target, Func<string> messageGetter)
        {
            Condition(target != null, messageGetter);
        }

        /// <summary>
        /// Asserts that a condition is true.
        /// </summary>
        /// <param name="cond">Condition to check.</param>
        /// <param name="message">Error message.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue([DoesNotReturnIf(false)] bool cond, string message = "Assert true failed.")
        {
            Condition(cond, message);
        }

        /// <summary>
        /// Asserts that a condition is true with lazy message evaluation.
        /// </summary>
        /// <param name="cond">Condition to check.</param>
        /// <param name="messageGetter">Function to generate error message only when assertion fails.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue([DoesNotReturnIf(false)] bool cond, Func<string> messageGetter)
        {
            Condition(cond, messageGetter);
        }

        /// <summary>
        /// Asserts that a nullable boolean is true.
        /// </summary>
        /// <param name="cond">Nullable condition to check.</param>
        /// <param name="message">Error message.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue([DoesNotReturnIf(false)] bool? cond, string message = "Assert true failed.")
        {
            Condition(cond == true, message);
        }

        /// <summary>
        /// Asserts that a nullable boolean is true with lazy message evaluation.
        /// </summary>
        /// <param name="cond">Nullable condition to check.</param>
        /// <param name="messageGetter">Function to generate error message only when assertion fails.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue([DoesNotReturnIf(false)] bool? cond, Func<string> messageGetter)
        {
            Condition(cond == true, messageGetter);
        }

        /// <summary>
        /// Asserts that a condition is false.
        /// </summary>
        /// <param name="cond">Condition to check.</param>
        /// <param name="message">Error message.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse([DoesNotReturnIf(true)] bool cond, string message = "Assert false failed.")
        {
            Condition(!cond, message);
        }

        /// <summary>
        /// Asserts that a condition is false with lazy message evaluation.
        /// </summary>
        /// <param name="cond">Condition to check.</param>
        /// <param name="messageGetter">Function to generate error message only when assertion fails.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse([DoesNotReturnIf(true)] bool cond, Func<string> messageGetter)
        {
            Condition(!cond, messageGetter);
        }

        /// <summary>
        /// Asserts that a nullable boolean is false.
        /// </summary>
        /// <param name="cond">Nullable condition to check.</param>
        /// <param name="message">Error message.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse([DoesNotReturnIf(true)] bool? cond, string message = "Assert false failed.")
        {
            Condition(cond == false, message);
        }

        /// <summary>
        /// Asserts that a nullable boolean is false with lazy message evaluation.
        /// </summary>
        /// <param name="cond">Nullable condition to check.</param>
        /// <param name="messageGetter">Function to generate error message only when assertion fails.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse([DoesNotReturnIf(true)] bool? cond, Func<string> messageGetter)
        {
            Condition(cond == false, messageGetter);
        }

        /// <summary>
        /// Asserts that a string is not null or empty.
        /// </summary>
        /// <param name="value">String to check.</param>
        /// <param name="message">Error message.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNullOrEmpty(string value, string message = "Assert not null or empty failed.")
        {
            Condition(value.IsNotNullOrEmpty(), message);
        }

        /// <summary>
        /// Asserts that a string is not null or empty with lazy message evaluation.
        /// </summary>
        /// <param name="value">String to check.</param>
        /// <param name="messageGetter">Function to generate error message only when assertion fails.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNullOrEmpty(string value, Func<string> messageGetter)
        {
            Condition(value.IsNotNullOrEmpty(), messageGetter);
        }

        /// <summary>
        /// Asserts that a list is not null or empty.
        /// </summary>
        /// <typeparam name="T">Type of list elements.</typeparam>
        /// <param name="value">List to check.</param>
        /// <param name="message">Error message.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNullOrEmpty<T>(IList<T> value, string message = "Assert not null or empty failed.")
        {
            Condition(value.IsNotNullOrEmpty(), message);
        }

        /// <summary>
        /// Asserts that a list is not null or empty with lazy message evaluation.
        /// </summary>
        /// <typeparam name="T">Type of list elements.</typeparam>
        /// <param name="value">List to check.</param>
        /// <param name="messageGetter">Function to generate error message only when assertion fails.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNullOrEmpty<T>(IList<T> value, Func<string> messageGetter)
        {
            Condition(value.IsNotNullOrEmpty(), messageGetter);
        }

        /// <summary>
        /// Asserts that a string is null or empty.
        /// </summary>
        /// <param name="value">String to check.</param>
        /// <param name="message">Error message.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNullOrEmpty(string value, string message = "Assert not null or empty failed.")
        {
            Condition(value.IsNullOrEmpty(), message);
        }

        /// <summary>
        /// Asserts that a string is null or empty with lazy message evaluation.
        /// </summary>
        /// <param name="value">String to check.</param>
        /// <param name="messageGetter">Function to generate error message only when assertion fails.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNullOrEmpty(string value, Func<string> messageGetter)
        {
            Condition(value.IsNullOrEmpty(), messageGetter);
        }

        /// <summary>
        /// Asserts that a list is null or empty.
        /// </summary>
        /// <typeparam name="T">Type of list elements.</typeparam>
        /// <param name="value">List to check.</param>
        /// <param name="message">Error message.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNullOrEmpty<T>(IList<T> value, string message = "Assert not null or empty failed.")
        {
            Condition(value.IsNullOrEmpty(), message);
        }

        /// <summary>
        /// Asserts that a list is null or empty with lazy message evaluation.
        /// </summary>
        /// <typeparam name="T">Type of list elements.</typeparam>
        /// <param name="value">List to check.</param>
        /// <param name="messageGetter">Function to generate error message only when assertion fails.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNullOrEmpty<T>(IList<T> value, Func<string> messageGetter)
        {
            Condition(value.IsNullOrEmpty(), messageGetter);
        }
    }
}
