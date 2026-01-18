using System;
using System.Collections.Generic;
using NUnit.Framework;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Core.Reflection.Implementations;

namespace Tests.Core.TypeMatching.Rules
{
    /// <summary>
    /// Tests for GenericConstraintsMatchRule type matching behavior.
    /// </summary>
    [TestFixture]
    public class TestGenericConstraintsMatchRule
    {
        /// <summary>
        /// Verifies that generic parameter constraints can be matched.
        /// </summary>
        [Test]
        public void CanMatch_GenericParameterConstraints_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericConstraintsMatchRule();
            var genericParams = typeof(List<>).GetGenericArguments();
            var candidate = new TypeMatchCandidate(typeof(List<>), 0, genericParams);
            var targets = new[] { typeof(int) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that non-generic source types cannot be matched.
        /// </summary>
        [Test]
        public void CanMatch_SourceTypeNotGeneric_ReturnsFalse()
        {
            // Arrange
            var rule = new GenericConstraintsMatchRule();
            var genericParams = typeof(int).GetGenericArguments(); // Empty array
            var candidate = new TypeMatchCandidate(typeof(int), 0, genericParams);
            var targets = Array.Empty<Type>();

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verifies that Match returns the constructed generic type.
        /// </summary>
        [Test]
        public void Match_ReturnsConstructedGenericType()
        {
            // Arrange
            var rule = new GenericConstraintsMatchRule();
            var genericParams = typeof(List<>).GetGenericArguments();
            var candidate = new TypeMatchCandidate(typeof(List<>), 0, genericParams);
            var targets = new[] { typeof(int) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(List<int>), result);
        }

        /// <summary>
        /// Verifies that class constraint works with class types.
        /// </summary>
        [Test]
        public void Match_ClassConstraint_ClassType_Succeeds()
        {
            // Arrange
            var rule = new GenericConstraintsMatchRule();
            var genericParams = typeof(List<>).GetGenericArguments();
            var candidate = new TypeMatchCandidate(typeof(List<>), 0, genericParams);
            var targets = new[] { typeof(string) };

            // Act & Assert
            Assert.IsTrue(rule.CanMatch(candidate, targets));
            Type result = rule.Match(candidate, targets);
            Assert.AreEqual(typeof(List<string>), result);
        }

        /// <summary>
        /// Verifies that struct constraint works with struct types.
        /// </summary>
        [Test]
        public void CanMatch_StructConstraint_StructType_Succeeds()
        {
            // Arrange
            var rule = new GenericConstraintsMatchRule();

            // Create a generic type with struct constraint
            var genericDefinition = typeof(StructConstraintContainer<>);
            var genericParams = genericDefinition.GetGenericArguments();
            var candidate = new TypeMatchCandidate(genericDefinition, 0, genericParams);
            var targets = new[] { typeof(int) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that multiple type parameters are handled correctly.
        /// </summary>
        [Test]
        public void Match_MultipleTypeParameters_ReturnsConstructedType()
        {
            // Arrange
            var rule = new GenericConstraintsMatchRule();
            var genericParams = typeof(Dictionary<,>).GetGenericArguments();
            var candidate = new TypeMatchCandidate(typeof(Dictionary<,>), 0, genericParams);
            var targets = new[] { typeof(string), typeof(int) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(Dictionary<string, int>), result);
        }

        /// <summary>
        /// Verifies that new() constraint works with types having parameterless constructors.
        /// </summary>
        [Test]
        public void CanMatch_NewConstraint_WithParameterlessConstructor_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericConstraintsMatchRule();
            var genericDefinition = typeof(NewConstraintContainer<>);
            var genericParams = genericDefinition.GetGenericArguments();
            var candidate = new TypeMatchCandidate(genericDefinition, 0, genericParams);
            var targets = new[] { typeof(List<int>) }; // List<int> has parameterless constructor

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
