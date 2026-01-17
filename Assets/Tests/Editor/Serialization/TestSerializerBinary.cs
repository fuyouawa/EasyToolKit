using System;
using System.Collections.Generic;
using NUnit.Framework;
using EasyToolKit.Serialization;

namespace Tests.Serialization
{
    /// <summary>Unit tests for EasySerializer serialization/deserialization functionality.</summary>
    [TestFixture]
    public class TestSerializerBinary
    {
        #region Primitive Types - Binary Format

        [Test]
        public void SerializeDeserialize_Int_ReturnsOriginalValue()
        {
            // Arrange
            int original = 42;
            var data = new EasySerializationData(SerializationFormat.Binary);

            // Act
            EasySerializer.Serialize(ref original, ref data);
            int deserialized = EasySerializer.Deserialize<int>(ref data);

            // Assert
            Assert.AreEqual(original, deserialized, "Deserialized int should match original value.");
        }

        [Test]
        public void SerializeDeserialize_Int_MinMax_ReturnsOriginalValues()
        {
            // Arrange
            int min = int.MinValue;
            int max = int.MaxValue;
            var minData = new EasySerializationData(SerializationFormat.Binary);
            var maxData = new EasySerializationData(SerializationFormat.Binary);

            // Act
            EasySerializer.Serialize(ref min, ref minData);
            EasySerializer.Serialize(ref max, ref maxData);
            int minDeserialized = EasySerializer.Deserialize<int>(ref minData);
            int maxDeserialized = EasySerializer.Deserialize<int>(ref maxData);

            // Assert
            Assert.AreEqual(min, minDeserialized, "Min value should be preserved.");
            Assert.AreEqual(max, maxDeserialized, "Max value should be preserved.");
        }

        [Test]
        public void SerializeDeserialize_Float_ReturnsOriginalValue()
        {
            // Arrange
            float original = 3.14159f;
            var data = new EasySerializationData(SerializationFormat.Binary);

            // Act
            EasySerializer.Serialize(ref original, ref data);
            float deserialized = EasySerializer.Deserialize<float>(ref data);

            // Assert
            Assert.AreEqual(original, deserialized, 0.0001f, "Deserialized float should match original value.");
        }

        [Test]
        public void SerializeDeserialize_Double_ReturnsOriginalValue()
        {
            // Arrange
            double original = 3.141592653589793;
            var data = new EasySerializationData(SerializationFormat.Binary);

            // Act
            EasySerializer.Serialize(ref original, ref data);
            double deserialized = EasySerializer.Deserialize<double>(ref data);

            // Assert
            Assert.AreEqual(original, deserialized, 0.0000001, "Deserialized double should match original value.");
        }

        [Test]
        public void SerializeDeserialize_Bool_ReturnsOriginalValue()
        {
            // Arrange
            bool originalTrue = true;
            bool originalFalse = false;
            var trueData = new EasySerializationData(SerializationFormat.Binary);
            var falseData = new EasySerializationData(SerializationFormat.Binary);

            // Act
            EasySerializer.Serialize(ref originalTrue, ref trueData);
            EasySerializer.Serialize(ref originalFalse, ref falseData);
            bool deserializedTrue = EasySerializer.Deserialize<bool>(ref trueData);
            bool deserializedFalse = EasySerializer.Deserialize<bool>(ref falseData);

            // Assert
            Assert.IsTrue(deserializedTrue, "True should be preserved.");
            Assert.IsFalse(deserializedFalse, "False should be preserved.");
        }

        [Test]
        public void SerializeDeserialize_String_ReturnsOriginalValue()
        {
            // Arrange
            string original = "Hello, EasyToolKit!";
            var data = new EasySerializationData(SerializationFormat.Binary);

            // Act
            EasySerializer.Serialize(ref original, ref data);
            string deserialized = EasySerializer.Deserialize<string>(ref data);

            // Assert
            Assert.AreEqual(original, deserialized, "Deserialized string should match original value.");
        }

        [Test]
        public void SerializeDeserialize_String_Empty_ReturnsOriginalValue()
        {
            // Arrange
            string original = string.Empty;
            var data = new EasySerializationData(SerializationFormat.Binary);

            // Act
            EasySerializer.Serialize(ref original, ref data);
            string deserialized = EasySerializer.Deserialize<string>(ref data);

            // Assert
            Assert.AreEqual(original, deserialized, "Empty string should be preserved.");
        }

