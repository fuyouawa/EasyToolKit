using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Core.Reflection.Implementations;

namespace Tests.Core.TypeMatching.Rules
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

        #region Array Source Type Tests

        /// <summary>
        /// Verifies that array source type T[] can be matched with concrete array type.
        /// </summary>
        [Test]
        public void CanMatch_SourceTypeIsGenericArray_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericTypeResolutionRule();
            Type genericT = typeof(List<>).GetGenericArguments()[0];
            Type genericArrayType = genericT.MakeArrayType();

            var candidate = new TypeMatchCandidate(genericArrayType, 0, new[] { genericArrayType });
            var targets = new[] { typeof(int[]) };

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
            var rule = new GenericTypeResolutionRule();
            Type genericT = typeof(List<>).GetGenericArguments()[0];
            Type genericArrayType = genericT.MakeArrayType();

            var candidate = new TypeMatchCandidate(genericArrayType, 0, new[] { genericArrayType });
            var targets = new[] { typeof(string[]) };

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
            var rule = new GenericTypeResolutionRule();
            Type genericT = typeof(List<>).GetGenericArguments()[0];
            Type generic2DArrayType = genericT.MakeArrayType(2);

            var candidate = new TypeMatchCandidate(generic2DArrayType, 0, new[] { generic2DArrayType });
            var targets = new[] { typeof(float[,]) };

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
            var rule = new GenericTypeResolutionRule();
            Type genericT = typeof(List<>).GetGenericArguments()[0];
            Type genericArrayType = genericT.MakeArrayType();

            var candidate = new TypeMatchCandidate(genericArrayType, 0, new[] { genericArrayType });
            var targets = new[] { typeof(List<int>[]) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(List<int>[]), result, "Should construct List<int>[] from T[] with T=List<int>");
        }

        /// <summary>
        /// Verifies that partially open generic array (List<T>[]) can be matched with concrete array type.
        /// </summary>
        [Test]
        public void CanMatch_SourceTypeIsPartiallyOpenGenericArray_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericTypeResolutionRule();
            Type genericArrayType = typeof(List<>).MakeArrayType();

            var candidate = new TypeMatchCandidate(genericArrayType, 0, new[] { genericArrayType });
            var targets = new[] { typeof(List<string>[]) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result, "Should be able to match List<T>[] source type with concrete type T");
        }

        /// <summary>
        /// Verifies that Match correctly constructs concrete array type from partially open generic array (e.g., List<int>[] from List<T>[]).
        /// </summary>
        [Test]
        public void Match_SourceTypeIsPartiallyOpenGenericArray_ReturnsConcreteArrayType()
        {
            // Arrange
            var rule = new GenericTypeResolutionRule();
            Type genericArrayType = typeof(List<>).MakeArrayType();

            var candidate = new TypeMatchCandidate(genericArrayType, 0, new[] { genericArrayType });
            var targets = new[] { typeof(List<string>[]) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(List<string>[]), result, "Should construct List<string>[] from List<T>[] with T=string");
        }

        /// <summary>
        /// Verifies that three-dimensional array source types can be correctly constructed.
        /// </summary>
        [Test]
        public void Match_SourceTypeIsGenericThreeDimensionalArray_ReturnsConcreteArrayType()
        {
            // Arrange
            var rule = new GenericTypeResolutionRule();
            Type genericT = typeof(List<>).GetGenericArguments()[0];
            Type generic3DArrayType = genericT.MakeArrayType(3);

            var candidate = new TypeMatchCandidate(generic3DArrayType, 0, new[] { generic3DArrayType });
            var targets = new[] { typeof(double[,,]) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(double[,,]), result, "Should construct double[,,] from T[,,] with T=double");
        }

        #endregion

        [Test]
        public void Test()
        {
            var rule = new GenericConstraintsMatchRule();
            Type genericParam = typeof(ArrayHandler<>).GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(typeof(ArrayHandler<>), 0, new[] { genericParam });

            // Act
            var results = rule.CanMatch(candidate, new[] { typeof(TestClass) });

            // Assert
            Assert.IsFalse(results);
        }
        [Test]
        public void Test2()
        {
            var rule = new GenericConstraintsMatchRule();
            Type genericParam = typeof(ArrayHandler<>).GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(typeof(ArrayHandler<>), 0, new[] { genericParam });

            // Act
            var results = rule.CanMatch(candidate, new[] { typeof(List<int>) });

            // Assert
            Assert.IsFalse(results);
        }
    }
}
