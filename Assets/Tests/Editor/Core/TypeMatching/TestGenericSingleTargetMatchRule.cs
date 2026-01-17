using System;
using System.Collections.Generic;
using NUnit.Framework;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Core.Reflection.Implementations;

namespace Tests.Core.TypeMatching
{
    /// <summary>
    /// Tests for GenericSingleTargetMatchRule type matching behavior.
    /// </summary>
    [TestFixture]
    public class TestGenericSingleTargetMatchRule
    {
        /// <summary>
        /// Verifies that valid generic type definition with single target returns true.
        /// </summary>
        [Test]
        public void CanMatch_ValidGenericSingleTarget_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericSingleTargetMatchRule();
            var candidate = new TypeMatchCandidate(typeof(List<>), 0, new[] { typeof(List<string>) });
            var targets = new[] { typeof(List<string>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that Match returns correctly constructed generic type.
        /// </summary>
        [Test]
        public void Match_ReturnsConstructedGenericType()
        {
            // Arrange
            var rule = new GenericSingleTargetMatchRule();
            var candidate = new TypeMatchCandidate(typeof(List<>), 0, new[] { typeof(List<int>) });
            var targets = new[] { typeof(List<int>) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(List<int>), result);
        }

        /// <summary>
        /// Verifies that multiple generic parameters are handled correctly.
        /// </summary>
        [Test]
        public void CanMatch_MultipleGenericParameters_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericSingleTargetMatchRule();
            var candidate = new TypeMatchCandidate(typeof(Dictionary<,>), 0, new[] { typeof(Dictionary<string, int>) });
            var targets = new[] { typeof(Dictionary<string, int>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that Match works with multiple generic parameters.
        /// </summary>
        [Test]
        public void Match_MultipleGenericParameters_ReturnsConstructedType()
        {
            // Arrange
            var rule = new GenericSingleTargetMatchRule();
            var candidate = new TypeMatchCandidate(typeof(Dictionary<,>), 0, new[] { typeof(Dictionary<string, float>) });
            var targets = new[] { typeof(Dictionary<string, float>) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(Dictionary<string, float>), result);
        }

        /// <summary>
        /// Verifies that non-generic source type returns false.
        /// </summary>
        [Test]
        public void CanMatch_NonGenericTypeDefinition_ReturnsFalse()
        {
            // Arrange
            var rule = new GenericSingleTargetMatchRule();
            var candidate = new TypeMatchCandidate(typeof(string), 0, new[] { typeof(List<int>) });
            var targets = new[] { typeof(List<int>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verifies that multiple targets return false.
        /// </summary>
        [Test]
        public void CanMatch_MultipleTargets_ReturnsFalse()
        {
            // Arrange
            var rule = new GenericSingleTargetMatchRule();
            var candidate = new TypeMatchCandidate(typeof(List<>), 0, new[] { typeof(List<int>) });
            var targets = new[] { typeof(List<int>), typeof(List<string>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verifies that non-generic constraint returns false.
        /// </summary>
        [Test]
        public void CanMatch_ConstraintNotGeneric_ReturnsFalse()
        {
            // Arrange
            var rule = new GenericSingleTargetMatchRule();
            var candidate = new TypeMatchCandidate(typeof(List<>), 0, new[] { typeof(string) });
            var targets = new[] { typeof(List<int>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verifies that non-generic target returns false.
        /// </summary>
        [Test]
        public void CanMatch_TargetNotGeneric_ReturnsFalse()
        {
            // Arrange
            var rule = new GenericSingleTargetMatchRule();
            var candidate = new TypeMatchCandidate(typeof(List<>), 0, new[] { typeof(List<int>) });
            var targets = new[] { typeof(string) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verifies that generic definition mismatch returns false.
        /// </summary>
        [Test]
        public void CanMatch_GenericDefinitionMismatch_ReturnsFalse()
        {
            // Arrange
            var rule = new GenericSingleTargetMatchRule();
            var candidate = new TypeMatchCandidate(typeof(List<>), 0, new[] { typeof(Dictionary<int, string>) });
            var targets = new[] { typeof(Dictionary<int, string>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verifies that generic parameter count mismatch returns false.
        /// </summary>
        [Test]
        public void CanMatch_GenericParameterCountMismatch_ReturnsFalse()
        {
            // Arrange
            var rule = new GenericSingleTargetMatchRule();
            var candidate = new TypeMatchCandidate(typeof(List<>), 0, new[] { typeof(Dictionary<int, string>) });
            var targets = new[] { typeof(Dictionary<int, string>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verifies that custom generic types can be matched.
        /// </summary>
        [Test]
        public void CanMatch_CustomGenericTypes_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericSingleTargetMatchRule();
            var candidate = new TypeMatchCandidate(typeof(GenericContainer<>), 0, new[] { typeof(GenericContainer<int>) });
            var targets = new[] { typeof(GenericContainer<int>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that Match works with custom generic types.
        /// </summary>
        [Test]
        public void Match_CustomGenericTypes_ReturnsConstructedType()
        {
            // Arrange
            var rule = new GenericSingleTargetMatchRule();
            var candidate = new TypeMatchCandidate(typeof(GenericContainer<>), 0, new[] { typeof(GenericContainer<string>) });
            var targets = new[] { typeof(GenericContainer<string>) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(GenericContainer<string>), result);
        }

        /// <summary>
        /// Verifies that nested generic types are handled correctly.
        /// </summary>
        [Test]
        public void CanMatch_NestedGenericTypes_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericSingleTargetMatchRule();
            var candidate = new TypeMatchCandidate(typeof(List<>), 0, new[] { typeof(List<List<int>>) });
            var targets = new[] { typeof(List<List<int>>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that Match works with nested generic types.
        /// </summary>
        [Test]
        public void Match_NestedGenericTypes_ReturnsConstructedType()
        {
            // Arrange
            var rule = new GenericSingleTargetMatchRule();
            var candidate = new TypeMatchCandidate(typeof(List<>), 0, new[] { typeof(List<List<string>>) });
            var targets = new[] { typeof(List<List<string>>) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(List<List<string>>), result);
        }

        #region Test Helpers

        /// <summary>
        /// Test helper class for generic type matching.
        /// </summary>
        private class GenericContainer<T> { }

        #endregion
    }
}
