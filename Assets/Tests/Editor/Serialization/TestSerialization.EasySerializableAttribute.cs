using System;
using System.Collections.Generic;
using NUnit.Framework;
using EasyToolKit.Serialization;

namespace Tests.Serialization
{
    /// <summary>Unit tests for EasySerializableAttribute functionality.</summary>
    [TestFixture]
    public class TestSerialization_EasySerializableAttribute
    {
        #region Default MemberFlags (AllFields)

        /// <summary>
        /// Verifies that default MemberFlags serializes all fields (public and non-public)
        /// but not properties.
        /// </summary>
        [Test]
        public void DefaultMemberFlags_SerializesAllFields_NotProperties()
        {
            // Arrange
            var original = new DefaultMemberFlagsClass(1, 2, 3, 4, 5, 6, 7);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<DefaultMemberFlagsClass>(data);

            // Assert - All fields should be serialized
            Assert.AreEqual(1, result.publicField, "Public field should be serialized");
            Assert.AreEqual(2, result.GetPrivateField(), "Private field should be serialized");
            Assert.AreEqual(3, result.GetProtectedField(), "Protected field should be serialized");
            Assert.AreEqual(4, result.internalField, "Internal field should be serialized");
            // Properties should NOT be serialized with default (AllFields)
            Assert.AreEqual(0, result.PublicProperty, "Public property should not be serialized");
            Assert.AreEqual(0, result.GetPrivateProperty(), "Private property should not be serialized");
            Assert.AreEqual(0, result.GetProtectedProperty(), "Protected property should not be serialized");
        }

        #endregion

        #region PublicFields Only

        /// <summary>
        /// Verifies that PublicFields only serializes public fields.
        /// </summary>
        [Test]
        public void PublicFieldsOnly_SerializesPublicFields_Only()
        {
            // Arrange
            var original = new PublicFieldsOnlyClass(1, 2, 3, 4);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<PublicFieldsOnlyClass>(data);

            // Assert
            Assert.AreEqual(1, result.publicField, "Public field should be serialized");
            Assert.AreEqual(0, result.GetPrivateField(), "Private field should not be serialized");
            Assert.AreEqual(0, result.PublicProperty, "Public property should not be serialized");
        }

        #endregion

        #region PublicProperties Only

        /// <summary>
        /// Verifies that PublicProperties only serializes public properties.
        /// </summary>
        [Test]
        public void PublicPropertiesOnly_SerializesPublicProperties_Only()
        {
            // Arrange
            var original = new PublicPropertiesOnlyClass(1, 2, 3, 4);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<PublicPropertiesOnlyClass>(data);

            // Assert
            Assert.AreEqual(3, result.PublicProperty, "Public property should be serialized");
            Assert.AreEqual(0, result.publicField, "Public field should not be serialized");
            Assert.AreEqual(0, result.GetPrivateProperty(), "Private property should not be serialized");
        }

        #endregion

        #region AllPublic (Fields and Properties)

        /// <summary>
        /// Verifies that AllPublic serializes all public members (fields and properties).
        /// </summary>
        [Test]
        public void AllPublic_SerializesPublicFieldsAndProperties()
        {
            // Arrange
            var original = new AllPublicClass(1, 2, 3, 4);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<AllPublicClass>(data);

            // Assert
            Assert.AreEqual(1, result.publicField, "Public field should be serialized");
            Assert.AreEqual(0, result.GetPrivateField(), "Private field should not be serialized");
            Assert.AreEqual(3, result.PublicProperty, "Public property should be serialized");
            Assert.AreEqual(0, result.GetPrivateProperty(), "Private property should not be serialized");
        }

        #endregion

        #region NonPublicFields Only

        /// <summary>
        /// Verifies that NonPublicFields serializes non-public fields only.
        /// </summary>
        [Test]
        public void NonPublicFieldsOnly_SerializesNonPublicFields_Only()
        {
            // Arrange
            var original = new NonPublicFieldsOnlyClass(1, 2);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<NonPublicFieldsOnlyClass>(data);

            // Assert
            Assert.AreEqual(0, result.publicField, "Public field should not be serialized");
            Assert.AreEqual(2, result.GetPrivateField(), "Private field should be serialized");
        }

        #endregion

        #region AllFields (Public and NonPublic)

        /// <summary>
        /// Verifies that AllFields serializes all fields regardless of visibility.
        /// </summary>
        [Test]
        public void AllFields_SerializesAllFields_ByVisibility()
        {
            // Arrange
            var original = new AllFieldsClass(1, 2, 3);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<AllFieldsClass>(data);

            // Assert
            Assert.AreEqual(1, result.publicField, "Public field should be serialized");
            Assert.AreEqual(2, result.GetPrivateField(), "Private field should be serialized");
            Assert.AreEqual(3, result.GetProtectedField(), "Protected field should be serialized");
        }

        #endregion

        #region AllProperties (Public and NonPublic)

