using System;
using System.IO;
using NUnit.Framework;
using EasyToolKit.Serialization;
using EasyToolKit.Serialization.Implementations;

namespace Tests.Serialization
{
    /// <summary>
    /// Unit tests for binary formatter optimization correctness and safety.
    /// </summary>
    [TestFixture]
    public class TestBinaryFormatterOptimization
    {
        #region Binary Format Compatibility - Roundtrip Correctness

        /// <summary>
        /// Verifies that integer roundtrip serialization/deserialization produces correct value.
        /// </summary>
        [Test]
        public void IntegerRoundtrip_SerializeDeserialize_ProducesOriginalValue()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter(128);
            var readFormatter = new BinaryReadingFormatter();
            int original = 42;

            // Act
            writeFormatter.Format(ref original);
            var data = writeFormatter.ToArray();
            readFormatter.SetBuffer(data);
            int result = 0;
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Verifies that negative integer roundtrip serialization/deserialization produces correct value.
        /// </summary>
        [Test]
        public void NegativeIntegerRoundtrip_SerializeDeserialize_ProducesOriginalValue()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter(128);
            var readFormatter = new BinaryReadingFormatter();
            int original = -9999;

            // Act
            writeFormatter.Format(ref original);
            var data = writeFormatter.ToArray();
            readFormatter.SetBuffer(data);
            int result = 0;
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual(-9999, result);
        }

        /// <summary>
        /// Verifies that string roundtrip serialization/deserialization produces correct value.
        /// </summary>
        [Test]
        public void StringRoundtrip_SerializeDeserialize_ProducesOriginalValue()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter(128);
            var readFormatter = new BinaryReadingFormatter();
            string original = "Hello, World!";

