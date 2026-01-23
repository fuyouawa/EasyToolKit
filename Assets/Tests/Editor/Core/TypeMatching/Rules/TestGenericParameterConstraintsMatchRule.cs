using System;
using System.Collections.Generic;
using NUnit.Framework;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Core.Reflection.Implementations;

namespace Tests.Core.TypeMatching.Rules
{
    /// <summary>
    /// Tests for GenericParameterConstraints type matching behavior.
    /// </summary>
    [TestFixture]
    public class TestGenericParameterConstraintsMatchRule
    {
        /// <summary>
        /// Verifies that generic parameter constraints can be matched.
        /// </summary>
        [Test]
        public void CanMatch_GenericParameterConstraints_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
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
            var rule = new GenericParameterConstraintsMatchRule();
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
            var rule = new GenericParameterConstraintsMatchRule();
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
            var rule = new GenericParameterConstraintsMatchRule();
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
            var rule = new GenericParameterConstraintsMatchRule();

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
            var rule = new GenericParameterConstraintsMatchRule();
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
            var rule = new GenericParameterConstraintsMatchRule();
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
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(DualParameterListHandler<,>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(List<int>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result, "Should be able to match when T2 satisfies IList<T1> constraint");
        }

        /// <summary>
        /// Verifies that IListHandler can match with List&lt;T&gt; constraint.
        /// When the handler's target is IList&lt;T&gt; and we provide List&lt;int&gt;,
        /// the system should correctly infer T=int.
        /// </summary>
        [Test]
        public void CanMatch_IListHandlerWithListConstraint_ValidParameters_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(IListHandler<>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(List<int>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result, "Should be able to match IListHandler<T> with List<int> target");
        }

        /// <summary>
        /// Verifies that IListHandler can match with array constraint.
        /// When the handler's target is IList&lt;T&gt; and we provide T[],
        /// the system should correctly infer the element type.
        /// </summary>
        [Test]
        public void CanMatch_IListHandlerWithArrayConstraint_ValidParameters_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(IListHandler<>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(int[]) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result, "Should be able to match IListHandler<T> with int[] target");
        }

        /// <summary>
        /// Verifies that ListHandler can match with List&lt;T&gt; constraint.
        /// When the handler's target is List&lt;T&gt; and we provide List&lt;string&gt;,
        /// the system should correctly infer T=string.
        /// </summary>
        [Test]
        public void CanMatch_ListHandlerWithListConstraint_ValidParameters_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(ListHandler<>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(List<string>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result, "Should be able to match ListHandler<T> with List<string> target");
        }

        /// <summary>
        /// Verifies that DualParameterEnumerableHandler can match with IEnumerable constraint.
        /// When T1 is constrained to IEnumerable&lt;T2&gt;, and targets provide List&lt;int&gt;,
        /// the system should correctly construct the generic type.
        /// </summary>
        [Test]
        public void CanMatch_DualParameterWithEnumerableConstraint_ValidParameters_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(DualParameterEnumerableHandler<,>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(List<int>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result, "Should be able to match when T1 satisfies IEnumerable<T2> constraint");
        }