        /// <summary>
        /// Verifies that AllProperties serializes all properties regardless of visibility.
        /// </summary>
        [Test]
        public void AllProperties_SerializesAllProperties_ByVisibility()
        {
            // Arrange
            var original = new AllPropertiesClass(1, 2, 3);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<AllPropertiesClass>(data);

            // Assert
            Assert.AreEqual(1, result.PublicProperty, "Public property should be serialized");
            Assert.AreEqual(2, result.GetPrivateProperty(), "Private property should be serialized");
            Assert.AreEqual(3, result.GetProtectedProperty(), "Protected property should be serialized");
        }

        #endregion

        #region All Members

        /// <summary>
        /// Verifies that All serializes all fields and properties regardless of visibility.
        /// </summary>
        [Test]
        public void AllMembers_SerializesAllFieldsAndProperties()
        {
            // Arrange
            var original = new AllMembersClass(1, 2, 3, 4);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<AllMembersClass>(data);

            // Assert
            Assert.AreEqual(1, result.publicField, "Public field should be serialized");
            Assert.AreEqual(2, result.GetPrivateField(), "Private field should be serialized");
            Assert.AreEqual(3, result.PublicProperty, "Public property should be serialized");
            Assert.AreEqual(4, result.GetPrivateProperty(), "Private property should be serialized");
        }

        #endregion

        #region RequireSerializeFieldOnNonPublic

        /// <summary>
        /// Verifies that when RequireSerializeFieldOnNonPublic is true,
        /// only non-public fields with [SerializeField] are serialized.
        /// </summary>
        [Test]
        public void RequireSerializeFieldOnNonPublic_OnlySerializeFieldFieldsAreSerialized()
        {
            // Arrange
            var original = new RequireSerializeFieldClass(1, 2, 3);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<RequireSerializeFieldClass>(data);

            // Assert
            Assert.AreEqual(1, result.publicField, "Public field should be serialized");
            Assert.AreEqual(2, result.GetPrivateFieldWithAttribute(), "Private field with [SerializeField] should be serialized");
            Assert.AreEqual(0, result.GetPrivateFieldWithoutAttribute(), "Private field without [SerializeField] should not be serialized");
        }

        /// <summary>
        /// Verifies that when RequireSerializeFieldOnNonPublic is false,
        /// all non-public fields are serialized based on MemberFlags.
        /// </summary>
        [Test]
        public void NotRequireSerializeFieldOnNonPublic_AllFieldsSerializedByFlags()
        {
            // Arrange
            var original = new NotRequireSerializeFieldClass(1, 2);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<NotRequireSerializeFieldClass>(data);

            // Assert - Default is AllFields, so both should be serialized
            Assert.AreEqual(1, result.publicField, "Public field should be serialized");
            Assert.AreEqual(2, result.GetPrivateField(), "Private field should be serialized without [SerializeField]");
        }

        #endregion

        #region AllocInherit

        /// <summary>
        /// Verifies that when AllocInherit is false, derived classes do not inherit
        /// the base class's EasySerializableAttribute configuration and cannot be serialized.
        /// </summary>
        [Test]
        public void AllocInheritFalse_DerivedClassNotSerializable()
        {
            // Arrange
            var original = new DerivedFromNoInherit(100, 200);

            // Act & Assert
            // Derived class has no [EasySerializable] attribute and cannot inherit from base (AllocInherit=false)
            var ex = Assert.Throws<SerializationException>(() =>
                EasySerializer.SerializeToBinary(ref original));

            Assert.That(ex.Message, Does.Contain("DerivedFromNoInherit"),
                "Exception message should mention the derived type name");
        }

        /// <summary>
        /// Verifies that when AllocInherit is true, derived classes inherit
        /// the base class's EasySerializableAttribute configuration.
        /// </summary>
        [Test]
        public void AllocInheritTrue_DerivedClassInheritsConfiguration()
        {
            // Arrange
            var original = new DerivedFromWithInherit(100, 999, 200);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<DerivedFromWithInherit>(data);

            // Assert - Base config is AllPublic, so public fields should be serialized (both base and derived)
            Assert.AreEqual(100, result.baseField, "Base field should be serialized (AllocInherit=true, AllPublic)");
            Assert.AreEqual(200, result.derivedField, "Derived public field should be serialized (inherited AllPublic)");
        }

        /// <summary>
        /// Verifies that when a derived class has [EasySerializable] attribute,
        /// both base and derived fields are serialized (attribute applies to entire object).
        /// </summary>
        [Test]
        public void DerivedWithAttribute_SerializesEntireObjectIncludingBase()
        {
            // Arrange
            var original = new DerivedWithAttribute(100, 200);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<DerivedWithAttribute>(data);

            // Assert - Derived class has [EasySerializable], so entire object is serialized (base + derived)
            Assert.AreEqual(100, result.baseField, "Base field should be serialized (derived class has attribute)");
            Assert.AreEqual(200, result.derivedField, "Derived field should be serialized (default AllFields)");
        }

        #endregion
    }
}
