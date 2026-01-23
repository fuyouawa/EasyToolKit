using System;
using NUnit.Framework;
using EasyToolKit.Core.Reflection;

namespace Tests.Core.Reflection
{
    /// <summary>
    /// Unit tests for ExpressionEvaluator components.
    /// </summary>
    public class TestExpressionEvaluator
    {
        #region Dynamic Expression - Instance Property Tests

        /// <summary>
        /// Verifies that Evaluate retrieves instance property value.
        /// </summary>
        [Test]
        public void Evaluate_InstanceProperty_ReturnsPropertyValue()
        {
            // Arrange
            var testObj = new TestClassForExpressionEvaluator { Name = "TestName" };
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "Name", typeof(TestClassForExpressionEvaluator));

            // Act
            var result = evaluator.Evaluate<string>(testObj);

            // Assert
            Assert.AreEqual("TestName", result);
        }

        /// <summary>
        /// Verifies that Evaluate retrieves nested property value.
        /// </summary>
        [Test]
        public void Evaluate_NestedProperty_ReturnsNestedValue()
        {
            // Arrange
            var testObj = new TestClassForExpressionEvaluator
            {
                Nested = new NestedClassForExpressionEvaluator
                {
                    Value = 42
                }
            };
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "Nested.Value", typeof(TestClassForExpressionEvaluator));

            // Act
            var result = evaluator.Evaluate<int>(testObj);

            // Assert
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Verifies that Evaluate retrieves int property value.
        /// </summary>
        [Test]
        public void Evaluate_IntProperty_ReturnsIntValue()
        {
            // Arrange
            var testObj = new TestClassForExpressionEvaluator { Level = 10 };
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "Level", typeof(TestClassForExpressionEvaluator));

            // Act
            var result = evaluator.Evaluate<int>(testObj);

