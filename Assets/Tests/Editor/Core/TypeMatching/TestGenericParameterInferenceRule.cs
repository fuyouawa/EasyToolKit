using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Core.Reflection.Implementations;

namespace Tests.Core.TypeMatching
{
    /// <summary>
    /// Tests for GenericParameterInferenceRule type matching behavior.
    /// </summary>
    [TestFixture]
    public class TestGenericParameterInferenceRule
    {
        #region Basic Functionality Tests

        /// <summary>
        /// Verifies that all generic parameter constraints can be matched and inferred.
        /// </summary>
        [Test]
        public void CanMatch_AllGenericParameters_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericParameterInferenceRule();
            var genericParams = typeof(List<>).GetGenericArguments();

            var candidate = new TypeMatchCandidate(typeof(List<>), 0, genericParams);
            var targets = new[] { typeof(int) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that Match returns the correctly inferred generic type.
        /// </summary>
        [Test]
        public void Match_AllGenericParameters_ReturnsConstructedType()
        {
            // Arrange
            var rule = new GenericParameterInferenceRule();
            var genericParams = typeof(List<>).GetGenericArguments();
            var candidate = new TypeMatchCandidate(typeof(List<>), 0, genericParams);
            var targets = new[] { typeof(string) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(List<string>), result);
        }

        #endregion

        #region Multiple Generic Parameters Tests

        /// <summary>
        /// Verifies that multiple generic parameters are correctly inferred.
        /// </summary>
        [Test]
        public void Match_MultipleGenericParameters_ReturnsConstructedType()
        {
            // Arrange
            var rule = new GenericParameterInferenceRule();
            var genericParams = typeof(Dictionary<,>).GetGenericArguments();
            var candidate = new TypeMatchCandidate(typeof(Dictionary<,>), 0, genericParams);
            var targets = new[] { typeof(string), typeof(int) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(Dictionary<string, int>), result);
        }

        #endregion

        #region Edge Case Tests

        /// <summary>
        /// Verifies that non-generic source types cannot be matched.
        /// </summary>
        [Test]
        public void CanMatch_SourceTypeNotGeneric_ReturnsFalse()
        {
            // Arrange
            var rule = new GenericParameterInferenceRule();
            var candidate = new TypeMatchCandidate(typeof(string), 0, new[] { typeof(string) });
            var targets = new[] { typeof(string) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verifies that constraints without generic parameters cannot be matched.
        /// </summary>
        [Test]
        public void CanMatch_NoGenericParametersInConstraints_ReturnsFalse()
        {
            // Arrange
            var rule = new GenericParameterInferenceRule();
            var candidate = new TypeMatchCandidate(typeof(List<>), 0, new[] { typeof(int), typeof(string) });
            var targets = new[] { typeof(int), typeof(string) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verifies that constraint mismatch (concrete type constraint differs from target) returns false.
        /// </summary>
        [Test]
        public void CanMatch_ConcreteConstraintMismatch_ReturnsFalse()
        {
            // Arrange
            var rule = new GenericParameterInferenceRule();
            var genericParams = typeof(List<>).GetGenericArguments();
            // Create mixed constraints: one generic param, one concrete type
            var candidate = new TypeMatchCandidate(typeof(List<>), 0, new[] { genericParams[0], typeof(int) });
            var targets = new[] { typeof(string), typeof(float) }; // Second target doesn't match constraint

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region Complex Scenarios

        /// <summary>
        /// Verifies that type parameters with constraints can be inferred when targets are compatible.
        /// </summary>
        [Test]
        public void CanMatch_TypeParameterWithConstraint_CompatibleTarget_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericParameterInferenceRule();
            var genericDefinition = typeof(ListWithConstraint<>);
            var genericParams = genericDefinition.GetGenericArguments();
            var candidate = new TypeMatchCandidate(genericDefinition, 0, genericParams);
            var targets = new[] { typeof(List<int>) }; // List<int> implements IEnumerable

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that inference fails when targets cannot satisfy all constraints.
        /// </summary>
        [Test]
        public void CanMatch_ConstraintsCannotBeInfered_ReturnsFalse()
        {
            // Arrange
            var rule = new GenericParameterInferenceRule();
            // Create a scenario where inference would fail
            // Dictionary<,> has no special constraints that would enable inference from partial parameters
            var genericParams = typeof(Dictionary<,>).GetGenericArguments();
            var candidate = new TypeMatchCandidate(typeof(Dictionary<,>), 0, genericParams);
            var targets = new[] { typeof(string) }; // Only one parameter for two-parameter type

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region Array Source Type Tests

        /// <summary>
        /// Verifies that array source type T[] can be matched with concrete array type.
        /// </summary>
        [Test]
        public void CanMatch_SourceTypeIsGenericArray_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericParameterInferenceRule();
            Type genericT = typeof(List<>).GetGenericArguments()[0];
            Type genericArrayType = genericT.MakeArrayType();

            var candidate = new TypeMatchCandidate(genericArrayType, 0, new[] { genericT });
            var targets = new[] { typeof(int) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result, "Should be able to match T[] source type with concrete type T");
        }

        /// <summary>
        /// Verifies that Match correctly constructs concrete array type (e.g., int[]) from generic array source type (T[]).
        /// </summary>
        [Test]
        public void Match_SourceTypeIsGenericArray_ReturnsConcreteArrayType()
        {
            // Arrange
            var rule = new GenericParameterInferenceRule();
            Type genericT = typeof(List<>).GetGenericArguments()[0];
            Type genericArrayType = genericT.MakeArrayType();

            var candidate = new TypeMatchCandidate(genericArrayType, 0, new[] { genericT });
            var targets = new[] { typeof(string) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(string[]), result, "Should construct string[] from T[] with T=string");
        }

        /// <summary>
        /// Verifies that multi-dimensional array source types can be correctly constructed.
        /// </summary>
        [Test]
        public void Match_SourceTypeIsGenericTwoDimensionalArray_ReturnsConcreteArrayType()
        {
            // Arrange
            var rule = new GenericParameterInferenceRule();
            Type genericT = typeof(List<>).GetGenericArguments()[0];
            Type generic2DArrayType = genericT.MakeArrayType(2);

            var candidate = new TypeMatchCandidate(generic2DArrayType, 0, new[] { genericT });
            var targets = new[] { typeof(float) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(float[,]), result, "Should construct float[,] from T[,] with T=float");
        }

        /// <summary>
        /// Verifies that generic array with nested generic type can be correctly constructed.
        /// </summary>
        [Test]
        public void Match_SourceTypeIsGenericArrayWithNestedGeneric_ReturnsConcreteArrayType()
        {
            // Arrange
            var rule = new GenericParameterInferenceRule();
            Type genericT = typeof(List<>).GetGenericArguments()[0];
            Type genericArrayType = genericT.MakeArrayType();

            var candidate = new TypeMatchCandidate(genericArrayType, 0, new[] { genericT });
            var targets = new[] { typeof(List<int>) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(List<int>[]), result, "Should construct List<int>[] from T[] with T=List<int>");
        }

        #endregion

        #region Test Helper Types

        /// <summary>
        /// Test class with IEnumerable constraint on generic parameter.
        /// </summary>
        private class ListWithConstraint<T> where T : IEnumerable
        {
        }

        #endregion
    }
}
