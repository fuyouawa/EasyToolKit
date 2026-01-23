using System;
using System.Linq;
using NUnit.Framework;
using EasyToolKit.Serialization;
using EasyToolKit.Serialization.Implementations;
using Tests.Serialization;

namespace Tests.Serialization.Core.Resolvers.Implementations
{
    /// <summary>Unit tests for GenericSerializationStructureResolver functionality.</summary>
    [TestFixture]
    public class TestGenericSerializationStructureResolver
    {
        #region CanResolve Tests

        /// <summary>
        /// Verifies that CanResolve returns false for basic value types like int.
        /// </summary>
        [Test]
        public void CanResolve_BasicValueType_ReturnsFalse()
        {
            // Arrange
            var resolver = new GenericSerializationStructureResolver();

            // Act
            bool canResolveInt = resolver.CanResolve(typeof(int));
            bool canResolveFloat = resolver.CanResolve(typeof(float));
            bool canResolveBool = resolver.CanResolve(typeof(bool));

            // Assert
            Assert.IsFalse(canResolveInt, "Should not resolve int (basic value type)");
            Assert.IsFalse(canResolveFloat, "Should not resolve float (basic value type)");
            Assert.IsFalse(canResolveBool, "Should not resolve bool (basic value type)");
        }

        /// <summary>
        /// Verifies that CanResolve returns false for string type.
        /// </summary>
        [Test]
        public void CanResolve_StringType_ReturnsFalse()
        {
            // Arrange
            var resolver = new GenericSerializationStructureResolver();

            // Act
            bool canResolve = resolver.CanResolve(typeof(string));

            // Assert
            Assert.IsFalse(canResolve, "Should not resolve string (basic value type)");
        }

        /// <summary>
        /// Verifies that CanResolve returns false for enum types.
        /// </summary>
        [Test]
        public void CanResolve_EnumType_ReturnsFalse()
        {
            // Arrange
            var resolver = new GenericSerializationStructureResolver();

            // Act
            bool canResolve = resolver.CanResolve(typeof(TestEnum));

            // Assert
            Assert.IsFalse(canResolve, "Should not resolve enum (basic value type)");
        }

        /// <summary>
        /// Verifies that CanResolve returns false for UnityEngine.Object derived types.
        /// </summary>
        [Test]
        public void CanResolve_UnityObjectType_ReturnsFalse()
        {
            // Arrange
            var resolver = new GenericSerializationStructureResolver();

            // Act
            bool canResolveGameObject = resolver.CanResolve(typeof(UnityEngine.GameObject));
            bool canResolveComponent = resolver.CanResolve(typeof(UnityEngine.Component));

            // Assert
            Assert.IsFalse(canResolveGameObject, "Should not resolve GameObject (UnityEngine.Object derived)");
            Assert.IsFalse(canResolveComponent, "Should not resolve Component (UnityEngine.Object derived)");
        }

        /// <summary>
        /// Verifies that CanResolve returns true for regular classes with EasySerializable attribute.
        /// </summary>
        [Test]
        public void CanResolve_ClassWithEasySerializable_ReturnsTrue()
        {
            // Arrange
            var resolver = new GenericSerializationStructureResolver();

            // Act
            bool canResolve = resolver.CanResolve(typeof(DefaultMemberFlagsClass));

            // Assert
            Assert.IsTrue(canResolve, "Should resolve class with EasySerializable attribute");
        }

        /// <summary>
        /// Verifies that CanResolve returns true for regular classes without EasySerializable attribute.
        /// </summary>
        [Test]
        public void CanResolve_ClassWithoutAttribute_ReturnsTrue()
        {
            // Arrange
            var resolver = new GenericSerializationStructureResolver();

            // Act
            bool canResolve = resolver.CanResolve(typeof(TestDataClass));

            // Assert
            Assert.IsTrue(canResolve, "Should resolve regular class without attribute (will use default flags)");
        }

        #endregion

        #region Resolve Tests - Default MemberFlags (AllFields)

        /// <summary>
        /// Verifies that Resolve with default MemberFlags returns all fields but not properties.
        /// </summary>
        [Test]
        public void Resolve_DefaultMemberFlags_ReturnsAllFields_NotProperties()
        {
            // Arrange
            var resolver = new GenericSerializationStructureResolver();

            // Act
            var members = resolver.Resolve(typeof(DefaultMemberFlagsClass));

            // Assert
            Assert.AreEqual(4, members.Length, "Should have 4 fields (public, private, protected, internal)");
            Assert.IsTrue(members.Any(m => m.Name == "publicField"), "Should include publicField");
            Assert.IsTrue(members.Any(m => m.Name == "privateField"), "Should include privateField");
            Assert.IsTrue(members.Any(m => m.Name == "protectedField"), "Should include protectedField");
            Assert.IsTrue(members.Any(m => m.Name == "internalField"), "Should include internalField");
            Assert.IsFalse(members.Any(m => m.Name == "PublicProperty"), "Should not include PublicProperty");
            Assert.IsFalse(members.Any(m => m.Name == "PrivateProperty"), "Should not include PrivateProperty");
        }

        #endregion

