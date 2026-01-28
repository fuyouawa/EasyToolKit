using System;
using System.Reflection;
using NUnit.Framework;
using EasyToolkit.Core.Reflection;

namespace Tests.Core.Reflection
{
    public class TestReflectionCompiler_TypedProperty
    {
        #region Typed Static Property Getter Tests

        /// <summary>
        /// Verifies that CreateStaticPropertyGetter&lt;TValue&gt; creates a strongly-typed getter that retrieves the static property value.
        /// </summary>
        [Test]
        public void CreateStaticPropertyGetterT_ValidProperty_ReturnsPropertyValue()
        {
            // Arrange
            TestClass.StaticProperty = 42;
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticProperty),
                MemberAccessFlags.PublicStatic);

            // Act
            var getter = ReflectionCompiler.CreateStaticPropertyGetter<int>(propertyInfo);
            var result = getter();

            // Assert
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Verifies that CreateStaticPropertyGetter&lt;TValue&gt; handles type conversion correctly.
        /// </summary>
        [Test]
        public void CreateStaticPropertyGetterT_WithTypeConversion_ReturnsConvertedValue()
        {
            // Arrange
            TestClass.StaticProperty = 42;
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticProperty),
                MemberAccessFlags.PublicStatic);

            // Act
            var getter = ReflectionCompiler.CreateStaticPropertyGetter<object>(propertyInfo);
            var result = getter();

            // Assert
            Assert.AreEqual(42, result);
            Assert.IsInstanceOf<object>(result);
        }

        /// <summary>
        /// Verifies that CreateStaticPropertyGetter&lt;TValue&gt; throws ArgumentNullException when propertyInfo is null.
        /// </summary>
        [Test]
        public void CreateStaticPropertyGetterT_NullPropertyInfo_ThrowsArgumentNullException()
        {
            // Arrange
            PropertyInfo propertyInfo = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateStaticPropertyGetter<int>(propertyInfo));
        }

        /// <summary>
        /// Verifies that CreateStaticPropertyGetter&lt;TValue&gt; throws ArgumentException when property does not have a getter.
        /// </summary>
        [Test]
        public void CreateStaticPropertyGetterT_WriteOnlyProperty_ThrowsArgumentException()
        {
            // Arrange
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticWriteOnlyProperty),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateStaticPropertyGetter<int>(propertyInfo));
            Assert.That(ex.Message, Does.Contain("does not have a getter"));
        }

        /// <summary>
        /// Verifies that CreateStaticPropertyGetter&lt;TValue&gt; throws ArgumentException when property is not static.
        /// </summary>
        [Test]
        public void CreateStaticPropertyGetterT_InstanceProperty_ThrowsArgumentException()
        {
            // Arrange
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.InstanceProperty),
                MemberAccessFlags.PublicInstance);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateStaticPropertyGetter<int>(propertyInfo));
            Assert.That(ex.Message, Does.Contain("not static"));
        }

        #endregion

        #region Typed Instance Property Getter Tests

        /// <summary>
        /// Verifies that CreateInstancePropertyGetter&lt;TInstance, TValue&gt; creates a strongly-typed getter that retrieves the instance property value.
        /// </summary>
        [Test]
        public void CreateInstancePropertyGetterT_ValidProperty_ReturnsPropertyValue()
        {
            // Arrange
            var testInstance = new TestClass { InstanceProperty = 42 };
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.InstanceProperty),
                MemberAccessFlags.PublicInstance);

            // Act
            var getter = ReflectionCompiler.CreateInstancePropertyGetter<TestClass, int>(propertyInfo);
            var result = getter(ref testInstance);

            // Assert
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Verifies that CreateInstancePropertyGetter&lt;TInstance, TValue&gt; handles type conversion correctly.
        /// </summary>
        [Test]
        public void CreateInstancePropertyGetterT_WithTypeConversion_ReturnsConvertedValue()
        {
            // Arrange
            var testInstance = new TestClass { InstanceProperty = 42 };
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.InstanceProperty),
                MemberAccessFlags.PublicInstance);

            // Act
            var getter = ReflectionCompiler.CreateInstancePropertyGetter<TestClass, object>(propertyInfo);
            var result = getter(ref testInstance);

            // Assert
            Assert.AreEqual(42, result);
            Assert.IsInstanceOf<object>(result);
        }

        /// <summary>
        /// Verifies that CreateInstancePropertyGetter&lt;TInstance, TValue&gt; throws ArgumentNullException when propertyInfo is null.
        /// </summary>
        [Test]
        public void CreateInstancePropertyGetterT_NullPropertyInfo_ThrowsArgumentNullException()
        {
            // Arrange
            PropertyInfo propertyInfo = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateInstancePropertyGetter<TestClass, int>(propertyInfo));
        }

        /// <summary>
        /// Verifies that CreateInstancePropertyGetter&lt;TInstance, TValue&gt; throws ArgumentException when property does not have a getter.
        /// </summary>
        [Test]
        public void CreateInstancePropertyGetterT_WriteOnlyProperty_ThrowsArgumentException()
        {
            // Arrange
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.WriteOnlyProperty),
                MemberAccessFlags.PublicInstance);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateInstancePropertyGetter<TestClass, int>(propertyInfo));
            Assert.That(ex.Message, Does.Contain("does not have a getter"));
        }

        /// <summary>
        /// Verifies that CreateInstancePropertyGetter&lt;TInstance, TValue&gt; throws ArgumentException when property is static.
        /// </summary>
        [Test]
        public void CreateInstancePropertyGetterT_StaticProperty_ThrowsArgumentException()
        {
            // Arrange
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticProperty),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateInstancePropertyGetter<TestClass, int>(propertyInfo));
            Assert.That(ex.Message, Does.Contain("not an instance property"));
        }

        #endregion

        #region Typed Static Property Setter Tests

        /// <summary>
        /// Verifies that CreateStaticPropertySetter&lt;TValue&gt; creates a strongly-typed setter that modifies the static property value.
        /// </summary>
        [Test]
        public void CreateStaticPropertySetterT_ValidProperty_SetsPropertyValue()
        {
            // Arrange
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticProperty),
                MemberAccessFlags.PublicStatic);
            var setter = ReflectionCompiler.CreateStaticPropertySetter<int>(propertyInfo);

            // Act
            setter(100);

            // Assert
            Assert.AreEqual(100, TestClass.StaticProperty);
        }

        /// <summary>
        /// Verifies that CreateStaticPropertySetter&lt;TValue&gt; handles type conversion correctly.
        /// </summary>
        [Test]
        public void CreateStaticPropertySetterT_WithTypeConversion_SetsPropertyValue()
        {
            // Arrange
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticProperty),
                MemberAccessFlags.PublicStatic);
            var setter = ReflectionCompiler.CreateStaticPropertySetter<object>(propertyInfo);

            // Act
            setter(200);

            // Assert
            Assert.AreEqual(200, TestClass.StaticProperty);
        }

        /// <summary>
        /// Verifies that CreateStaticPropertySetter&lt;TValue&gt; throws ArgumentNullException when propertyInfo is null.
        /// </summary>
        [Test]
        public void CreateStaticPropertySetterT_NullPropertyInfo_ThrowsArgumentNullException()
        {
            // Arrange
            PropertyInfo propertyInfo = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateStaticPropertySetter<int>(propertyInfo));
        }

        /// <summary>
        /// Verifies that CreateStaticPropertySetter&lt;TValue&gt; throws ArgumentException when property does not have a setter.
        /// </summary>
        [Test]
        public void CreateStaticPropertySetterT_ReadOnlyProperty_ThrowsArgumentException()
        {
            // Arrange
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticReadOnlyProperty),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateStaticPropertySetter<int>(propertyInfo));
            Assert.That(ex.Message, Does.Contain("does not have a setter"));
        }

        /// <summary>
        /// Verifies that CreateStaticPropertySetter&lt;TValue&gt; throws ArgumentException when property is not static.
        /// </summary>
        [Test]
        public void CreateStaticPropertySetterT_InstanceProperty_ThrowsArgumentException()
        {
            // Arrange
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.InstanceProperty),
                MemberAccessFlags.PublicInstance);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateStaticPropertySetter<int>(propertyInfo));
            Assert.That(ex.Message, Does.Contain("not static"));
        }

        #endregion

        #region Typed Instance Property Setter Tests

        /// <summary>
        /// Verifies that CreateInstancePropertySetter&lt;TInstance, TValue&gt; creates a strongly-typed setter that modifies the instance property value.
        /// </summary>
        [Test]
        public void CreateInstancePropertySetterT_ValidProperty_SetsPropertyValue()
        {
            // Arrange
            var testInstance = new TestClass { InstanceProperty = 0 };
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.InstanceProperty),
                MemberAccessFlags.PublicInstance);
            var setter = ReflectionCompiler.CreateInstancePropertySetter<TestClass, int>(propertyInfo);

            // Act
            setter(ref testInstance, 100);

            // Assert
            Assert.AreEqual(100, testInstance.InstanceProperty);
        }

        /// <summary>
        /// Verifies that CreateInstancePropertySetter&lt;TInstance, TValue&gt; handles type conversion correctly.
        /// </summary>
        [Test]
        public void CreateInstancePropertySetterT_WithTypeConversion_SetsPropertyValue()
        {
            // Arrange
            var testInstance = new TestClass { InstanceProperty = 0 };
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.InstanceProperty),
                MemberAccessFlags.PublicInstance);
            var setter = ReflectionCompiler.CreateInstancePropertySetter<TestClass, object>(propertyInfo);

            // Act
            setter(ref testInstance, 200);

            // Assert
            Assert.AreEqual(200, testInstance.InstanceProperty);
        }

        /// <summary>
        /// Verifies that CreateInstancePropertySetter&lt;TInstance, TValue&gt; throws ArgumentNullException when propertyInfo is null.
        /// </summary>
        [Test]
        public void CreateInstancePropertySetterT_NullPropertyInfo_ThrowsArgumentNullException()
        {
            // Arrange
            PropertyInfo propertyInfo = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateInstancePropertySetter<TestClass, int>(propertyInfo));
        }

        /// <summary>
        /// Verifies that CreateInstancePropertySetter&lt;TInstance, TValue&gt; throws ArgumentException when property does not have a setter.
        /// </summary>
        [Test]
        public void CreateInstancePropertySetterT_ReadOnlyProperty_ThrowsArgumentException()
        {
            // Arrange
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.ReadOnlyProperty),
                MemberAccessFlags.PublicInstance);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateInstancePropertySetter<TestClass, int>(propertyInfo));
            Assert.That(ex.Message, Does.Contain("does not have a setter"));
        }

        /// <summary>
        /// Verifies that CreateInstancePropertySetter&lt;TInstance, TValue&gt; throws ArgumentException when property is static.
        /// </summary>
        [Test]
        public void CreateInstancePropertySetterT_StaticProperty_ThrowsArgumentException()
        {
            // Arrange
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticProperty),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateInstancePropertySetter<TestClass, int>(propertyInfo));
            Assert.That(ex.Message, Does.Contain("not an instance property"));
        }

        #endregion
    }
}
