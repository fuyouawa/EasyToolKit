using System;
using NUnit.Framework;
using EasyToolKit.Serialization;
using EasyToolKit.Serialization.Implementations;

namespace Tests.Serialization
{
    /// <summary>
    /// Unit tests for formatter data access APIs.
    /// </summary>
    [TestFixture]
    public class TestFormatterDataAccess
    {
        #region BinaryWritingFormatter - GetBuffer, GetPosition, GetLength, ToArray, Reset

        /// <summary>
        /// Verifies that GetBuffer returns the internal buffer array.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_GetBuffer_ReturnsInternalBuffer()
        {
            // Arrange
            var formatter = new BinaryWritingFormatter(128);

            // Act
            var buffer = formatter.GetBuffer();

            // Assert
            Assert.IsNotNull(buffer);
            Assert.AreEqual(128, buffer.Length);
        }

        /// <summary>
        /// Verifies that GetPosition returns the current write position.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_GetPosition_AfterWrite_ReturnsCorrectPosition()
        {
            // Arrange
            var formatter = new BinaryWritingFormatter(128);
            int value = 42;

            // Act
            formatter.Format(ref value);
            int position = formatter.GetPosition();

            // Assert
            Assert.Greater(position, 0);
        }

        /// <summary>
        /// Verifies that GetLength returns the written byte count.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_GetLength_AfterWrite_ReturnsCorrectLength()
        {
            // Arrange
            var formatter = new BinaryWritingFormatter(128);
            int value = 42;

            // Act
            formatter.Format(ref value);
            int length = formatter.GetLength();

            // Assert
            Assert.Greater(length, 0);
        }

        /// <summary>
        /// Verifies that ToArray creates a copy of written data.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_ToArray_ReturnsCorrectCopyOfWrittenData()
        {
            // Arrange
            var formatter = new BinaryWritingFormatter(128);
            int value = 42;
            formatter.Format(ref value);

            // Act
            byte[] array = formatter.ToArray();

            // Assert
            Assert.IsNotNull(array);
            Assert.AreEqual(formatter.GetLength(), array.Length);
        }

        /// <summary>
        /// Verifies that Reset clears position for reuse.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_Reset_ClearsPositionAndLength()
        {
            // Arrange
            var formatter = new BinaryWritingFormatter(128);
            int value = 42;
            formatter.Format(ref value);

            // Act
            formatter.Reset();
            int position = formatter.GetPosition();
            int length = formatter.GetLength();

            // Assert
            Assert.AreEqual(0, position);
            Assert.AreEqual(0, length);
        }

        /// <summary>
        /// Verifies that Reset retains the internal buffer.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_Reset_RetainsInternalBuffer()
        {
            // Arrange
            var formatter = new BinaryWritingFormatter(128);
            var bufferBefore = formatter.GetBuffer();
            int value = 42;
            formatter.Format(ref value);

            // Act
            formatter.Reset();
            var bufferAfter = formatter.GetBuffer();

            // Assert
            Assert.AreSame(bufferBefore, bufferAfter);
        }

        /// <summary>
        /// Verifies object pool reuse pattern: Write -> Reset -> Write.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_ObjectPoolReuse_WriteResetWrite_WorksCorrectly()
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

            // Assert - Both encode to same varint length (1 byte after zigzag)
            Assert.AreEqual(data1.Length, data2.Length);
            CollectionAssert.AreNotEqual(data1, data2);
        }

        #endregion

        #region BinaryReadingFormatter - SetBuffer, GetBuffer, GetPosition, GetRemainingLength

        /// <summary>
        /// Verifies that SetBuffer correctly sets data and resets position.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_SetBuffer_SetsDataAndResetsPosition()
        {
            // Arrange
            var formatter = new BinaryReadingFormatter();
            var data = new byte[] { 1, 2, 3, 4 };

            // Act
            formatter.SetBuffer(data);
            int position = formatter.GetPosition();

            // Assert
            Assert.AreEqual(0, position);
            Assert.AreEqual(4, formatter.GetBuffer().Length);
        }

        /// <summary>
        /// Verifies that GetBuffer returns the current data buffer.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_GetBuffer_AfterSetBuffer_ReturnsCorrectBuffer()
        {
            // Arrange
            var formatter = new BinaryReadingFormatter();
            var data = new byte[] { 1, 2, 3, 4 };
            formatter.SetBuffer(data);

            // Act
            var buffer = formatter.GetBuffer();

            // Assert
            Assert.AreEqual(data.Length, buffer.Length);
        }

        /// <summary>
        /// Verifies that GetPosition returns current read position.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_GetPosition_AfterRead_ReturnsCorrectPosition()
        {
            // Arrange
            var formatter = new BinaryReadingFormatter();
            var writeFormatter = new BinaryWritingFormatter(128);
            int originalValue = 42;
            writeFormatter.Format(ref originalValue);
            var data = writeFormatter.ToArray();

            // Act
            formatter.SetBuffer(data);
            int readValue = 0;
            formatter.Format(ref readValue);
            int position = formatter.GetPosition();

            // Assert
            Assert.Greater(position, 0);
            Assert.AreEqual(originalValue, readValue);
        }

        /// <summary>
        /// Verifies object pool reuse pattern: SetBuffer -> Read -> SetBuffer -> Read.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_ObjectPoolReuse_SetBufferReadSetBufferRead_WorksCorrectly()
        {
            // Arrange
            var formatter = new BinaryReadingFormatter();
            var writeFormatter = new BinaryWritingFormatter(128);

            int value1 = 42;
            writeFormatter.Reset();
            writeFormatter.Format(ref value1);
            var data1 = writeFormatter.ToArray();

            int value2 = 99;
            writeFormatter.Reset();
            writeFormatter.Format(ref value2);
            var data2 = writeFormatter.ToArray();

            // Act
            int read1 = 0;
            formatter.SetBuffer(data1);
            formatter.Format(ref read1);

            int read2 = 0;
            formatter.SetBuffer(data2);
            formatter.Format(ref read2);

            // Assert
            Assert.AreEqual(42, read1);
            Assert.AreEqual(99, read2);
        }

        #endregion

        #region Helper Methods

        private int GetVarint32Size(uint value)
        {
            int size = 1;
            while (value > 0x7F)
            {
                value >>= 7;
                size++;
            }
            return size;
        }

        #endregion
    }
}
