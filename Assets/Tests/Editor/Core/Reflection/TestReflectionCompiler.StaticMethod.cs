using System;
using System.Reflection;
using NUnit.Framework;
using EasyToolKit.Core.Reflection;

namespace Tests.Core.Reflection
{
    public class TestReflectionCompiler_StaticMethod
    {
        #region Static Method Tests

        /// <summary>
        /// Verifies that CreateStaticMethodInvoker creates an invoker that calls the static method correctly.
        /// </summary>
        [Test]
        public void CreateStaticMethodInvoker_VoidMethod_CallsMethod()
        {
            // Arrange
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.StaticMethod),
                MemberAccessFlags.PublicStatic);

            // Act
            var invoker = ReflectionCompiler.CreateStaticMethodInvoker(methodInfo);
            var result = invoker();

            // Assert
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Verifies that CreateStaticMethodInvoker creates an invoker that passes arguments correctly.
        /// </summary>
        [Test]
        public void CreateStaticMethodInvoker_MethodWithArgs_PassesArguments()
        {
            // Arrange
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.StaticMethodWithArgs),
                MemberAccessFlags.PublicStatic);

            // Act
            var invoker = ReflectionCompiler.CreateStaticMethodInvoker(methodInfo);
            var result = invoker(10, 20);

            // Assert
            Assert.AreEqual(30, result);
        }

        /// <summary>
        /// Verifies that CreateStaticMethodInvoker throws ArgumentNullException when methodInfo is null.
        /// </summary>
        [Test]
        public void CreateStaticMethodInvoker_NullMethodInfo_ThrowsArgumentNullException()
        {
            // Arrange
            var methodInfo = null as MethodInfo;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateStaticMethodInvoker(methodInfo));
        }

        /// <summary>
        /// Verifies that CreateStaticMethodInvoker throws ArgumentException when method is not static.
        /// </summary>
        [Test]
        public void CreateStaticMethodInvoker_InstanceMethod_ThrowsArgumentException()
        {
            // Arrange
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.InstanceMethod),
                MemberAccessFlags.PublicInstance);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionCompiler.CreateStaticMethodInvoker(methodInfo));
            Assert.That(ex.Message, Does.Contain("not static"));
        }

        #endregion

        #region Static Void Method Tests

        /// <summary>
        /// Verifies that CreateStaticVoidMethodInvoker creates an invoker that calls the static void method correctly.
        /// </summary>
        [Test]
        public void CreateStaticVoidMethodInvoker_VoidMethod_CallsMethod()
        {
            // Arrange
            TestClass.StaticField = 0;
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.StaticVoidMethod),
                MemberAccessFlags.PublicStatic);

            // Act
            var invoker = ReflectionCompiler.CreateStaticVoidMethodInvoker(methodInfo);
            invoker();

            // Assert
            Assert.AreEqual(100, TestClass.StaticField);
        }

        /// <summary>
        /// Verifies that CreateStaticVoidMethodInvoker creates an invoker that passes arguments correctly.
        /// </summary>
        [Test]
        public void CreateStaticVoidMethodInvoker_MethodWithArgs_PassesArguments()
        {
            // Arrange
            TestClass.StaticField = 0;
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.StaticVoidMethodWithArgs),
                MemberAccessFlags.PublicStatic);

            // Act
            var invoker = ReflectionCompiler.CreateStaticVoidMethodInvoker(methodInfo);
            invoker(15, 25);

            // Assert
            Assert.AreEqual(40, TestClass.StaticField);
        }

        /// <summary>
        /// Verifies that CreateStaticVoidMethodInvoker throws ArgumentNullException when methodInfo is null.
        /// </summary>
        [Test]
        public void CreateStaticVoidMethodInvoker_NullMethodInfo_ThrowsArgumentNullException()
        {
            // Arrange
            var methodInfo = null as MethodInfo;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateStaticVoidMethodInvoker(methodInfo));
        }

        /// <summary>
        /// Verifies that CreateStaticVoidMethodInvoker throws ArgumentException when method is not static.
        /// </summary>
        [Test]
        public void CreateStaticVoidMethodInvoker_InstanceMethod_ThrowsArgumentException()
        {
            // Arrange
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.InstanceVoidMethod),
                MemberAccessFlags.PublicInstance);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionCompiler.CreateStaticVoidMethodInvoker(methodInfo));
            Assert.That(ex.Message, Does.Contain("not static"));
        }

        /// <summary>
        /// Verifies that CreateStaticVoidMethodInvoker throws ArgumentException when method does not return void.
        /// </summary>
        [Test]
        public void CreateStaticVoidMethodInvoker_NonVoidMethod_ThrowsArgumentException()
        {
            // Arrange
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.StaticMethod),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionCompiler.CreateStaticVoidMethodInvoker(methodInfo));
            Assert.That(ex.Message, Does.Contain("does not return void"));
        }

        #endregion
    }
}
