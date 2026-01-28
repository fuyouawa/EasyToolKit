using NUnit.Framework;
using EasyToolkit.Serialization.Processors;

namespace Tests.Serialization
{
    /// <summary>
    /// Unit tests for SerializationProcessorFactory to verify correct processor matching.
    /// </summary>
    [TestFixture]
    public class TestSerializationProcessorFactory
    {
        #region Integer Types - Specialized Processors

        /// <summary>
        /// Verifies that sbyte type uses Int8Processor.
        /// </summary>
        [Test]
        public void GetProcessor_SByteType_ReturnsInt8Processor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(sbyte));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(Int8Processor), processor.GetType());
        }

        /// <summary>
        /// Verifies that byte type uses UInt8Processor.
        /// </summary>
        [Test]
        public void GetProcessor_ByteType_ReturnsUInt8Processor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(byte));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(UInt8Processor), processor.GetType());
        }

        /// <summary>
        /// Verifies that short type uses Int16Processor.
        /// </summary>
        [Test]
        public void GetProcessor_Int16Type_ReturnsInt16Processor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(short));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(Int16Processor), processor.GetType());
        }

        /// <summary>
        /// Verifies that ushort type uses UInt16Processor.
        /// </summary>
        [Test]
        public void GetProcessor_UInt16Type_ReturnsUInt16Processor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(ushort));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(UInt16Processor), processor.GetType());
        }

        /// <summary>
        /// Verifies that int type uses Int32Processor.
        /// </summary>
        [Test]
        public void GetProcessor_Int32Type_ReturnsInt32Processor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(int));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(Int32Processor), processor.GetType());
        }

        /// <summary>
        /// Verifies that uint type uses UInt32Processor.
        /// </summary>
        [Test]
        public void GetProcessor_UInt32Type_ReturnsUInt32Processor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(uint));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(UInt32Processor), processor.GetType());
        }

        /// <summary>
        /// Verifies that long type uses Int64Processor.
        /// </summary>
        [Test]
        public void GetProcessor_Int64Type_ReturnsInt64Processor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(long));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(Int64Processor), processor.GetType());
        }

        /// <summary>
        /// Verifies that ulong type uses UInt64Processor.
        /// </summary>
        [Test]
        public void GetProcessor_UInt64Type_ReturnsUInt64Processor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(ulong));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(UInt64Processor), processor.GetType());
        }

        #endregion

        #region Floating Point Types - Specialized Processors

        /// <summary>
        /// Verifies that float type uses SingleProcessor.
        /// </summary>
        [Test]
        public void GetProcessor_SingleType_ReturnsSingleProcessor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(float));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(SingleProcessor), processor.GetType());
        }

        /// <summary>
        /// Verifies that double type uses DoubleProcessor.
        /// </summary>
        [Test]
        public void GetProcessor_DoubleType_ReturnsDoubleProcessor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(double));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(DoubleProcessor), processor.GetType());
        }

        #endregion

        #region Other Primitive Types - Specialized Processors

        /// <summary>
        /// Verifies that bool type uses BoolProcessor.
        /// </summary>
        [Test]
        public void GetProcessor_BoolType_ReturnsBoolProcessor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(bool));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(BoolProcessor), processor.GetType());
        }

        /// <summary>
        /// Verifies that string type uses StringProcessor.
        /// </summary>
        [Test]
        public void GetProcessor_StringType_ReturnsStringProcessor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(string));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(StringProcessor), processor.GetType());
        }

        /// <summary>
        /// Verifies that enum type uses EnumProcessor.
        /// </summary>
        [Test]
        public void GetProcessor_EnumType_ReturnsEnumProcessor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(TestEnum));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(EnumProcessor<TestEnum>), processor.GetType());
        }

        #endregion

        #region Unmanaged Types - GenericPrimitiveProcessor

        /// <summary>
        /// Verifies that unmanaged struct type uses GenericPrimitiveProcessor.
        /// This tests that GenericPrimitiveProcessor handles types without specialized processors.
        /// </summary>
        [Test]
        public void GetProcessor_UnmanagedStructType_ReturnsGenericPrimitiveProcessor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(TestUnmanagedStruct));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(GenericPrimitiveProcessor<TestUnmanagedStruct>), processor.GetType());
        }

        #endregion

        #region Collection Types

        /// <summary>
        /// Verifies that byte array type uses UInt8ArrayProcessor.
        /// </summary>
        [Test]
        public void GetProcessor_ByteArrayType_ReturnsUInt8ArrayProcessor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(byte[]));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(UInt8ArrayProcessor), processor.GetType());
        }

        /// <summary>
        /// Verifies that sbyte array type uses Int8ArrayProcessor.
        /// </summary>
        [Test]
        public void GetProcessor_SByteArrayType_ReturnsInt8ArrayProcessor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(sbyte[]));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(Int8ArrayProcessor), processor.GetType());
        }

        /// <summary>
        /// Verifies that short array type uses Int16ArrayProcessor.
        /// </summary>
        [Test]
        public void GetProcessor_Int16ArrayType_ReturnsInt16ArrayProcessor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(short[]));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(Int16ArrayProcessor), processor.GetType());
        }

        /// <summary>
        /// Verifies that ushort array type uses UInt16ArrayProcessor.
        /// </summary>
        [Test]
        public void GetProcessor_UInt16ArrayType_ReturnsUInt16ArrayProcessor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(ushort[]));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(UInt16ArrayProcessor), processor.GetType());
        }

        /// <summary>
        /// Verifies that int array type uses Int32ArrayProcessor.
        /// </summary>
        [Test]
        public void GetProcessor_Int32ArrayType_ReturnsInt32ArrayProcessor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(int[]));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(Int32ArrayProcessor), processor.GetType());
        }

        /// <summary>
        /// Verifies that uint array type uses UInt32ArrayProcessor.
        /// </summary>
        [Test]
        public void GetProcessor_UInt32ArrayType_ReturnsUInt32ArrayProcessor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(uint[]));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(UInt32ArrayProcessor), processor.GetType());
        }

        /// <summary>
        /// Verifies that long array type uses Int64ArrayProcessor.
        /// </summary>
        [Test]
        public void GetProcessor_Int64ArrayType_ReturnsInt64ArrayProcessor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(long[]));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(Int64ArrayProcessor), processor.GetType());
        }

        /// <summary>
        /// Verifies that ulong array type uses UInt64ArrayProcessor.
        /// </summary>
        [Test]
        public void GetProcessor_UInt64ArrayType_ReturnsUInt64ArrayProcessor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(ulong[]));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(UInt64ArrayProcessor), processor.GetType());
        }

        /// <summary>
        /// Verifies that unmanaged struct array type uses GenericPrimitiveArrayProcessor.
        /// </summary>
        [Test]
        public void GetProcessor_UnmanagedStructArrayType_ReturnsGenericPrimitiveArrayProcessor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(TestUnmanagedStruct[]));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(GenericPrimitiveArrayProcessor<TestUnmanagedStruct>), processor.GetType());
        }

        #endregion

        #region Generic Types - GenericProcessor

        /// <summary>
        /// Verifies that class with [Serializable] uses GenericProcessor.
        /// </summary>
        [Test]
        public void GetProcessor_SerializableClass_ReturnsGenericProcessor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(TestDataClass));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(GenericProcessor<TestDataClass>), processor.GetType());
        }

        /// <summary>
        /// Verifies that class with [EasySerializable] uses GenericProcessor.
        /// </summary>
        [Test]
        public void GetProcessor_EasySerializableClass_ReturnsGenericProcessor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(AllMembersClass));

            // Assert
            Assert.IsNotNull(processor);
            Assert.AreEqual(typeof(GenericProcessor<AllMembersClass>), processor.GetType());
        }

        #endregion

        #region Edge Cases - Critical Tests

        /// <summary>
        /// Verifies that requesting the same type twice returns cached processor.
        /// </summary>
        [Test]
        public void GetProcessor_SameTypeTwice_ReturnsSameCachedInstance()
        {
            // Arrange & Act
            var processor1 = SerializationProcessorFactory.GetProcessor(typeof(int));
            var processor2 = SerializationProcessorFactory.GetProcessor(typeof(int));

            // Assert
            Assert.AreSame(processor1, processor2);
        }

        /// <summary>
        /// Verifies that primitive types never return GenericProcessor.
        /// This is critical - ensures specialized processors have higher priority than GenericProcessor.
        /// </summary>
        [Test]
        public void GetProcessor_PrimitiveTypes_DoesNotReturnGenericProcessor()
        {
            // Arrange & Act
            var intProcessor = SerializationProcessorFactory.GetProcessor(typeof(int));
            var floatProcessor = SerializationProcessorFactory.GetProcessor(typeof(float));
            var doubleProcessor = SerializationProcessorFactory.GetProcessor(typeof(double));
            var sbyteProcessor = SerializationProcessorFactory.GetProcessor(typeof(sbyte));
            var byteProcessor = SerializationProcessorFactory.GetProcessor(typeof(byte));

            // Assert - None of these should be GenericProcessor
            Assert.AreNotEqual(typeof(GenericProcessor<int>), intProcessor.GetType(),
                "int should use Int32Processor, not GenericProcessor<int>");
            Assert.AreNotEqual(typeof(GenericProcessor<float>), floatProcessor.GetType(),
                "float should use SingleProcessor, not GenericProcessor<float>");
            Assert.AreNotEqual(typeof(GenericProcessor<double>), doubleProcessor.GetType(),
                "double should use DoubleProcessor, not GenericProcessor<double>");
            Assert.AreNotEqual(typeof(GenericProcessor<sbyte>), sbyteProcessor.GetType(),
                "sbyte should use Int8Processor, not GenericProcessor<sbyte>");
            Assert.AreNotEqual(typeof(GenericProcessor<byte>), byteProcessor.GetType(),
                "byte should use UInt8Processor, not GenericProcessor<byte>");
        }

        /// <summary>
        /// Verifies that GenericPrimitiveProcessor is used for unmanaged structs without specialized processors.
        /// This ensures GenericPrimitiveProcessor doesn't conflict with GenericProcessor.
        /// </summary>
        [Test]
        public void GetProcessor_UnmanagedStructType_DoesNotReturnGenericProcessor()
        {
            // Arrange & Act
            var processor = SerializationProcessorFactory.GetProcessor(typeof(TestUnmanagedStruct));

            // Assert
            Assert.AreEqual(typeof(GenericPrimitiveProcessor<TestUnmanagedStruct>), processor.GetType(),
                "TestUnmanagedStruct should use GenericPrimitiveProcessor<TestUnmanagedStruct>, not GenericProcessor<TestUnmanagedStruct>");
        }

        #endregion
    }
}
