using System;
using System.Reflection;
using NUnit.Framework;
using EasyToolKit.Core.Reflection;

namespace Tests.Core.Reflection
{
    public class TestClass
    {
        public static int StaticField;
        public static int StaticProperty { get; set; }
        public static int StaticMethod() => 42;
        public static int StaticMethodWithArgs(int a, int b) => a + b;
        public static void StaticVoidMethod() { StaticField = 100; }
        public static void StaticVoidMethodWithArgs(int a, int b) { StaticField = a + b; }
        public static readonly int StaticReadOnlyField = 10;
        public static int StaticWriteOnlyProperty { private get; set; }
        public static int StaticReadOnlyProperty { get; private set; } = 10;

        public int InstanceField;
        public int InstanceProperty { get; set; }
        public int InstanceMethod() => 100;
        public int InstanceMethodWithArgs(int x, int y) => x * y;
        public void InstanceVoidMethod() { InstanceField = 200; }
        public void InstanceVoidMethodWithArgs(int x, int y) { InstanceField = x * y; }

        public readonly int ReadOnlyField = 10;
        public int WriteOnlyProperty { private get; set; }
        public int ReadOnlyProperty { get; private set; } = 10;
    }

    public class TestClassWithConstructor
    {
        public int Value;

        public TestClassWithConstructor()
        {
            Value = 0;
        }

        public TestClassWithConstructor(int value)
        {
            Value = value;
        }

        public TestClassWithConstructor(int a, int b)
        {
            Value = a + b;
        }
    }

    public struct TestStruct
    {
        public int Field;

        public TestStruct(int value)
        {
            Field = value;
        }
    }

    public class TestClassForAccessor
    {
        public static int StaticField = 10;
        public static string StaticProperty { get; set; } = "Static";

        public int InstanceField = 42;
        public string InstanceProperty { get; set; } = "Instance";

        public NestedClassForAccessor Nested = new NestedClassForAccessor();

        public static NestedClassForAccessor StaticNested = new NestedClassForAccessor();
    }

    public class NestedClassForAccessor
    {
        public double NestedField = 3.14;
        public string NestedProperty { get; set; } = "Nested";
    }

    public class TestInvokerClass
    {
        public static int StaticMethodCallCount = 0;
        public int InstanceMethodCallCount = 0;

        public static string StaticMethod()
        {
            StaticMethodCallCount++;
            return "StaticResult";
        }

        public static string StaticMethodWithParams(string input, int count)
        {
            StaticMethodCallCount++;
            return $"{input}_{count}";
        }

        public static int StaticOverloadMethod() => 100;
        public static int StaticOverloadMethod(int x) => x * 2;
        public static int StaticOverloadMethod(int x, int y) => x + y;

        public string InstanceMethod()
        {
            InstanceMethodCallCount++;
            return "InstanceResult";
        }

        public string InstanceMethodWithParams(string prefix, string suffix)
        {
            InstanceMethodCallCount++;
            return $"{prefix}-{suffix}";
        }

        public int InstanceOverloadMethod() => 200;
        public int InstanceOverloadMethod(int x) => x * 3;
        public int InstanceOverloadMethod(int x, int y) => x - y;

        public NestedInvokerClass Nested = new NestedInvokerClass();

        public static NestedInvokerClass StaticNested = new NestedInvokerClass();
    }

    public class NestedInvokerClass
    {
        public double NestedMethod()
        {
            return 3.14159;
        }

        public double NestedMethodWithParams(double baseValue, double multiplier)
        {
            return baseValue * multiplier;
        }

        public string NestedProperty { get; set; } = "Nested";
    }
}
