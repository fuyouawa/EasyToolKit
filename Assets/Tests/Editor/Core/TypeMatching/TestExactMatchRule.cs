using System;
using System.Collections.Generic;
using NUnit.Framework;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Core.Reflection.Implementations;

namespace Tests.Core.TypeMatching
{
    /// <summary>
    /// Tests for ExactMatchRule type matching behavior.
    /// </summary>
    [TestFixture]
    public class TestExactMatchRule
    {
        /// <summary>
        /// Verifies that exact type matching returns true when types match.
        /// </summary>
        [Test]
        public void CanMatch_ExactTypeMatch_ReturnsTrue()
        {
            // Arrange
            var rule = new ExactMatchRule();
            var candidate = new TypeMatchCandidate(typeof(string), 0, new[] { typeof(string) });
            var targets = new[] { typeof(string) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that generic type definitions cannot be matched.
        /// </summary>
        [Test]
        public void CanMatch_GenericTypeDefinition_ReturnsFalse()
        {
            // Arrange
            var rule = new ExactMatchRule();
            var candidate = new TypeMatchCandidate(typeof(List<>), 0, new[] { typeof(int) });
            var targets = new[] { typeof(int) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verifies that target count mismatch returns false.
        /// </summary>
        [Test]
        public void CanMatch_TargetCountMismatch_ReturnsFalse()
        {
            // Arrange
            var rule = new ExactMatchRule();
            var candidate = new TypeMatchCandidate(typeof(string), 0, new[] { typeof(string), typeof(int) });
            var targets = new[] { typeof(string) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verifies that type mismatch returns false.
        /// </summary>
        [Test]
        public void CanMatch_TypeMismatch_ReturnsFalse()
        {
            // Arrange
            var rule = new ExactMatchRule();
            var candidate = new TypeMatchCandidate(typeof(string), 0, new[] { typeof(string) });
            var targets = new[] { typeof(int) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verifies that Match returns the source type for exact matches.
        /// </summary>
        [Test]
        public void Match_ReturnsSourceType()
        {
            // Arrange
            var rule = new ExactMatchRule();
            var candidate = new TypeMatchCandidate(typeof(string), 0, new[] { typeof(string) });
            var targets = new[] { typeof(string) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        /// <summary>
        /// Verifies that all constraints match when all types are compatible.
        /// </summary>
        [Test]
        public void CanMatch_MultipleConstraints_AllMatch_ReturnsTrue()
        {
            // Arrange
            var rule = new ExactMatchRule();
            var candidate = new TypeMatchCandidate(typeof(Action<int, string>), 0,
                new[] { typeof(int), typeof(string) });
            var targets = new[] { typeof(int), typeof(string) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that partial constraint mismatch returns false.
        /// </summary>
        [Test]
        public void CanMatch_MultipleConstraints_PartialMismatch_ReturnsFalse()
        {
            // Arrange
            var rule = new ExactMatchRule();
            var candidate = new TypeMatchCandidate(typeof(Action<int, string>), 0,
                new[] { typeof(int), typeof(string) });
            var targets = new[] { typeof(int), typeof(float) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
