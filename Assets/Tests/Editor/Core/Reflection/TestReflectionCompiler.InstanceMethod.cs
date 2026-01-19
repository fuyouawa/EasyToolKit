using System;
using System.Reflection;
using NUnit.Framework;
using EasyToolKit.Core.Reflection;

namespace Tests.Core.Reflection
{
    public class TestReflectionCompiler_InstanceMethod
    {
        #region Instance Method Tests

        /// <summary>
        /// Verifies that CreateInstanceMethodInvoker creates an invoker that calls the instance method correctly.
        /// </summary>
        [Test]
        public void CreateInstanceMethodInvoker_VoidMethod_CallsMethod()
        {
            // Arrange
            var testInstance = new TestClass();
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.InstanceMethod),
                MemberAccessFlags.PublicInstance);

            // Act
            var invoker = ReflectionCompiler.CreateInstanceMethodInvoker(methodInfo);
            var result = invoker(testInstance);

            // Assert
            Assert.AreEqual(100, result);
        }

        /// <summary>
        /// Verifies that CreateInstanceMethodInvoker creates an invoker that passes arguments correctly.
        /// </summary>
        [Test]
        public void CreateInstanceMethodInvoker_MethodWithArgs_PassesArguments()
        {
            // Arrange
            var testInstance = new TestClass();
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.InstanceMethodWithArgs),
                MemberAccessFlags.PublicInstance);

            // Act
            var invoker = ReflectionCompiler.CreateInstanceMethodInvoker(methodInfo);
            var result = invoker(testInstance, 5, 6);

            // Assert
            Assert.AreEqual(30, result);
        }

        /// <summary>
        /// Verifies that CreateInstanceMethodInvoker throws ArgumentNullException when methodInfo is null.
        /// </summary>
        [Test]
        public void CreateInstanceMethodInvoker_NullMethodInfo_ThrowsArgumentNullException()
        {
            // Arrange
            var methodInfo = null as MethodInfo;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ReflectionCompiler.CreateInstanceMethodInvoker(methodInfo));
        }

        /// <summary>
        /// Verifies that CreateInstanceMethodInvoker throws ArgumentException when method is static.
        /// </summary>
        [Test]
        public void CreateInstanceMethodInvoker_StaticMethod_ThrowsArgumentException()
        {
            // Arrange
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.StaticMethod),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionCompiler.CreateInstanceMethodInvoker(methodInfo));
            Assert.That(ex.Message, Does.Contain("not an instance method"));
        }

        #endregion

        #region Instance Void Method Tests

        /// <summary>
        /// Verifies that CreateInstanceVoidMethodInvoker creates an invoker that calls the instance void method correctly.
        /// </summary>
        [Test]
        public void CreateInstanceVoidMethodInvoker_VoidMethod_CallsMethod()
        {
            // Arrange
            var testInstance = new TestClass();
            testInstance.InstanceField = 0;
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.InstanceVoidMethod),
                MemberAccessFlags.PublicInstance);

            // Act
            var invoker = ReflectionCompiler.CreateInstanceVoidMethodInvoker(methodInfo);
            invoker(testInstance);

            // Assert
            Assert.AreEqual(200, testInstance.InstanceField);
        }

        /// <summary>
        /// Verifies that CreateInstanceVoidMethodInvoker creates an invoker that passes arguments correctly.
        /// </summary>
        [Test]
        public void CreateInstanceVoidMethodInvoker_MethodWithArgs_PassesArguments()
        {
            // Arrange
            var testInstance = new TestClass();
            testInstance.InstanceField = 0;
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.InstanceVoidMethodWithArgs),
                MemberAccessFlags.PublicInstance);

            // Act
            var invoker = ReflectionCompiler.CreateInstanceVoidMethodInvoker(methodInfo);
            invoker(testInstance, 7, 8);

            // Assert
            Assert.AreEqual(56, testInstance.InstanceField);
        }

        /// <summary>
        /// Verifies that CreateInstanceVoidMethodInvoker throws ArgumentNullException when methodInfo is null.
        /// </summary>
        [Test]
        public void CreateInstanceVoidMethodInvoker_NullMethodInfo_ThrowsArgumentNullException()
        {
            // Arrange
            var methodInfo = null as MethodInfo;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ReflectionCompiler.CreateInstanceVoidMethodInvoker(methodInfo));
        }

        /// <summary>
        /// Verifies that CreateInstanceVoidMethodInvoker throws ArgumentException when method is static.
        /// </summary>
        [Test]
        public void CreateInstanceVoidMethodInvoker_StaticMethod_ThrowsArgumentException()
        {
            // Arrange
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.StaticVoidMethod),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionCompiler.CreateInstanceVoidMethodInvoker(methodInfo));
            Assert.That(ex.Message, Does.Contain("not an instance method"));
        }

        /// <summary>
        /// Verifies that CreateInstanceVoidMethodInvoker throws ArgumentException when method does not return void.
        /// </summary>
        [Test]
        public void CreateInstanceVoidMethodInvoker_NonVoidMethod_ThrowsArgumentException()
        {
            // Arrange
            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.InstanceMethod),
                MemberAccessFlags.PublicInstance);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionCompiler.CreateInstanceVoidMethodInvoker(methodInfo));
            Assert.That(ex.Message, Does.Contain("does not return void"));
        }

        #endregion
    }
}
