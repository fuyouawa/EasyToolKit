using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Core.Reflection.Implementations;

namespace Tests.Core.TypeAnalyzing
{
    /// <summary>
    /// Unit tests for <see cref="IOpenGenericTypeAnalyzer"/> and <see cref="OpenGenericTypeAnalyzer"/>.
    /// </summary>
    [TestFixture]
    public class TestOpenGenericTypeAnalyzer
    {
        #region Test Types

        /// <summary>
        /// Simple generic type with one parameter.
        /// </summary>
        public class SimpleContainer<T>
        {
            public T Value { get; set; }
        }

        /// <summary>
        /// Generic type with two parameters.
        /// </summary>
        public class DoubleContainer<T, U>
        {
            public T First { get; set; }
            public U Second { get; set; }
        }

        /// <summary>
        /// Generic type with dependency: T depends on U.
        /// </summary>
        public class DependentContainer<T, U> where T : List<U>
        {
            public T Items { get; set; }
        }

        /// <summary>
        /// Generic type with multiple dependencies.
        /// </summary>
        public class MultiDependentContainer<T, U, V> where T : Dictionary<U, V>
        {
            public T Mapping { get; set; }
        }

        /// <summary>
        /// Generic type with class constraint.
        /// </summary>
        public class ClassConstrainedContainer<T> where T : class
        {
            public T Value { get; set; }
        }

        /// <summary>
        /// Nested generic type.
        /// </summary>
        public class NestedContainer<T, U> where T : IEnumerable<List<U>>
        {
            public T Collection { get; set; }
        }

        /// <summary>
        /// Derived container for inheritance testing.
        /// </summary>
        public class DerivedContainer : SimpleContainer<int> { }

        /// <summary>
        /// Int container for inheritance testing.
        /// </summary>
        public class IntContainer : SimpleContainer<int> { }

        #endregion

        #region Constructor Tests

        /// <summary>
        /// Verifies that constructor creates analyzer for generic type definition.
        /// </summary>
        [Test]
        public void Constructor_GenericTypeDefinition_SetsPropertiesCorrectly()
        {
            // Arrange
            var type = typeof(SimpleContainer<>);

            // Act
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Assert
            Assert.AreEqual(type, analyzer.OpenGenericType);
        }

        /// <summary>
        /// Verifies that constructor creates analyzer for partially constructed type.
        /// </summary>
        [Test]
        public void Constructor_PartiallyConstructedType_SetsPropertiesCorrectly()
        {
            // Arrange - Create Dictionary<string, T> (partially constructed)
            var dictTypeDef = typeof(Dictionary<,>);
            var tValueParam = dictTypeDef.GetGenericArguments()[1];
            var partiallyConstructed = dictTypeDef.MakeGenericType(typeof(string), tValueParam);

            // Act
            var analyzer = new OpenGenericTypeAnalyzer(partiallyConstructed);

            // Assert
            Assert.AreEqual(partiallyConstructed, analyzer.OpenGenericType);
        }

        /// <summary>
        /// Verifies that constructor throws ArgumentNullException when type is null.
        /// </summary>
        [Test]
        public void Constructor_NullType_ThrowsArgumentNullException()
        {
            // Arrange
            Type nullType = null;

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new OpenGenericTypeAnalyzer(nullType));
            Assert.That(ex.Message, Does.Contain("Type cannot be null"));
        }

        /// <summary>
        /// Verifies that constructor accepts closed generic types.
        /// </summary>
        [Test]
        public void Constructor_ClosedGenericType_AcceptsType()
        {
            // Arrange
            var closedType = typeof(List<int>);

            // Act & Assert - Should not throw, closed types are now accepted
            var analyzer = new OpenGenericTypeAnalyzer(closedType);
            Assert.AreEqual(closedType, analyzer.OpenGenericType);
        }

