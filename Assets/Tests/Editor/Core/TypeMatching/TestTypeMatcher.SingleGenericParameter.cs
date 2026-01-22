using System;
using System.Collections.Generic;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Core.Reflection.Implementations;
using NUnit.Framework;

namespace Tests.Core.TypeMatching
{
    /// <summary>
    /// Tests for TypeMatcher with single generic parameter scenarios.
    /// Tests focus on fixed-point matching with specific rules and candidates.
    /// </summary>
    [TestFixture]
    public class TestTypeMatcher_SingleGenericParameter
    {
        #region ExactMatchRule Tests

        /// <summary>
        /// Verifies that ExactMatchRule can match a concrete type candidate with exact target type.
        /// </summary>
        [Test]
        public void GetMatches_ExactMatchRule_ConcreteTypeCandidate_ReturnsMatch()
        {
            // Arrange
            var rule = new ExactMatchRule();
            var matcher = TypeMatcherFactory.Create(rule);
            matcher.SetTypeMatchCandidates(new[]
            {
                new TypeMatchCandidate(typeof(IntHandler), 0, new[] { typeof(int) })
            });

            // Act
            var results = matcher.GetMatches(typeof(int));

            // Assert
            Assert.AreEqual(1, results.Length, "Should return exactly one match");
            Assert.AreEqual(typeof(IntHandler), results[0].MatchedType, "Should match IntHandler type");
        }

        #endregion

        #region GenericConstraintsMatchRule Tests

        /// <summary>
        /// Verifies that GenericConstraintsMatchRule can infer generic parameter from target type.
        /// </summary>
        [Test]
        public void GetMatches_GenericConstraintsMatchRule_GenericCandidate_ReturnsConstructedType()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var matcher = TypeMatcherFactory.Create(rule);
            Type genericParam = typeof(GenericHandler<>).GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            matcher.SetTypeMatchCandidates(new[]
            {
                new TypeMatchCandidate(typeof(GenericHandler<>), 0, new[] { genericParam })
            });

            // Act
            var results = matcher.GetMatches(typeof(int));

            // Assert
            Assert.AreEqual(1, results.Length, "Should return exactly one match");
            Assert.AreEqual(typeof(GenericHandler<int>), results[0].MatchedType,
                "Should construct GenericHandler<int> from GenericHandler<T>");
        }

        /// <summary>
        /// Verifies that GenericConstraintsMatchRule can infer generic parameter with string target type.
        /// </summary>
        [Test]
        public void GetMatches_GenericConstraintsMatchRule_StringTarget_ReturnsConstructedType()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var matcher = TypeMatcherFactory.Create(rule);
            Type genericParam = typeof(GenericHandler<>).GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            matcher.SetTypeMatchCandidates(new[]
            {
                new TypeMatchCandidate(typeof(GenericHandler<>), 0, new[] { genericParam })
            });

            // Act
            var results = matcher.GetMatches(typeof(string));

            // Assert
            Assert.AreEqual(1, results.Length, "Should return exactly one match");
            Assert.AreEqual(typeof(GenericHandler<string>), results[0].MatchedType,
                "Should construct GenericHandler<string> from GenericHandler<T>");
        }

        /// <summary>
        /// Verifies that GenericConstraintsMatchRule does not match ArrayHandler&lt;T&gt; with List&lt;int&gt; target.
        /// ArrayHandler expects T[] constraint, but List&lt;int&gt; is not an array type.
        /// </summary>
        [Test]
        public void GetMatches_GenericConstraintsMatchRule_ArrayHandlerWithListTarget_ReturnsNoMatch()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var matcher = TypeMatcherFactory.Create(rule);
            Type genericParam = typeof(ArrayHandler<>).GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            matcher.SetTypeMatchCandidates(new[]
            {
                new TypeMatchCandidate(typeof(ArrayHandler<>), 0, new[] { genericParam })
            });

            // Act
            var results = matcher.GetMatches(typeof(List<int>));

            // Assert
            Assert.AreEqual(0, results.Length, "Should return no matches because List<int> is not an array type (T[])");
        }

        #endregion

        #region Multiple Candidates Tests

        /// <summary>
        /// Verifies that multiple candidates with different priorities return results ordered by priority.
        /// </summary>
        [Test]
        public void GetMatches_MultipleCandidatesWithPriorities_ReturnsOrderedByPriority()
        {
            // Arrange
            var matcher = TypeMatcherFactory.Create(new ExactMatchRule());
            matcher.SetTypeMatchCandidates(new[]
            {
                new TypeMatchCandidate(typeof(LowPriorityIntHandler), 0, new[] { typeof(int) }),
                new TypeMatchCandidate(typeof(HighPriorityIntHandler), 100, new[] { typeof(int) })
            });

            // Act
            var results = matcher.GetMatches(typeof(int));

            // Assert
            Assert.AreEqual(2, results.Length, "Should return two matches");
            Assert.AreEqual(typeof(HighPriorityIntHandler), results[0].MatchedType,
                "High priority handler should be first");
            Assert.AreEqual(typeof(LowPriorityIntHandler), results[1].MatchedType,
                "Low priority handler should be second");
        }

        #endregion

        #region No Match Tests

        /// <summary>
        /// Verifies that no matches are returned when target type doesn't match any candidate constraints.
        /// </summary>
        [Test]
        public void GetMatches_NoMatchingCandidate_ReturnsEmpty()
        {
            // Arrange
            var matcher = TypeMatcherFactory.Create(new ExactMatchRule());
            matcher.SetTypeMatchCandidates(new[]
            {
                new TypeMatchCandidate(typeof(IntHandler), 0, new[] { typeof(int) })
            });

            // Act
            var results = matcher.GetMatches(typeof(string));

            // Assert
            Assert.AreEqual(0, results.Length, "Should return no matches");
        }

        #endregion
    }
}
