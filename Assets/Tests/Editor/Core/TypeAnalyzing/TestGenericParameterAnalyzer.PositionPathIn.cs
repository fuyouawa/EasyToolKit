using System;
using System.Collections.Generic;
using NUnit.Framework;
using EasyToolkit.Core.Reflection;

namespace Tests.Core.TypeAnalyzing
{
    /// <summary>
    /// Tests for the FindPositionPathIn method of IGenericParameterAnalyzer.
    /// </summary>
    [TestFixture]
    public class TestGenericParameterAnalyzer_PositionPathIn
    {
        #region Direct Generic Parameter

        /// <summary>
        /// Tests that FindPositionPathIn returns correct path for a direct generic parameter (e.g., List&lt;T&gt;).
        /// </summary>
        [Test]
        public void FindPositionPathIn_DirectGenericParameter_ReturnsSingleElementPath()
        {
            // Arrange
            var genericParam = typeof(List<>).GetGenericArguments()[0];
            var analyzer = TypeAnalyzerFactory.GetGenericParameterAnalyzer(genericParam);
            var targetType = typeof(List<>);

            // Act
            var path = analyzer.FindPositionPathIn(targetType);

            // Assert
            Assert.IsNotNull(path, "Path should not be null for direct generic parameter");
            Assert.AreEqual(1, path.Length, "Path should have one element");
            Assert.AreEqual(0, path[0], "T should be at position 0 in List<T>");
        }

        #endregion

        #region Nested Generic Parameter

        /// <summary>
        /// Tests that FindPositionPathIn returns correct path for nested generic parameter (e.g., Dictionary&lt;int, List&lt;T&gt;&gt;).
        /// </summary>
        [Test]
        public void FindPositionPathIn_NestedGenericParameter_ReturnsMultiElementPath()
        {
            // Arrange
            var listGenericParam = typeof(List<>).GetGenericArguments()[0];
            var analyzer = TypeAnalyzerFactory.GetGenericParameterAnalyzer(listGenericParam);
            var targetType = typeof(Dictionary<,>).MakeGenericType(typeof(int), typeof(List<>).MakeGenericType(listGenericParam));

            // Act
            var path = analyzer.FindPositionPathIn(targetType);

            // Assert
            Assert.IsNotNull(path, "Path should not be null for nested generic parameter");
            Assert.AreEqual(2, path.Length, "Path should have two elements");
            Assert.AreEqual(1, path[0], "List<T> should be at position 1 in Dictionary<int, List<T>>");
            Assert.AreEqual(0, path[1], "T should be at position 0 in List<T>");
        }

        /// <summary>
        /// Tests that FindPositionPathIn handles deeply nested generic parameters.
        /// </summary>
        [Test]
        public void FindPositionPathIn_DeeplyNestedGenericParameter_ReturnsCorrectPath()
        {
            // Arrange
            var listGenericParam = typeof(List<>).GetGenericArguments()[0];
            var analyzer = TypeAnalyzerFactory.GetGenericParameterAnalyzer(listGenericParam);

            // Create type: Dictionary<int, Dictionary<string, List<T>>>
            var listType = typeof(List<>).MakeGenericType(listGenericParam);
            var innerDictType = typeof(Dictionary<,>).MakeGenericType(typeof(string), listType);
            var outerDictType = typeof(Dictionary<,>).MakeGenericType(typeof(int), innerDictType);

            // Act
            var path = analyzer.FindPositionPathIn(outerDictType);

            // Assert
            Assert.IsNotNull(path, "Path should not be null for deeply nested generic parameter");
            Assert.AreEqual(3, path.Length, "Path should have three elements");
            Assert.AreEqual(1, path[0], "Dictionary<string, List<T>> should be at position 1");
            Assert.AreEqual(1, path[1], "List<T> should be at position 1 in inner Dictionary");
            Assert.AreEqual(0, path[2], "T should be at position 0 in List<T>");
        }

        #endregion

        #region Multiple Generic Parameters

