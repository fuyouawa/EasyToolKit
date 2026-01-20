using System;
using System.Reflection;
using NUnit.Framework;
using EasyToolKit.Core.Reflection;

namespace Tests.Core.Reflection
{
    public class TestReflectionCompiler_StaticProperty
    {
        #region Static Property Tests

        /// <summary>
        /// Verifies that CreateStaticPropertyGetter creates a getter that retrieves the static property value.
        /// </summary>
        [Test]
        public void CreateStaticPropertyGetter_ValidProperty_ReturnsPropertyValue()
        {
            // Arrange
            TestClass.StaticProperty = 42;
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticProperty),
                MemberAccessFlags.PublicStatic);

            // Act
            var getter = ReflectionCompiler.CreateStaticPropertyGetter(propertyInfo);
            var result = getter();

            // Assert
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Verifies that CreateStaticPropertyGetter throws ArgumentNullException when propertyInfo is null.
        /// </summary>
        [Test]
        public void CreateStaticPropertyGetter_NullPropertyInfo_ThrowsArgumentNullException()
        {
            // Arrange
            var propertyInfo = null as PropertyInfo;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateStaticPropertyGetter(propertyInfo));
        }

        /// <summary>
        /// Verifies that CreateStaticPropertyGetter can create a getter for a write-only property with private getter.
        /// </summary>
        [Test]
        public void CreateStaticPropertyGetter_WriteOnlyProperty_ReturnsPropertyValue()
        {
            // Arrange
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticWriteOnlyProperty),
                MemberAccessFlags.PublicStatic);
            var getter = ReflectionCompiler.CreateStaticPropertyGetter(propertyInfo);

            // Act - Set value using the property's public setter
            TestClass.StaticWriteOnlyProperty = 42;

            // Assert - Can read value using the compiled getter (accesses private getter)
            var result = getter();
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Verifies that CreateStaticPropertyGetter throws ArgumentException when property is not static.
        /// </summary>
        [Test]
        public void CreateStaticPropertyGetter_InstanceProperty_ThrowsArgumentException()
        {
            // Arrange
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.InstanceProperty),
                MemberAccessFlags.PublicInstance);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionCompiler.CreateStaticPropertyGetter(propertyInfo));
            Assert.That(ex.Message, Does.Contain("not static"));
        }

        /// <summary>
        /// Verifies that CreateStaticPropertySetter creates a setter that modifies the static property value.
        /// </summary>
        [Test]
        public void CreateStaticPropertySetter_ValidProperty_SetsPropertyValue()
        {
            // Arrange
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticProperty),
                MemberAccessFlags.PublicStatic);
            var setter = ReflectionCompiler.CreateStaticPropertySetter(propertyInfo);

            // Act
            setter(100);

            // Assert
            Assert.AreEqual(100, TestClass.StaticProperty);
        }

        /// <summary>
        /// Verifies that CreateStaticPropertySetter throws ArgumentNullException when propertyInfo is null.
        /// </summary>
        [Test]
        public void CreateStaticPropertySetter_NullPropertyInfo_ThrowsArgumentNullException()
        {
            // Arrange
            var propertyInfo = null as PropertyInfo;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateStaticPropertySetter(propertyInfo));
        }

        /// <summary>
        /// Verifies that CreateStaticPropertySetter can create a setter for a read-only property with private setter.
        /// </summary>
        [Test]
        public void CreateStaticPropertySetter_ReadOnlyProperty_SetsPropertyValue()
        {
            // Arrange
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticReadOnlyProperty),
                MemberAccessFlags.PublicStatic);
            var setter = ReflectionCompiler.CreateStaticPropertySetter(propertyInfo);

            // Act - Set value using the compiled setter (accesses private setter)
            setter(100);

            // Assert - Can read value using the property's public getter
            Assert.AreEqual(100, TestClass.StaticReadOnlyProperty);
        }

        /// <summary>
        /// Verifies that CreateStaticPropertySetter throws ArgumentException when property is not static.
        /// </summary>
        [Test]
        public void CreateStaticPropertySetter_InstanceProperty_ThrowsArgumentException()
        {
            // Arrange
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.InstanceProperty),
                MemberAccessFlags.PublicInstance);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionCompiler.CreateStaticPropertySetter(propertyInfo));
            Assert.That(ex.Message, Does.Contain("not static"));
        }

        #endregion
    }
}
