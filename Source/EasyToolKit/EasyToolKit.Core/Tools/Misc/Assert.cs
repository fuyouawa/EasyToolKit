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

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsEqual<T>(T x, T y, string message = "Assert equal failed.")
        {
            Condition(EqualityComparer<T>.Default.Equals(y), message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotEqual<T>(T x, T y, string message = "Assert not equal failed.")
        {
            Condition(!EqualityComparer<T>.Default.Equals(y), message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull(object target, string message = "Assert null failed.")
        {
            Condition(target == null, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull<T>(T target, string message = "Assert null failed.")
        {
            Condition(target == null, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull(object target, string message = "Assert not null failed.")
        {
            Condition(target != null, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull<T>(T target, string message = "Assert not null failed.")
        {
            Condition(target != null, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue([DoesNotReturnIf(false)] bool cond, string message = "Assert true failed.")
        {
            Condition(cond, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue([DoesNotReturnIf(false)] bool? cond, string message = "Assert true failed.")
        {
            Condition(cond == true, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse([DoesNotReturnIf(true)] bool cond, string message = "Assert false failed.")
        {
            Condition(!cond, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse([DoesNotReturnIf(true)] bool? cond, string message = "Assert false failed.")
        {
            Condition(cond == false, message);
        }
    }
}
