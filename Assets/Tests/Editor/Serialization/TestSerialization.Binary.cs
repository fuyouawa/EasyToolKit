using System;
using System.Collections.Generic;
using NUnit.Framework;
using EasyToolKit.Serialization;

namespace Tests.Serialization
{
    /// <summary>
    /// Unit tests for binary serialization functionality.
    /// </summary>
    [TestFixture]
    public class TestSerialization_Binary
    {
        #region Primitive Types

        /// <summary>
        /// Verifies that serializing and deserializing an integer produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Integer_ReturnsOriginalValue()
        {
            // Arrange
            int original = 42;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            int result = EasySerializer.DeserializeFromBinary<int>(data);

            // Assert
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a negative integer produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_NegativeInteger_ReturnsOriginalValue()
        {
            // Arrange
            int original = -9999;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            int result = EasySerializer.DeserializeFromBinary<int>(data);

            // Assert
            Assert.AreEqual(-9999, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a boolean produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_BooleanTrue_ReturnsOriginalValue()
        {
            // Arrange
            bool original = true;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            bool result = EasySerializer.DeserializeFromBinary<bool>(data);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a boolean false produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_BooleanFalse_ReturnsOriginalValue()
        {
            // Arrange
            bool original = false;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            bool result = EasySerializer.DeserializeFromBinary<bool>(data);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a float produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Float_ReturnsOriginalValue()
        {
            // Arrange
            float original = 3.14159f;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            float result = EasySerializer.DeserializeFromBinary<float>(data);

            // Assert
            Assert.AreEqual(3.14159f, result, 0.00001f);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a double produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Double_ReturnsOriginalValue()
        {
            // Arrange
            double original = 123.456789;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            double result = EasySerializer.DeserializeFromBinary<double>(data);

            // Assert
            Assert.AreEqual(123.456789, result, 0.0000001);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a sbyte (int8) produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_SByte_ReturnsOriginalValue()
        {
            // Arrange
            sbyte original = 99;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            sbyte result = EasySerializer.DeserializeFromBinary<sbyte>(data);

            // Assert
            Assert.AreEqual(99, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a negative sbyte (int8) produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_SByteNegative_ReturnsOriginalValue()
        {
            // Arrange
            sbyte original = -128;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            sbyte result = EasySerializer.DeserializeFromBinary<sbyte>(data);

            // Assert
            Assert.AreEqual(-128, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a short (int16) produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Short_ReturnsOriginalValue()
        {
            // Arrange
            short original = 10000;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            short result = EasySerializer.DeserializeFromBinary<short>(data);

            // Assert
            Assert.AreEqual(10000, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a negative short (int16) produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_ShortNegative_ReturnsOriginalValue()
        {
            // Arrange
            short original = -32768;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            short result = EasySerializer.DeserializeFromBinary<short>(data);

            // Assert
            Assert.AreEqual(-32768, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a long (int64) produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Long_ReturnsOriginalValue()
        {
            // Arrange
            long original = 12345678901234;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            long result = EasySerializer.DeserializeFromBinary<long>(data);

            // Assert
            Assert.AreEqual(12345678901234, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a negative long (int64) produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_LongNegative_ReturnsOriginalValue()
        {
            // Arrange
            long original = -98765432109876;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            long result = EasySerializer.DeserializeFromBinary<long>(data);

            // Assert
            Assert.AreEqual(-98765432109876, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a byte (uint8) produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Byte_ReturnsOriginalValue()
        {
            // Arrange
            byte original = 255;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            byte result = EasySerializer.DeserializeFromBinary<byte>(data);

            // Assert
            Assert.AreEqual(255, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a ushort (uint16) produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_UShort_ReturnsOriginalValue()
        {
            // Arrange
            ushort original = 65535;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            ushort result = EasySerializer.DeserializeFromBinary<ushort>(data);

            // Assert
            Assert.AreEqual(65535, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a uint (uint32) produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_UInt_ReturnsOriginalValue()
        {
            // Arrange
            uint original = 4294967295;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            uint result = EasySerializer.DeserializeFromBinary<uint>(data);

            // Assert
            Assert.AreEqual(4294967295u, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a ulong (uint64) produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_ULong_ReturnsOriginalValue()
        {
            // Arrange
            ulong original = 18446744073709551615;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            ulong result = EasySerializer.DeserializeFromBinary<ulong>(data);

            // Assert
            Assert.AreEqual(18446744073709551615ul, result);
        }

        #endregion

        #region String

        /// <summary>
        /// Verifies that serializing and deserializing a string produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_String_ReturnsOriginalValue()
        {
            // Arrange
            string original = "Hello, World!";

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            string result = EasySerializer.DeserializeFromBinary<string>(data);

            // Assert
            Assert.AreEqual("Hello, World!", result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing an empty string produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_EmptyString_ReturnsOriginalValue()
        {
            // Arrange
            string original = string.Empty;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            string result = EasySerializer.DeserializeFromBinary<string>(data);

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a null string produces null.
        /// </summary>
        [Test]
        public void SerializeDeserialize_NullString_ReturnsNull()
        {
            // Arrange
            string original = null;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            string result = EasySerializer.DeserializeFromBinary<string>(data);

            // Assert
            Assert.IsNull(result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a string with unicode characters produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_UnicodeString_ReturnsOriginalValue()
        {
            // Arrange
            string original = "Hello 世界! 🌍";

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            string result = EasySerializer.DeserializeFromBinary<string>(data);

            // Assert
            Assert.AreEqual("Hello 世界! 🌍", result);
        }

        #endregion

        #region Enum

        /// <summary>
        /// Verifies that serializing and deserializing an enum produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_EnumOptionA_ReturnsOriginalValue()
        {
            // Arrange
            TestEnum original = TestEnum.OptionA;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            TestEnum result = EasySerializer.DeserializeFromBinary<TestEnum>(data);

            // Assert
            Assert.AreEqual(TestEnum.OptionA, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing an enum with a non-zero value produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_EnumOptionC_ReturnsOriginalValue()
        {
            // Arrange
            TestEnum original = TestEnum.OptionC;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            TestEnum result = EasySerializer.DeserializeFromBinary<TestEnum>(data);

            // Assert
            Assert.AreEqual(TestEnum.OptionC, result);
        }

        #endregion

        #region Unity Types

        /// <summary>
        /// Verifies that serializing and deserializing a Vector3 produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Vector3_ReturnsOriginalValue()
        {
            // Arrange
            var original = new UnityEngine.Vector3(1.5f, 2.5f, 3.5f);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<UnityEngine.Vector3>(data);

            // Assert
            Assert.AreEqual(1.5f, result.x, 0.00001f);
            Assert.AreEqual(2.5f, result.y, 0.00001f);
            Assert.AreEqual(3.5f, result.z, 0.00001f);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a Vector2 produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Vector2_ReturnsOriginalValue()
        {
            // Arrange
            var original = new UnityEngine.Vector2(10.5f, -5.25f);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<UnityEngine.Vector2>(data);

            // Assert
            Assert.AreEqual(10.5f, result.x, 0.00001f);
            Assert.AreEqual(-5.25f, result.y, 0.00001f);
        }

        #endregion

        #region Arrays

        /// <summary>
        /// Verifies that serializing and deserializing an int array produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_IntArray_ReturnsOriginalValue()
        {
            // Arrange
            int[] original = { 1, 2, 3, 4, 5 };

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            int[] result = EasySerializer.DeserializeFromBinary<int[]>(data);

            // Assert
            Assert.AreEqual(5, result.Length);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(3, result[2]);
            Assert.AreEqual(4, result[3]);
            Assert.AreEqual(5, result[4]);
        }

        /// <summary>
        /// Verifies that serializing and deserializing an empty array produces an empty array.
        /// </summary>
        [Test]
        public void SerializeDeserialize_EmptyArray_ReturnsEmptyArray()
        {
            // Arrange
            int[] original = Array.Empty<int>();

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            int[] result = EasySerializer.DeserializeFromBinary<int[]>(data);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a null array produces null.
        /// </summary>
        [Test]
        public void SerializeDeserialize_NullArray_ReturnsNull()
        {
            // Arrange
            int[] original = null;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            int[] result = EasySerializer.DeserializeFromBinary<int[]>(data);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region Lists

        /// <summary>
        /// Verifies that serializing and deserializing a list of integers produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_IntList_ReturnsOriginalValue()
        {
            // Arrange
            var original = new List<int> { 10, 20, 30, 40, 50 };

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            List<int> result = EasySerializer.DeserializeFromBinary<List<int>>(data);

            // Assert
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(10, result[0]);
            Assert.AreEqual(20, result[1]);
            Assert.AreEqual(30, result[2]);
            Assert.AreEqual(40, result[3]);
            Assert.AreEqual(50, result[4]);
        }

        /// <summary>
        /// Verifies that serializing and deserializing an empty list produces null.
        /// Empty lists are treated as null during deserialization.
        /// </summary>
        [Test]
        public void SerializeDeserialize_EmptyList_ReturnsNull_EmptyListBecomesNull()
        {
            // Arrange
            var original = new List<int>();

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            List<int> result = EasySerializer.DeserializeFromBinary<List<int>>(data);

            // Assert
            Assert.IsNull(result, "Empty lists should deserialize to null");
        }

        /// <summary>
        /// Verifies that serializing and deserializing a null list produces null.
        /// </summary>
        [Test]
        public void SerializeDeserialize_NullList_ReturnsNull()
        {
            // Arrange
            List<int> original = null;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            List<int> result = EasySerializer.DeserializeFromBinary<List<int>>(data);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region Complex Objects

        /// <summary>
        /// Verifies that serializing and deserializing a complex object produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_ComplexObject_ReturnsOriginalValue()
        {
            // Arrange
            var original = new TestDataClass
            {
                Id = 100,
                Name = "TestPlayer",
                Health = 75.5f,
                IsActive = true,
                Position = new UnityEngine.Vector3(1, 2, 3),
                Scores = new List<int> { 100, 200, 300 },
                Data = new byte[] { 11, 22, 33, 44, 55 }
            };

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<TestDataClass>(data);

            // Assert
            Assert.AreEqual(100, result.Id);
            Assert.AreEqual("TestPlayer", result.Name);
            Assert.AreEqual(75.5f, result.Health, 0.0001f);
            Assert.IsTrue(result.IsActive);
            Assert.AreEqual(1, result.Position.x, 0.00001f);
            Assert.AreEqual(2, result.Position.y, 0.00001f);
            Assert.AreEqual(3, result.Position.z, 0.00001f);
            Assert.AreEqual(3, result.Scores.Count);
            Assert.AreEqual(100, result.Scores[0]);
            Assert.AreEqual(200, result.Scores[1]);
            Assert.AreEqual(300, result.Scores[2]);
            Assert.AreEqual(11, result.Data[0]);
            Assert.AreEqual(22, result.Data[1]);
            Assert.AreEqual(33, result.Data[2]);
            Assert.AreEqual(44, result.Data[3]);
            Assert.AreEqual(55, result.Data[4]);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a complex object with null list produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_ComplexObjectWithNullList_ReturnsOriginalValue()
        {
            // Arrange
            var original = new TestDataClass
            {
                Id = 1,
                Name = "NullScores",
                Health = 100f,
                IsActive = false,
                Position = UnityEngine.Vector3.zero,
                Scores = null,
                Data = null
            };

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<TestDataClass>(data);

            // Assert
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("NullScores", result.Name);
            Assert.AreEqual(100f, result.Health, 0.0001f);
            Assert.IsFalse(result.IsActive);
            Assert.AreEqual(UnityEngine.Vector3.zero, result.Position);
            Assert.IsNull(result.Scores);
            Assert.AreEqual(Array.Empty<byte>(), result.Data);
        }

        #endregion
    }
}
