using System;
using System.Reflection;
using NUnit.Framework;
using EasyToolkit.Core.Reflection;

namespace Tests.Core.Reflection
{
    public class TestReflectionCompiler_InstanceField
    {
        #region Instance Field Tests

        /// <summary>
        /// Verifies that CreateInstanceFieldGetter creates a getter that retrieves the instance field value.
        /// </summary>
        [Test]
        public void CreateInstanceFieldGetter_ValidField_ReturnsFieldValue()
        {
            // Arrange
            var testInstance = new TestClass { InstanceField = 42 };
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.InstanceField),
                MemberAccessFlags.PublicInstance);

            // Act
            var getter = ReflectionCompiler.CreateInstanceFieldGetter(fieldInfo);
            var result = getter(testInstance);

            // Assert
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Verifies that CreateInstanceFieldGetter throws ArgumentNullException when fieldInfo is null.
        /// </summary>
        [Test]
        public void CreateInstanceFieldGetter_NullFieldInfo_ThrowsArgumentNullException()
        {
            // Arrange
            FieldInfo fieldInfo = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateInstanceFieldGetter(fieldInfo));
        }

        /// <summary>
        /// Verifies that CreateInstanceFieldGetter throws ArgumentException when field is static.
        /// </summary>
        [Test]
        public void CreateInstanceFieldGetter_StaticField_ThrowsArgumentException()
        {
            // Arrange
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.StaticField),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateInstanceFieldGetter(fieldInfo));
            Assert.That(ex.Message, Does.Contain("not an instance field"));
        }

        /// <summary>
        /// Verifies that CreateInstanceFieldSetter creates a setter that modifies the instance field value.
        /// </summary>
        [Test]
        public void CreateInstanceFieldSetter_ValidField_SetsFieldValue()
        {
            // Arrange
            var testInstance = new TestClass { InstanceField = 0 };
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.InstanceField),
                MemberAccessFlags.PublicInstance);
            var setter = ReflectionCompiler.CreateInstanceFieldSetter(fieldInfo);

            // Act
            setter(testInstance, 100);

            // Assert
            Assert.AreEqual(100, testInstance.InstanceField);
        }

        /// <summary>
        /// Verifies that CreateInstanceFieldSetter throws ArgumentNullException when fieldInfo is null.
        /// </summary>
        [Test]
        public void CreateInstanceFieldSetter_NullFieldInfo_ThrowsArgumentNullException()
        {
            // Arrange
            FieldInfo fieldInfo = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateInstanceFieldSetter(fieldInfo));
        }

        /// <summary>
        /// Verifies that CreateInstanceFieldSetter throws ArgumentException when field is static.
        /// </summary>
        [Test]
        public void CreateInstanceFieldSetter_StaticField_ThrowsArgumentException()
        {
            // Arrange
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.StaticField),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateInstanceFieldSetter(fieldInfo));
            Assert.That(ex.Message, Does.Contain("not an instance field"));
        }

        /// <summary>
        /// Verifies that CreateInstanceFieldSetter throws ArgumentException when field is read-only.
        /// </summary>
        [Test]
        public void CreateInstanceFieldSetter_ReadOnlyField_ThrowsArgumentException()
        {
            // Arrange
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.ReadOnlyField),
                MemberAccessFlags.PublicInstance);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateInstanceFieldSetter(fieldInfo));
            Assert.That(ex.Message, Does.Contain("read-only"));
        }

        #endregion
    }
}
