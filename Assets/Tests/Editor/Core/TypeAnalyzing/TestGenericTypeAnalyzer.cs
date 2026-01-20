using System;
using System.Collections.Generic;
using NUnit.Framework;
using EasyToolKit.Core.Reflection;

namespace Tests.Core.TypeAnalyzing
{
    /// <summary>
    /// Tests for the <see cref="IGenericTypeAnalyzer"/> functionality.
    /// </summary>
    [TestFixture]
    public class TestGenericTypeAnalyzer
    {
        #region Test Types

        // Simple generic type without constraints
        public class SimpleContainer<T>
        {
            public T Value { get; set; }
        }

        // Generic type with parameter dependency: T1 depends on T2
        public class DependencyContainer<T1, T2>
            where T1 : List<T2>
        {
            public T1 Items { get; set; }
        }

        // Generic type with multiple constraint types
        public class MultiConstraintContainer<T>
            where T : class, new()
        {
            public T Value { get; set; }
        }

        // Complex generic type with multiple parameter dependencies
        public class ComplexContainer<T1, T2, T3>
            where T1 : IEnumerable<T2>
            where T2 : IList<T3>
            where T3 : struct
        {
            public T1 Collection { get; set; }
        }

        // Dictionary-like generic type
        public class DictionaryLike<TKey, TValue>
            where TKey : notnull
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
        }

        #endregion

        #region Basic Analysis Tests

        [Test]
        public void GetGenericTypeAnalyzer_SimpleType_ReturnsAnalyzer()
        {
            // Arrange
            var type = typeof(SimpleContainer<>);

            // Act
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);

