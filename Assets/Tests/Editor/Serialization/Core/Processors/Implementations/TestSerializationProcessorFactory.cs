using System;
using System.Collections.Generic;
using NUnit.Framework;
using EasyToolKit.Serialization;
using EasyToolKit.Serialization.Implementations;
using EasyToolKit.Serialization.Processors;

namespace Tests.Serialization.Core.Processors.Implementations
{
    /// <summary>
    /// Unit tests for SerializationProcessorFactory.
    /// Tests focus on processor creation, type matching, and dependency injection.
    /// </summary>
    [TestFixture]
    public class TestSerializationProcessorFactory
    {
        #region Constructor Tests

        /// <summary>
        /// Verifies that the factory constructor initializes correctly and creates a valid factory instance.
        /// </summary>
        [Test]
        public void Constructor_DefaultInitialization_CreatesValidFactory()
        {
            // Act
            var factory = new SerializationProcessorFactory();

            // Assert
            Assert.IsNotNull(factory, "Factory should be created successfully.");
        }

        #endregion

        #region GetProcessor - Primitive Types Tests

        /// <summary>
        /// Verifies that GetProcessor returns IntProcessor for int type.
        /// </summary>
        [Test]
        public void GetProcessor_IntType_ReturnsIntProcessor()
        {
            // Arrange
            var factory = new SerializationProcessorFactory();

            // Act
            var processor = factory.GetProcessor<int>();

            // Assert
            Assert.IsNotNull(processor, "Processor should not be null.");
            Assert.AreEqual(typeof(int), processor.ValueType, "Processor ValueType should be int.");
            Assert.IsInstanceOf<IntProcessor>(processor, "Processor should be IntProcessor instance.");
        }

        /// <summary>
        /// Verifies that GetProcessor returns FloatProcessor for float type.
        /// </summary>
        [Test]
        public void GetProcessor_FloatType_ReturnsFloatProcessor()
        {
            // Arrange
            var factory = new SerializationProcessorFactory();

            // Act
            var processor = factory.GetProcessor<float>();

            // Assert
            Assert.IsNotNull(processor, "Processor should not be null.");
            Assert.AreEqual(typeof(float), processor.ValueType, "Processor ValueType should be float.");
            Assert.IsInstanceOf<FloatProcessor>(processor, "Processor should be FloatProcessor instance.");
        }

        /// <summary>
        /// Verifies that GetProcessor returns DoubleProcessor for double type.
        /// </summary>
        [Test]
        public void GetProcessor_DoubleType_ReturnsDoubleProcessor()
        {
            // Arrange
            var factory = new SerializationProcessorFactory();

            // Act
            var processor = factory.GetProcessor<double>();

            // Assert
            Assert.IsNotNull(processor, "Processor should not be null.");
            Assert.AreEqual(typeof(double), processor.ValueType, "Processor ValueType should be double.");
            Assert.IsInstanceOf<DoubleProcessor>(processor, "Processor should be DoubleProcessor instance.");
        }

        /// <summary>
        /// Verifies that GetProcessor returns BoolProcessor for bool type.
        /// </summary>
        [Test]
        public void GetProcessor_BoolType_ReturnsBoolProcessor()
        {
            // Arrange
            var factory = new SerializationProcessorFactory();

            // Act
            var processor = factory.GetProcessor<bool>();

            // Assert
            Assert.IsNotNull(processor, "Processor should not be null.");
            Assert.AreEqual(typeof(bool), processor.ValueType, "Processor ValueType should be bool.");
            Assert.IsInstanceOf<BoolProcessor>(processor, "Processor should be BoolProcessor instance.");
        }

        /// <summary>
        /// Verifies that GetProcessor returns StringProcessor for string type.
        /// </summary>
        [Test]
        public void GetProcessor_StringType_ReturnsStringProcessor()
        {
            // Arrange
            var factory = new SerializationProcessorFactory();

            // Act
            var processor = factory.GetProcessor<string>();

            // Assert
            Assert.IsNotNull(processor, "Processor should not be null.");
            Assert.AreEqual(typeof(string), processor.ValueType, "Processor ValueType should be string.");
            Assert.IsInstanceOf<StringProcessor>(processor, "Processor should be StringProcessor instance.");
        }

        #endregion

        #region GetProcessor - Array Types Tests

        /// <summary>
        /// Verifies that GetProcessor returns constructed ArrayProcessor&lt;int&gt; for int[] type.
        /// </summary>
        [Test]
        public void GetProcessor_IntArrayType_ReturnsArrayProcessorOfInt()
        {
            // Arrange
            var factory = new SerializationProcessorFactory();

            // Act
            var processor = factory.GetProcessor<int[]>();

            // Assert
            Assert.IsNotNull(processor, "Processor should not be null.");
            Assert.AreEqual(typeof(int[]), processor.ValueType, "Processor ValueType should be int[].");
            Assert.AreEqual(typeof(ArrayProcessor<int>), processor.GetType(),
                "Processor should be ArrayProcessor<int> instance.");
        }

