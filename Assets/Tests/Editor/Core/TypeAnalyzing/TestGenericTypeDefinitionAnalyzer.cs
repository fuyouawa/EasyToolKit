using System;
using System.Collections.Generic;
using NUnit.Framework;
using EasyToolkit.Core.Reflection;

namespace Tests.Core.TypeAnalyzing
{
    /// <summary>
    /// Tests for the <see cref="IGenericTypeDefinitionAnalyzer"/> functionality.
    /// </summary>
    [TestFixture]
    public class TestGenericTypeDefinitionAnalyzer
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
            var analyzer = TypeAnalyzerFactory.GetGenericTypeDefinitionAnalyzer(type);

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
            var analyzer = TypeAnalyzerFactory.GetGenericTypeDefinitionAnalyzer(type);

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
        public void GetParameterByName_DependencyType_T1ReferencesT2()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeDefinitionAnalyzer(type);

            // Act
            var t1 = analyzer.GetParameterByName("T1");

            // Assert
            Assert.IsNotNull(t1);
            Assert.IsTrue(t1.HasDependencies);
            Assert.AreEqual(1, t1.ReferencedParameters.Count);
            Assert.AreEqual("T2", t1.ReferencedParameters[0].Name);
        }

        [Test]
        public void GetParameterByName_DependencyType_T2IsIndependent()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeDefinitionAnalyzer(type);

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
            var analyzer = TypeAnalyzerFactory.GetGenericTypeDefinitionAnalyzer(type);

            // Act
            var independentParams = analyzer.GetIndependentParameters();

            // Assert
            Assert.AreEqual(1, independentParams.Count);
            Assert.AreEqual("T2", independentParams[0].Name);
        }

        #endregion

        #region Type Argument Validation Tests

        [Test]
        public void ValidateTypeArguments_DependencyType_ValidArguments_ReturnsTrue()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeDefinitionAnalyzer(type);

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
            var analyzer = TypeAnalyzerFactory.GetGenericTypeDefinitionAnalyzer(type);

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
            var analyzer = TypeAnalyzerFactory.GetGenericTypeDefinitionAnalyzer(type);

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
            var analyzer = TypeAnalyzerFactory.GetGenericTypeDefinitionAnalyzer(type);

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
            var analyzer = TypeAnalyzerFactory.GetGenericTypeDefinitionAnalyzer(type);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                analyzer.ValidateTypeArguments(typeof(List<int>));
            });
        }

        #endregion

        #region Type Argument Inference Tests

        [Test]
        public void TryInferTypeArguments_DependencyType_CanInferT2FromListT()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeDefinitionAnalyzer(type);

            // Act - T1 is List<int>, T2 is null (to be inferred)
            var result = analyzer.TryInferTypeArguments(new Type[] { typeof(List<int>), null }, out var inferredTypes);

            // Assert
            Assert.IsTrue(result, "Should infer T2 from List<int>");
            Assert.AreEqual(typeof(List<int>), inferredTypes[0]);
            Assert.AreEqual(typeof(int), inferredTypes[1]);
        }

        [Test]
        public void TryInferTypeArguments_DependencyType_CanInferT2FromListString()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeDefinitionAnalyzer(type);

            // Act - T1 is List<string>, T2 is null (to be inferred)
            var result = analyzer.TryInferTypeArguments(new Type[] { typeof(List<string>), null }, out var inferredTypes);

            // Assert
            Assert.IsTrue(result, "Should infer T2 from List<string>");
            Assert.AreEqual(typeof(List<string>), inferredTypes[0]);
            Assert.AreEqual(typeof(string), inferredTypes[1]);
        }

        [Test]
        public void TryInferTypeArguments_AllKnownTypes_ReturnsFalse()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeDefinitionAnalyzer(type);

            // Act - Both types are already known
            var result = analyzer.TryInferTypeArguments(new Type[] { typeof(List<int>), typeof(int) }, out var inferredTypes);

            // Assert
            Assert.IsFalse(result, "Should return false when all types are already known");
            Assert.AreEqual(typeof(List<int>), inferredTypes[0]);
            Assert.AreEqual(typeof(int), inferredTypes[1]);
        }

        [Test]
        public void TryInferTypeArguments_AllNull_ReturnsFalse()
        {
            // Arrange
            var type = typeof(DependencyContainer<,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeDefinitionAnalyzer(type);

            // Act - Both types are null (nothing to infer from)
            var result = analyzer.TryInferTypeArguments(new Type[] { null, null }, out var inferredTypes);

            // Assert
            Assert.IsFalse(result, "Should return false when all types are null");
            Assert.IsNull(inferredTypes[0]);
            Assert.IsNull(inferredTypes[1]);
        }

        [Test]
        public void TryInferTypeArguments_SimpleType_NoDependencies_ReturnsFalse()
        {
            // Arrange
            var type = typeof(SimpleContainer<>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeDefinitionAnalyzer(type);

            // Act - T is null but has no dependencies
            var result = analyzer.TryInferTypeArguments(new Type[] { null }, out var inferredTypes);

            // Assert
            Assert.IsFalse(result, "Should return false when no dependencies exist");
            Assert.IsNull(inferredTypes[0]);
        }

        [Test]
        public void TryInferTypeArguments_ComplexType_CanInferChain()
        {
            // Arrange
            var type = typeof(ComplexContainer<,,>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeDefinitionAnalyzer(type);

            // Act - T1 is List<List<int>>, T2 is null, T3 is null
            // T1: IEnumerable<T2> = List<List<int>> → T2 should be List<int>
            // T2: IList<T3> = List<int> → T3 should be int
            var result = analyzer.TryInferTypeArguments(new Type[] { typeof(List<List<int>>), null, null }, out var inferredTypes);

            // Assert
            Assert.IsTrue(result, "Should infer T2 and T3 from List<List<int>>");
            Assert.AreEqual(typeof(List<List<int>>), inferredTypes[0]);
            Assert.AreEqual(typeof(List<int>), inferredTypes[1]);
            Assert.AreEqual(typeof(int), inferredTypes[2]);
        }

        [Test]
        public void TryInferTypeArguments_NullInput_ThrowsArgumentNullException()
        {
            // Arrange
            var type = typeof(SimpleContainer<>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeDefinitionAnalyzer(type);

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
            var analyzer = TypeAnalyzerFactory.GetGenericTypeDefinitionAnalyzer(type);

            // Act & Assert - Only provide one argument instead of two
            Assert.Throws<ArgumentException>(() =>
            {
                analyzer.TryInferTypeArguments(new Type[] { typeof(List<int>) }, out _);
            });
        }

        #endregion

        #region Edge Cases Tests

        [Test]
        public void GetParameterByName_NonExistentName_ReturnsNull()
        {
            // Arrange
            var type = typeof(SimpleContainer<>);
            var analyzer = TypeAnalyzerFactory.GetGenericTypeDefinitionAnalyzer(type);

            // Act
            var parameter = analyzer.GetParameterByName("NonExistent");

            // Assert
            Assert.IsNull(parameter);
        }

        #endregion
    }
}
