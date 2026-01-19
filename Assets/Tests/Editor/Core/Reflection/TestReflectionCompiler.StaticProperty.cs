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
        /// Verifies that CreateStaticPropertyGetter throws ArgumentException when property has no getter.
        /// </summary>
        [Test]
        public void CreateStaticPropertyGetter_WriteOnlyProperty_ThrowsArgumentException()
        {
            // Arrange
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticWriteOnlyProperty),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionCompiler.CreateStaticPropertyGetter(propertyInfo));
            Assert.That(ex.Message, Does.Contain("does not have a getter"));
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
        /// Verifies that CreateStaticPropertySetter throws ArgumentException when property has no setter.
        /// </summary>
        [Test]
        public void CreateStaticPropertySetter_ReadOnlyProperty_ThrowsArgumentException()
        {
            // Arrange
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticReadOnlyProperty),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                ReflectionCompiler.CreateStaticPropertySetter(propertyInfo));
            Assert.That(ex.Message, Does.Contain("does not have a setter"));
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