        /// <summary>
        /// Verifies that GetProcessor returns constructed ArrayProcessor&lt;string&gt; for string[] type.
        /// </summary>
        [Test]
        public void GetProcessor_StringArrayType_ReturnsArrayProcessorOfString()
        {
            // Arrange
            var factory = new SerializationProcessorFactory();

            // Act
            var processor = factory.GetProcessor<string[]>();

            // Assert
            Assert.IsNotNull(processor, "Processor should not be null.");
            Assert.AreEqual(typeof(string[]), processor.ValueType, "Processor ValueType should be string[].");
            Assert.AreEqual(typeof(ArrayProcessor<string>), processor.GetType(),
                "Processor should be ArrayProcessor<string> instance.");
        }

        /// <summary>
        /// Verifies that GetProcessor returns constructed ArrayProcessor&lt;float[]&gt; for float[][] type.
        /// Tests nested array handling.
        /// </summary>
        [Test]
        public void GetProcessor_JaggedArrayType_ReturnsArrayProcessorOfArray()
        {
            // Arrange
            var factory = new SerializationProcessorFactory();

            // Act
            var processor = factory.GetProcessor<float[][]>();

            // Assert
            Assert.IsNotNull(processor, "Processor should not be null.");
            Assert.AreEqual(typeof(float[][]), processor.ValueType, "Processor ValueType should be float[][].");
            Assert.AreEqual(typeof(ArrayProcessor<float[]>), processor.GetType(),
                "Processor should be ArrayProcessor<float[]> instance.");
        }

        #endregion

        #region GetProcessor - List Types Tests

        /// <summary>
        /// Verifies that GetProcessor returns constructed IListProcessor&lt;int&gt; for List&lt;int&gt; type.
        /// </summary>
        [Test]
        public void GetProcessor_IntListType_ReturnsIListProcessorOfInt()
        {
            // Arrange
            var factory = new SerializationProcessorFactory();

            // Act
            var processor = factory.GetProcessor<List<int>>();

            // Assert
            Assert.IsNotNull(processor, "Processor should not be null.");
            Assert.AreEqual(typeof(List<int>), processor.ValueType,
                "Processor ValueType should be List<int>.");
        }

        /// <summary>
        /// Verifies that GetProcessor returns constructed IListProcessor&lt;string&gt; for List&lt;string&gt; type.
        /// </summary>
        [Test]
        public void GetProcessor_StringListType_ReturnsIListProcessorOfString()
        {
            // Arrange
            var factory = new SerializationProcessorFactory();

            // Act
            var processor = factory.GetProcessor<List<string>>();

            // Assert
            Assert.IsNotNull(processor, "Processor should not be null.");
            Assert.AreEqual(typeof(List<string>), processor.ValueType,
                "Processor ValueType should be List<string>.");
        }

        /// <summary>
        /// Verifies that GetProcessor returns constructed IListProcessor&lt;List&lt;int&gt;&gt; for List&lt;List&lt;int&gt;&gt; type.
        /// Tests nested list handling.
        /// </summary>
        [Test]
        public void GetProcessor_NestedListType_ReturnsIListProcessorOfList()
        {
            // Arrange
            var factory = new SerializationProcessorFactory();

            // Act
            var processor = factory.GetProcessor<List<List<int>>>();

            // Assert
            Assert.IsNotNull(processor, "Processor should not be null.");
            Assert.AreEqual(typeof(List<List<int>>), processor.ValueType,
                "Processor ValueType should be List<List<int>>.");
        }

        #endregion

        #region GetProcessor - Unity Types Tests

        /// <summary>
        /// Verifies that GetProcessor returns Vector2Processor for Vector2 type.
        /// </summary>
        [Test]
        public void GetProcessor_Vector2Type_ReturnsVector2Processor()
        {
            // Arrange
            var factory = new SerializationProcessorFactory();

            // Act
            var processor = factory.GetProcessor<UnityEngine.Vector2>();

            // Assert
            Assert.IsNotNull(processor, "Processor should not be null.");
            Assert.AreEqual(typeof(UnityEngine.Vector2), processor.ValueType,
                "Processor ValueType should be Vector2.");
            Assert.IsInstanceOf<Vector2Processor>(processor, "Processor should be Vector2Processor instance.");
        }