        #endregion

        #region Unity Types

        [Test]
        public void SerializeDeserialize_Vector2_ReturnsOriginalValue()
        {
            // Arrange
            UnityEngine.Vector2 original = new(1.5f, 2.5f);
            var data = new EasySerializationData(SerializationFormat.Binary);

            // Act
            EasySerializer.Serialize(ref original, ref data);
            UnityEngine.Vector2 deserialized = EasySerializer.Deserialize<UnityEngine.Vector2>(ref data);

            // Assert
            Assert.AreEqual(original.x, deserialized.x, 0.0001f, "X component should match.");
            Assert.AreEqual(original.y, deserialized.y, 0.0001f, "Y component should match.");
        }

        [Test]
        public void SerializeDeserialize_Vector3_ReturnsOriginalValue()
        {
            // Arrange
            UnityEngine.Vector3 original = new(1.5f, 2.5f, 3.5f);
            var data = new EasySerializationData(SerializationFormat.Binary);

            // Act
            EasySerializer.Serialize(ref original, ref data);
            UnityEngine.Vector3 deserialized = EasySerializer.Deserialize<UnityEngine.Vector3>(ref data);

            // Assert
            Assert.AreEqual(original.x, deserialized.x, 0.0001f, "X component should match.");
            Assert.AreEqual(original.y, deserialized.y, 0.0001f, "Y component should match.");
            Assert.AreEqual(original.z, deserialized.z, 0.0001f, "Z component should match.");
        }

        [Test]
        public void SerializeDeserialize_Color_ReturnsOriginalValue()
        {
            // Arrange
            UnityEngine.Color original = new(0.5f, 0.75f, 1.0f, 0.25f);
            var data = new EasySerializationData(SerializationFormat.Binary);

            // Act
            EasySerializer.Serialize(ref original, ref data);
            UnityEngine.Color deserialized = EasySerializer.Deserialize<UnityEngine.Color>(ref data);

            // Assert
            Assert.AreEqual(original.r, deserialized.r, 0.001f, "R component should match.");
            Assert.AreEqual(original.g, deserialized.g, 0.001f, "G component should match.");
            Assert.AreEqual(original.b, deserialized.b, 0.001f, "B component should match.");
            Assert.AreEqual(original.a, deserialized.a, 0.001f, "A component should match.");
        }

        #endregion

        #region Collection Types

        [Test]
        public void SerializeDeserialize_IntArray_ReturnsOriginalValues()
        {
            // Arrange
            int[] original = new[] { 1, 2, 3, 4, 5 };
            var data = new EasySerializationData(SerializationFormat.Binary);

            // Act
            EasySerializer.Serialize(ref original, ref data);
            int[] deserialized = EasySerializer.Deserialize<int[]>(ref data);

            // Assert
            Assert.IsNotNull(deserialized, "Deserialized array should not be null.");
            Assert.AreEqual(original.Length, deserialized.Length, "Array length should match.");
            for (int i = 0; i < original.Length; i++)
            {
                Assert.AreEqual(original[i], deserialized[i], $"Element at index {i} should match.");
            }
        }

        [Test]
        public void SerializeDeserialize_IntArray_Empty_ReturnsEmptyArray()
        {
            // Arrange
            int[] original = Array.Empty<int>();
            var data = new EasySerializationData(SerializationFormat.Binary);

            // Act
            EasySerializer.Serialize(ref original, ref data);
            int[] deserialized = EasySerializer.Deserialize<int[]>(ref data);

            // Assert
            Assert.IsNotNull(deserialized, "Deserialized array should not be null.");
            Assert.AreEqual(0, deserialized.Length, "Array should be empty.");
        }

        [Test]
        public void SerializeDeserialize_StringArray_ReturnsOriginalValues()
        {
            // Arrange
            string[] original = new[] { "apple", "banana", "cherry" };
            var data = new EasySerializationData(SerializationFormat.Binary);

            // Act
            EasySerializer.Serialize(ref original, ref data);
            string[] deserialized = EasySerializer.Deserialize<string[]>(ref data);

            // Assert
            Assert.IsNotNull(deserialized, "Deserialized array should not be null.");
            Assert.AreEqual(original.Length, deserialized.Length, "Array length should match.");
            for (int i = 0; i < original.Length; i++)
            {
                Assert.AreEqual(original[i], deserialized[i], $"Element at index {i} should match.");
            }
        }