        /// <summary>
        /// Tests that FindPositionPathIn correctly identifies the second generic parameter in a two-parameter type.
        /// </summary>
        [Test]
        public void FindPositionPathIn_SecondGenericParameter_ReturnsCorrectIndex()
        {
            // Arrange
            var dictGenericParams = typeof(Dictionary<,>).GetGenericArguments();
            var valueParam = dictGenericParams[1]; // TValue parameter
            var analyzer = TypeAnalyzerFactory.GetGenericParameterAnalyzer(valueParam);
            var targetType = typeof(Dictionary<,>);

            // Act
            var path = analyzer.FindPositionPathIn(targetType);

            // Assert
            Assert.IsNotNull(path, "Path should not be null for second generic parameter");
            Assert.AreEqual(1, path.Length, "Path should have one element");
            Assert.AreEqual(1, path[0], "TValue should be at position 1 in Dictionary<TKey, TValue>");
        }

        #endregion

        #region Parameter Not Found

        /// <summary>
        /// Tests that FindPositionPathIn returns null when the generic parameter is not present in the target type.
        /// </summary>
        [Test]
        public void FindPositionPathIn_ParameterNotPresentInType_ReturnsNull()
        {
            // Arrange
            var listGenericParam = typeof(List<>).GetGenericArguments()[0];
            var analyzer = TypeAnalyzerFactory.GetGenericParameterAnalyzer(listGenericParam);
            var targetType = typeof(Dictionary<int, string>); // No generic parameters

            // Act
            var path = analyzer.FindPositionPathIn(targetType);

            // Assert
            Assert.IsNull(path, "Path should be null when parameter is not present");
        }

        /// <summary>
        /// Tests that FindPositionPathIn returns null when searching for a different generic parameter.
        /// </summary>
        [Test]
        public void FindPositionPathIn_DifferentGenericParameter_ReturnsNull()
        {
            // Arrange
            var dictGenericParams = typeof(Dictionary<,>).GetGenericArguments();
            var keyParam = dictGenericParams[0]; // TKey
            var listGenericParam = typeof(List<>).GetGenericArguments()[0]; // T
            var analyzer = TypeAnalyzerFactory.GetGenericParameterAnalyzer(keyParam);
            var targetType = typeof(List<>).MakeGenericType(listGenericParam);

            // Act
            var path = analyzer.FindPositionPathIn(targetType);

            // Assert
            Assert.IsNull(path, "Path should be null when searching for different parameter");
        }

        #endregion

        #region Null Target Type

        /// <summary>
        /// Tests that FindPositionPathIn returns null when the target type is null.
        /// </summary>
        [Test]
        public void FindPositionPathIn_NullTargetType_ReturnsNull()
        {
            // Arrange
            var listGenericParam = typeof(List<>).GetGenericArguments()[0];
            var analyzer = TypeAnalyzerFactory.GetGenericParameterAnalyzer(listGenericParam);

            // Act
            var path = analyzer.FindPositionPathIn(null);

            // Assert
            Assert.IsNull(path, "Path should be null for null target type");
        }

        #endregion

        #region Complex Scenarios

        /// <summary>
        /// Tests that FindPositionPathIn works with custom generic types containing the parameter.
        /// </summary>
        [Test]
        public void FindPositionPathIn_CustomGenericType_ReturnsCorrectPath()
        {
            // Arrange
            var genericParam = typeof(TestCustomGenericClass<>).GetGenericArguments()[0];
            var analyzer = TypeAnalyzerFactory.GetGenericParameterAnalyzer(genericParam);
            var targetType = typeof(TestCustomGenericClass<>);

            // Act
            var path = analyzer.FindPositionPathIn(targetType);

            // Assert
            Assert.IsNotNull(path, "Path should not be null for custom generic type");
            Assert.AreEqual(1, path.Length, "Path should have one element");
            Assert.AreEqual(0, path[0], "T should be at position 0 in TestCustomGenericClass<T>");
        }

        #endregion

        #region Helper Types

        /// <summary>
        /// Helper class for testing custom generic types.
        /// </summary>
        /// <typeparam name="T">The generic type parameter.</typeparam>
        private class TestCustomGenericClass<T>
        {
            public T Value { get; set; }
        }

        #endregion
    }
}