            // Act
            writeFormatter.Format(ref original);
            var data = writeFormatter.ToArray();
            readFormatter.SetBuffer(data);
            string result = null;
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual("Hello, World!", result);
        }

        /// <summary>
        /// Verifies that null string roundtrip serialization/deserialization produces empty string.
        /// Note: Current implementation serializes null as empty string.
        /// </summary>
        [Test]
        public void NullStringRoundtrip_SerializeDeserialize_ProducesEmptyString()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter(128);
            var readFormatter = new BinaryReadingFormatter();
            string original = null;

            // Act
            writeFormatter.Format(ref original);
            var data = writeFormatter.ToArray();
            readFormatter.SetBuffer(data);
            string result = "not null";
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        /// <summary>
        /// Verifies that empty string roundtrip serialization/deserialization produces empty string.
        /// </summary>
        [Test]
        public void EmptyStringRoundtrip_SerializeDeserialize_ProducesEmptyString()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter(128);
            var readFormatter = new BinaryReadingFormatter();
            string original = string.Empty;

            // Act
            writeFormatter.Format(ref original);
            var data = writeFormatter.ToArray();
            readFormatter.SetBuffer(data);
            string result = "not empty";
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        /// <summary>
        /// Verifies that Unicode string roundtrip serialization/deserialization produces correct value.
        /// </summary>
        [Test]
        public void UnicodeStringRoundtrip_SerializeDeserialize_ProducesOriginalValue()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter(128);
            var readFormatter = new BinaryReadingFormatter();
            string original = "Hello 世界! 🌍";

            // Act
            writeFormatter.Format(ref original);
            var data = writeFormatter.ToArray();
            readFormatter.SetBuffer(data);
            string result = null;
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual("Hello 世界! 🌍", result);
        }

        #endregion

        #region Buffer Overflow Safety

        /// <summary>
        /// Verifies that reading past buffer end throws EndOfStreamException.
        /// </summary>
        [Test]
        public void ReadPastBufferEnd_ThrowsEndOfStreamException()
        {
            // Arrange
            var formatter = new BinaryReadingFormatter();
            var emptyData = Array.Empty<byte>();
            formatter.SetBuffer(emptyData);
            int value = 0;

            // Act & Assert
            Assert.Throws<EndOfStreamException>(() => formatter.Format(ref value));
        }

        /// <summary>
        /// Verifies that reading partial data throws EndOfStreamException.
        /// </summary>
        [Test]
        public void ReadPartialStringData_ThrowsEndOfStreamException()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter(128);
            var readFormatter = new BinaryReadingFormatter();
            string original = "Hello World";
            writeFormatter.Format(ref original);
            var data = writeFormatter.ToArray();

            // Truncate the data to simulate corruption
            var truncatedData = new byte[data.Length / 2];
            Array.Copy(data, truncatedData, truncatedData.Length);

            readFormatter.SetBuffer(truncatedData);
            string result = null;

            // Act & Assert
            Assert.Throws<EndOfStreamException>(() => readFormatter.Format(ref result));
        }

        /// <summary>
        /// Verifies that reading zero-byte varint works correctly.
        /// </summary>
        [Test]
        public void ReadZeroByteVarint_WorksCorrectly()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter(128);
            var readFormatter = new BinaryReadingFormatter();
            uint original = 0;

            // Act
            writeFormatter.Format(ref original);
            var data = writeFormatter.ToArray();
            readFormatter.SetBuffer(data);
            uint result = 999;
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual(0, result);
        }

        #endregion

        #region Float/Double Encoding

        /// <summary>
        /// Verifies float encoding/decoding correctness.
        /// </summary>
        [Test]
        public void FloatRoundtrip_SerializeDeserialize_ProducesOriginalValue()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter(128);
            var readFormatter = new BinaryReadingFormatter();
            float original = 3.14159f;

            // Act
            writeFormatter.Format(ref original);
            var data = writeFormatter.ToArray();
            readFormatter.SetBuffer(data);
            float result = 0;
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual(3.14159f, result, 0.00001f);
        }

        /// <summary>
        /// Verifies double encoding/decoding correctness.
        /// </summary>
        [Test]
        public void DoubleRoundtrip_SerializeDeserialize_ProducesOriginalValue()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter(128);
            var readFormatter = new BinaryReadingFormatter();
            double original = 123.456789;

            // Act
            writeFormatter.Format(ref original);
            var data = writeFormatter.ToArray();
            readFormatter.SetBuffer(data);
            double result = 0;
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual(123.456789, result, 0.0000001);
        }

        #endregion

        #region Object Pool Reuse

        /// <summary>
        /// Verifies that writing formatter can be reused after Reset.
        /// </summary>
        [Test]
        public void WritingFormatterReuse_ResetAndWrite_WorksCorrectly()
        {
            // Arrange
            var formatter = new BinaryWritingFormatter(128);
            int value1 = 42;
            int value2 = 50;

            // Act
            formatter.Format(ref value1);
            var data1 = formatter.ToArray();

            formatter.Reset();
            formatter.Format(ref value2);
            var data2 = formatter.ToArray();

            // Assert - Same length but different content (both encode to 1 byte after zigzag)
            Assert.AreEqual(data1.Length, data2.Length);
            CollectionAssert.AreNotEqual(data1, data2);
        }

        /// <summary>
        /// Verifies that reading formatter can be reused after SetBuffer.
        /// </summary>
        [Test]
        public void ReadingFormatterReuse_SetBufferAndRead_WorksCorrectly()
        {
            // Arrange
            var formatter = new BinaryReadingFormatter();
            var writeFormatter = new BinaryWritingFormatter(128);

            int value1 = 42;
            writeFormatter.Format(ref value1);
            var data1 = writeFormatter.ToArray();

            int value2 = 99;
            writeFormatter.Reset();
            writeFormatter.Format(ref value2);
            var data2 = writeFormatter.ToArray();

            // Act
            formatter.SetBuffer(data1);
            int result1 = 0;
            formatter.Format(ref result1);

            formatter.SetBuffer(data2);
            int result2 = 0;
            formatter.Format(ref result2);

            // Assert
            Assert.AreEqual(42, result1);
            Assert.AreEqual(99, result2);
        }

        #endregion
    }
}
