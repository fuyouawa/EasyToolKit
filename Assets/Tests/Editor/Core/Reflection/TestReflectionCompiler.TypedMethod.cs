using System;
using System.Reflection;
using NUnit.Framework;
using EasyToolkit.Core.Reflection;

namespace Tests.Core.Reflection
{
    /// <summary>
    /// Unit tests for strongly-typed method invoker creation in ReflectionCompiler.
    /// </summary>
    public class TestReflectionCompiler_TypedMethod
    {
        #region Static Methods with Return Value

        /// <summary>
        /// Verifies that CreateStaticMethodInvoker&lt;TResult&gt; creates an invoker that calls the static method correctly.
        /// </summary>
        [Test]
        public void CreateStaticMethodInvokerT_NoParams_CallsMethod()
        {
            // Arrange
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.StaticMethod),
                MemberAccessFlags.PublicStatic);

            // Act
            var invoker = ReflectionCompiler.CreateStaticMethodInvoker<int>(methodInfo);
            var result = invoker();

            // Assert
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Verifies that CreateStaticMethodInvoker&lt;TArg1, TResult&gt; creates an invoker that passes arguments correctly.
        /// </summary>
        [Test]
        public void CreateStaticMethodInvokerT_OneParam_PassesArguments()
        {
            // Arrange
            var methodInfo = typeof(TestClassForTypedMethods).GetMethod(nameof(TestClassForTypedMethods.StaticMethodOneParam),
                MemberAccessFlags.PublicStatic);

            // Act
            var invoker = ReflectionCompiler.CreateStaticMethodInvoker<int, int>(methodInfo);
            var result = invoker(10);

            // Assert
            Assert.AreEqual(20, result);
        }

