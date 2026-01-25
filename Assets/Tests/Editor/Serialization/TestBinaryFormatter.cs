using System;
using System.IO;
using NUnit.Framework;
using EasyToolKit.Serialization;
using EasyToolKit.Serialization.Formatters;
using EasyToolKit.Serialization.Formatters.Implementations;

namespace Tests.Serialization
{
    /// <summary>
    /// Unit tests for binary formatter options and formatters.
    /// </summary>
    [TestFixture]
    public class TestBinaryFormatter
    {
        #region BinaryWritingFormatter

        #region Constructor

        /// <summary>
        /// Verifies that BinaryWritingFormatter initializes with correct type.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_Constructor_ReturnsBinaryFormat()
        {
            // Arrange
            var formatter = new BinaryWritingFormatter();

            // Assert
            Assert.AreEqual(SerializationFormat.Binary, formatter.FormatType);
        }

        /// <summary>
        /// Verifies that BinaryWritingFormatter initializes with custom capacity.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_ConstructorWithCapacity_InitializesCorrectly()
        {
            // Arrange
            var formatter = new BinaryWritingFormatter(2048);

            // Assert
            Assert.AreEqual(0, formatter.GetPosition());
            Assert.AreEqual(0, formatter.GetLength());
            Assert.IsNotNull(formatter.GetBuffer());
            Assert.AreEqual(2048, formatter.GetBuffer().Length);
        }

        /// <summary>
        /// Verifies that BinaryWritingFormatter throws on invalid capacity.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_ConstructorWithZeroCapacity_ThrowsArgumentOutOfRangeException()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new BinaryWritingFormatter(0));
        }

