using System;
using NUnit.Framework;
using EasyToolKit.Core.Reflection;

namespace Tests.Core.Reflection
{
    /// <summary>
    /// Tests for the ReflectionPathFactory Accessor functionality.
    /// </summary>
    public class TestReflectionPathFactory_Accessor
    {
        #region BuildAccessor Tests

        /// <summary>
        /// Verifies that BuildAccessor throws ArgumentException when memberPath is null.
        /// </summary>
        [Test]
        public void BuildAccessor_NullPath_ThrowsArgumentException()
        {
            // Arrange
            string memberPath = null;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => ReflectionPathFactory.BuildAccessor(memberPath));
        }

        /// <summary>
        /// Verifies that BuildAccessor throws ArgumentException when memberPath is empty.
        /// </summary>
        [Test]
        public void BuildAccessor_EmptyPath_ThrowsArgumentException()
        {
            // Arrange
            string memberPath = string.Empty;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => ReflectionPathFactory.BuildAccessor(memberPath));
        }

        /// <summary>
        /// Verifies that BuildAccessor throws ArgumentException when memberPath is whitespace.
        /// </summary>
        [Test]
        public void BuildAccessor_WhitespacePath_ThrowsArgumentException()
        {
            // Arrange
            string memberPath = "   ";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => ReflectionPathFactory.BuildAccessor(memberPath));
        }

        /// <summary>
        /// Verifies that BuildAccessor returns a valid IAccessorBuilder for a valid path.
        /// </summary>
        [Test]
        public void BuildAccessor_ValidPath_ReturnsAccessorBuilder()
        {
            // Arrange
            string memberPath = "InstanceField";

            // Act
            var builder = ReflectionPathFactory.BuildAccessor(memberPath);

            // Assert
            Assert.IsNotNull(builder);
            Assert.IsInstanceOf<IAccessorBuilder>(builder);
        }

        #endregion

        #region BuildStaticGetter Tests

        /// <summary>
        /// Verifies that BuildStaticGetter retrieves value from a static field.
        /// </summary>
        [Test]
        public void BuildStaticGetter_StaticField_ReturnsFieldValue()
        {
            // Arrange
            TestClassForAccessor.StaticField = 100;
            var builder = ReflectionPathFactory.BuildAccessor("StaticField");

            // Act
            var getter = builder.BuildStaticGetter(typeof(TestClassForAccessor));
            var result = getter();

            // Assert
            Assert.AreEqual(100, result);
        }

        /// <summary>
        /// Verifies that BuildStaticGetter retrieves value from a static property.
        /// </summary>
        [Test]
        public void BuildStaticGetter_StaticProperty_ReturnsPropertyValue()
        {
            // Arrange
            TestClassForAccessor.StaticProperty = "TestValue";
            var builder = ReflectionPathFactory.BuildAccessor("StaticProperty");

            // Act
            var getter = builder.BuildStaticGetter(typeof(TestClassForAccessor));
            var result = getter();

            // Assert
            Assert.AreEqual("TestValue", result);
        }

        /// <summary>
        /// Verifies that BuildStaticGetter can navigate nested static paths.
        /// </summary>
        [Test]
        public void BuildStaticGetter_NestedStaticPath_ReturnsNestedValue()
        {
            // Arrange
            TestClassForAccessor.StaticNested.NestedField = 2.71;
            var builder = ReflectionPathFactory.BuildAccessor("StaticNested.NestedField");

            // Act
            var getter = builder.BuildStaticGetter(typeof(TestClassForAccessor));
            var result = getter();

            // Assert
            Assert.AreEqual(2.71, result);
        }

        /// <summary>
        /// Verifies that BuildStaticGetter throws ArgumentException for invalid member path.
        /// </summary>
        [Test]
        public void BuildStaticGetter_InvalidPath_ThrowsArgumentException()
        {
            // Arrange
            var builder = ReflectionPathFactory.BuildAccessor("NonExistentField");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => builder.BuildStaticGetter(typeof(TestClassForAccessor)));
        }

        #endregion

        #region BuildInstanceGetter Tests

        /// <summary>
        /// Verifies that BuildInstanceGetter retrieves value from an instance field.
        /// </summary>
        [Test]
        public void BuildInstanceGetter_InstanceField_ReturnsFieldValue()
        {
            // Arrange
            var testInstance = new TestClassForAccessor { InstanceField = 42 };
            var builder = ReflectionPathFactory.BuildAccessor("InstanceField");

            // Act
            var getter = builder.BuildInstanceGetter(typeof(TestClassForAccessor));
            var result = getter(testInstance);

            // Assert
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Verifies that BuildInstanceGetter retrieves value from an instance property.
        /// </summary>
        [Test]
        public void BuildInstanceGetter_InstanceProperty_ReturnsPropertyValue()
        {
            // Arrange
            var testInstance = new TestClassForAccessor { InstanceProperty = "PropertyValue" };
            var builder = ReflectionPathFactory.BuildAccessor("InstanceProperty");

            // Act
            var getter = builder.BuildInstanceGetter(typeof(TestClassForAccessor));
            var result = getter(testInstance);

            // Assert
            Assert.AreEqual("PropertyValue", result);
        }

        /// <summary>
        /// Verifies that BuildInstanceGetter can navigate nested instance paths.
        /// </summary>
        [Test]
        public void BuildInstanceGetter_NestedInstancePath_ReturnsNestedValue()
        {
            // Arrange
            var testInstance = new TestClassForAccessor { Nested = new NestedClassForAccessor { NestedField = 1.618 } };
            var builder = ReflectionPathFactory.BuildAccessor("Nested.NestedField");

            // Act
            var getter = builder.BuildInstanceGetter(typeof(TestClassForAccessor));
            var result = getter(testInstance);

            // Assert
            Assert.AreEqual(1.618, result);
        }

        /// <summary>
        /// Verifies that BuildInstanceGetter can navigate deeply nested paths.
        /// </summary>
        [Test]
        public void BuildInstanceGetter_DeepNestedPath_ReturnsCorrectValue()
        {
            // Arrange
            var testInstance = new TestClassForAccessor { Nested = new NestedClassForAccessor { NestedProperty = "DeepValue" } };
            var builder = ReflectionPathFactory.BuildAccessor("Nested.NestedProperty");

            // Act
            var getter = builder.BuildInstanceGetter(typeof(TestClassForAccessor));
            var result = getter(testInstance);

            // Assert
            Assert.AreEqual("DeepValue", result);
        }

        /// <summary>
        /// Verifies that BuildInstanceGetter throws ArgumentException for invalid member path.
        /// </summary>
        [Test]
        public void BuildInstanceGetter_InvalidPath_ThrowsArgumentException()
        {
            // Arrange
            var builder = ReflectionPathFactory.BuildAccessor("NonExistentMember");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => builder.BuildInstanceGetter(typeof(TestClassForAccessor)));
        }

        #endregion

        #region BuildInstanceSetter Tests

        /// <summary>
        /// Verifies that BuildInstanceSetter sets value to an instance field.
        /// </summary>
        [Test]
        public void BuildInstanceSetter_InstanceField_SetsFieldValue()
        {
            // Arrange
            var testInstance = new TestClassForAccessor { InstanceField = 0 };
            var builder = ReflectionPathFactory.BuildAccessor("InstanceField");

            // Act
            var setter = builder.BuildInstanceSetter(typeof(TestClassForAccessor));
            setter(testInstance, 999);

            // Assert
            Assert.AreEqual(999, testInstance.InstanceField);
        }

        /// <summary>
        /// Verifies that BuildInstanceSetter sets value to an instance property.
        /// </summary>
        [Test]
        public void BuildInstanceSetter_InstanceProperty_SetsPropertyValue()
        {
            // Arrange
            var testInstance = new TestClassForAccessor { InstanceProperty = "OldValue" };
            var builder = ReflectionPathFactory.BuildAccessor("InstanceProperty");

            // Act
            var setter = builder.BuildInstanceSetter(typeof(TestClassForAccessor));
            setter(testInstance, "NewValue");

            // Assert
            Assert.AreEqual("NewValue", testInstance.InstanceProperty);
        }

        /// <summary>
        /// Verifies that BuildInstanceSetter can set values on nested paths.
        /// </summary>
        [Test]
        public void BuildInstanceSetter_NestedPath_SetsNestedValue()
        {
            // Arrange
            var testInstance = new TestClassForAccessor { Nested = new NestedClassForAccessor() };
            var builder = ReflectionPathFactory.BuildAccessor("Nested.NestedField");

            // Act
            var setter = builder.BuildInstanceSetter(typeof(TestClassForAccessor));
            setter(testInstance, 5.55);

            // Assert
            Assert.AreEqual(5.55, testInstance.Nested.NestedField);
        }

        /// <summary>
        /// Verifies that BuildInstanceSetter can set values on deeply nested property paths.
        /// </summary>
        [Test]
        public void BuildInstanceSetter_DeepNestedPath_SetsCorrectValue()
        {
            // Arrange
            var testInstance = new TestClassForAccessor { Nested = new NestedClassForAccessor() };
            var builder = ReflectionPathFactory.BuildAccessor("Nested.NestedProperty");

            // Act
            var setter = builder.BuildInstanceSetter(typeof(TestClassForAccessor));
            setter(testInstance, "UpdatedNested");

            // Assert
            Assert.AreEqual("UpdatedNested", testInstance.Nested.NestedProperty);
        }

        /// <summary>
        /// Verifies that BuildInstanceSetter throws ArgumentException for invalid member path.
        /// </summary>
        [Test]
        public void BuildInstanceSetter_InvalidPath_ThrowsArgumentException()
        {
            // Arrange
            var builder = ReflectionPathFactory.BuildAccessor("NonExistentMember");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => builder.BuildInstanceSetter(typeof(TestClassForAccessor)));
        }

        #endregion

        #region BuildStaticSetter Tests

        /// <summary>
        /// Verifies that BuildStaticSetter sets value to a static field.
        /// </summary>
        [Test]
        public void BuildStaticSetter_StaticField_SetsFieldValue()
        {
            // Arrange
            TestClassForAccessor.StaticField = 0;
            var builder = ReflectionPathFactory.BuildAccessor("StaticField");

            // Act
            var setter = builder.BuildStaticSetter(typeof(TestClassForAccessor));
            setter(777);

            // Assert
            Assert.AreEqual(777, TestClassForAccessor.StaticField);
        }

        /// <summary>
        /// Verifies that BuildStaticSetter sets value to a static property.
        /// </summary>
        [Test]
        public void BuildStaticSetter_StaticProperty_SetsPropertyValue()
        {
            // Arrange
            TestClassForAccessor.StaticProperty = "OldStatic";
            var builder = ReflectionPathFactory.BuildAccessor("StaticProperty");

            // Act
            var setter = builder.BuildStaticSetter(typeof(TestClassForAccessor));
            setter("NewStatic");

            // Assert
            Assert.AreEqual("NewStatic", TestClassForAccessor.StaticProperty);
        }

        /// <summary>
        /// Verifies that BuildStaticSetter can set values on nested static paths.
        /// </summary>
        [Test]
        public void BuildStaticSetter_NestedStaticPath_SetsNestedValue()
        {
            // Arrange
            TestClassForAccessor.StaticNested.NestedField = 0.0;
            var builder = ReflectionPathFactory.BuildAccessor("StaticNested.NestedField");

            // Act
            var setter = builder.BuildStaticSetter(typeof(TestClassForAccessor));
            setter(8.88);

            // Assert
            Assert.AreEqual(8.88, TestClassForAccessor.StaticNested.NestedField);
        }

        /// <summary>
        /// Verifies that BuildStaticSetter throws ArgumentException for invalid member path.
        /// </summary>
        [Test]
        public void BuildStaticSetter_InvalidPath_ThrowsArgumentException()
        {
            // Arrange
            var builder = ReflectionPathFactory.BuildAccessor("NonExistentField");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => builder.BuildStaticSetter(typeof(TestClassForAccessor)));
        }

        #endregion
    }
}