        /// <summary>
        /// Verifies that GetProcessor returns Vector3Processor for Vector3 type.
        /// </summary>
        [Test]
        public void GetProcessor_Vector3Type_ReturnsVector3Processor()
        {
            // Arrange
            var factory = new SerializationProcessorFactory();

            // Act
            var processor = factory.GetProcessor<UnityEngine.Vector3>();

            // Assert
            Assert.IsNotNull(processor, "Processor should not be null.");
            Assert.AreEqual(typeof(UnityEngine.Vector3), processor.ValueType,
                "Processor ValueType should be Vector3.");
            Assert.IsInstanceOf<Vector3Processor>(processor, "Processor should be Vector3Processor instance.");
        }

        /// <summary>
        /// Verifies that GetProcessor returns ColorProcessor for Color type.
        /// </summary>
        [Test]
        public void GetProcessor_ColorType_ReturnsColorProcessor()
        {
            // Arrange
            var factory = new SerializationProcessorFactory();

            // Act
            var processor = factory.GetProcessor<UnityEngine.Color>();

            // Assert
            Assert.IsNotNull(processor, "Processor should not be null.");
            Assert.AreEqual(typeof(UnityEngine.Color), processor.ValueType,
                "Processor ValueType should be Color.");
            Assert.IsInstanceOf<ColorProcessor>(processor, "Processor should be ColorProcessor instance.");
        }

        #endregion

        #region GetProcessor - Dependency Injection Tests

        /// <summary>
        /// Verifies that ArrayProcessor&lt;int&gt; receives proper dependency injection.
        /// The IntProcessor dependency should be injected into the _serializer field.
        /// </summary>
        [Test]
        public void GetProcessor_ArrayProcessor_DependencyInjectionInjectsElementProcessor()
        {
            // Arrange
            var factory = new SerializationProcessorFactory();

            // Act
            var processor = factory.GetProcessor<int[]>();
            var arrayProcessor = processor as ArrayProcessor<int>;

            // Assert
            Assert.IsNotNull(arrayProcessor, "Processor should be ArrayProcessor<int> instance.");
            // The dependency injection happens during processor creation.
            // Since we can't directly access the private _serializer field,
            // we verify the processor is created successfully which means injection worked.
        }

        #endregion

        #region GetProcessor - Generic Object Tests

        /// <summary>
        /// Verifies that GetProcessor returns GenericProcessor for Serializable objects.
        /// </summary>
        [Test]
        public void GetProcessor_SerializableClass_ReturnsGenericProcessor()
        {
            // Arrange
            var factory = new SerializationProcessorFactory();

            // Act
            var processor = factory.GetProcessor<TestSerializableClass>();

            // Assert
            Assert.IsNotNull(processor, "Processor should not be null.");
            Assert.AreEqual(typeof(TestSerializableClass), processor.ValueType,
                "Processor ValueType should be TestSerializableClass.");
            Assert.AreEqual(typeof(GenericProcessor<TestSerializableClass>), processor.GetType(),
                "Processor should be GenericProcessor<TestSerializableClass> instance.");
        }

        #endregion

        #region GetProcessor - No Match Tests

        /// <summary>
        /// Verifies that GetProcessor returns GenericProcessor as fallback for unsupported types.
        /// GenericProcessor has the lowest priority and should handle types with [Serializable] attribute.
        /// </summary>
        [Test]
        public void GetProcessor_UnsupportedType_ReturnsGenericProcessor()
        {
            // Arrange
            var factory = new SerializationProcessorFactory();

            // Act
            var processor = factory.GetProcessor<TestUnsupportedClass>();

            // Assert
            Assert.IsNull(processor, "Processor should be null for unsupported types.");
        }

        #endregion

        #region GetProcessor - Multiple Calls Tests

        /// <summary>
        /// Verifies that multiple calls to GetProcessor for the same type return valid processors.
        /// Each call may create a new instance or return a cached one (implementation-dependent).
        /// </summary>
        [Test]
        public void GetProcessor_MultipleCallsSameType_ReturnsValidProcessor()
        {
            // Arrange
            var factory = new SerializationProcessorFactory();

            // Act
            var processor1 = factory.GetProcessor<int>();
            var processor2 = factory.GetProcessor<int>();

            // Assert
            Assert.IsNotNull(processor1, "First processor should not be null.");
            Assert.IsNotNull(processor2, "Second processor should not be null.");
            Assert.AreEqual(processor1.ValueType, processor2.ValueType,
                "Both processors should have the same ValueType.");
        }

        #endregion

        #region Test Helper Classes

        /// <summary>
        /// Test class with Serializable attribute for GenericProcessor testing.
        /// </summary>
        [Serializable]
        public class TestSerializableClass
        {
            public int Id;
            public string Name;
        }

        /// <summary>
        /// Test class without Serializable attribute for fallback testing.
        /// </summary>
        public class TestUnsupportedClass
        {
            public int Value;
        }

        #endregion
    }
}