            // Assert
            Assert.IsNotNull(analyzer);
            Assert.AreEqual(type, analyzer.Type);
            Assert.AreEqual(1, analyzer.Parameters.Count);
        }

        [Test]
        public void Parameters_DependencyType_ReturnsTwoParameters()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);

            // Act
            var parameters = analyzer.Parameters;

            // Assert
            Assert.AreEqual(2, parameters.Count);
            Assert.AreEqual("T1", parameters[0].Name);
            Assert.AreEqual("T2", parameters[1].Name);
        }

        #endregion

        #region Dependency Analysis Tests

        [Test]
        public void GetParameterByName_DependencyType_T1DependsOnT2()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);

            // Act
            var t1 = analyzer.GetParameterByName("T1");

            // Assert
            Assert.IsNotNull(t1);
            Assert.IsTrue(t1.HasDependencies);
            Assert.AreEqual(1, t1.DependsOnParameters.Count);
            Assert.AreEqual("T2", t1.DependsOnParameters[0].Name);
        }

        [Test]
        public void GetParameterByName_DependencyType_T2IsIndependent()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);

            // Act
            var t2 = analyzer.GetParameterByName("T2");

            // Assert
            Assert.IsNotNull(t2);
            Assert.IsFalse(t2.HasDependencies);
            Assert.IsTrue(t2.IsDependencyForOthers);
        }

        [Test]
        public void GetIndependentParameters_DependencyType_ReturnsT2()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);

            // Act
            var independentParams = analyzer.GetIndependentParameters();

            // Assert
            Assert.AreEqual(1, independentParams.Count);
            Assert.AreEqual("T2", independentParams[0].Name);
        }

        #endregion

        #region Constraint Tests

        [Test]
        public void Parameters_MultiConstraintType_HasCorrectConstraints()
        {
            // Arrange
            var type = typeof(MultiConstraintContainer<>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);

            // Act
            var parameter = analyzer.GetParameterByName("T");

            // Assert
            Assert.IsTrue(parameter.HasConstraints);
            Assert.AreEqual(GenericParameterSpecialConstraints.ReferenceType | GenericParameterSpecialConstraints.DefaultConstructor,
                parameter.SpecialConstraints);
        }

        [Test]
        public void Parameters_StructConstraint_HasValueTypeConstraint()
        {
            // Arrange
            var type = typeof(ComplexContainer<,,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);

            // Act
            var t3 = analyzer.GetParameterByName("T3");

            // Assert
            Assert.IsTrue(t3.HasConstraints);
            // struct constraint includes both ValueType and DefaultConstructor
            Assert.AreEqual(GenericParameterSpecialConstraints.ValueType | GenericParameterSpecialConstraints.DefaultConstructor,
                t3.SpecialConstraints);
        }

        #endregion

        #region Type Argument Validation Tests

        [Test]
        public void ValidateTypeArguments_DependencyType_ValidArguments_ReturnsTrue()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);

            // Act
            var isValid = analyzer.ValidateTypeArguments(typeof(List<int>), typeof(int));

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        public void ValidateTypeArguments_DependencyType_InvalidArguments_ReturnsFalse()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);

            // Act - T1 must be List<T2>, but we pass List<string> for T1 and int for T2
            var isValid = analyzer.ValidateTypeArguments(typeof(List<string>), typeof(int));

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void ValidateTypeArguments_MultiConstraintType_ValidClassWithConstructor_ReturnsTrue()
        {
            // Arrange
            var type = typeof(MultiConstraintContainer<>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);

            // Act
            var isValid = analyzer.ValidateTypeArguments(typeof(List<int>));

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        public void ValidateTypeArguments_MultiConstraintType_Struct_ReturnsFalse()
        {
            // Arrange
            var type = typeof(MultiConstraintContainer<>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);

            // Act
            var isValid = analyzer.ValidateTypeArguments(typeof(int));

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void ValidateTypeArguments_WrongArgumentCount_ThrowsArgumentException()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                analyzer.ValidateTypeArguments(typeof(List<int>));
            });
        }

        #endregion

        #region Edge Cases Tests

        [Test]
        public void GetParameterByName_NonExistentName_ReturnsNull()
        {
            // Arrange
            var type = typeof(SimpleContainer<>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);

            // Act
            var parameter = analyzer.GetParameterByName("NonExistent");

            // Assert
            Assert.IsNull(parameter);
        }

        #endregion

        #region Type Argument Inference Tests

        [Test]
        public void TryInferTypeArguments_DependencyType_CanInferT2FromT1()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);
            var t1 = typeof(List<int>).GetGenericTypeDefinition();
            var t2Parameter = type.GetGenericArguments()[1];

            // Act - T1 is List<int>, T2 is unknown (generic parameter)
            var result = analyzer.TryInferTypeArguments(new[] { typeof(List<int>), t2Parameter }, out var inferredTypes);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(typeof(List<int>), inferredTypes[0]);
            Assert.AreEqual(typeof(int), inferredTypes[1]);
        }

        [Test]
        public void TryInferTypeArguments_DependencyType_CanInferT2FromListString()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);
            var t2Parameter = type.GetGenericArguments()[1];

            // Act - T1 is List<string>, T2 is unknown
            var result = analyzer.TryInferTypeArguments(new[] { typeof(List<string>), t2Parameter }, out var inferredTypes);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(typeof(List<string>), inferredTypes[0]);
            Assert.AreEqual(typeof(string), inferredTypes[1]);
        }

        [Test]
        public void TryInferTypeArguments_SimpleType_NoDependencies_ReturnsFalse()
        {
            // Arrange
            var type = typeof(SimpleContainer<>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);
            var tParameter = type.GetGenericArguments()[0];

            // Act - T is unknown and has no dependencies
            var result = analyzer.TryInferTypeArguments(new[] { tParameter }, out var inferredTypes);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(tParameter, inferredTypes[0]);
        }

        [Test]
        public void TryInferTypeArguments_AllKnownTypes_ReturnsFalse()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);

            // Act - Both types are already known
            var result = analyzer.TryInferTypeArguments(new[] { typeof(List<int>), typeof(int) }, out var inferredTypes);

            // Assert
            Assert.IsFalse(result, "Should return false when all types are already known");
            Assert.AreEqual(typeof(List<int>), inferredTypes[0]);
            Assert.AreEqual(typeof(int), inferredTypes[1]);
        }

        [Test]
        public void TryInferTypeArguments_AllUnknown_ReturnsFalse()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);
            var t1Parameter = type.GetGenericArguments()[0];
            var t2Parameter = type.GetGenericArguments()[1];

            // Act - Both types are unknown
            var result = analyzer.TryInferTypeArguments(new[] { t1Parameter, t2Parameter }, out var inferredTypes);

            // Assert
            Assert.IsFalse(result, "Should return false when no types can be inferred");
            Assert.AreEqual(t1Parameter, inferredTypes[0]);
            Assert.AreEqual(t2Parameter, inferredTypes[1]);
        }

        [Test]
        public void TryInferTypeArguments_ComplexType_CanInferChain()
        {
            // Arrange
            var type = typeof(ComplexContainer<,,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);
            var t1Parameter = type.GetGenericArguments()[0];
            var t2Parameter = type.GetGenericArguments()[1];
            var t3Parameter = type.GetGenericArguments()[2];

            // Act - T1 is List<int[]>, T2 and T3 are unknown
            // T1: IEnumerable<T2> = List<int[]> → T2 should be int[]
            // T2: IList<T3> = int[] → T3 should be int
            var result = analyzer.TryInferTypeArguments(
                new[] { typeof(List<int[]>), t2Parameter, t3Parameter },
                out var inferredTypes);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(typeof(List<int[]>), inferredTypes[0]);
            Assert.AreEqual(typeof(int[]), inferredTypes[1]);
            Assert.AreEqual(typeof(int), inferredTypes[2]);
        }

        [Test]
        public void TryInferTypeArguments_ComplexType_PartialInference()
        {
            // Arrange
            var type = typeof(ComplexContainer<,,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);
            var t1Parameter = type.GetGenericArguments()[0];
            var t2Parameter = type.GetGenericArguments()[1];

            // Act - T1 is known, T2 is unknown, T3 is int (but T2 is not inferred)
            // This tests scenario where T2 is provided as generic parameter but T3 is known
            var result = analyzer.TryInferTypeArguments(
                new[] { typeof(List<string>), t2Parameter, typeof(string) },
                out var inferredTypes);

            // Assert - T2 should be inferred as string from List<string>
            Assert.IsTrue(result);
            Assert.AreEqual(typeof(List<string>), inferredTypes[0]);
            Assert.AreEqual(typeof(string), inferredTypes[1]);
            Assert.AreEqual(typeof(string), inferredTypes[2]);
        }

        [Test]
        public void TryInferTypeArguments_NullTypeArguments_ThrowsArgumentNullException()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                analyzer.TryInferTypeArguments(null, out _);
            });
        }

        [Test]
        public void TryInferTypeArguments_WrongArgumentCount_ThrowsArgumentException()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                analyzer.TryInferTypeArguments(new[] { typeof(List<int>) }, out _);
            });
        }

        [Test]
        public void TryInferTypeArguments_DictionaryLikeType_NoInferencePossible()
        {
            // Arrange
            var type = typeof(DictionaryLike<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeAnalyzer(type);
            var tKeyParameter = type.GetGenericArguments()[0];
            var tValueParameter = type.GetGenericArguments()[1];

            // Act - TKey is unknown, TValue is unknown
            // DictionaryLike has notnull constraint on TKey but no type dependency
            var result = analyzer.TryInferTypeArguments(
                new[] { tKeyParameter, tValueParameter },
                out var inferredTypes);

            // Assert
            Assert.IsFalse(result, "Should return false when no type dependencies exist");
            Assert.AreEqual(tKeyParameter, inferredTypes[0]);
            Assert.AreEqual(tValueParameter, inferredTypes[1]);
        }

        #endregion
    }
}
