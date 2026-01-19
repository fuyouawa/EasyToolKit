using System;
using System.Reflection;
using NUnit.Framework;
using EasyToolKit.Core.Reflection;

namespace Tests.Core.Reflection
{
    public class TestReflectionCompiler_Constructor
    {
        #region Constructor Tests

        /// <summary>
        /// Verifies that CreateConstructorInvoker creates an invoker that calls the constructor correctly.
        /// </summary>
        [Test]
        public void CreateConstructorInvoker_ParameterlessConstructor_CreatesInstance()
        {
            // Arrange
            var constructorInfo = typeof(TestClassWithConstructor).GetConstructor(Type.EmptyTypes);

            // Act
            var invoker = ReflectionCompiler.CreateConstructorInvoker(constructorInfo);
            var result = invoker() as TestClassWithConstructor;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Value);
        }

        /// <summary>
        /// Verifies that CreateConstructorInvoker creates an invoker that passes arguments to the constructor.
        /// </summary>
        [Test]
        public void CreateConstructorInvoker_ConstructorWithArgs_PassesArguments()
        {
            // Arrange
            var constructorInfo = typeof(TestClassWithConstructor).GetConstructor(new[] { typeof(int) });

            // Act
            var invoker = ReflectionCompiler.CreateConstructorInvoker(constructorInfo);
            var result = invoker(42) as TestClassWithConstructor;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(42, result.Value);
        }

        /// <summary>
        /// Verifies that CreateConstructorInvoker creates an invoker that passes multiple arguments.
        /// </summary>
        [Test]
        public void CreateConstructorInvoker_ConstructorWithMultipleArgs_PassesArguments()
        {
            // Arrange
            var constructorInfo = typeof(TestClassWithConstructor).GetConstructor(new[] { typeof(int), typeof(int) });

            // Act
            var invoker = ReflectionCompiler.CreateConstructorInvoker(constructorInfo);
            var result = invoker(10, 20) as TestClassWithConstructor;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(30, result.Value);
        }

        /// <summary>
        /// Verifies that CreateConstructorInvoker works with struct types.
        /// </summary>
        [Test]
        public void CreateConstructorInvoker_StructConstructor_CreatesInstance()
        {
            // Arrange
            var constructorInfo = typeof(TestStruct).GetConstructor(new[] { typeof(int) });

            // Act
            var invoker = ReflectionCompiler.CreateConstructorInvoker(constructorInfo);
            var result = invoker(42);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<TestStruct>(result);
        }

        /// <summary>
        /// Verifies that CreateConstructorInvoker throws ArgumentNullException when constructorInfo is null.
        /// </summary>
        [Test]
        public void CreateConstructorInvoker_NullConstructorInfo_ThrowsArgumentNullException()
        {
            // Arrange
            var constructorInfo = null as ConstructorInfo;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ReflectionCompiler.CreateConstructorInvoker(constructorInfo));
        }

        /// <summary>
        /// Verifies that CreateParameterlessConstructorInvoker&lt;TReturn&gt; creates a strongly-typed invoker.
        /// </summary>
        [Test]
        public void CreateParameterlessConstructorInvokerT_ParameterlessConstructor_CreatesInstance()
        {
            // Arrange
            var constructorInfo = typeof(TestClassWithConstructor).GetConstructor(Type.EmptyTypes);

            // Act
            var invoker = ReflectionCompiler.CreateParameterlessConstructorInvoker<TestClassWithConstructor>(constructorInfo);
            var result = invoker();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Value);
        }

        /// <summary>
        /// Verifies that CreateParameterlessConstructorInvoker&lt;TReturn&gt; throws ArgumentException for parameterized constructor.
        /// </summary>
        [Test]
        public void CreateParameterlessConstructorInvokerT_ConstructorWithArgs_ThrowsArgumentException()
        {
            // Arrange
            var constructorInfo = typeof(TestClassWithConstructor).GetConstructor(new[] { typeof(int) });

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionCompiler.CreateParameterlessConstructorInvoker<TestClassWithConstructor>(constructorInfo));
            Assert.That(ex.Message, Does.Contain("must be parameterless"));
        }

        /// <summary>
        /// Verifies that CreateParameterlessConstructorInvoker&lt;TReturn&gt; throws ArgumentNullException when constructorInfo is null.
        /// </summary>
        [Test]
        public void CreateParameterlessConstructorInvokerT_NullConstructorInfo_ThrowsArgumentNullException()
        {
            // Arrange
            var constructorInfo = null as ConstructorInfo;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ReflectionCompiler.CreateParameterlessConstructorInvoker<TestClassWithConstructor>(constructorInfo));
        }

        #endregion

        #region Non-Generic ParameterlessConstructorInvoker Tests

        /// <summary>
        /// Verifies that CreateParameterlessConstructorInvoker creates an invoker for parameterless constructors.
        /// </summary>
        [Test]
        public void CreateParameterlessConstructorInvoker_ParameterlessConstructor_CreatesInstance()
        {
            // Arrange
            var constructorInfo = typeof(TestClassWithConstructor).GetConstructor(Type.EmptyTypes);

            // Act
            var invoker = ReflectionCompiler.CreateParameterlessConstructorInvoker(constructorInfo);
            var result = invoker() as TestClassWithConstructor;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Value);
        }

        /// <summary>
        /// Verifies that CreateParameterlessConstructorInvoker throws ArgumentException for parameterized constructor.
        /// </summary>
        [Test]
        public void CreateParameterlessConstructorInvoker_ConstructorWithArgs_ThrowsArgumentException()
        {
            // Arrange
            var constructorInfo = typeof(TestClassWithConstructor).GetConstructor(new[] { typeof(int) });

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionCompiler.CreateParameterlessConstructorInvoker(constructorInfo));
            Assert.That(ex.Message, Does.Contain("must be parameterless"));
        }

        /// <summary>
        /// Verifies that CreateParameterlessConstructorInvoker throws ArgumentNullException when constructorInfo is null.
        /// </summary>
        [Test]
        public void CreateParameterlessConstructorInvoker_NullConstructorInfo_ThrowsArgumentNullException()
        {
            // Arrange
            var constructorInfo = null as ConstructorInfo;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ReflectionCompiler.CreateParameterlessConstructorInvoker(constructorInfo));
        }

        /// <summary>
        /// Verifies that CreateParameterlessConstructorInvoker with autoFillParameters creates instance with default values.
        /// </summary>
        [Test]
        public void CreateParameterlessConstructorInvoker_AutoFillParameters_CreatesInstanceWithDefaults()
        {
            // Arrange
            var constructorInfo = typeof(TestClassWithConstructor).GetConstructor(new[] { typeof(int) });

            // Act
            var invoker = ReflectionCompiler.CreateParameterlessConstructorInvoker(constructorInfo, autoFillParameters: true);
            var result = invoker() as TestClassWithConstructor;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Value); // int default is 0
        }

        #endregion
    }
}