        /// <summary>
        /// Verifies that DualParameterEnumerableHandler can match with array constraint.
        /// When T1 is constrained to IEnumerable&lt;T2&gt;, and targets provide string[],
        /// the system should correctly construct the generic type.
        /// </summary>
        [Test]
        public void CanMatch_DualParameterWithEnumerableConstraintAndArray_ValidParameters_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(DualParameterEnumerableHandler<,>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(string[]) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result, "Should be able to match when T1 is array and satisfies IEnumerable<T2> constraint");
        }

        /// <summary>
        /// Verifies that ArrayHandler can match with array constraint.
        /// When the handler's target is T[] and we provide int[],
        /// the system should correctly infer T=int.
        /// </summary>
        [Test]
        public void CanMatch_ArrayHandlerWithArrayConstraint_ValidParameters_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(ArrayHandler<>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(int[]) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result, "Should be able to match ArrayHandler<T> with int[] target");
        }

        /// <summary>
        /// Verifies that IListHandler Match returns correctly constructed generic type.
        /// </summary>
        [Test]
        public void Match_IListHandlerWithListConstraint_ReturnsCorrectGenericType()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(IListHandler<>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(List<double>) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(IListHandler<double>), result, "Should construct IListHandler<double>");
        }

        /// <summary>
        /// Verifies that DualParameterListHandler Match returns correctly constructed generic type.
        /// </summary>
        [Test]
        public void Match_DualParameterWithListConstraint_ReturnsCorrectGenericType()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(DualParameterListHandler<,>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(List<float>) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(DualParameterListHandler<List<float>, float>), result,
                "Should construct DualParameterListHandler<List<float>, float>");
        }

        /// <summary>
        /// Verifies that DualParameterEnumerableHandler Match returns correctly constructed generic type with array.
        /// </summary>
        [Test]
        public void Match_DualParameterWithEnumerableConstraintAndArray_ReturnsCorrectGenericType()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(DualParameterEnumerableHandler<,>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(uint[]) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(DualParameterEnumerableHandler<uint[], uint>), result,
                "Should construct DualParameterEnumerableHandler<uint[], uint>");
        }

        /// <summary>
        /// Verifies that IListHandler cannot match with incompatible type.
        /// </summary>
        [Test]
        public void CanMatch_IListHandlerWithIncompatibleType_ReturnsFalse()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(IListHandler<>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(int) }; // int does not implement IList<T>

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsFalse(result, "Should not match IListHandler<T> with non-IList<int> target");
        }

        #endregion

        #region Multi-Parameter Generic Constraint Tests

        /// <summary>
        /// Verifies that DictionaryHandler can match with Dictionary constraint.
        /// When the handler's target is Dictionary&lt;TKey, TValue&gt; and we provide Dictionary&lt;string, int&gt;,
        /// the system should correctly infer TKey=string and TValue=int.
        /// </summary>
        [Test]
        public void CanMatch_DictionaryHandlerWithDictionaryConstraint_ValidParameters_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(DictionaryHandler<,>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(Dictionary<string, int>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result, "Should be able to match DictionaryHandler<TKey, TValue> with Dictionary<string, int> target");
        }

        /// <summary>
        /// Verifies that IDictionaryHandler can match with Dictionary constraint.
        /// When the handler's target is IDictionary&lt;TKey, TValue&gt; and we provide Dictionary&lt;int, string&gt;,
        /// the system should correctly infer TKey=int and TValue=string.
        /// </summary>
        [Test]
        public void CanMatch_IDictionaryHandlerWithDictionaryConstraint_ValidParameters_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(IDictionaryHandler<,>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(Dictionary<int, string>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result, "Should be able to match IDictionaryHandler<TKey, TValue> with Dictionary<int, string> target");
        }

        /// <summary>
        /// Verifies that IDictionaryHandler can match with SortedDictionary constraint.
        /// When the handler's target is IDictionary&lt;TKey, TValue&gt; and we provide SortedDictionary&lt;float, double&gt;,
        /// the system should correctly infer the type parameters.
        /// </summary>
        [Test]
        public void CanMatch_IDictionaryHandlerWithSortedDictionaryConstraint_ValidParameters_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(IDictionaryHandler<,>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(SortedDictionary<float, double>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result, "Should be able to match IDictionaryHandler<TKey, TValue> with SortedDictionary<float, double> target");
        }

        /// <summary>
        /// Verifies that TripleParameterDictionaryHandler can match with IDictionary constraint.
        /// When T1 is constrained to IDictionary&lt;TKey, TValue&gt;, and targets provide Dictionary&lt;string, int&gt;,
        /// the system should correctly construct the generic type.
        /// </summary>
        [Test]
        public void CanMatch_TripleParameterDictionaryWithDictionaryConstraint_ValidParameters_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(TripleParameterDictionaryHandler<,,>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(Dictionary<string, int>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result, "Should be able to match when T1 satisfies IDictionary<TKey, TValue> constraint");
        }

        /// <summary>
        /// Verifies that TripleParameterDictionaryHandler can match with SortedDictionary constraint.
        /// </summary>
        [Test]
        public void CanMatch_TripleParameterDictionaryWithSortedDictionaryConstraint_ValidParameters_ReturnsTrue()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(TripleParameterDictionaryHandler<,,>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(SortedDictionary<int, float>) };

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsTrue(result, "Should be able to match TripleParameterDictionaryHandler with SortedDictionary<int, float> target");
        }

        /// <summary>
        /// Verifies that DictionaryHandler Match returns correctly constructed generic type.
        /// </summary>
        [Test]
        public void Match_DictionaryHandlerWithDictionaryConstraint_ReturnsCorrectGenericType()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(DictionaryHandler<,>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(Dictionary<string, bool>) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(DictionaryHandler<string, bool>), result,
                "Should construct DictionaryHandler<string, bool>");
        }

        /// <summary>
        /// Verifies that IDictionaryHandler Match returns correctly constructed generic type.
        /// </summary>
        [Test]
        public void Match_IDictionaryHandlerWithDictionaryConstraint_ReturnsCorrectGenericType()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(IDictionaryHandler<,>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(Dictionary<uint, long>) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(IDictionaryHandler<uint, long>), result,
                "Should construct IDictionaryHandler<uint, long>");
        }

        /// <summary>
        /// Verifies that TripleParameterDictionaryHandler Match returns correctly constructed generic type.
        /// </summary>
        [Test]
        public void Match_TripleParameterDictionaryWithDictionaryConstraint_ReturnsCorrectGenericType()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(TripleParameterDictionaryHandler<,,>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(Dictionary<double, short>) };

            // Act
            Type result = rule.Match(candidate, targets);

            // Assert
            Assert.AreEqual(typeof(TripleParameterDictionaryHandler<Dictionary<double, short>, double, short>), result,
                "Should construct TripleParameterDictionaryHandler<Dictionary<double, short>, double, short>");
        }

        /// <summary>
        /// Verifies that IDictionaryHandler cannot match with incompatible type.
        /// </summary>
        [Test]
        public void CanMatch_IDictionaryHandlerWithIncompatibleType_ReturnsFalse()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(IDictionaryHandler<,>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(List<int>) }; // List<int> does not implement IDictionary<TKey, TValue>

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsFalse(result, "Should not match IDictionaryHandler<TKey, TValue> with non-dictionary target");
        }

        /// <summary>
        /// Verifies that IDictionaryHandler cannot match with single-dimensional array.
        /// </summary>
        [Test]
        public void CanMatch_IDictionaryHandlerWithArray_ReturnsFalse()
        {
            // Arrange
            var rule = new GenericParameterConstraintsMatchRule();
            var genericDefinition = typeof(IDictionaryHandler<,>);
            var constraint = genericDefinition.GetGenericArgumentsRelativeTo(typeof(IHandler<>))[0];
            var candidate = new TypeMatchCandidate(genericDefinition, 0, new[] { constraint });

            var targets = new[] { typeof(int[]) }; // int[] does not implement IDictionary<TKey, TValue>

            // Act
            bool result = rule.CanMatch(candidate, targets);

            // Assert
            Assert.IsFalse(result, "Should not match IDictionaryHandler<TKey, TValue> with array target");
        }

        #endregion
    }
}
