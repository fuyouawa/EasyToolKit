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

        #region Type Inference Tests

        /// <summary>
        /// Verifies that type parameters can be inferred from constraints.
        /// When T2 is constrained to IList&lt;T1&gt;, and targets provide T1=int and T2=List&lt;int&gt;,
        /// the system should correctly construct the generic type.
        /// </summary>
        [Test]
        public void CanMatch_DualParameterWithListConstraint_ValidParameters_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericConstraintsMatchRule();
            var genericDefinition = typeof(DualParameterListHandler<,>);
            var genericParams = genericDefinition.GetGenericArguments();
            var candidate = new TypeMatchCandidate(genericDefinition, 0, genericParams);

            var targets = new[] { typeof(List<int>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result, "Should be able to match when T2 satisfies IList<T1> constraint");
        }

        /// <summary>
        /// Verifies that dual parameter handler with list constraint can be constructed.
        /// </summary>
        [Test]
        public void Match_DualParameterWithListConstraint_ReturnsConstructedType()
        {
            // Arrange
            var rule = new GenericConstraintsMatchRule();
            var genericDefinition = typeof(DualParameterListHandler<,>);
            var genericParams = genericDefinition.GetGenericArguments();
            var candidate = new TypeMatchCandidate(genericDefinition, 0, genericParams);

            // T1 = int (element type), T2 = List<int> (list type)
            var targets = new[] { typeof(int), typeof(List<int>) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(DualParameterListHandler<int, List<int>>), result);
        }

        /// <summary>
        /// Verifies that type parameters can be inferred with string element type.
        /// </summary>
        [Test]
        public void CanMatch_DualParameterWithListConstraint_StringType_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericConstraintsMatchRule();
            var genericDefinition = typeof(DualParameterListHandler<,>);
            var genericParams = genericDefinition.GetGenericArguments();
            var candidate = new TypeMatchCandidate(genericDefinition, 0, genericParams);

            // T1 = string (element type), T2 = List<string> (list type)
            var targets = new[] { typeof(string), typeof(List<string>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result, "Should be able to match when T2 satisfies IList<T1> constraint with string type");
        }

        /// <summary>
        /// Verifies that dual parameter handler can be constructed with string element type.
        /// </summary>
        [Test]
        public void Match_DualParameterWithListConstraint_StringType_ReturnsConstructedType()
        {
            // Arrange
            var rule = new GenericConstraintsMatchRule();
            var genericDefinition = typeof(DualParameterListHandler<,>);
            var genericParams = genericDefinition.GetGenericArguments();
            var candidate = new TypeMatchCandidate(genericDefinition, 0, genericParams);

            // T1 = string (element type), T2 = List<string> (list type)
            var targets = new[] { typeof(string), typeof(List<string>) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(DualParameterListHandler<string, List<string>>), result);
        }

        /// <summary>
        /// Verifies that mismatched type parameters are rejected.
        /// When T2 is constrained to IList&lt;T1&gt;, providing T2=List&lt;string&gt; when T1=int should fail.
        /// </summary>
        [Test]
        public void CanMatch_DualParameterWithListConstraint_MismatchedParameters_ReturnsFalse()
        {
            // Arrange
            var rule = new GenericConstraintsMatchRule();
            var genericDefinition = typeof(DualParameterListHandler<,>);
            var genericParams = genericDefinition.GetGenericArguments();
            var candidate = new TypeMatchCandidate(genericDefinition, 0, genericParams);

            // T1 = int, T2 = List<string> (mismatch: T2 should be IList<int>, not IList<string>)
            var targets = new[] { typeof(int), typeof(List<string>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsFalse(result, "Should fail to match when T2 does not satisfy IList<T1> constraint");
        }

        /// <summary>
        /// Verifies that IEnumerable constraints work with dual parameters.
        /// When T2 is constrained to IEnumerable&lt;T1&gt;, and targets provide T1=string and T2=List&lt;string&gt;,
        /// the system should correctly construct the generic type.
        /// </summary>
        [Test]
        public void CanMatch_DualParameterWithEnumerableConstraint_ValidParameters_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericConstraintsMatchRule();
            var genericDefinition = typeof(DualParameterEnumerableHandler<,>);
            var genericParams = genericDefinition.GetGenericArguments();
            var candidate = new TypeMatchCandidate(genericDefinition, 0, genericParams);

            // T1 = string (element type), T2 = List<string> (which is IEnumerable<string>)
            var targets = new[] { typeof(string), typeof(List<string>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result, "Should be able to match when T2 satisfies IEnumerable<T1> constraint");
        }

        /// <summary>
        /// Verifies that dual parameter handler with enumerable constraint can be constructed.
        /// </summary>
        [Test]
        public void Match_DualParameterWithEnumerableConstraint_ReturnsConstructedType()
        {
            // Arrange
            var rule = new GenericConstraintsMatchRule();
            var genericDefinition = typeof(DualParameterEnumerableHandler<,>);
            var genericParams = genericDefinition.GetGenericArguments();
            var candidate = new TypeMatchCandidate(genericDefinition, 0, genericParams);

            // T1 = string (element type), T2 = List<string> (which is IEnumerable<string>)
            var targets = new[] { typeof(string), typeof(List<string>) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(DualParameterEnumerableHandler<string, List<string>>), result);
        }

        /// <summary>
        /// Verifies that enumerable constraints work with arrays.
        /// Arrays implement IEnumerable&lt;T&gt;, so T2=int[] should satisfy IEnumerable&lt;int&gt; constraint.
        /// </summary>
        [Test]
        public void CanMatch_DualParameterWithEnumerableConstraint_ArrayType_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericConstraintsMatchRule();
            var genericDefinition = typeof(DualParameterEnumerableHandler<,>);
            var genericParams = genericDefinition.GetGenericArguments();
            var candidate = new TypeMatchCandidate(genericDefinition, 0, genericParams);

            // T1 = int (element type), T2 = int[] (which is IEnumerable<int>)
            var targets = new[] { typeof(int), typeof(int[]) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result, "Should be able to match when T2 is an array satisfying IEnumerable<T1> constraint");
        }

        /// <summary>
        /// Verifies that dual parameter handler with enumerable constraint can be constructed with arrays.
        /// </summary>
        [Test]
        public void Match_DualParameterWithEnumerableConstraint_ArrayType_ReturnsConstructedType()
        {
            // Arrange
            var rule = new GenericConstraintsMatchRule();
            var genericDefinition = typeof(DualParameterEnumerableHandler<,>);
            var genericParams = genericDefinition.GetGenericArguments();
            var candidate = new TypeMatchCandidate(genericDefinition, 0, genericParams);

            // T1 = int (element type), T2 = int[] (which is IEnumerable<int>)
            var targets = new[] { typeof(int), typeof(int[]) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(DualParameterEnumerableHandler<int, int[]>), result);
        }

        #endregion
    }
}