        #region Resolve Tests - PublicFields Only

        /// <summary>
        /// Verifies that Resolve with PublicFields only returns public fields.
        /// </summary>
        [Test]
        public void Resolve_PublicFieldsOnly_ReturnsPublicFields_Only()
        {
            // Arrange
            var resolver = new GenericSerializationStructureResolver();

            // Act
            var members = resolver.Resolve(typeof(PublicFieldsOnlyClass));

            // Assert
            Assert.AreEqual(1, members.Length, "Should have 1 public field");
            Assert.AreEqual("publicField", members[0].Name, "Should be publicField");
            Assert.IsFalse(members.Any(m => m.Name == "privateField"), "Should not include privateField");
            Assert.IsFalse(members.Any(m => m.Name == "PublicProperty"), "Should not include PublicProperty");
        }

        #endregion

        #region Resolve Tests - PublicProperties Only

        /// <summary>
        /// Verifies that Resolve with PublicProperties only returns public properties.
        /// </summary>
        [Test]
        public void Resolve_PublicPropertiesOnly_ReturnsPublicProperties_Only()
        {
            // Arrange
            var resolver = new GenericSerializationStructureResolver();

            // Act
            var members = resolver.Resolve(typeof(PublicPropertiesOnlyClass));

            // Assert
            Assert.AreEqual(1, members.Length, "Should have 1 public property");
            Assert.AreEqual("PublicProperty", members[0].Name, "Should be PublicProperty");
            Assert.IsFalse(members.Any(m => m.Name == "publicField"), "Should not include publicField");
            Assert.IsFalse(members.Any(m => m.Name == "PrivateProperty"), "Should not include PrivateProperty");
        }

        #endregion

        #region Resolve Tests - AllPublic (Fields and Properties)

        /// <summary>
        /// Verifies that Resolve with AllPublic returns all public members (fields and properties).
        /// </summary>
        [Test]
        public void Resolve_AllPublic_ReturnsPublicFieldsAndProperties()
        {
            // Arrange
            var resolver = new GenericSerializationStructureResolver();

            // Act
            var members = resolver.Resolve(typeof(AllPublicClass));

            // Assert
            Assert.AreEqual(2, members.Length, "Should have 2 public members (field + property)");
            Assert.IsTrue(members.Any(m => m.Name == "publicField"), "Should include publicField");
            Assert.IsTrue(members.Any(m => m.Name == "PublicProperty"), "Should include PublicProperty");
            Assert.IsFalse(members.Any(m => m.Name == "privateField"), "Should not include privateField");
            Assert.IsFalse(members.Any(m => m.Name == "PrivateProperty"), "Should not include PrivateProperty");
        }

        #endregion

        #region Resolve Tests - NonPublicFields Only

        /// <summary>
        /// Verifies that Resolve with NonPublicFields returns non-public fields only.
        /// </summary>
        [Test]
        public void Resolve_NonPublicFieldsOnly_ReturnsNonPublicFields_Only()
        {
            // Arrange
            var resolver = new GenericSerializationStructureResolver();

            // Act
            var members = resolver.Resolve(typeof(NonPublicFieldsOnlyClass));

            // Assert
            Assert.AreEqual(1, members.Length, "Should have 1 non-public field");
            Assert.AreEqual("privateField", members[0].Name, "Should be privateField");
            Assert.IsFalse(members.Any(m => m.Name == "publicField"), "Should not include publicField");
        }

        #endregion

        #region Resolve Tests - AllFields (Public and NonPublic)

        /// <summary>
        /// Verifies that Resolve with AllFields returns all fields regardless of visibility.
        /// </summary>
        [Test]
        public void Resolve_AllFields_ReturnsAllFields_ByVisibility()
        {
            // Arrange
            var resolver = new GenericSerializationStructureResolver();

            // Act
            var members = resolver.Resolve(typeof(AllFieldsClass));

            // Assert
            Assert.AreEqual(3, members.Length, "Should have 3 fields (public, private, protected)");
            Assert.IsTrue(members.Any(m => m.Name == "publicField"), "Should include publicField");
            Assert.IsTrue(members.Any(m => m.Name == "privateField"), "Should include privateField");
            Assert.IsTrue(members.Any(m => m.Name == "protectedField"), "Should include protectedField");
        }

        #endregion

        #region Resolve Tests - AllProperties (Public and NonPublic)

        /// <summary>
        /// Verifies that Resolve with AllProperties returns all properties regardless of visibility.
        /// </summary>
        [Test]
        public void Resolve_AllProperties_ReturnsAllProperties_ByVisibility()
        {
            // Arrange
            var resolver = new GenericSerializationStructureResolver();

            // Act
            var members = resolver.Resolve(typeof(AllPropertiesClass));

            // Assert
            Assert.AreEqual(3, members.Length, "Should have 3 properties (public, private, protected)");
            Assert.IsTrue(members.Any(m => m.Name == "PublicProperty"), "Should include PublicProperty");
            Assert.IsTrue(members.Any(m => m.Name == "PrivateProperty"), "Should include PrivateProperty");
            Assert.IsTrue(members.Any(m => m.Name == "ProtectedProperty"), "Should include ProtectedProperty");
        }

