using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Core.Reflection.Implementations;

namespace Tests.Core.TypeMatching
{
    /// <summary>
    /// Tests for GenericTypeResolutionRule type matching behavior.
    /// </summary>
    [TestFixture]
    public class TestGenericTypeResolutionRule
    {
        #region Basic Functionality Tests

        /// <summary>
        /// Verifies that a partially open generic type can resolve generic arguments from a concrete target.
        /// </summary>
        [Test]
        public void CanMatch_PartiallyOpenGeneric_ResolvesArguments_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericTypeResolutionRule();
            // Handler: Dictionary<string, T>
            var handlerType = typeof(Dictionary<,>).MakeGenericType(
                typeof(string),
                typeof(Dictionary<,>).GetGenericArguments()[1]);
            var candidate = new TypeMatchCandidate(handlerType, 0, new[] { handlerType });
            var targets = new[] { typeof(Dictionary<string, int>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that Match returns the correctly constructed generic type with resolved arguments.
        /// </summary>
        [Test]
        public void Match_PartiallyOpenGeneric_ReturnsConstructedType()
        {
            // Arrange
            var rule = new GenericTypeResolutionRule();
            // Handler: Dictionary<string, T>
            var handlerType = typeof(Dictionary<,>).MakeGenericType(
                typeof(string),
                typeof(Dictionary<,>).GetGenericArguments()[1]);
            var candidate = new TypeMatchCandidate(handlerType, 0, new[] { handlerType });
            var targets = new[] { typeof(Dictionary<string, float>) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(Dictionary<string, float>), result);
        }

        #endregion
    }
}