        [Test]
        public void SerializeDeserialize_ListInt_ReturnsOriginalValues()
        {
            // Arrange
            var original = new List<int> { 10, 20, 30, 40, 50 };
            var data = new EasySerializationData(SerializationFormat.Binary);

            // Act
            EasySerializer.Serialize(ref original, ref data);
            List<int> deserialized = EasySerializer.Deserialize<List<int>>(ref data);

            // Assert
            Assert.IsNotNull(deserialized, "Deserialized list should not be null.");
            Assert.AreEqual(original.Count, deserialized.Count, "List count should match.");
            for (int i = 0; i < original.Count; i++)
            {
                Assert.AreEqual(original[i], deserialized[i], $"Element at index {i} should match.");
            }
        }

        #endregion

        #region Null Values

        [Test]
        public void SerializeDeserialize_NullString_ReturnsDefault()
        {
            // Arrange
            string original = null;
            var data = new EasySerializationData(SerializationFormat.Binary);

            // Act
            EasySerializer.Serialize(ref original, ref data);
            string deserialized = EasySerializer.Deserialize<string>(ref data);

            // Assert
            Assert.IsNull(deserialized, "Null string should deserialize to null.");
        }

        [Test]
        public void SerializeDeserialize_NullArray_ReturnsDefault()
        {
            // Arrange
            int[] original = null;
            var data = new EasySerializationData(SerializationFormat.Binary);

            // Act
            EasySerializer.Serialize(ref original, ref data);
            int[] deserialized = EasySerializer.Deserialize<int[]>(ref data);

            // Assert
            Assert.IsNull(deserialized, "Null array should deserialize to null.");
        }

        #endregion

        #region Enum Types

        [Test]
        public void SerializeDeserialize_TestEnum_ReturnsOriginalValue()
        {
            // Arrange
            TestEnum original = TestEnum.OptionB;
            var data = new EasySerializationData(SerializationFormat.Binary);

            // Act
            EasySerializer.Serialize(ref original, ref data);
            TestEnum deserialized = EasySerializer.Deserialize<TestEnum>(ref data);

            // Assert
            Assert.AreEqual(original, deserialized, "Deserialized enum should match original value.");
        }

        #endregion

        #region Generic Object

        [Test]
        public void SerializeDeserialize_TestDataClass_ReturnsOriginalValues()
        {
            // Arrange
            var original = new TestDataClass
            {
                Id = 100,
                Name = "TestPlayer",
                Health = 95.5f,
                IsActive = true,
                Position = new UnityEngine.Vector3(10, 20, 30),
                Scores = new List<int> { 80, 90, 100 }
            };
            var data = new EasySerializationData(SerializationFormat.Binary);

            // Act
            EasySerializer.Serialize(ref original, ref data);
            TestDataClass deserialized =
                EasySerializer.Deserialize<TestDataClass>(ref data);

            // Assert
            Assert.IsNotNull(deserialized, "Deserialized object should not be null.");
            Assert.AreEqual(original.Id, deserialized.Id, "Id should match.");
            Assert.AreEqual(original.Name, deserialized.Name, "Name should match.");
            Assert.AreEqual(original.Health, deserialized.Health, 0.001f, "Health should match.");
            Assert.AreEqual(original.IsActive, deserialized.IsActive, "IsActive should match.");
            Assert.AreEqual(original.Position.x, deserialized.Position.x, 0.001f, "Position X should match.");
            Assert.AreEqual(original.Scores.Count, deserialized.Scores.Count, "Scores count should match.");
        }

        #endregion
    }

    /// <summary>Test enum for serialization testing.</summary>
    public enum TestEnum
    {
        OptionA = 0,
        OptionB = 1,
        OptionC = 2
    }

    /// <summary>Test data class for serialization testing.</summary>
    [Serializable]
    public class TestDataClass
    {
        public int Id;
        public string Name;
        public float Health;
        public bool IsActive;
        public UnityEngine.Vector3 Position;
        public List<int> Scores;
    }
}