        /// <summary>
        /// Verifies that CreateStaticMethodInvoker&lt;TArg1, TArg2, TResult&gt; creates an invoker that passes two arguments correctly.
        /// </summary>
        [Test]
        public void CreateStaticMethodInvokerT_TwoParams_PassesArguments()
        {
            // Arrange
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.StaticMethodWithArgs),
                MemberAccessFlags.PublicStatic);

            // Act
            var invoker = ReflectionCompiler.CreateStaticMethodInvoker<int, int, int>(methodInfo);
            var result = invoker(10, 20);

            // Assert
            Assert.AreEqual(30, result);
        }

        /// <summary>
        /// Verifies that CreateStaticMethodInvoker&lt;TArg1, TArg2, TArg3, TResult&gt; creates an invoker that passes three arguments correctly.
        /// </summary>
        [Test]
        public void CreateStaticMethodInvokerT_ThreeParams_PassesArguments()
        {
            // Arrange
            var methodInfo = typeof(TestClassForTypedMethods).GetMethod(nameof(TestClassForTypedMethods.StaticMethodThreeParams),
                MemberAccessFlags.PublicStatic);

            // Act
            var invoker = ReflectionCompiler.CreateStaticMethodInvoker<int, int, int, int>(methodInfo);
            var result = invoker(1, 2, 3);

            // Assert
            Assert.AreEqual(6, result);
        }

        /// <summary>
        /// Verifies that CreateStaticMethodInvoker&lt;TArg1, TArg2, TArg3, TArg4, TResult&gt; creates an invoker that passes four arguments correctly.
        /// </summary>
        [Test]
        public void CreateStaticMethodInvokerT_FourParams_PassesArguments()
        {
            // Arrange
            var methodInfo = typeof(TestClassForTypedMethods).GetMethod(nameof(TestClassForTypedMethods.StaticMethodFourParams),
                MemberAccessFlags.PublicStatic);

            // Act
            var invoker = ReflectionCompiler.CreateStaticMethodInvoker<int, int, int, int, int>(methodInfo);
            var result = invoker(1, 2, 3, 4);

            // Assert
            Assert.AreEqual(10, result);
        }

        /// <summary>
        /// Verifies that CreateStaticMethodInvoker&lt;TResult&gt; throws ArgumentNullException when methodInfo is null.
        /// </summary>
        [Test]
        public void CreateStaticMethodInvokerT_NullMethodInfo_ThrowsArgumentNullException()
        {
            // Arrange
            MethodInfo methodInfo = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateStaticMethodInvoker<int>(methodInfo));
        }

        /// <summary>
        /// Verifies that CreateStaticMethodInvoker&lt;TResult&gt; throws ArgumentException when method is not static.
        /// </summary>
        [Test]
        public void CreateStaticMethodInvokerT_InstanceMethod_ThrowsArgumentException()
        {
            // Arrange
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.InstanceMethod),
                MemberAccessFlags.PublicInstance);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateStaticMethodInvoker<int>(methodInfo));
            Assert.That(ex.Message, Does.Contain("not static"));
        }

        /// <summary>
        /// Verifies that CreateStaticMethodInvoker&lt;TArg1, TResult&gt; throws ArgumentException when parameter count is incorrect.
        /// </summary>
        [Test]
        public void CreateStaticMethodInvokerT_IncorrectParameterCount_ThrowsArgumentException()
        {
            // Arrange
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.StaticMethod),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateStaticMethodInvoker<int, int>(methodInfo));
            Assert.That(ex.Message, Does.Contain("must have 1 parameter"));
        }

        #endregion

        #region Static Void Methods without Return Value

        /// <summary>
        /// Verifies that CreateStaticVoidMethodInvoker&lt;TArg1&gt; creates an invoker that calls the static void method correctly.
        /// </summary>
        [Test]
        public void CreateStaticVoidMethodInvokerT_OneParam_CallsMethod()
        {
            // Arrange
            TestClassForTypedMethods.StaticField = 0;
            var methodInfo = typeof(TestClassForTypedMethods).GetMethod(nameof(TestClassForTypedMethods.StaticVoidMethodOneParam),
                MemberAccessFlags.PublicStatic);

            // Act
            var invoker = ReflectionCompiler.CreateStaticVoidMethodInvoker<int>(methodInfo);
            invoker(50);

            // Assert
            Assert.AreEqual(50, TestClassForTypedMethods.StaticField);
        }

        /// <summary>
        /// Verifies that CreateStaticVoidMethodInvoker&lt;TArg1, TArg2&gt; creates an invoker that passes two arguments correctly.
        /// </summary>
        [Test]
        public void CreateStaticVoidMethodInvokerT_TwoParams_PassesArguments()
        {
            // Arrange
            TestClass.StaticField = 0;
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.StaticVoidMethodWithArgs),
                MemberAccessFlags.PublicStatic);

            // Act
            var invoker = ReflectionCompiler.CreateStaticVoidMethodInvoker<int, int>(methodInfo);
            invoker(15, 25);

            // Assert
            Assert.AreEqual(40, TestClass.StaticField);
        }

        /// <summary>
        /// Verifies that CreateStaticVoidMethodInvoker&lt;TArg1, TArg2, TArg3&gt; creates an invoker that passes three arguments correctly.
        /// </summary>
        [Test]
        public void CreateStaticVoidMethodInvokerT_ThreeParams_PassesArguments()
        {
            // Arrange
            TestClassForTypedMethods.StaticField = 0;
            var methodInfo = typeof(TestClassForTypedMethods).GetMethod(nameof(TestClassForTypedMethods.StaticVoidMethodThreeParams),
                MemberAccessFlags.PublicStatic);

            // Act
            var invoker = ReflectionCompiler.CreateStaticVoidMethodInvoker<int, int, int>(methodInfo);
            invoker(10, 20, 30);

            // Assert
            Assert.AreEqual(60, TestClassForTypedMethods.StaticField);
        }

        /// <summary>
        /// Verifies that CreateStaticVoidMethodInvoker&lt;TArg1, TArg2, TArg3, TArg4&gt; creates an invoker that passes four arguments correctly.
        /// </summary>
        [Test]
        public void CreateStaticVoidMethodInvokerT_FourParams_PassesArguments()
        {
            // Arrange
            TestClassForTypedMethods.StaticField = 0;
            var methodInfo = typeof(TestClassForTypedMethods).GetMethod(nameof(TestClassForTypedMethods.StaticVoidMethodFourParams),
                MemberAccessFlags.PublicStatic);

            // Act
            var invoker = ReflectionCompiler.CreateStaticVoidMethodInvoker<int, int, int, int>(methodInfo);
            invoker(5, 10, 15, 20);

            // Assert
            Assert.AreEqual(50, TestClassForTypedMethods.StaticField);
        }

        /// <summary>
        /// Verifies that CreateStaticVoidMethodInvoker&lt;TArg1&gt; throws ArgumentNullException when methodInfo is null.
        /// </summary>
        [Test]
        public void CreateStaticVoidMethodInvokerT_NullMethodInfo_ThrowsArgumentNullException()
        {
            // Arrange
            MethodInfo methodInfo = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateStaticVoidMethodInvoker<int>(methodInfo));
        }

        /// <summary>
        /// Verifies that CreateStaticVoidMethodInvoker&lt;TArg1&gt; throws ArgumentException when method is not static.
        /// </summary>
        [Test]
        public void CreateStaticVoidMethodInvokerT_InstanceMethod_ThrowsArgumentException()
        {
            // Arrange
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.InstanceVoidMethod),
                MemberAccessFlags.PublicInstance);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateStaticVoidMethodInvoker<int>(methodInfo));
            Assert.That(ex.Message, Does.Contain("not static"));
        }

        /// <summary>
        /// Verifies that CreateStaticVoidMethodInvoker&lt;TArg1&gt; throws ArgumentException when method does not return void.
        /// </summary>
        [Test]
        public void CreateStaticVoidMethodInvokerT_NonVoidMethod_ThrowsArgumentException()
        {
            // Arrange
            var methodInfo = typeof(TestClassForTypedMethods).GetMethod(nameof(TestClassForTypedMethods.StaticMethodOneParam),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateStaticVoidMethodInvoker<int>(methodInfo));
            Assert.That(ex.Message, Does.Contain("does not return void"));
        }

        #endregion

        #region Instance Methods with Return Value

        /// <summary>
        /// Verifies that CreateInstanceMethodInvoker&lt;TInstance, TResult&gt; creates an invoker that calls the instance method correctly.
        /// </summary>
        [Test]
        public void CreateInstanceMethodInvokerT_NoParams_CallsMethod()
        {
            // Arrange
            var testInstance = new TestClass();
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.InstanceMethod),
                MemberAccessFlags.PublicInstance);

            // Act
            var invoker = ReflectionCompiler.CreateInstanceMethodInvoker<TestClass, int>(methodInfo);
            var result = invoker(ref testInstance);

            // Assert
            Assert.AreEqual(100, result);
        }

        /// <summary>
        /// Verifies that CreateInstanceMethodInvoker&lt;TInstance, TArg1, TResult&gt; creates an invoker that passes arguments correctly.
        /// </summary>
        [Test]
        public void CreateInstanceMethodInvokerT_OneParam_PassesArguments()
        {
            // Arrange
            var testInstance = new TestClassForTypedMethods();
            var methodInfo = typeof(TestClassForTypedMethods).GetMethod(nameof(TestClassForTypedMethods.InstanceMethodOneParam),
                MemberAccessFlags.PublicInstance);

            // Act
            var invoker = ReflectionCompiler.CreateInstanceMethodInvoker<TestClassForTypedMethods, int, int>(methodInfo);
            var result = invoker(ref testInstance, 10);

            // Assert
            Assert.AreEqual(20, result);
        }

        /// <summary>
        /// Verifies that CreateInstanceMethodInvoker&lt;TInstance, TArg1, TArg2, TResult&gt; creates an invoker that passes two arguments correctly.
        /// </summary>
        [Test]
        public void CreateInstanceMethodInvokerT_TwoParams_PassesArguments()
        {
            // Arrange
            var testInstance = new TestClass();
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.InstanceMethodWithArgs),
                MemberAccessFlags.PublicInstance);

            // Act
            var invoker = ReflectionCompiler.CreateInstanceMethodInvoker<TestClass, int, int, int>(methodInfo);
            var result = invoker(ref testInstance, 5, 6);

            // Assert
            Assert.AreEqual(30, result);
        }

        /// <summary>
        /// Verifies that CreateInstanceMethodInvoker&lt;TInstance, TArg1, TArg2, TArg3, TResult&gt; creates an invoker that passes three arguments correctly.
        /// </summary>
        [Test]
        public void CreateInstanceMethodInvokerT_ThreeParams_PassesArguments()
        {
            // Arrange
            var testInstance = new TestClassForTypedMethods();
            var methodInfo = typeof(TestClassForTypedMethods).GetMethod(nameof(TestClassForTypedMethods.InstanceMethodThreeParams),
                MemberAccessFlags.PublicInstance);

            // Act
            var invoker = ReflectionCompiler.CreateInstanceMethodInvoker<TestClassForTypedMethods, int, int, int, int>(methodInfo);
            var result = invoker(ref testInstance, 2, 3, 4);

            // Assert
            Assert.AreEqual(9, result);
        }

        /// <summary>
        /// Verifies that CreateInstanceMethodInvoker&lt;TInstance, TArg1, TArg2, TArg3, TArg4, TResult&gt; creates an invoker that passes four arguments correctly.
        /// </summary>
        [Test]
        public void CreateInstanceMethodInvokerT_FourParams_PassesArguments()
        {
            // Arrange
            var testInstance = new TestClassForTypedMethods();
            var methodInfo = typeof(TestClassForTypedMethods).GetMethod(nameof(TestClassForTypedMethods.InstanceMethodFourParams),
                MemberAccessFlags.PublicInstance);

            // Act
            var invoker = ReflectionCompiler.CreateInstanceMethodInvoker<TestClassForTypedMethods, int, int, int, int, int>(methodInfo);
            var result = invoker(ref testInstance, 1, 2, 3, 4);

            // Assert
            Assert.AreEqual(10, result);
        }

        /// <summary>
        /// Verifies that CreateInstanceMethodInvoker&lt;TInstance, TResult&gt; throws ArgumentNullException when methodInfo is null.
        /// </summary>
        [Test]
        public void CreateInstanceMethodInvokerT_NullMethodInfo_ThrowsArgumentNullException()
        {
            // Arrange
            MethodInfo methodInfo = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateInstanceMethodInvoker<TestClass, int>(methodInfo));
        }

        /// <summary>
        /// Verifies that CreateInstanceMethodInvoker&lt;TInstance, TResult&gt; throws ArgumentException when method is static.
        /// </summary>
        [Test]
        public void CreateInstanceMethodInvokerT_StaticMethod_ThrowsArgumentException()
        {
            // Arrange
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.StaticMethod),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateInstanceMethodInvoker<TestClass, int>(methodInfo));
            Assert.That(ex.Message, Does.Contain("not an instance method"));
        }

        /// <summary>
        /// Verifies that CreateInstanceMethodInvoker&lt;TInstance, TArg1, TResult&gt; throws ArgumentException when parameter count is incorrect.
        /// </summary>
        [Test]
        public void CreateInstanceMethodInvokerT_IncorrectParameterCount_ThrowsArgumentException()
        {
            // Arrange
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.InstanceMethod),
                MemberAccessFlags.PublicInstance);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateInstanceMethodInvoker<TestClass, int, int>(methodInfo));
            Assert.That(ex.Message, Does.Contain("must have 1 parameter"));
        }

        #endregion

        #region Instance Void Methods without Return Value

        /// <summary>
        /// Verifies that CreateInstanceVoidMethodInvoker&lt;TInstance&gt; creates an invoker that calls the instance void method correctly.
        /// </summary>
        [Test]
        public void CreateInstanceVoidMethodInvokerT_NoParams_CallsMethod()
        {
            // Arrange
            var testInstance = new TestClass { InstanceField = 0 };
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.InstanceVoidMethod),
                MemberAccessFlags.PublicInstance);

            // Act
            var invoker = ReflectionCompiler.CreateInstanceVoidMethodInvoker<TestClass>(methodInfo);
            invoker(ref testInstance);

            // Assert
            Assert.AreEqual(200, testInstance.InstanceField);
        }

        /// <summary>
        /// Verifies that CreateInstanceVoidMethodInvoker&lt;TInstance, TArg1&gt; creates an invoker that passes arguments correctly.
        /// </summary>
        [Test]
        public void CreateInstanceVoidMethodInvokerT_OneParam_PassesArguments()
        {
            // Arrange
            var testInstance = new TestClassForTypedMethods { InstanceField = 0 };
            var methodInfo = typeof(TestClassForTypedMethods).GetMethod(nameof(TestClassForTypedMethods.InstanceVoidMethodOneParam),
                MemberAccessFlags.PublicInstance);

            // Act
            var invoker = ReflectionCompiler.CreateInstanceVoidMethodInvoker<TestClassForTypedMethods, int>(methodInfo);
            invoker(ref testInstance, 75);

            // Assert
            Assert.AreEqual(75, testInstance.InstanceField);
        }

        /// <summary>
        /// Verifies that CreateInstanceVoidMethodInvoker&lt;TInstance, TArg1, TArg2&gt; creates an invoker that passes two arguments correctly.
        /// </summary>
        [Test]
        public void CreateInstanceVoidMethodInvokerT_TwoParams_PassesArguments()
        {
            // Arrange
            var testInstance = new TestClass { InstanceField = 0 };
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.InstanceVoidMethodWithArgs),
                MemberAccessFlags.PublicInstance);

            // Act
            var invoker = ReflectionCompiler.CreateInstanceVoidMethodInvoker<TestClass, int, int>(methodInfo);
            invoker(ref testInstance, 7, 8);

            // Assert
            Assert.AreEqual(56, testInstance.InstanceField);
        }

        /// <summary>
        /// Verifies that CreateInstanceVoidMethodInvoker&lt;TInstance, TArg1, TArg2, TArg3&gt; creates an invoker that passes three arguments correctly.
        /// </summary>
        [Test]
        public void CreateInstanceVoidMethodInvokerT_ThreeParams_PassesArguments()
        {
            // Arrange
            var testInstance = new TestClassForTypedMethods { InstanceField = 0 };
            var methodInfo = typeof(TestClassForTypedMethods).GetMethod(nameof(TestClassForTypedMethods.InstanceVoidMethodThreeParams),
                MemberAccessFlags.PublicInstance);

            // Act
            var invoker = ReflectionCompiler.CreateInstanceVoidMethodInvoker<TestClassForTypedMethods, int, int, int>(methodInfo);
            invoker(ref testInstance, 5, 10, 15);

            // Assert
            Assert.AreEqual(30, testInstance.InstanceField);
        }

        /// <summary>
        /// Verifies that CreateInstanceVoidMethodInvoker&lt;TInstance, TArg1, TArg2, TArg3, TArg4&gt; creates an invoker that passes four arguments correctly.
        /// </summary>
        [Test]
        public void CreateInstanceVoidMethodInvokerT_FourParams_PassesArguments()
        {
            // Arrange
            var testInstance = new TestClassForTypedMethods { InstanceField = 0 };
            var methodInfo = typeof(TestClassForTypedMethods).GetMethod(nameof(TestClassForTypedMethods.InstanceVoidMethodFourParams),
                MemberAccessFlags.PublicInstance);

            // Act
            var invoker = ReflectionCompiler.CreateInstanceVoidMethodInvoker<TestClassForTypedMethods, int, int, int, int>(methodInfo);
            invoker(ref testInstance, 1, 2, 3, 4);

            // Assert
            Assert.AreEqual(10, testInstance.InstanceField);
        }

        /// <summary>
        /// Verifies that CreateInstanceVoidMethodInvoker&lt;TInstance&gt; throws ArgumentNullException when methodInfo is null.
        /// </summary>
        [Test]
        public void CreateInstanceVoidMethodInvokerT_NullMethodInfo_ThrowsArgumentNullException()
        {
            // Arrange
            MethodInfo methodInfo = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateInstanceVoidMethodInvoker<TestClass>(methodInfo));
        }

        /// <summary>
        /// Verifies that CreateInstanceVoidMethodInvoker&lt;TInstance&gt; throws ArgumentException when method is static.
        /// </summary>
        [Test]
        public void CreateInstanceVoidMethodInvokerT_StaticMethod_ThrowsArgumentException()
        {
            // Arrange
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.StaticVoidMethod),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateInstanceVoidMethodInvoker<TestClass>(methodInfo));
            Assert.That(ex.Message, Does.Contain("not an instance method"));
        }

        /// <summary>
        /// Verifies that CreateInstanceVoidMethodInvoker&lt;TInstance&gt; throws ArgumentException when method does not return void.
        /// </summary>
        [Test]
        public void CreateInstanceVoidMethodInvokerT_NonVoidMethod_ThrowsArgumentException()
        {
            // Arrange
            var methodInfo = typeof(TestClassForTypedMethods).GetMethod(nameof(TestClassForTypedMethods.InstanceMethodOneParam),
                MemberAccessFlags.PublicInstance);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateInstanceVoidMethodInvoker<TestClassForTypedMethods>(methodInfo));
            Assert.That(ex.Message, Does.Contain("does not return void"));
        }

        #endregion
    }

    /// <summary>
    /// Helper class for testing typed method invokers with various parameter counts.
    /// </summary>
    public class TestClassForTypedMethods
    {
        public static int StaticField { get; set; }
        public int InstanceField { get; set; }

        // Static methods with return value
        public static int StaticMethodOneParam(int x) => x * 2;
        public static int StaticMethodThreeParams(int a, int b, int c) => a + b + c;
        public static int StaticMethodFourParams(int a, int b, int c, int d) => a + b + c + d;

        // Static void methods
        public static void StaticVoidMethodOneParam(int x) { StaticField = x; }
        public static void StaticVoidMethodThreeParams(int a, int b, int c) { StaticField = a + b + c; }
        public static void StaticVoidMethodFourParams(int a, int b, int c, int d) { StaticField = a + b + c + d; }

        // Instance methods with return value
        public int InstanceMethodOneParam(int x) => x * 2;
        public int InstanceMethodThreeParams(int a, int b, int c) => a + b + c;
        public int InstanceMethodFourParams(int a, int b, int c, int d) => a + b + c + d;

        // Instance void methods
        public void InstanceVoidMethodOneParam(int x) { InstanceField = x; }
        public void InstanceVoidMethodThreeParams(int a, int b, int c) { InstanceField = a + b + c; }
        public void InstanceVoidMethodFourParams(int a, int b, int c, int d) { InstanceField = a + b + c + d; }
    }
}
