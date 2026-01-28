using System;
using System.Reflection;
using NUnit.Framework;
using EasyToolkit.Core.Reflection;

namespace Tests.Core.Reflection
{
    public class TestReflectionCompiler_StaticField
    {
        #region Static Field Tests

        /// <summary>
        /// Verifies that CreateStaticFieldGetter creates a getter that retrieves the static field value.
        /// </summary>
        [Test]
        public void CreateStaticFieldGetter_ValidField_ReturnsFieldValue()
        {
            // Arrange
            TestClass.StaticField = 42;
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.StaticField),
                MemberAccessFlags.PublicStatic);

            // Act
            var getter = ReflectionCompiler.CreateStaticFieldGetter(fieldInfo);
            var result = getter();

            // Assert
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Verifies that CreateStaticFieldGetter throws ArgumentNullException when fieldInfo is null.
        /// </summary>
        [Test]
        public void CreateStaticFieldGetter_NullFieldInfo_ThrowsArgumentNullException()
        {
            // Arrange
            FieldInfo fieldInfo = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateStaticFieldGetter(fieldInfo));
        }

        /// <summary>
        /// Verifies that CreateStaticFieldGetter throws ArgumentException when field is not static.
        /// </summary>
        [Test]
        public void CreateStaticFieldGetter_InstanceField_ThrowsArgumentException()
        {
            // Arrange
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.InstanceField),
                MemberAccessFlags.PublicInstance);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateStaticFieldGetter(fieldInfo));
            Assert.That(ex.Message, Does.Contain("not static"));
        }

        /// <summary>
        /// Verifies that CreateStaticFieldSetter creates a setter that modifies the static field value.
        /// </summary>
        [Test]
        public void CreateStaticFieldSetter_ValidField_SetsFieldValue()
        {
            // Arrange
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.StaticField),
                MemberAccessFlags.PublicStatic);
            var setter = ReflectionCompiler.CreateStaticFieldSetter(fieldInfo);

            // Act
            setter(100);

            // Assert
            Assert.AreEqual(100, TestClass.StaticField);
        }

        /// <summary>
        /// Verifies that CreateStaticFieldSetter throws ArgumentNullException when fieldInfo is null.
        /// </summary>
        [Test]
        public void CreateStaticFieldSetter_NullFieldInfo_ThrowsArgumentNullException()
        {
            // Arrange
            FieldInfo fieldInfo = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateStaticFieldSetter(fieldInfo));
        }

        /// <summary>
        /// Verifies that CreateStaticFieldSetter throws ArgumentException when field is not static.
        /// </summary>
        [Test]
        public void CreateStaticFieldSetter_InstanceField_ThrowsArgumentException()
        {
            // Arrange
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.InstanceField),
                MemberAccessFlags.PublicInstance);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateStaticFieldSetter(fieldInfo));
            Assert.That(ex.Message, Does.Contain("not static"));
        }

        /// <summary>
        /// Verifies that CreateStaticFieldSetter throws ArgumentException when field is read-only.
        /// </summary>
        [Test]
        public void CreateStaticFieldSetter_ReadOnlyField_ThrowsArgumentException()
        {
            // Arrange
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.StaticReadOnlyField),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateStaticFieldSetter(fieldInfo));
            Assert.That(ex.Message, Does.Contain("read-only"));
        }

        #endregion
    }
}
