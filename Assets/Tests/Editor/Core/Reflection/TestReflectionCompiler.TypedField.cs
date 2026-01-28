using System;
using System.Reflection;
using NUnit.Framework;
using EasyToolkit.Core.Reflection;

namespace Tests.Core.Reflection
{
    public class TestReflectionCompiler_TypedField
    {
        #region Typed Static Field Getter Tests

        /// <summary>
        /// Verifies that CreateStaticFieldGetter&lt;TValue&gt; creates a strongly-typed getter that retrieves the static field value.
        /// </summary>
        [Test]
        public void CreateStaticFieldGetterT_ValidField_ReturnsFieldValue()
        {
            // Arrange
            TestClass.StaticField = 42;
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.StaticField),
                MemberAccessFlags.PublicStatic);

            // Act
            var getter = ReflectionCompiler.CreateStaticFieldGetter<int>(fieldInfo);
            var result = getter();

            // Assert
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Verifies that CreateStaticFieldGetter&lt;TValue&gt; handles type conversion correctly.
        /// </summary>
        [Test]
        public void CreateStaticFieldGetterT_WithTypeConversion_ReturnsConvertedValue()
        {
            // Arrange
            TestClass.StaticField = 42;
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.StaticField),
                MemberAccessFlags.PublicStatic);

            // Act
            var getter = ReflectionCompiler.CreateStaticFieldGetter<object>(fieldInfo);
            var result = getter();

            // Assert
            Assert.AreEqual(42, result);
            Assert.IsInstanceOf<object>(result);
        }

        /// <summary>
        /// Verifies that CreateStaticFieldGetter&lt;TValue&gt; throws ArgumentNullException when fieldInfo is null.
        /// </summary>
        [Test]
        public void CreateStaticFieldGetterT_NullFieldInfo_ThrowsArgumentNullException()
        {
            // Arrange
            FieldInfo fieldInfo = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateStaticFieldGetter<int>(fieldInfo));
        }

        /// <summary>
        /// Verifies that CreateStaticFieldGetter&lt;TValue&gt; throws ArgumentException when field is not static.
        /// </summary>
        [Test]
        public void CreateStaticFieldGetterT_InstanceField_ThrowsArgumentException()
        {
            // Arrange
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.InstanceField),
                MemberAccessFlags.PublicInstance);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateStaticFieldGetter<int>(fieldInfo));
            Assert.That(ex.Message, Does.Contain("not static"));
        }

        #endregion

        #region Typed Instance Field Getter Tests

        /// <summary>
        /// Verifies that CreateInstanceFieldGetter&lt;TInstance, TValue&gt; creates a strongly-typed getter that retrieves the instance field value.
        /// </summary>
        [Test]
        public void CreateInstanceFieldGetterT_ValidField_ReturnsFieldValue()
        {
            // Arrange
            var testInstance = new TestClass { InstanceField = 42 };
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.InstanceField),
                MemberAccessFlags.PublicInstance);

            // Act
            var getter = ReflectionCompiler.CreateInstanceFieldGetter<TestClass, int>(fieldInfo);
            var result = getter(ref testInstance);

            // Assert
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Verifies that CreateInstanceFieldGetter&lt;TInstance, TValue&gt; handles type conversion correctly.
        /// </summary>
        [Test]
        public void CreateInstanceFieldGetterT_WithTypeConversion_ReturnsConvertedValue()
        {
            // Arrange
            var testInstance = new TestClass { InstanceField = 42 };
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.InstanceField),
                MemberAccessFlags.PublicInstance);

            // Act
            var getter = ReflectionCompiler.CreateInstanceFieldGetter<TestClass, object>(fieldInfo);
            var result = getter(ref testInstance);

            // Assert
            Assert.AreEqual(42, result);
            Assert.IsInstanceOf<object>(result);
        }

        /// <summary>
        /// Verifies that CreateInstanceFieldGetter&lt;TInstance, TValue&gt; throws ArgumentNullException when fieldInfo is null.
        /// </summary>
        [Test]
        public void CreateInstanceFieldGetterT_NullFieldInfo_ThrowsArgumentNullException()
        {
            // Arrange
            FieldInfo fieldInfo = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateInstanceFieldGetter<TestClass, int>(fieldInfo));
        }

        /// <summary>
        /// Verifies that CreateInstanceFieldGetter&lt;TInstance, TValue&gt; throws ArgumentException when field is static.
        /// </summary>
        [Test]
        public void CreateInstanceFieldGetterT_StaticField_ThrowsArgumentException()
        {
            // Arrange
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.StaticField),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateInstanceFieldGetter<TestClass, int>(fieldInfo));
            Assert.That(ex.Message, Does.Contain("not an instance field"));
        }

        #endregion

        #region Typed Static Field Setter Tests

        /// <summary>
        /// Verifies that CreateStaticFieldSetter&lt;TValue&gt; creates a strongly-typed setter that modifies the static field value.
        /// </summary>
        [Test]
        public void CreateStaticFieldSetterT_ValidField_SetsFieldValue()
        {
            // Arrange
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.StaticField),
                MemberAccessFlags.PublicStatic);
            var setter = ReflectionCompiler.CreateStaticFieldSetter<int>(fieldInfo);

            // Act
            setter(100);

            // Assert
            Assert.AreEqual(100, TestClass.StaticField);
        }

        /// <summary>
        /// Verifies that CreateStaticFieldSetter&lt;TValue&gt; handles type conversion correctly.
        /// </summary>
        [Test]
        public void CreateStaticFieldSetterT_WithTypeConversion_SetsFieldValue()
        {
            // Arrange
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.StaticField),
                MemberAccessFlags.PublicStatic);
            var setter = ReflectionCompiler.CreateStaticFieldSetter<object>(fieldInfo);

            // Act
            setter(200);

            // Assert
            Assert.AreEqual(200, TestClass.StaticField);
        }

        /// <summary>
        /// Verifies that CreateStaticFieldSetter&lt;TValue&gt; throws ArgumentNullException when fieldInfo is null.
        /// </summary>
        [Test]
        public void CreateStaticFieldSetterT_NullFieldInfo_ThrowsArgumentNullException()
        {
            // Arrange
            FieldInfo fieldInfo = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateStaticFieldSetter<int>(fieldInfo));
        }

        /// <summary>
        /// Verifies that CreateStaticFieldSetter&lt;TValue&gt; throws ArgumentException when field is not static.
        /// </summary>
        [Test]
        public void CreateStaticFieldSetterT_InstanceField_ThrowsArgumentException()
        {
            // Arrange
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.InstanceField),
                MemberAccessFlags.PublicInstance);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateStaticFieldSetter<int>(fieldInfo));
            Assert.That(ex.Message, Does.Contain("not static"));
        }

        /// <summary>
        /// Verifies that CreateStaticFieldSetter&lt;TValue&gt; throws ArgumentException when field is read-only.
        /// </summary>
        [Test]
        public void CreateStaticFieldSetterT_ReadOnlyField_ThrowsArgumentException()
        {
            // Arrange
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.StaticReadOnlyField),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateStaticFieldSetter<int>(fieldInfo));
            Assert.That(ex.Message, Does.Contain("read-only"));
        }

        #endregion

        #region Typed Instance Field Setter Tests

        /// <summary>
        /// Verifies that CreateInstanceFieldSetter&lt;TInstance, TValue&gt; creates a strongly-typed setter that modifies the instance field value.
        /// </summary>
        [Test]
        public void CreateInstanceFieldSetterT_ValidField_SetsFieldValue()
        {
            // Arrange
            var testInstance = new TestClass { InstanceField = 0 };
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.InstanceField),
                MemberAccessFlags.PublicInstance);
            var setter = ReflectionCompiler.CreateInstanceFieldSetter<TestClass, int>(fieldInfo);

            // Act
            setter(ref testInstance, 100);

            // Assert
            Assert.AreEqual(100, testInstance.InstanceField);
        }

        /// <summary>
        /// Verifies that CreateInstanceFieldSetter&lt;TInstance, TValue&gt; handles type conversion correctly.
        /// </summary>
        [Test]
        public void CreateInstanceFieldSetterT_WithTypeConversion_SetsFieldValue()
        {
            // Arrange
            var testInstance = new TestClass { InstanceField = 0 };
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.InstanceField),
                MemberAccessFlags.PublicInstance);
            var setter = ReflectionCompiler.CreateInstanceFieldSetter<TestClass, object>(fieldInfo);

            // Act
            setter(ref testInstance, 200);

            // Assert
            Assert.AreEqual(200, testInstance.InstanceField);
        }

        /// <summary>
        /// Verifies that CreateInstanceFieldSetter&lt;TInstance, TValue&gt; throws ArgumentNullException when fieldInfo is null.
        /// </summary>
        [Test]
        public void CreateInstanceFieldSetterT_NullFieldInfo_ThrowsArgumentNullException()
        {
            // Arrange
            FieldInfo fieldInfo = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionCompiler.CreateInstanceFieldSetter<TestClass, int>(fieldInfo));
        }

        /// <summary>
        /// Verifies that CreateInstanceFieldSetter&lt;TInstance, TValue&gt; throws ArgumentException when field is static.
        /// </summary>
        [Test]
        public void CreateInstanceFieldSetterT_StaticField_ThrowsArgumentException()
        {
            // Arrange
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.StaticField),
                MemberAccessFlags.PublicStatic);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateInstanceFieldSetter<TestClass, int>(fieldInfo));
            Assert.That(ex.Message, Does.Contain("not an instance field"));
        }

        /// <summary>
        /// Verifies that CreateInstanceFieldSetter&lt;TInstance, TValue&gt; throws ArgumentException when field is read-only.
        /// </summary>
        [Test]
        public void CreateInstanceFieldSetterT_ReadOnlyField_ThrowsArgumentException()
        {
            // Arrange
            var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.ReadOnlyField),
                MemberAccessFlags.PublicInstance);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => ReflectionCompiler.CreateInstanceFieldSetter<TestClass, int>(fieldInfo));
            Assert.That(ex.Message, Does.Contain("read-only"));
        }

        /// <summary>
        /// Verifies that CreateInstanceFieldSetter&lt;TInstance, TValue&gt; correctly modifies struct fields without losing changes.
        /// </summary>
        [Test]
        public void CreateInstanceFieldSetterT_StructInstance_ModifiesFieldWithoutLosingChanges()
        {
            // Arrange
            var testStruct = new TestStruct(10);
            var fieldInfo = typeof(TestStruct).GetField(nameof(TestStruct.Field),
                MemberAccessFlags.PublicInstance);
            var setter = ReflectionCompiler.CreateInstanceFieldSetter<TestStruct, int>(fieldInfo);

            // Act
            setter(ref testStruct, 100);

            // Assert
            Assert.AreEqual(100, testStruct.Field, "Struct field should be modified to 100");
        }

        /// <summary>
        /// Verifies that CreateInstanceFieldGetter&lt;TInstance, TValue&gt; and CreateInstanceFieldSetter&lt;TInstance, TValue&gt;
        /// work correctly together for struct instances, ensuring modifications are preserved.
        /// </summary>
        [Test]
        public void CreateInstanceFieldGetterSetterT_StructInstance_PreservesModifications()
        {
            // Arrange
            var testStruct = new TestStruct(5);
            var fieldInfo = typeof(TestStruct).GetField(nameof(TestStruct.Field),
                MemberAccessFlags.PublicInstance);
            var getter = ReflectionCompiler.CreateInstanceFieldGetter<TestStruct, int>(fieldInfo);
            var setter = ReflectionCompiler.CreateInstanceFieldSetter<TestStruct, int>(fieldInfo);

            // Act - Read initial value
            var initialValue = getter(ref testStruct);
            // Modify the field
            setter(ref testStruct, 50);
            // Read the modified value
            var modifiedValue = getter(ref testStruct);

            // Assert
            Assert.AreEqual(5, initialValue, "Initial value should be 5");
            Assert.AreEqual(50, modifiedValue, "Modified value should be 50");
            Assert.AreEqual(50, testStruct.Field, "Struct field should retain the modified value");
        }

        #endregion
    }
}