            // Assert
            Assert.AreEqual(10, result);
        }

        /// <summary>
        /// Verifies that Evaluate retrieves bool property value.
        /// </summary>
        [Test]
        public void Evaluate_BoolProperty_ReturnsBoolValue()
        {
            // Arrange
            var testObj = new TestClassForExpressionEvaluator { IsActive = true };
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "IsActive", typeof(TestClassForExpressionEvaluator));

            // Act
            var result = evaluator.Evaluate<bool>(testObj);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region Dynamic Expression - Instance Field Tests

        /// <summary>
        /// Verifies that Evaluate retrieves instance field value.
        /// </summary>
        [Test]
        public void Evaluate_InstanceField_ReturnsFieldValue()
        {
            // Arrange
            var testObj = new TestClassForExpressionEvaluator();
            testObj.PublicField = "FieldValue";
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "PublicField", typeof(TestClassForExpressionEvaluator));

            // Act
            var result = evaluator.Evaluate<string>(testObj);

            // Assert
            Assert.AreEqual("FieldValue", result);
        }

        /// <summary>
        /// Verifies that Evaluate retrieves nested field value.
        /// </summary>
        [Test]
        public void Evaluate_NestedField_ReturnsNestedFieldValue()
        {
            // Arrange
            var testObj = new TestClassForExpressionEvaluator
            {
                Nested = new NestedClassForExpressionEvaluator()
            };
            testObj.Nested.PublicField = 99;
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "Nested.PublicField", typeof(TestClassForExpressionEvaluator));

            // Act
            var result = evaluator.Evaluate<int>(testObj);

            // Assert
            Assert.AreEqual(99, result);
        }

        #endregion

        #region Dynamic Expression - Instance Method Tests

        /// <summary>
        /// Verifies that Evaluate calls instance method and returns result.
        /// </summary>
        [Test]
        public void Evaluate_InstanceMethod_ReturnsMethodResult()
        {
            // Arrange
            var testObj = new TestClassForExpressionEvaluator();
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "GetScore()", typeof(TestClassForExpressionEvaluator));

            // Act
            var result = evaluator.Evaluate<int>(testObj);

            // Assert
            Assert.AreEqual(100, result);
        }

        /// <summary>
        /// Verifies that Evaluate calls method with parameters.
        /// </summary>
        [Test]
        public void Evaluate_MethodWithParameters_ReturnsMethodResult()
        {
            // Arrange
            var testObj = new TestClassForExpressionEvaluator();
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "Calculate(5, 3)", typeof(TestClassForExpressionEvaluator));

            // Act
            var result = evaluator.Evaluate<int>(testObj);

            // Assert
            Assert.AreEqual(15, result);
        }

        /// <summary>
        /// Verifies that Evaluate calls nested method.
        /// </summary>
        [Test]
        public void Evaluate_NestedMethod_ReturnsMethodResult()
        {
            // Arrange
            var testObj = new TestClassForExpressionEvaluator
            {
                Nested = new NestedClassForExpressionEvaluator()
            };
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "Nested.GetNestedValue()", typeof(TestClassForExpressionEvaluator));

            // Act
            var result = evaluator.Evaluate<double>(testObj);

            // Assert
            Assert.AreEqual(3.14, result);
        }

        #endregion

        #region Dynamic Expression - Static Member Tests

        /// <summary>
        /// Verifies that Evaluate retrieves static property value using -t: and -p: syntax.
        /// </summary>
        [Test]
        public void Evaluate_StaticPropertyWithTypeAndPath_ReturnsStaticValue()
        {
            // Arrange
            StaticClassForExpressionEvaluator.StaticValue = 42;
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "-t:StaticClassForExpressionEvaluator -p:StaticValue", null);

            // Act
            var result = evaluator.Evaluate<int>(null);

            // Assert
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Verifies that Evaluate retrieves static field value using -t: and -p: syntax.
        /// </summary>
        [Test]
        public void Evaluate_StaticFieldWithTypeAndPath_ReturnsStaticValue()
        {
            // Arrange
            StaticClassForExpressionEvaluator.StaticField = "TestField";
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "-t:StaticClassForExpressionEvaluator -p:StaticField", null);

            // Act
            var result = evaluator.Evaluate<string>(null);

            // Assert
            Assert.AreEqual("TestField", result);
        }

        /// <summary>
        /// Verifies that Evaluate calls static method using -t: and -p: syntax.
        /// </summary>
        [Test]
        public void Evaluate_StaticMethodWithTypeAndPath_ReturnsMethodResult()
        {
            // Arrange
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "-t:StaticClassForExpressionEvaluator -p:GetStaticScore()", null);

            // Act
            var result = evaluator.Evaluate<int>(null);

            // Assert
            Assert.AreEqual(200, result);
        }

        /// <summary>
        /// Verifies that Evaluate calls static method with parameters using -t: and -p: syntax.
        /// </summary>
        [Test]
        public void Evaluate_StaticMethodWithParameters_ReturnsMethodResult()
        {
            // Arrange
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "-t:StaticClassForExpressionEvaluator -p:Multiply(4, 5)", null);

            // Act
            var result = evaluator.Evaluate<int>(null);

            // Assert
            Assert.AreEqual(20, result);
        }

        /// <summary>
        /// Verifies that Evaluate throws error when -t: is specified without -p:.
        /// </summary>
        [Test]
        public void Evaluate_TypeWithoutPath_HasValidationError()
        {
            // Arrange
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "-t:StaticClassForExpressionEvaluator", null);

            // Act
            var hasError = evaluator.TryGetError(out var errorMessage);

            // Assert
            Assert.IsTrue(hasError);
            Assert.That(errorMessage, Does.Contain("-p:"));
        }

        /// <summary>
        /// Verifies that Evaluate throws error when source type is null and -t: is not specified.
        /// </summary>
        [Test]
        public void Evaluate_NullSourceTypeWithoutTypeArgument_HasValidationError()
        {
            // Arrange
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "SomeProperty", null);

            // Act
            var hasError = evaluator.TryGetError(out var errorMessage);

            // Assert
            Assert.IsTrue(hasError);
            Assert.That(errorMessage, Does.Contain("Source type"));
        }

        #endregion

        #region Expression Flag Tests

        /// <summary>
        /// Verifies that WithExpressionFlag evaluates expression when prefixed with @.
        /// </summary>
        [Test]
        public void WithExpressionFlag_PrefixedExpression_EvaluatesExpression()
        {
            // Arrange
            var testObj = new TestClassForExpressionEvaluator { Name = "DynamicName" };
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "@Name", typeof(TestClassForExpressionEvaluator), requireExpressionFlag: true);

            // Act
            var result = evaluator.Evaluate<string>(testObj);

            // Assert
            Assert.AreEqual("DynamicName", result);
        }

        /// <summary>
        /// Verifies that WithExpressionFlag returns literal value when not prefixed with @.
        /// </summary>
        [Test]
        public void WithExpressionFlag_NoPrefix_ReturnsLiteralValue()
        {
            // Arrange
            var testObj = new TestClassForExpressionEvaluator { Name = "DynamicName" };
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "LiteralText", typeof(TestClassForExpressionEvaluator), requireExpressionFlag: true);

            // Act
            var result = evaluator.Evaluate<string>(testObj);

            // Assert
            Assert.AreEqual("LiteralText", result);
        }

        /// <summary>
        /// Verifies that WithExpressionFlag handles nested expressions with @ prefix.
        /// </summary>
        [Test]
        public void WithExpressionFlag_NestedExpressionWithPrefix_EvaluatesNestedExpression()
        {
            // Arrange
            var testObj = new TestClassForExpressionEvaluator
            {
                Nested = new NestedClassForExpressionEvaluator
                {
                    Value = 42
                }
            };
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "@Nested.Value", typeof(TestClassForExpressionEvaluator), requireExpressionFlag: true);

            // Act
            var result = evaluator.Evaluate<int>(testObj);

            // Assert
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Verifies that WithExpressionFlag handles method calls with @ prefix.
        /// </summary>
        [Test]
        public void WithExpressionFlag_MethodWithPrefix_EvaluatesMethod()
        {
            // Arrange
            var testObj = new TestClassForExpressionEvaluator();
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "@GetScore()", typeof(TestClassForExpressionEvaluator), requireExpressionFlag: true);

            // Act
            var result = evaluator.Evaluate<int>(testObj);

            // Assert
            Assert.AreEqual(100, result);
        }

        #endregion

        #region Error Handling Tests

        /// <summary>
        /// Verifies that Evaluate throws InvalidOperationException when expression is invalid.
        /// </summary>
        [Test]
        public void Evaluate_InvalidExpression_ThrowsInvalidOperationException()
        {
            // Arrange
            var testObj = new TestClassForExpressionEvaluator();
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "NonExistentProperty", typeof(TestClassForExpressionEvaluator));

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                evaluator.Evaluate<string>(testObj));
        }

        /// <summary>
        /// Verifies that Evaluate provides error message through TryGetError for invalid expressions.
        /// </summary>
        [Test]
        public void Evaluate_InvalidExpression_TryGetErrorReturnsTrue()
        {
            // Arrange
            var testObj = new TestClassForExpressionEvaluator();
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "NonExistentProperty", typeof(TestClassForExpressionEvaluator));

            // Act
            var hasError = evaluator.TryGetError(out var errorMessage);

            // Assert
            Assert.IsTrue(hasError);
            Assert.IsNotNull(errorMessage);
        }

        /// <summary>
        /// Verifies that Evaluate handles empty or whitespace expressions.
        /// </summary>
        [Test]
        public void Evaluate_EmptyExpression_ReturnsLiteral()
        {
            // Arrange
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "", typeof(TestClassForExpressionEvaluator));

            // Act
            var result = evaluator.Evaluate<string>(null);

            // Assert
            Assert.AreEqual("", result);
        }

        /// <summary>
        /// Verifies that Evaluate throws error for invalid type in -t: argument.
        /// </summary>
        [Test]
        public void Evaluate_InvalidTypeName_HasValidationError()
        {
            // Arrange
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "-t:NonExistentType -p:SomeProperty", null);

            // Act
            var hasError = evaluator.TryGetError(out var errorMessage);

            // Assert
            Assert.IsTrue(hasError);
            Assert.That(errorMessage, Does.Contain("Failed to bind type"));
        }

        #endregion

        #region CreateEvaluator Method Tests

        /// <summary>
        /// Verifies that CreateEvaluator returns valid evaluator with default parameters.
        /// </summary>
        [Test]
        public void CreateEvaluator_DefaultParameters_ReturnsValidEvaluator()
        {
            // Arrange & Act
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "Name", typeof(TestClassForExpressionEvaluator));

            // Assert
            Assert.IsNotNull(evaluator);
        }

        /// <summary>
        /// Verifies that CreateEvaluator with requireExpressionFlag works correctly.
        /// </summary>
        [Test]
        public void CreateEvaluator_WithExpressionFlag_ReturnsValidEvaluator()
        {
            // Arrange & Act
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "@Name", typeof(TestClassForExpressionEvaluator), requireExpressionFlag: true);

            // Assert
            Assert.IsNotNull(evaluator);
        }

        /// <summary>
        /// Verifies that CreateEvaluator throws ArgumentException when expression is null and flag is required.
        /// </summary>
        [Test]
        public void CreateEvaluator_NullExpressionWithFlagRequired_ThrowsArgumentException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentException>(() =>
                ExpressionEvaluatorFactory.CreateEvaluator(
                    null, typeof(TestClassForExpressionEvaluator), requireExpressionFlag: true));
        }

        /// <summary>
        /// Verifies that CreateEvaluator throws ArgumentException when expression is whitespace and flag is required.
        /// </summary>
        [Test]
        public void CreateEvaluator_WhitespaceExpressionWithFlagRequired_ThrowsArgumentException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentException>(() =>
                ExpressionEvaluatorFactory.CreateEvaluator(
                    "   ", typeof(TestClassForExpressionEvaluator), requireExpressionFlag: true));
        }

        /// <summary>
        /// Verifies that CreateEvaluator throws ArgumentException with parameter name when expression is invalid.
        /// </summary>
        [Test]
        public void CreateEvaluator_NullExpressionWithFlagRequired_ExceptionContainsParameterName()
        {
            // Arrange & Act
            var exception = Assert.Throws<ArgumentException>(() =>
                ExpressionEvaluatorFactory.CreateEvaluator(
                    null, typeof(TestClassForExpressionEvaluator), requireExpressionFlag: true));

            // Assert
            Assert.AreEqual("expressionPath", exception.ParamName);
        }

        /// <summary>
        /// Verifies that CreateEvaluator handles requireExpressionFlag parameter correctly.
        /// </summary>
        [Test]
        public void CreateEvaluator_ExpressionFlagParameter_WorksAsExpected()
        {
            // Arrange
            var testObj = new TestClassForExpressionEvaluator { Name = "TestName" };

            // Act - With expression flag false (default), should evaluate
            var evaluator1 = ExpressionEvaluatorFactory.CreateEvaluator(
                "Name", typeof(TestClassForExpressionEvaluator), requireExpressionFlag: false);
            var result1 = evaluator1.Evaluate<string>(testObj);

            // Act - With expression flag true and no @ prefix, should return literal
            var evaluator2 = ExpressionEvaluatorFactory.CreateEvaluator(
                "Name", typeof(TestClassForExpressionEvaluator), requireExpressionFlag: true);
            var result2 = evaluator2.Evaluate<string>(testObj);

            // Assert
            Assert.AreEqual("TestName", result1);
            Assert.AreEqual("Name", result2);
        }

        #endregion

        #region Type-Safe Extension Tests

        /// <summary>
        /// Verifies that generic Evaluate extension method provides type-safe result.
        /// </summary>
        [Test]
        public void EvaluateGeneric_ValidExpression_ReturnsTypedResult()
        {
            // Arrange
            var testObj = new TestClassForExpressionEvaluator { Level = 10 };
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "Level", typeof(TestClassForExpressionEvaluator));

            // Act
            var result = evaluator.Evaluate<int>(testObj);

            // Assert
            Assert.AreEqual(10, result);
            Assert.IsInstanceOf<int>(result);
        }

        /// <summary>
        /// Verifies that generic Evaluate handles null reference types correctly.
        /// </summary>
        [Test]
        public void EvaluateGeneric_NullReferenceType_ReturnsNull()
        {
            // Arrange
            var testObj = new TestClassForExpressionEvaluator { Name = null };
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "Name", typeof(TestClassForExpressionEvaluator));

            // Act
            var result = evaluator.Evaluate<string>(testObj);

            // Assert
            Assert.IsNull(result);
        }

        /// <summary>
        /// Verifies that generic Evaluate throws InvalidCastException for type mismatch.
        /// </summary>
        [Test]
        public void EvaluateGeneric_TypeMismatch_ThrowsInvalidCastException()
        {
            // Arrange
            var testObj = new TestClassForExpressionEvaluator { Name = "TextValue" };
            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                "Name", typeof(TestClassForExpressionEvaluator));

            // Act & Assert
            Assert.Throws<InvalidCastException>(() =>
                evaluator.Evaluate<int>(testObj));
        }

        #endregion
    }

    #region Test Helper Classes

    /// <summary>
    /// Test class for expression evaluator tests.
    /// </summary>
    public class TestClassForExpressionEvaluator
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }
        public string PublicField;

        public NestedClassForExpressionEvaluator Nested { get; set; }

        public int GetScore()
        {
            return 100;
        }

        public int Calculate(int a, int b)
        {
            return a * b;
        }
    }

    /// <summary>
    /// Nested test class for expression evaluator tests.
    /// </summary>
    public class NestedClassForExpressionEvaluator
    {
        public int Value { get; set; }
        public int PublicField;

        public double GetNestedValue()
        {
            return 3.14;
        }
    }

    /// <summary>
    /// Static test class for expression evaluator tests.
    /// </summary>
    public static class StaticClassForExpressionEvaluator
    {
        public static int StaticValue { get; set; }
        public static string StaticField;

        public static int GetStaticScore()
        {
            return 200;
        }

        public static int Multiply(int a, int b)
        {
            return a * b;
        }
    }

    #endregion
}
