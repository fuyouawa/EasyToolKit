using System;
using System.Reflection;
using NUnit.Framework;
using EasyToolkit.Core.Reflection;

namespace Tests.Core.Reflection
{
    public class TestReflectionCompiler_InstanceProperty
    {
        #region Instance Property Tests

        /// <summary>
        /// Verifies that CreateInstancePropertyGetter creates a getter that retrieves the instance property value.
        /// </summary>
        [Test]
        public void CreateInstancePropertyGetter_ValidProperty_ReturnsPropertyValue()
        {
            // Arrange
            var testInstance = new TestClass { InstanceProperty = 42 };
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.InstanceProperty),
                MemberAccessFlags.PublicInstance);

            // Act
            var getter = ReflectionCompiler.CreateInstancePropertyGetter(propertyInfo);
            var result = getter(testInstance);

            // Assert
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Verifies that CreateInstancePropertyGetter throws ArgumentNullException when propertyInfo is null.
        /// </summary>
        [Test]
        public void CreateInstancePropertyGetter_NullPropertyInfo_ThrowsArgumentNullException()
        {
            // Arrange
            var propertyInfo = null as PropertyInfo;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ReflectionCompiler.CreateInstancePropertyGetter(propertyInfo));
        }

        /// <summary>
        /// Verifies that CreateInstancePropertyGetter can create a getter for a write-only property with private getter.
        /// </summary>
        [Test]
        public void CreateInstancePropertyGetter_WriteOnlyProperty_ReturnsPropertyValue()
        {
            // Arrange
            var testInstance = new TestClass();
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.WriteOnlyProperty),
                MemberAccessFlags.PublicInstance);
            var getter = ReflectionCompiler.CreateInstancePropertyGetter(propertyInfo);

            // Act - Set value using the property's public setter
            testInstance.WriteOnlyProperty = 42;

            // Assert - Can read value using the compiled getter (accesses private getter)
            var result = getter(testInstance);
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Verifies that CreateInstancePropertyGetter throws ArgumentException when property is static.
        /// </summary>
        [Test]
        public void CreateInstancePropertyGetter_StaticProperty_ThrowsArgumentException()
        {
            // Arrange
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticProperty),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionCompiler.CreateInstancePropertyGetter(propertyInfo));
            Assert.That(ex.Message, Does.Contain("not an instance property"));
        }

        /// <summary>
        /// Verifies that CreateInstancePropertySetter creates a setter that modifies the instance property value.
        /// </summary>
        [Test]
        public void CreateInstancePropertySetter_ValidProperty_SetsPropertyValue()
        {
            // Arrange
            var testInstance = new TestClass { InstanceProperty = 0 };
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.InstanceProperty),
                MemberAccessFlags.PublicInstance);
            var setter = ReflectionCompiler.CreateInstancePropertySetter(propertyInfo);

            // Act
            setter(testInstance, 100);

            // Assert
            Assert.AreEqual(100, testInstance.InstanceProperty);
        }

        /// <summary>
        /// Verifies that CreateInstancePropertySetter throws ArgumentNullException when propertyInfo is null.
        /// </summary>
        [Test]
        public void CreateInstancePropertySetter_NullPropertyInfo_ThrowsArgumentNullException()
        {
            // Arrange
            var propertyInfo = null as PropertyInfo;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ReflectionCompiler.CreateInstancePropertySetter(propertyInfo));
        }

        /// <summary>
        /// Verifies that CreateInstancePropertySetter can create a setter for a read-only property with private setter.
        /// </summary>
        [Test]
        public void CreateInstancePropertySetter_ReadOnlyProperty_SetsPropertyValue()
        {
            // Arrange
            var testInstance = new TestClass();
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.ReadOnlyProperty),
                MemberAccessFlags.PublicInstance);
            var setter = ReflectionCompiler.CreateInstancePropertySetter(propertyInfo);

            // Act - Set value using the compiled setter (accesses private setter)
            setter(testInstance, 100);

            // Assert - Can read value using the property's public getter
            Assert.AreEqual(100, testInstance.ReadOnlyProperty);
        }

        /// <summary>
        /// Verifies that CreateInstancePropertySetter throws ArgumentException when property is static.
        /// </summary>
        [Test]
        public void CreateInstancePropertySetter_StaticProperty_ThrowsArgumentException()
        {
            // Arrange
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticProperty),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionCompiler.CreateInstancePropertySetter(propertyInfo));
            Assert.That(ex.Message, Does.Contain("not an instance property"));
        }

        #endregion
    }
}