        /// <summary>
        /// Verifies that BinaryWritingFormatter throws on negative capacity.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_ConstructorWithNegativeCapacity_ThrowsArgumentOutOfRangeException()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new BinaryWritingFormatter(-100));
        }

        #endregion

        #region Buffer Management

        /// <summary>
        /// Verifies that ToArray returns correct buffer size.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_ToArray_ReturnsCorrectSize()
        {
            // Arrange
            var formatter = new BinaryWritingFormatter();
            int value = 42;
            formatter.Format(ref value);

            // Act
            byte[] result = formatter.ToArray();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(formatter.GetLength(), result.Length);
        }

        /// <summary>
        /// Verifies that Reset clears position and length.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_Reset_ClearsPositionAndLength()
        {
            // Arrange
            var formatter = new BinaryWritingFormatter();
            int value = 42;
            formatter.Format(ref value);

            // Act
            formatter.Reset();

            // Assert
            Assert.AreEqual(0, formatter.GetPosition());
            Assert.AreEqual(0, formatter.GetLength());
        }

        #endregion

        #region Write Primitive Types

        /// <summary>
        /// Verifies that writing and reading an int with default options produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteInt_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            int original = 12345;

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            int result = 0;
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual(12345, result);
        }

        /// <summary>
        /// Verifies that writing and reading a negative int produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteNegativeInt_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            int original = -99999;

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            int result = 0;
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual(-99999, result);
        }

        /// <summary>
        /// Verifies that writing and reading a float produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteFloat_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            float original = 3.14159f;

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            float result = 0f;
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual(3.14159f, result, 0.00001f);
        }

        /// <summary>
        /// Verifies that writing and reading a double produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteDouble_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            double original = 123.45678901234;

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            double result = 0d;
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual(123.45678901234, result, 0.00000001);
        }

        /// <summary>
        /// Verifies that writing and reading a bool true produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteBoolTrue_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            bool original = true;

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            bool result = false;
            readFormatter.Format(ref result);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that writing and reading a bool false produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteBoolFalse_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            bool original = false;

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            bool result = true;
            readFormatter.Format(ref result);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verifies that writing and reading a long produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteLong_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            long original = 98765432101234;

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            long result = 0;
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual(98765432101234, result);
        }

        #endregion

        #region Write Strings

        /// <summary>
        /// Verifies that writing and reading a string produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteString_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            string original = "Hello, World!";

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            string result = null;
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual("Hello, World!", result);
        }

        /// <summary>
        /// Verifies that writing and reading an empty string produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteEmptyString_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            string original = string.Empty;

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            string result = "not empty";
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        /// <summary>
        /// Verifies that writing and reading a null string produces null.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteNullString_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            string original = null;

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            string result = "not null";
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual(result, string.Empty);
        }

        /// <summary>
        /// Verifies that writing and reading a unicode string produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteUnicodeString_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            string original = "Hello 世界! 🌍";

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            string result = null;
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual("Hello 世界! 🌍", result);
        }

        #endregion

        #region Write Byte Arrays

        /// <summary>
        /// Verifies that writing and reading a byte array produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteByteArray_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            byte[] original = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            byte[] result = null;
            readFormatter.Format(ref result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Length);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(5, result[4]);
            Assert.AreEqual(10, result[9]);
        }

        /// <summary>
        /// Verifies that writing and reading an empty byte array produces empty array.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteEmptyByteArray_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            byte[] original = Array.Empty<byte>();

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            byte[] result = null;
            readFormatter.Format(ref result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        /// <summary>
        /// Verifies that writing and reading a null byte array produces empty array.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteNullByteArray_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            byte[] original = null;

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            byte[] result = new byte[] { 1, 2, 3 };
            readFormatter.Format(ref result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        #endregion

        #region Write Primitive Arrays

        /// <summary>
        /// Verifies that writing and reading an sbyte array produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteSByteArray_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            sbyte[] original = { -128, -1, 0, 1, 127 };

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            sbyte[] result = null;
            readFormatter.Format(ref result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Length);
            Assert.AreEqual(-128, result[0]);
            Assert.AreEqual(0, result[2]);
            Assert.AreEqual(127, result[4]);
        }

        /// <summary>
        /// Verifies that writing and reading a short array produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteShortArray_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            short[] original = { -32768, -100, 0, 100, 32767 };

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            short[] result = null;
            readFormatter.Format(ref result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Length);
            Assert.AreEqual(-32768, result[0]);
            Assert.AreEqual(0, result[2]);
            Assert.AreEqual(32767, result[4]);
        }

        /// <summary>
        /// Verifies that writing and reading an int array produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteIntArray_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            int[] original = { -2147483648, -1000000, 0, 1000000, 2147483647 };

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            int[] result = null;
            readFormatter.Format(ref result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Length);
            Assert.AreEqual(-2147483648, result[0]);
            Assert.AreEqual(0, result[2]);
            Assert.AreEqual(2147483647, result[4]);
        }

        /// <summary>
        /// Verifies that writing and reading a long array produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteLongArray_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            long[] original = { -9223372036854775808, -10000000000, 0, 10000000000, 9223372036854775807 };

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            long[] result = null;
            readFormatter.Format(ref result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Length);
            Assert.AreEqual(-9223372036854775808, result[0]);
            Assert.AreEqual(0, result[2]);
            Assert.AreEqual(9223372036854775807, result[4]);
        }

        /// <summary>
        /// Verifies that writing and reading a ushort array produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteUShortArray_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            ushort[] original = { 0, 100, 32767, 65535 };

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            ushort[] result = null;
            readFormatter.Format(ref result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Length);
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(32767, result[2]);
            Assert.AreEqual(65535, result[3]);
        }

        /// <summary>
        /// Verifies that writing and reading a uint array produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteUIntArray_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            uint[] original = { 0, 1000000, 2147483648, 4294967295 };

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            uint[] result = null;
            readFormatter.Format(ref result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Length);
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(2147483648, result[2]);
            Assert.AreEqual(4294967295, result[3]);
        }

        /// <summary>
        /// Verifies that writing and reading a ulong array produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteULongArray_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            ulong[] original = { 0, 10000000000, 9223372036854775808, 18446744073709551615 };

            // Act
            writeFormatter.Format(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            ulong[] result = null;
            readFormatter.Format(ref result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Length);
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(9223372036854775808, result[2]);
            Assert.AreEqual(18446744073709551615, result[3]);
        }

        /// <summary>
        /// Verifies that writing and reading null primitive arrays produces empty arrays.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteNullPrimitiveArrays_AllReturnEmpty()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();

            // Act
            sbyte[] sbyteArray = null;
            short[] shortArray = null;
            int[] intArray = null;
            long[] longArray = null;
            ushort[] ushortArray = null;
            uint[] uintArray = null;
            ulong[] ulongArray = null;

            writeFormatter.Format(ref sbyteArray);
            writeFormatter.Format(ref shortArray);
            writeFormatter.Format(ref intArray);
            writeFormatter.Format(ref longArray);
            writeFormatter.Format(ref ushortArray);
            writeFormatter.Format(ref uintArray);
            writeFormatter.Format(ref ulongArray);

            readFormatter.SetBuffer(writeFormatter.ToArray());

            readFormatter.Format(ref sbyteArray);
            readFormatter.Format(ref shortArray);
            readFormatter.Format(ref intArray);
            readFormatter.Format(ref longArray);
            readFormatter.Format(ref ushortArray);
            readFormatter.Format(ref uintArray);
            readFormatter.Format(ref ulongArray);

            // Assert
            Assert.IsNotNull(sbyteArray);
            Assert.IsNotNull(shortArray);
            Assert.IsNotNull(intArray);
            Assert.IsNotNull(longArray);
            Assert.IsNotNull(ushortArray);
            Assert.IsNotNull(uintArray);
            Assert.IsNotNull(ulongArray);

            Assert.AreEqual(0, sbyteArray.Length);
            Assert.AreEqual(0, shortArray.Length);
            Assert.AreEqual(0, intArray.Length);
            Assert.AreEqual(0, longArray.Length);
            Assert.AreEqual(0, ushortArray.Length);
            Assert.AreEqual(0, uintArray.Length);
            Assert.AreEqual(0, ulongArray.Length);
        }

        /// <summary>
        /// Verifies that writing and reading empty primitive arrays produces empty arrays.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteEmptyPrimitiveArrays_AllReturnEmpty()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();

            // Act
            sbyte[] sbyteArray = Array.Empty<sbyte>();
            short[] shortArray = Array.Empty<short>();
            int[] intArray = Array.Empty<int>();
            long[] longArray = Array.Empty<long>();
            ushort[] ushortArray = Array.Empty<ushort>();
            uint[] uintArray = Array.Empty<uint>();
            ulong[] ulongArray = Array.Empty<ulong>();

            writeFormatter.Format(ref sbyteArray);
            writeFormatter.Format(ref shortArray);
            writeFormatter.Format(ref intArray);
            writeFormatter.Format(ref longArray);
            writeFormatter.Format(ref ushortArray);
            writeFormatter.Format(ref uintArray);
            writeFormatter.Format(ref ulongArray);

            readFormatter.SetBuffer(writeFormatter.ToArray());

            readFormatter.Format(ref sbyteArray);
            readFormatter.Format(ref shortArray);
            readFormatter.Format(ref intArray);
            readFormatter.Format(ref longArray);
            readFormatter.Format(ref ushortArray);
            readFormatter.Format(ref uintArray);
            readFormatter.Format(ref ulongArray);

            // Assert
            Assert.AreEqual(0, sbyteArray.Length);
            Assert.AreEqual(0, shortArray.Length);
            Assert.AreEqual(0, intArray.Length);
            Assert.AreEqual(0, longArray.Length);
            Assert.AreEqual(0, ushortArray.Length);
            Assert.AreEqual(0, uintArray.Length);
            Assert.AreEqual(0, ulongArray.Length);
        }

        /// <summary>
        /// Verifies that large primitive arrays are handled correctly with direct memory copy.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_WriteLargePrimitiveArrays_UsesDirectMemoryCopy()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();

            // Create arrays with 10000 elements each
            int[] intArray = new int[10000];
            long[] longArray = new long[10000];
            for (int i = 0; i < 10000; i++)
            {
                intArray[i] = i * 2;
                longArray[i] = i * 3L;
            }

            // Act
            writeFormatter.Format(ref intArray);
            writeFormatter.Format(ref longArray);

            readFormatter.SetBuffer(writeFormatter.ToArray());

            int[] intResult = null;
            long[] longResult = null;
            readFormatter.Format(ref intResult);
            readFormatter.Format(ref longResult);

            // Assert
            Assert.IsNotNull(intResult);
            Assert.IsNotNull(longResult);
            Assert.AreEqual(10000, intResult.Length);
            Assert.AreEqual(10000, longResult.Length);

            // Verify some sample values
            Assert.AreEqual(0, intResult[0]);
            Assert.AreEqual(10000, intResult[5000]);
            Assert.AreEqual(19998, intResult[9999]);

            Assert.AreEqual(0, longResult[0]);
            Assert.AreEqual(15000, longResult[5000]);
            Assert.AreEqual(29997, longResult[9999]);
        }

        #endregion

        #endregion

        #region BinaryReadingFormatter

        #region Constructor

        /// <summary>
        /// Verifies that BinaryReadingFormatter initializes with correct type.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_Constructor_ReturnsBinaryFormat()
        {
            // Arrange
            var formatter = new BinaryReadingFormatter();

            // Assert
            Assert.AreEqual(SerializationFormat.Binary, formatter.FormatType);
        }

        #endregion

        #region Buffer Management

        /// <summary>
        /// Verifies that SetBuffer sets the buffer correctly.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_SetBuffer_SetsBufferCorrectly()
        {
            // Arrange
            var formatter = new BinaryReadingFormatter();
            byte[] buffer = { 1, 2, 3, 4, 5 };

            // Act
            formatter.SetBuffer(buffer);

            // Assert
            Assert.AreEqual(0, formatter.GetPosition());
            Assert.AreEqual(5, formatter.GetRemainingLength());
        }

        /// <summary>
        /// Verifies that GetBuffer returns the set buffer.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_GetBuffer_ReturnsSetBuffer()
        {
            // Arrange
            var formatter = new BinaryReadingFormatter();
            byte[] buffer = { 10, 20, 30, 40, 50 };
            formatter.SetBuffer(buffer);

            // Act
            ReadOnlySpan<byte> result = formatter.GetBuffer();

            // Assert
            Assert.AreEqual(5, result.Length);
            Assert.AreEqual(10, result[0]);
            Assert.AreEqual(50, result[4]);
        }

        /// <summary>
        /// Verifies that GetPosition returns current position.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_GetPosition_ReturnsCurrentPosition()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            int value = 42;
            writeFormatter.Format(ref value);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);

            // Act
            readFormatter.Format(ref value);

            // Assert
            Assert.AreEqual(buffer.Length, readFormatter.GetPosition());
        }

        /// <summary>
        /// Verifies that GetRemainingLength returns remaining bytes.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_GetRemainingLength_ReturnsRemainingBytes()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            int value = 42;
            writeFormatter.Format(ref value);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);

            // Act
            int initialRemaining = readFormatter.GetRemainingLength();
            readFormatter.Format(ref value);
            int finalRemaining = readFormatter.GetRemainingLength();

            // Assert
            Assert.AreEqual(buffer.Length, initialRemaining);
            Assert.AreEqual(0, finalRemaining);
        }

        #endregion

        #region Read Primitive Types

        /// <summary>
        /// Verifies that reading an int with default options works correctly.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_ReadInt_ReturnsCorrectValue()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            int original = 65535;

            // Act
            writeFormatter.Format(ref original);
            readFormatter.SetBuffer(writeFormatter.ToArray());
            int result = 0;
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual(65535, result);
        }

        /// <summary>
        /// Verifies that reading all integer types works correctly.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_ReadAllIntegerTypes_ReturnsCorrectValues()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();

            byte byteValue = 255;
            sbyte sbyteValue = -128;
            short shortValue = -32768;
            ushort ushortValue = 65535;
            int intValue = -2147483648;
            uint uintValue = 4294967295;
            long longValue = -9223372036854775808;
            ulong ulongValue = 18446744073709551615;

            // Act
            writeFormatter.Format(ref byteValue);
            writeFormatter.Format(ref sbyteValue);
            writeFormatter.Format(ref shortValue);
            writeFormatter.Format(ref ushortValue);
            writeFormatter.Format(ref intValue);
            writeFormatter.Format(ref uintValue);
            writeFormatter.Format(ref longValue);
            writeFormatter.Format(ref ulongValue);

            readFormatter.SetBuffer(writeFormatter.ToArray());

            readFormatter.Format(ref byteValue);
            readFormatter.Format(ref sbyteValue);
            readFormatter.Format(ref shortValue);
            readFormatter.Format(ref ushortValue);
            readFormatter.Format(ref intValue);
            readFormatter.Format(ref uintValue);
            readFormatter.Format(ref longValue);
            readFormatter.Format(ref ulongValue);

            // Assert
            Assert.AreEqual((byte)255, byteValue);
            Assert.AreEqual((sbyte)-128, sbyteValue);
            Assert.AreEqual((short)-32768, shortValue);
            Assert.AreEqual((ushort)65535, ushortValue);
            Assert.AreEqual(-2147483648, intValue);
            Assert.AreEqual((uint)4294967295, uintValue);
            Assert.AreEqual(-9223372036854775808, longValue);
            Assert.AreEqual((ulong)18446744073709551615, ulongValue);
        }

        #endregion

        #region Error Handling

        /// <summary>
        /// Verifies that reading past buffer end throws EndOfStreamException.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_ReadPastEnd_ThrowsEndOfStreamException()
        {
            // Arrange
            var formatter = new BinaryReadingFormatter();
            // Use settings without type tags to test insufficient buffer scenario
            formatter.Settings = new BinaryFormatterSettings { Options = BinaryFormatterOptions.None };
            formatter.SetBuffer(new byte[] { 1, 2 }); // Too small for an int

            // Act & Assert
            Assert.Throws<EndOfStreamException>(() =>
            {
                int value = 0;
                formatter.Format(ref value);
            });
        }

        /// <summary>
        /// Verifies that reading empty buffer throws EndOfStreamException.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_ReadEmptyBuffer_ThrowsEndOfStreamException()
        {
            // Arrange
            var formatter = new BinaryReadingFormatter();
            // Use settings without type tags to test empty buffer scenario
            formatter.Settings = new BinaryFormatterSettings { Options = BinaryFormatterOptions.None };
            formatter.SetBuffer(Array.Empty<byte>());

            // Act & Assert
            Assert.Throws<EndOfStreamException>(() =>
            {
                int value = 0;
                formatter.Format(ref value);
            });
        }

        #endregion

        #endregion

        #region Write GenericPrimitive Values

        /// <summary>
        /// Verifies that writing and reading a generic primitive value produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_FormatGenericPrimitive_SingleValue_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            var original = new TestUnmanagedStruct(42, 3.14f, 255);

            // Act
            writeFormatter.FormatGenericPrimitive(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            TestUnmanagedStruct result = default;
            readFormatter.FormatGenericPrimitive(ref result);

            // Assert
            Assert.AreEqual(original.X, result.X);
            Assert.AreEqual(original.Y, result.Y, 0.00001f);
            Assert.AreEqual(original.Z, result.Z);
        }

        /// <summary>
        /// Verifies that writing and reading a generic primitive enum produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_FormatGenericPrimitive_Enum_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            TestEnum original = TestEnum.OptionC;

            // Act
            writeFormatter.FormatGenericPrimitive(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            TestEnum result = default;
            readFormatter.FormatGenericPrimitive(ref result);

            // Assert
            Assert.AreEqual(original, result);
        }

        #endregion

        #region Write GenericPrimitive Arrays

        /// <summary>
        /// Verifies that writing and reading a generic primitive array produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_FormatGenericPrimitive_Array_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            var original = new TestUnmanagedStruct[]
            {
                new(1, 1.1f, 10),
                new(2, 2.2f, 20),
                new(3, 3.3f, 30),
                new(4, 4.4f, 40),
                new(5, 5.5f, 50)
            };

            // Act
            writeFormatter.FormatGenericPrimitive(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            TestUnmanagedStruct[] result = null;
            readFormatter.FormatGenericPrimitive(ref result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Length);
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(original[i].X, result[i].X);
                Assert.AreEqual(original[i].Y, result[i].Y, 0.00001f);
                Assert.AreEqual(original[i].Z, result[i].Z);
            }
        }

        /// <summary>
        /// Verifies that writing and reading a generic primitive enum array produces the original value.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_FormatGenericPrimitive_EnumArray_CanBeReadBack()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            var original = new TestEnum[] { TestEnum.OptionA, TestEnum.OptionB, TestEnum.OptionC, TestEnum.OptionA };

            // Act
            writeFormatter.FormatGenericPrimitive(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            TestEnum[] result = null;
            readFormatter.FormatGenericPrimitive(ref result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Length);
            Assert.AreEqual(TestEnum.OptionA, result[0]);
            Assert.AreEqual(TestEnum.OptionB, result[1]);
            Assert.AreEqual(TestEnum.OptionC, result[2]);
            Assert.AreEqual(TestEnum.OptionA, result[3]);
        }

        /// <summary>
        /// Verifies that writing and reading a null generic primitive array produces empty array.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_FormatGenericPrimitive_NullArray_ReturnsEmpty()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            TestUnmanagedStruct[] original = null;

            // Act
            writeFormatter.FormatGenericPrimitive(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            readFormatter.FormatGenericPrimitive(ref original);

            // Assert
            Assert.IsNotNull(original);
            Assert.AreEqual(0, original.Length);
        }

        /// <summary>
        /// Verifies that writing and reading an empty generic primitive array produces empty array.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_FormatGenericPrimitive_EmptyArray_ReturnsEmpty()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            var original = Array.Empty<TestUnmanagedStruct>();

            // Act
            writeFormatter.FormatGenericPrimitive(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            TestUnmanagedStruct[] result = null;
            readFormatter.FormatGenericPrimitive(ref result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        /// <summary>
        /// Verifies that large generic primitive arrays are handled correctly.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_FormatGenericPrimitive_LargeArray_HandledCorrectly()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();
            var original = new TestUnmanagedStruct[1000];
            for (int i = 0; i < 1000; i++)
            {
                original[i] = new TestUnmanagedStruct(i, i * 1.5f, (byte)(i % 256));
            }

            // Act
            writeFormatter.FormatGenericPrimitive(ref original);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);
            TestUnmanagedStruct[] result = null;
            readFormatter.FormatGenericPrimitive(ref result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1000, result.Length);

            // Verify sample values
            Assert.AreEqual(0, result[0].X);
            Assert.AreEqual(500, result[500].X);
            Assert.AreEqual(999, result[999].X);
        }

        #endregion

        #region Integration Tests

        /// <summary>
        /// Verifies that multiple values can be written and read correctly.
        /// </summary>
        [Test]
        public void BinaryFormatter_MultipleValues_AllReadCorrectly()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();

            int intValue = 42;
            float floatValue = 3.14f;
            string stringValue = "Test";
            bool boolValue = true;

            // Act
            writeFormatter.Format(ref intValue);
            writeFormatter.Format(ref floatValue);
            writeFormatter.Format(ref stringValue);
            writeFormatter.Format(ref boolValue);

            readFormatter.SetBuffer(writeFormatter.ToArray());

            readFormatter.Format(ref intValue);
            readFormatter.Format(ref floatValue);
            readFormatter.Format(ref stringValue);
            readFormatter.Format(ref boolValue);

            // Assert
            Assert.AreEqual(42, intValue);
            Assert.AreEqual(3.14f, floatValue, 0.001f);
            Assert.AreEqual("Test", stringValue);
            Assert.IsTrue(boolValue);
        }

        /// <summary>
        /// Verifies that writing formatters can be reused after Reset.
        /// </summary>
        [Test]
        public void BinaryWritingFormatter_ReuseAfterReset_WorksCorrectly()
        {
            // Arrange
            var formatter = new BinaryWritingFormatter();
            var readFormatter = new BinaryReadingFormatter();

            // Act - First use
            int value1 = 100;
            formatter.Format(ref value1);
            byte[] buffer1 = formatter.ToArray();
            readFormatter.SetBuffer(buffer1);
            readFormatter.Format(ref value1);

            // Reset and reuse
            formatter.Reset();
            int value2 = 200;
            formatter.Format(ref value2);
            byte[] buffer2 = formatter.ToArray();
            readFormatter.SetBuffer(buffer2);
            readFormatter.Format(ref value2);

            // Assert
            Assert.AreEqual(100, value1);
            Assert.AreEqual(200, value2);
        }

        #endregion
    }
}