        #endregion

        #region Resolve Tests - All Members

        /// <summary>
        /// Verifies that Resolve with All returns all fields and properties regardless of visibility.
        /// </summary>
        [Test]
        public void Resolve_AllMembers_ReturnsAllFieldsAndProperties()
        {
            // Arrange
            var resolver = new GenericSerializationStructureResolver();

            // Act
            var members = resolver.Resolve(typeof(AllMembersClass));

            // Assert
            Assert.AreEqual(4, members.Length, "Should have 4 members (2 fields + 2 properties)");
            Assert.IsTrue(members.Any(m => m.Name == "publicField"), "Should include publicField");
            Assert.IsTrue(members.Any(m => m.Name == "privateField"), "Should include privateField");
            Assert.IsTrue(members.Any(m => m.Name == "PublicProperty"), "Should include PublicProperty");
            Assert.IsTrue(members.Any(m => m.Name == "PrivateProperty"), "Should include PrivateProperty");
        }

        #endregion

        #region Resolve Tests - RequireSerializeFieldOnNonPublic

        /// <summary>
        /// Verifies that when RequireSerializeFieldOnNonPublic is true,
        /// only non-public fields with [SerializeField] are included.
        /// </summary>
        [Test]
        public void Resolve_RequireSerializeFieldOnNonPublic_OnlySerializeFieldFieldsIncluded()
        {
            // Arrange
            var resolver = new GenericSerializationStructureResolver();

            // Act
            var members = resolver.Resolve(typeof(RequireSerializeFieldClass));

            // Assert
            Assert.AreEqual(2, members.Length, "Should have 2 fields (public + private with SerializeField)");
            Assert.IsTrue(members.Any(m => m.Name == "publicField"), "Should include publicField");
            Assert.IsTrue(members.Any(m => m.Name == "privateFieldWithAttribute"), "Should include privateFieldWithAttribute");
            Assert.IsFalse(members.Any(m => m.Name == "privateFieldWithoutAttribute"), "Should not include privateFieldWithoutAttribute");
        }

        /// <summary>
        /// Verifies that when RequireSerializeFieldOnNonPublic is false,
        /// all non-public fields are included based on MemberFlags.
        /// </summary>
        [Test]
        public void Resolve_NotRequireSerializeFieldOnNonPublic_AllFieldsIncludedByFlags()
        {
            // Arrange
            var resolver = new GenericSerializationStructureResolver();

            // Act
            var members = resolver.Resolve(typeof(NotRequireSerializeFieldClass));

            // Assert - Default is AllFields, so both should be included
            Assert.AreEqual(2, members.Length, "Should have 2 fields (public + private)");
            Assert.IsTrue(members.Any(m => m.Name == "publicField"), "Should include publicField");
            Assert.IsTrue(members.Any(m => m.Name == "privateField"), "Should include privateField");
        }

        #endregion

        #region Resolve Tests - MemberDefinition Properties

        /// <summary>
        /// Verifies that resolved member definitions have correct properties set.
        /// </summary>
        [Test]
        public void Resolve_MemberDefinition_HasCorrectProperties()
        {
            // Arrange
            var resolver = new GenericSerializationStructureResolver();

            // Act
            var members = resolver.Resolve(typeof(PublicFieldsOnlyClass));

            // Assert
            Assert.IsNotEmpty(members, "Should have at least one member");
            var member = members[0];
            Assert.IsNotNull(member.Name, "Name should be set");
            Assert.IsNotNull(member.MemberType, "MemberType should be set");
            Assert.IsNotNull(member.MemberInfo, "MemberInfo should be set");
            Assert.IsFalse(member.IsRequired, "IsRequired should be false by default");
            Assert.IsNull(member.DefaultValue, "DefaultValue should be null by default");
        }

        /// <summary>
        /// Verifies that MemberType is correctly set for field members.
        /// </summary>
        [Test]
        public void Resolve_FieldMember_MemberTypeIsFieldType()
        {
            // Arrange
            var resolver = new GenericSerializationStructureResolver();

            // Act
            var members = resolver.Resolve(typeof(PublicFieldsOnlyClass));
            var intFieldMember = members.FirstOrDefault(m => m.Name == "publicField");

            // Assert
            Assert.IsNotNull(intFieldMember, "Should find publicField member");
            Assert.AreEqual(typeof(int), intFieldMember.MemberType, "MemberType should be int");
        }

        /// <summary>
        /// Verifies that MemberType is correctly set for property members.
        /// </summary>
        [Test]
        public void Resolve_PropertyMember_MemberTypeIsPropertyType()
        {
            // Arrange
            var resolver = new GenericSerializationStructureResolver();

            // Act
            var members = resolver.Resolve(typeof(PublicPropertiesOnlyClass));
            var intPropertyMember = members.FirstOrDefault(m => m.Name == "PublicProperty");

            // Assert
            Assert.IsNotNull(intPropertyMember, "Should find PublicProperty member");
            Assert.AreEqual(typeof(int), intPropertyMember.MemberType, "MemberType should be int");
        }

        #endregion
    }
}