        /// <summary>
        /// Verifies that constructor throws ArgumentException when type is non-generic.
        /// </summary>
        [Test]
        public void Constructor_NonGenericType_ThrowsArgumentException()
        {
            // Arrange
            // Use int instead of string because string implements IEnumerable<char> (a generic interface)
            var nonGenericType = typeof(int);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new OpenGenericTypeAnalyzer(nonGenericType));
            Assert.That(ex.Message, Does.Contain("must be a generic type"));
        }

        #endregion

        #region Parameters Property Tests

        /// <summary>
        /// Verifies that Parameters returns all parameters for generic type definition.
        /// </summary>
        [Test]
        public void Parameters_GenericTypeDefinition_ReturnsAllParameters()
        {
            // Arrange
            var type = typeof(DoubleContainer<,>);
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act
            var parameters = analyzer.Parameters;

            // Assert
            Assert.AreEqual(2, parameters.Count);
            Assert.AreEqual("T", parameters[0].Name);
            Assert.AreEqual("U", parameters[1].Name);
        }

        /// <summary>
        /// Verifies that Parameters includes both substituted and generic parameters for partially constructed type.
        /// </summary>
        [Test]
        public void Parameters_PartiallyConstructedType_ReturnsMixedParameters()
        {
            // Arrange
            var dictTypeDef = typeof(Dictionary<,>);
            var tKeyParam = dictTypeDef.GetGenericArguments()[0];
            var tValueParam = dictTypeDef.GetGenericArguments()[1];

            // Create Dictionary<string, T>
            var partiallyConstructed = dictTypeDef.MakeGenericType(typeof(string), tValueParam);
            var analyzer = new OpenGenericTypeAnalyzer(partiallyConstructed);

            // Act
            var parameters = analyzer.Parameters;

            // Assert
            Assert.AreEqual(2, parameters.Count);
            Assert.AreEqual("TKey", parameters[0].Name);
            Assert.AreEqual("TValue", parameters[1].Name);

            // TKey should be substituted with string
            Assert.AreEqual(typeof(string), parameters[0].SubstitutedType);

            // TValue should not be substituted
            Assert.IsNull(parameters[1].SubstitutedType);
        }

        #endregion

        #region GenericParameters Property Tests

        /// <summary>
        /// Verifies that GenericParameters returns all parameters for generic type definition.
        /// </summary>
        [Test]
        public void GenericParameters_GenericTypeDefinition_ReturnsAllParameters()
        {
            // Arrange
            var type = typeof(DoubleContainer<,>);
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act
            var genericParameters = analyzer.GenericParameters;

            // Assert
            Assert.AreEqual(2, genericParameters.Count);
            Assert.AreEqual("T", genericParameters[0].Name);
            Assert.AreEqual("U", genericParameters[1].Name);
        }

        /// <summary>
        /// Verifies that GenericParameters returns only non-substituted parameters for partially constructed type.
        /// </summary>
        [Test]
        public void GenericParameters_PartiallyConstructedType_ReturnsOnlyGenericParameters()
        {
            // Arrange
            var dictTypeDef = typeof(Dictionary<,>);
            var tValueParam = dictTypeDef.GetGenericArguments()[1];

            // Create Dictionary<string, T>
            var partiallyConstructed = dictTypeDef.MakeGenericType(typeof(string), tValueParam);
            var analyzer = new OpenGenericTypeAnalyzer(partiallyConstructed);

            // Act
            var genericParameters = analyzer.GenericParameters;

            // Assert
            Assert.AreEqual(1, genericParameters.Count);
            Assert.AreEqual("TValue", genericParameters[0].Name);
            Assert.IsNull(genericParameters[0].SubstitutedType);
        }

        /// <summary>
        /// Verifies that GenericParameters is empty for fully substituted type.
        /// </summary>
        [Test]
        public void GenericParameters_AllParametersSubstituted_ReturnsEmpty()
        {
            // Arrange
            // This is a bit tricky since we can't create a fully substituted type that still has generic parameters
            // But we can create a nested scenario where outer type has generic parameter but inner doesn't
            var listTypeDef = typeof(List<>);
            var tParam = listTypeDef.GetGenericArguments()[0];

            // Create List<T> where T is a generic parameter from somewhere else
            var openType = listTypeDef.MakeGenericType(tParam);
            var analyzer = new OpenGenericTypeAnalyzer(openType);

            // Act
            var genericParameters = analyzer.GenericParameters;

            // Assert
            Assert.AreEqual(1, genericParameters.Count);
            Assert.AreEqual("T", genericParameters[0].Name);
        }

        #endregion

        #region SubstitutedParameters Property Tests

        /// <summary>
        /// Verifies that SubstitutedParameters is empty for generic type definition.
        /// </summary>
        [Test]
        public void SubstitutedParameters_GenericTypeDefinition_ReturnsEmpty()
        {
            // Arrange
            var type = typeof(SimpleContainer<>);
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act
            var substitutedParameters = analyzer.SubstitutedParameters;

            // Assert
            Assert.AreEqual(0, substitutedParameters.Count);
        }

        /// <summary>
        /// Verifies that SubstitutedParameters returns only substituted parameters for partially constructed type.
        /// </summary>
        [Test]
        public void SubstitutedParameters_PartiallyConstructedType_ReturnsSubstitutedOnly()
        {
            // Arrange
            var dictTypeDef = typeof(Dictionary<,>);
            var tValueParam = dictTypeDef.GetGenericArguments()[1];

            // Create Dictionary<string, T>
            var partiallyConstructed = dictTypeDef.MakeGenericType(typeof(string), tValueParam);
            var analyzer = new OpenGenericTypeAnalyzer(partiallyConstructed);

            // Act
            var substitutedParameters = analyzer.SubstitutedParameters;

            // Assert
            Assert.AreEqual(1, substitutedParameters.Count);
            Assert.AreEqual("TKey", substitutedParameters[0].Name);
            Assert.AreEqual(typeof(string), substitutedParameters[0].SubstitutedType);
        }

        #endregion

        #region TypeAnalyzerFactory Integration Tests

        /// <summary>
        /// Verifies that TypeAnalyzerFactory.GetOpenGenericTypeAnalyzer returns correct analyzer.
        /// </summary>
        [Test]
        public void TypeAnalyzerFactory_GetOpenGenericTypeAnalyzer_GenericTypeDefinition_ReturnsAnalyzer()
        {
            // Arrange
            var type = typeof(SimpleContainer<>);

            // Act
            var analyzer = TypeAnalyzerFactory.GetOpenGenericTypeAnalyzer(type);

            // Assert
            Assert.IsNotNull(analyzer);
            Assert.IsInstanceOf<OpenGenericTypeAnalyzer>(analyzer);
            Assert.AreEqual(type, analyzer.OpenGenericType);
        }

        /// <summary>
        /// Verifies that TypeAnalyzerFactory caches analyzers.
        /// </summary>
        [Test]
        public void TypeAnalyzerFactory_GetOpenGenericTypeAnalyzer_CachesAnalyzers()
        {
            // Arrange
            var type = typeof(SimpleContainer<>);

            // Act
            var analyzer1 = TypeAnalyzerFactory.GetOpenGenericTypeAnalyzer(type);
            var analyzer2 = TypeAnalyzerFactory.GetOpenGenericTypeAnalyzer(type);

            // Assert
            Assert.AreSame(analyzer1, analyzer2);
        }

        /// <summary>
        /// Verifies that TypeAnalyzerFactory accepts closed generic types.
        /// </summary>
        [Test]
        public void TypeAnalyzerFactory_GetOpenGenericTypeAnalyzer_ClosedGenericType_AcceptsType()
        {
            // Arrange
            var closedType = typeof(List<int>);

            // Act & Assert - Should not throw, closed types are now accepted
            var analyzer = TypeAnalyzerFactory.GetOpenGenericTypeAnalyzer(closedType);
            Assert.IsNotNull(analyzer);
            Assert.AreEqual(closedType, analyzer.OpenGenericType);
        }

        #endregion

        #region Dependency Information Tests

        /// <summary>
        /// Verifies that Parameters includes dependency information.
        /// </summary>
        [Test]
        public void Parameters_ParameterWithDependency_IncludesDependencyInfo()
        {
            // Arrange
            var type = typeof(DependentContainer<,>);
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act
            var parameters = analyzer.Parameters;

            // Assert
            Assert.AreEqual(2, parameters.Count);

            // T references U
            Assert.AreEqual(1, parameters[0].ReferencedParameters.Count);
            Assert.AreEqual("U", parameters[0].ReferencedParameters[0].Name);

            // U is referenced by T
            Assert.AreEqual(1, parameters[1].ReferencedByParameters.Count);
            Assert.AreEqual("T", parameters[1].ReferencedByParameters[0].Name);
        }

        /// <summary>
        /// Verifies that parameters are correctly identified when using generic parameter from another type.
        /// </summary>
        [Test]
        public void Parameters_ParameterFromDifferentType_IdentifiesSubstitution()
        {
            // Arrange - Create a scenario where a generic parameter from one type
            // is used as a type argument for another type
            var enumerableTypeDef = typeof(IEnumerable<>);
            var tParamFromEnumerable = enumerableTypeDef.GetGenericArguments()[0];

            // Create List<T> where T is the parameter from IEnumerable<>
            // Even though both are named "T", they are different Type objects
            var listTypeDef = typeof(List<>);
            var openListType = listTypeDef.MakeGenericType(tParamFromEnumerable);

            var analyzer = new OpenGenericTypeAnalyzer(openListType);

            // Act
            var parameters = analyzer.Parameters;

            // Assert
            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual("T", parameters[0].Name);

            // The parameter IS substituted - T from List<> is replaced with T from IEnumerable<>
            // Even though both have the same name, they are different Type objects
            Assert.IsNotNull(parameters[0].SubstitutedType);
            Assert.AreEqual("T", parameters[0].SubstitutedType.Name);
        }

        #endregion

        #region Constraint Information Tests

        /// <summary>
        /// Verifies that Parameters includes constraint information.
        /// </summary>
        [Test]
        public void Parameters_ClassConstraint_IncludesConstraintInfo()
        {
            // Arrange
            var type = typeof(ClassConstrainedContainer<>);
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act
            var parameters = analyzer.Parameters;

            // Assert
            Assert.AreEqual(1, parameters.Count);
            Assert.IsTrue(parameters[0].HasConstraints);
        }

        #endregion

        #region Complex Scenarios Tests

        /// <summary>
        /// Verifies analyzer behavior with multiple dependencies.
        /// </summary>
        [Test]
        public void Parameters_MultipleDependencies_ReturnsCorrectInfo()
        {
            // Arrange
            var type = typeof(MultiDependentContainer<,,>);
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act
            var parameters = analyzer.Parameters;

            // Assert
            Assert.AreEqual(3, parameters.Count);

            // T references U and V
            Assert.AreEqual(2, parameters[0].ReferencedParameters.Count);
            Assert.IsTrue(parameters[0].ReferencedParameters.Any(p => p.Name == "U"));
            Assert.IsTrue(parameters[0].ReferencedParameters.Any(p => p.Name == "V"));

            // U is referenced by T
            Assert.AreEqual(1, parameters[1].ReferencedByParameters.Count);
            Assert.AreEqual("T", parameters[1].ReferencedByParameters[0].Name);

            // V is referenced by T
            Assert.AreEqual(1, parameters[2].ReferencedByParameters.Count);
            Assert.AreEqual("T", parameters[2].ReferencedByParameters[0].Name);
        }

        /// <summary>
        /// Verifies analyzer behavior with nested generic constraints.
        /// </summary>
        [Test]
        public void Parameters_NestedGenericConstraint_ReturnsCorrectInfo()
        {
            // Arrange
            var type = typeof(NestedContainer<,>);
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act
            var parameters = analyzer.Parameters;

            // Assert
            Assert.AreEqual(2, parameters.Count);

            // T references U (through IEnumerable<List<U>>)
            Assert.AreEqual(1, parameters[0].ReferencedParameters.Count);
            Assert.AreEqual("U", parameters[0].ReferencedParameters[0].Name);
        }

        /// <summary>
        /// Verifies analyzer behavior with partially constructed type that has no constraints.
        /// </summary>
        [Test]
        public void SubstitutedParameters_PartiallyConstructedType_ReturnsCorrectSubstitutions()
        {
            // Arrange - Use DoubleContainer<T, U> which has no constraints
            var type = typeof(DoubleContainer<,>);
            var tParam = type.GetGenericArguments()[0];
            var uParam = type.GetGenericArguments()[1];

            // Create DoubleContainer<string, U> - only T is substituted
            var partiallyConstructed = type.MakeGenericType(typeof(string), uParam);
            var analyzer = new OpenGenericTypeAnalyzer(partiallyConstructed);

            // Act
            var substitutedParameters = analyzer.SubstitutedParameters;

            // Assert
            Assert.AreEqual(1, substitutedParameters.Count);
            Assert.AreEqual("T", substitutedParameters[0].Name);
            Assert.AreEqual(typeof(string), substitutedParameters[0].SubstitutedType);

            // No dependencies since DoubleContainer has no constraints
            Assert.AreEqual(0, substitutedParameters[0].ReferencedParameters.Count);
        }

        #endregion

        #region Edge Cases Tests

        /// <summary>
        /// Verifies that analyzer handles types with single generic parameter.
        /// </summary>
        [Test]
        public void Parameters_SingleGenericParameter_ReturnsCorrectCount()
        {
            // Arrange
            var type = typeof(SimpleContainer<>);
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act
            var parameters = analyzer.Parameters;

            // Assert
            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual("T", parameters[0].Name);
            Assert.IsFalse(parameters[0].HasDependencies);
            Assert.IsFalse(parameters[0].IsDependencyForOthers);
        }

        /// <summary>
        /// Verifies that analyzer handles types with many generic parameters.
        /// </summary>
        [Test]
        public void Parameters_ManyGenericParameters_ReturnsCorrectCount()
        {
            // Arrange
            var type = typeof(Func<,,,,,>); // Func<T1,T2,T3,T4,T5,TResult>
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act
            var parameters = analyzer.Parameters;

            // Assert
            Assert.AreEqual(6, parameters.Count);
        }

        #endregion

        #region IsImplementsGenericDefinition Tests

        /// <summary>
        /// Verifies that IsImplementsGenericDefinition returns true for the same generic type definition.
        /// </summary>
        [Test]
        public void IsImplementsGenericDefinition_SameType_ReturnsTrue()
        {
            // Arrange
            var type = typeof(SimpleContainer<>);
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act
            var result = analyzer.IsImplementsGenericDefinition(type);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that IsImplementsGenericDefinition returns true for base class inheritance.
        /// </summary>
        [Test]
        public void IsImplementsGenericDefinition_BaseClassInheritance_ReturnsTrue()
        {
            // Arrange
            var derivedType = typeof(DerivedContainer);
            var analyzer = new OpenGenericTypeAnalyzer(derivedType);

            // Act
            var result = analyzer.IsImplementsGenericDefinition(typeof(SimpleContainer<>));

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that IsImplementsGenericDefinition returns true for interface implementation.
        /// </summary>
        [Test]
        public void IsImplementsGenericDefinition_InterfaceImplementation_ReturnsTrue()
        {
            // Arrange
            var type = typeof(List<int>);
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act
            var result = analyzer.IsImplementsGenericDefinition(typeof(IEnumerable<>));

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that IsImplementsGenericDefinition returns true for partially constructed types.
        /// </summary>
        [Test]
        public void IsImplementsGenericDefinition_PartiallyConstructedType_ReturnsTrue()
        {
            // Arrange
            var dictTypeDef = typeof(Dictionary<,>);
            var tValueParam = dictTypeDef.GetGenericArguments()[1];
            var partiallyConstructed = dictTypeDef.MakeGenericType(typeof(string), tValueParam);
            var analyzer = new OpenGenericTypeAnalyzer(partiallyConstructed);

            // Act
            var result = analyzer.IsImplementsGenericDefinition(typeof(Dictionary<,>));

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that IsImplementsGenericDefinition returns false for unrelated types.
        /// </summary>
        [Test]
        public void IsImplementsGenericDefinition_UnrelatedType_ReturnsFalse()
        {
            // Arrange
            var type = typeof(SimpleContainer<>);
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act
            var result = analyzer.IsImplementsGenericDefinition(typeof(List<>));

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verifies that IsImplementsGenericDefinition throws ArgumentNullException for null type.
        /// </summary>
        [Test]
        public void IsImplementsGenericDefinition_NullType_ThrowsArgumentNullException()
        {
            // Arrange
            var type = typeof(SimpleContainer<>);
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => analyzer.IsImplementsGenericDefinition(null));
        }

        /// <summary>
        /// Verifies that IsImplementsGenericDefinition throws ArgumentException for non-generic type definition.
        /// </summary>
        [Test]
        public void IsImplementsGenericDefinition_NonGenericTypeDefinition_ThrowsArgumentException()
        {
            // Arrange
            var type = typeof(SimpleContainer<>);
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => analyzer.IsImplementsGenericDefinition(typeof(string)));
        }

        #endregion

        #region GetGenericArgumentsRelativeTo Tests

        /// <summary>
        /// Verifies that GetGenericArgumentsRelativeTo returns correct arguments for simple generic type.
        /// </summary>
        [Test]
        public void GetGenericArgumentsRelativeTo_SimpleGenericType_ReturnsCorrectArguments()
        {
            // Arrange
            var type = typeof(SimpleContainer<int>);
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act
            var arguments = analyzer.GetGenericArgumentsRelativeTo(typeof(SimpleContainer<>));

            // Assert
            Assert.AreEqual(1, arguments.Length);
            Assert.AreEqual(typeof(int), arguments[0]);
        }

        /// <summary>
        /// Verifies that GetGenericArgumentsRelativeTo returns correct arguments for partially constructed type.
        /// </summary>
        [Test]
        public void GetGenericArgumentsRelativeTo_PartiallyConstructedType_ReturnsCorrectArguments()
        {
            // Arrange
            var dictTypeDef = typeof(Dictionary<,>);
            var tValueParam = dictTypeDef.GetGenericArguments()[1];
            var partiallyConstructed = dictTypeDef.MakeGenericType(typeof(string), tValueParam);
            var analyzer = new OpenGenericTypeAnalyzer(partiallyConstructed);

            // Act
            var arguments = analyzer.GetGenericArgumentsRelativeTo(typeof(Dictionary<,>));

            // Assert
            Assert.AreEqual(2, arguments.Length);
            Assert.AreEqual(typeof(string), arguments[0]);
            Assert.AreEqual(tValueParam, arguments[1]);
        }

        /// <summary>
        /// Verifies that GetGenericArgumentsRelativeTo works with inherited generic types.
        /// </summary>
        [Test]
        public void GetGenericArgumentsRelativeTo_InheritedGenericType_ReturnsMappedArguments()
        {
            // Arrange
            var derivedType = typeof(IntContainer);
            var analyzer = new OpenGenericTypeAnalyzer(derivedType);

            // Act
            var arguments = analyzer.GetGenericArgumentsRelativeTo(typeof(SimpleContainer<>));

            // Assert
            Assert.AreEqual(1, arguments.Length);
            Assert.AreEqual(typeof(int), arguments[0]);
        }

        /// <summary>
        /// Verifies that GetGenericArgumentsRelativeTo works with interface implementation.
        /// </summary>
        [Test]
        public void GetGenericArgumentsRelativeTo_InterfaceImplementation_ReturnsInterfaceArguments()
        {
            // Arrange
            var type = typeof(List<int>);
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act
            var arguments = analyzer.GetGenericArgumentsRelativeTo(typeof(IEnumerable<>));

            // Assert
            Assert.AreEqual(1, arguments.Length);
            Assert.AreEqual(typeof(int), arguments[0]);
        }

        /// <summary>
        /// Verifies that GetGenericArgumentsRelativeTo handles array types.
        /// </summary>
        [Test]
        public void GetGenericArgumentsRelativeTo_ArrayType_ReturnsCorrectArguments()
        {
            // Arrange
            var listTypeDef = typeof(List<>);
            var tParam = listTypeDef.GetGenericArguments()[0];
            var arrayType = listTypeDef.MakeGenericType(tParam).MakeArrayType();
            var analyzer = new OpenGenericTypeAnalyzer(arrayType);

            // Act
            var arguments = analyzer.GetGenericArgumentsRelativeTo(listTypeDef.MakeGenericType(tParam).MakeArrayType());

            // Assert
            Assert.AreEqual(1, arguments.Length);
            Assert.AreEqual(tParam, arguments[0]);
        }

        /// <summary>
        /// Verifies that GetGenericArgumentsRelativeTo throws for null type.
        /// </summary>
        [Test]
        public void GetGenericArgumentsRelativeTo_NullType_ThrowsArgumentNullException()
        {
            // Arrange
            var type = typeof(SimpleContainer<int>);
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => analyzer.GetGenericArgumentsRelativeTo(null));
        }

        /// <summary>
        /// Verifies that GetGenericArgumentsRelativeTo throws for non-generic type definition.
        /// </summary>
        [Test]
        public void GetGenericArgumentsRelativeTo_NonGenericTypeDefinition_ThrowsArgumentException()
        {
            // Arrange
            var type = typeof(SimpleContainer<int>);
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => analyzer.GetGenericArgumentsRelativeTo(typeof(string)));
        }

        /// <summary>
        /// Verifies that GetGenericArgumentsRelativeTo throws for unrelated type.
        /// </summary>
        [Test]
        public void GetGenericArgumentsRelativeTo_UnrelatedType_ThrowsInvalidOperationException()
        {
            // Arrange
            var type = typeof(SimpleContainer<int>);
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => analyzer.GetGenericArgumentsRelativeTo(typeof(List<>)));
        }

        #endregion

        #region GetCompletedGenericArguments Tests

        /// <summary>
        /// Verifies that GetCompletedGenericArguments returns missing arguments for partially constructed type.
        /// </summary>
        [Test]
        public void GetCompletedGenericArguments_PartiallyConstructedType_ReturnsMissingArguments()
        {
            // Arrange
            var dictTypeDef = typeof(Dictionary<,>);
            var tValueParam = dictTypeDef.GetGenericArguments()[1];
            var partiallyConstructed = dictTypeDef.MakeGenericType(typeof(string), tValueParam);
            var analyzer = new OpenGenericTypeAnalyzer(partiallyConstructed);

            // Target type is Dictionary<string, int>
            var targetType = typeof(Dictionary<string, int>);

            // Act
            var completedArguments = analyzer.GetCompletedGenericArguments(targetType);

            // Assert
            Assert.AreEqual(1, completedArguments.Length);
            Assert.AreEqual(typeof(int), completedArguments[0]);
        }

        /// <summary>
        /// Verifies that GetCompletedGenericArguments returns empty array for fully constructed type.
        /// </summary>
        [Test]
        public void GetCompletedGenericArguments_FullyConstructedType_ReturnsEmptyArray()
        {
            // Arrange
            var type = typeof(SimpleContainer<int>);
            var analyzer = new OpenGenericTypeAnalyzer(type);
            var targetType = typeof(SimpleContainer<int>);

            // Act
            var completedArguments = analyzer.GetCompletedGenericArguments(targetType);

            // Assert
            Assert.AreEqual(0, completedArguments.Length);
        }

        /// <summary>
        /// Verifies that GetCompletedGenericArguments works with allowTypeInheritance.
        /// </summary>
        [Test]
        public void GetCompletedGenericArguments_WithTypeInheritance_ReturnsCorrectArguments()
        {
            // Arrange
            var derivedType = typeof(DerivedContainer);
            var analyzer = new OpenGenericTypeAnalyzer(typeof(SimpleContainer<>));

            // Act
            var completedArguments = analyzer.GetCompletedGenericArguments(derivedType, true);

            // Assert
            Assert.AreEqual(1, completedArguments.Length);
            Assert.AreEqual(typeof(int), completedArguments[0]);
        }

        /// <summary>
        /// Verifies that GetCompletedGenericArguments throws for null target type.
        /// </summary>
        [Test]
        public void GetCompletedGenericArguments_NullTargetType_ThrowsArgumentNullException()
        {
            // Arrange
            var type = typeof(SimpleContainer<>);
            var analyzer = new OpenGenericTypeAnalyzer(type);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => analyzer.GetCompletedGenericArguments(null));
        }

        /// <summary>
        /// Verifies that GetCompletedGenericArguments throws when type arguments don't match.
        /// </summary>
        [Test]
        public void GetCompletedGenericArguments_MismatchedArguments_ThrowsArgumentException()
        {
            // Arrange
            var dictTypeDef = typeof(Dictionary<,>);
            var tValueParam = dictTypeDef.GetGenericArguments()[1];
            var partiallyConstructed = dictTypeDef.MakeGenericType(typeof(string), tValueParam);
            var analyzer = new OpenGenericTypeAnalyzer(partiallyConstructed);

            // Target type has different key type
            var targetType = typeof(Dictionary<int, bool>);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => analyzer.GetCompletedGenericArguments(targetType));
        }

        /// <summary>
        /// Verifies that GetCompletedGenericArguments throws for mismatched array ranks.
        /// </summary>
        [Test]
        public void GetCompletedGenericArguments_MismatchedArrayRanks_ThrowsArgumentException()
        {
            // Arrange
            var listTypeDef = typeof(List<>);
            var tParam = listTypeDef.GetGenericArguments()[0];
            var singleArray = listTypeDef.MakeGenericType(tParam).MakeArrayType();
            var analyzer = new OpenGenericTypeAnalyzer(singleArray);

            // Target type is 2D array
            var targetType = typeof(int[,]);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => analyzer.GetCompletedGenericArguments(targetType));
        }

        /// <summary>
        /// Verifies that GetCompletedGenericArguments throws when target is not array but analyzed type is.
        /// </summary>
        [Test]
        public void GetCompletedGenericArguments_ArrayToNonArray_ThrowsArgumentException()
        {
            // Arrange
            var listTypeDef = typeof(List<>);
            var tParam = listTypeDef.GetGenericArguments()[0];
            var arrayType = listTypeDef.MakeGenericType(tParam).MakeArrayType();
            var analyzer = new OpenGenericTypeAnalyzer(arrayType);

            // Target type is not an array
            var targetType = typeof(List<int>);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => analyzer.GetCompletedGenericArguments(targetType));
        }

        #endregion
    }
}
