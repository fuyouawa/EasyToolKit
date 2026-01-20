using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Core.Reflection.Implementations;

namespace Tests.Core.TypeAnalyzing
{
    /// <summary>
    /// Unit tests for GenericParameterAnalyzer.
    /// </summary>
    public class TestGenericParameterAnalyzer
    {
        #region Constructor

        /// <summary>
        /// Verifies that constructor creates analyzer with correct properties for simple generic parameter.
        /// </summary>
        [Test]
        public void Constructor_SimpleGenericParameter_SetsPropertiesCorrectly()
        {
            // Arrange
            var genericParameter = typeof(SimpleGenericClass<>).GetGenericArguments()[0];

            // Act
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Assert
            Assert.AreEqual(genericParameter, analyzer.ParameterType);
            Assert.AreEqual("T", analyzer.Name);
            Assert.AreEqual(0, analyzer.Position);
        }

        /// <summary>
        /// Verifies that constructor throws ArgumentNullException when generic parameter type is null.
        /// </summary>
        [Test]
        public void Constructor_NullType_ThrowsArgumentNullException()
        {
            // Arrange
            Type nullType = null;

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new GenericParameterAnalyzer(nullType));
            Assert.That(ex.Message, Does.Contain("Generic parameter type cannot be null"));
        }

        /// <summary>
        /// Verifies that constructor throws ArgumentException when type is not a generic parameter.
        /// </summary>
        [Test]
        public void Constructor_NonGenericParameter_ThrowsArgumentException()
        {
            // Arrange
            var nonGenericParameter = typeof(string);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new GenericParameterAnalyzer(nonGenericParameter));
            Assert.That(ex.Message, Does.Contain("must be a generic parameter"));
        }

        /// <summary>
        /// Verifies that constructor throws ArgumentException when type is a method generic parameter.
        /// </summary>
        [Test]
        public void Constructor_MethodGenericParameter_ThrowsArgumentException()
        {
            // Arrange
            var method = typeof(TestClassWithMethodGeneric).GetMethod(nameof(TestClassWithMethodGeneric.GenericMethod));
            var methodGenericParameter = method.GetGenericArguments()[0];

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new GenericParameterAnalyzer(methodGenericParameter));
            Assert.That(ex.Message, Does.Contain("must be a type generic parameter"));
        }

        #endregion

        #region Basic Properties

        /// <summary>
        /// Verifies that Name returns correct name for multiple generic parameters.
        /// </summary>
        [Test]
        public void Name_MultipleGenericParameters_ReturnsCorrectNames()
        {
            // Arrange
            var genericParameters = typeof(DoubleGenericClass<,>).GetGenericArguments();

            // Act
            var analyzer1 = new GenericParameterAnalyzer(genericParameters[0]);
            var analyzer2 = new GenericParameterAnalyzer(genericParameters[1]);

            // Assert
            Assert.AreEqual("T", analyzer1.Name);
            Assert.AreEqual("U", analyzer2.Name);
        }

        /// <summary>
        /// Verifies that Position returns correct position for multiple generic parameters.
        /// </summary>
        [Test]
        public void Position_MultipleGenericParameters_ReturnsCorrectPositions()
        {
            // Arrange
            var genericParameters = typeof(TripleGenericClass<,,>).GetGenericArguments();

            // Act
            var analyzer1 = new GenericParameterAnalyzer(genericParameters[0]);
            var analyzer2 = new GenericParameterAnalyzer(genericParameters[1]);
            var analyzer3 = new GenericParameterAnalyzer(genericParameters[2]);

            // Assert
            Assert.AreEqual(0, analyzer1.Position);
            Assert.AreEqual(1, analyzer2.Position);
            Assert.AreEqual(2, analyzer3.Position);
        }

        #endregion

        #region Special Constraints

        /// <summary>
        /// Verifies that SpecialConstraints is None when no constraints are applied.
        /// </summary>
        [Test]
        public void SpecialConstraints_NoConstraints_ReturnsNone()
        {
            // Arrange
            var genericParameter = typeof(SimpleGenericClass<>).GetGenericArguments()[0];

            // Act
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Assert
            Assert.AreEqual(GenericParameterSpecialConstraints.None, analyzer.SpecialConstraints);
        }

        /// <summary>
        /// Verifies that SpecialConstraints contains ReferenceType when class constraint is applied.
        /// </summary>
        [Test]
        public void SpecialConstraints_ClassConstraint_ReturnsReferenceType()
        {
            // Arrange
            var genericParameter = typeof(ClassConstrainedClass<>).GetGenericArguments()[0];

            // Act
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Assert
            Assert.AreEqual(GenericParameterSpecialConstraints.ReferenceType, analyzer.SpecialConstraints);
        }

        /// <summary>
        /// Verifies that SpecialConstraints contains ValueType and DefaultConstructor when struct constraint is applied.
        /// </summary>
        /// <remarks>
        /// In C#, struct constraint implicitly includes default constructor constraint,
        /// as all value types have parameterless constructors.
        /// </remarks>
        [Test]
        public void SpecialConstraints_StructConstraint_ReturnsValueTypeAndDefaultConstructor()
        {
            // Arrange
            var genericParameter = typeof(StructConstrainedClass<>).GetGenericArguments()[0];

            // Act
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Assert
            var expected = GenericParameterSpecialConstraints.ValueType | GenericParameterSpecialConstraints.DefaultConstructor;
            Assert.AreEqual(expected, analyzer.SpecialConstraints);
        }

        /// <summary>
        /// Verifies that SpecialConstraints contains DefaultConstructor when new() constraint is applied.
        /// </summary>
        [Test]
        public void SpecialConstraints_NewConstraint_ReturnsDefaultConstructor()
        {
            // Arrange
            var genericParameter = typeof(NewConstrainedClass<>).GetGenericArguments()[0];

            // Act
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Assert
            Assert.AreEqual(GenericParameterSpecialConstraints.DefaultConstructor, analyzer.SpecialConstraints);
        }

        /// <summary>
        /// Verifies that SpecialConstraints contains multiple constraints when combined.
        /// </summary>
        [Test]
        public void SpecialConstraints_ClassAndNewConstraints_ReturnsCombinedFlags()
        {
            // Arrange
            var genericParameter = typeof(ClassAndNewConstrainedClass<>).GetGenericArguments()[0];

            // Act
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Assert
            var expected = GenericParameterSpecialConstraints.ReferenceType | GenericParameterSpecialConstraints.DefaultConstructor;
            Assert.AreEqual(expected, analyzer.SpecialConstraints);
        }

        #endregion

        #region Type Constraints

        /// <summary>
        /// Verifies that TypeConstraints is empty when no type constraints are applied.
        /// </summary>
        [Test]
        public void TypeConstraints_NoTypeConstraints_ReturnsEmptyList()
        {
            // Arrange
            var genericParameter = typeof(SimpleGenericClass<>).GetGenericArguments()[0];

            // Act
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Assert
            Assert.AreEqual(0, analyzer.TypeConstraints.Count);
        }

        /// <summary>
        /// Verifies that TypeConstraints contains base class constraint.
        /// </summary>
        [Test]
        public void TypeConstraints_BaseClassConstraint_ReturnsBaseClass()
        {
            // Arrange
            var genericParameter = typeof(BaseClassConstrainedClass<>).GetGenericArguments()[0];

            // Act
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Assert
            Assert.AreEqual(1, analyzer.TypeConstraints.Count);
            Assert.IsTrue(analyzer.TypeConstraints.Contains(typeof(TestBaseClass)));
        }

        /// <summary>
        /// Verifies that TypeConstraints contains interface constraint.
        /// </summary>
        [Test]
        public void TypeConstraints_InterfaceConstraint_ReturnsInterface()
        {
            // Arrange
            var genericParameter = typeof(InterfaceConstrainedClass<>).GetGenericArguments()[0];

            // Act
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Assert
            Assert.AreEqual(1, analyzer.TypeConstraints.Count);
            Assert.IsTrue(analyzer.TypeConstraints.Contains(typeof(ITestInterface)));
        }

        /// <summary>
        /// Verifies that TypeConstraints contains multiple interface constraints.
        /// </summary>
        [Test]
        public void TypeConstraints_MultipleInterfaceConstraints_ReturnsAllInterfaces()
        {
            // Arrange
            var genericParameter = typeof(MultipleInterfaceConstrainedClass<>).GetGenericArguments()[0];

            // Act
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Assert
            Assert.AreEqual(2, analyzer.TypeConstraints.Count);
            Assert.IsTrue(analyzer.TypeConstraints.Contains(typeof(ITestInterface)));
            Assert.IsTrue(analyzer.TypeConstraints.Contains(typeof(IAnotherInterface)));
        }

        #endregion

        #region DependsOn

        /// <summary>
        /// Verifies that DependsOn is empty when parameter has no dependencies.
        /// </summary>
        [Test]
        public void DependsOn_NoDependencies_ReturnsEmptyList()
        {
            // Arrange
            var genericParameter = typeof(SimpleGenericClass<>).GetGenericArguments()[0];

            // Act
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Assert
            Assert.AreEqual(0, analyzer.DependsOn.Count);
        }

        /// <summary>
        /// Verifies that DependsOn correctly identifies dependency when constraint uses another generic parameter.
        /// </summary>
        [Test]
        public void DependsOn_ConstraintUsesGenericParameter_ReturnsDependency()
        {
            // Arrange
            var genericParameters = typeof(DependentGenericClass<,>).GetGenericArguments();
            var tParameter = genericParameters[0];

            // Act
            var analyzer = new GenericParameterAnalyzer(tParameter);

            // Assert
            Assert.AreEqual(1, analyzer.DependsOn.Count);
            Assert.AreEqual(genericParameters[1], analyzer.DependsOn[0]);
        }

        /// <summary>
        /// Verifies that DependsOn correctly identifies multiple dependencies.
        /// </summary>
        [Test]
        public void DependsOn_MultipleDependencies_ReturnsAllDependencies()
        {
            // Arrange
            var genericParameters = typeof(MultiDependentGenericClass<,,>).GetGenericArguments();
            var tParameter = genericParameters[0];

            // Act
            var analyzer = new GenericParameterAnalyzer(tParameter);

            // Assert
            Assert.AreEqual(2, analyzer.DependsOn.Count);
            Assert.IsTrue(analyzer.DependsOn.Contains(genericParameters[1]));
            Assert.IsTrue(analyzer.DependsOn.Contains(genericParameters[2]));
        }

        /// <summary>
        /// Verifies that DependsOn correctly handles nested generic type constraints.
        /// </summary>
        [Test]
        public void DependsOn_NestedGenericConstraint_ReturnsDependency()
        {
            // Arrange
            var genericParameters = typeof(NestedDependentGenericClass<,>).GetGenericArguments();
            var tParameter = genericParameters[0];

            // Act
            var analyzer = new GenericParameterAnalyzer(tParameter);

            // Assert
            Assert.AreEqual(1, analyzer.DependsOn.Count);
            Assert.AreEqual(genericParameters[1], analyzer.DependsOn[0]);
        }

        #endregion

        #region DependedOnBy

        /// <summary>
        /// Verifies that DependedOnBy is empty when no other parameters depend on this parameter.
        /// </summary>
        [Test]
        public void DependedOnBy_NoDependents_ReturnsEmptyList()
        {
            // Arrange
            var genericParameters = typeof(DoubleGenericClass<,>).GetGenericArguments();
            var uParameter = genericParameters[1];

            // Act
            var analyzer = new GenericParameterAnalyzer(uParameter);

            // Assert
            Assert.AreEqual(0, analyzer.DependedOnBy.Count);
        }

        /// <summary>
        /// Verifies that DependedOnBy correctly identifies parameters that depend on this parameter.
        /// </summary>
        [Test]
        public void DependedOnBy_ParameterIsDependency_ReturnsDependent()
        {
            // Arrange
            var genericParameters = typeof(DependentGenericClass<,>).GetGenericArguments();
            var uParameter = genericParameters[1];

            // Act
            var analyzer = new GenericParameterAnalyzer(uParameter);

            // Assert
            // T depends on U (T : List<U>), so U is depended on by T
            Assert.AreEqual(1, analyzer.DependedOnBy.Count);
            Assert.AreEqual(genericParameters[0], analyzer.DependedOnBy[0]);
        }

        #endregion

        #region ParameterInfo

        /// <summary>
        /// Verifies that ParameterInfo creates complete GenericParameterInfo object.
        /// </summary>
        [Test]
        public void ParameterInfo_SimpleParameter_ReturnsCompleteInfo()
        {
            // Arrange
            var genericParameter = typeof(SimpleGenericClass<>).GetGenericArguments()[0];
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Act
            var info = analyzer.ParameterInfo;

            // Assert
            Assert.IsNotNull(info);
            Assert.AreEqual("T", info.Name);
            Assert.AreEqual(0, info.Position);
            Assert.AreEqual(GenericParameterSpecialConstraints.None, info.SpecialConstraints);
            Assert.AreEqual(0, info.TypeConstraints.Count);
            Assert.AreEqual(0, info.DependsOnParameters.Count);
            Assert.AreEqual(0, info.DependedOnByParameters.Count);
        }

        /// <summary>
        /// Verifies that ParameterInfo includes constraint information.
        /// </summary>
        [Test]
        public void ParameterInfo_ConstrainedParameter_ReturnsInfoWithConstraints()
        {
            // Arrange
            var genericParameter = typeof(BaseClassConstrainedClass<>).GetGenericArguments()[0];
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Act
            var info = analyzer.ParameterInfo;

            // Assert
            Assert.IsNotNull(info);
            Assert.AreEqual(1, info.TypeConstraints.Count);
            Assert.AreEqual(typeof(TestBaseClass), info.TypeConstraints[0]);
            Assert.IsTrue(info.HasConstraints);
        }

        /// <summary>
        /// Verifies that ParameterInfo includes dependency information.
        /// </summary>
        [Test]
        public void ParameterInfo_ParameterWithDependencies_ReturnsInfoWithDependencies()
        {
            // Arrange
            var genericParameters = typeof(DependentGenericClass<,>).GetGenericArguments();
            var tParameter = genericParameters[0];
            var analyzer = new GenericParameterAnalyzer(tParameter);

            // Act
            var info = analyzer.ParameterInfo;

            // Assert
            Assert.IsNotNull(info);
            Assert.AreEqual(1, info.DependsOnParameters.Count);
            Assert.AreEqual(genericParameters[1], info.DependsOnParameters[0]);
            Assert.IsTrue(info.HasDependencies);
        }

        #endregion

        #region SatisfiesConstraints

        /// <summary>
        /// Verifies that SatisfiesConstraints returns true when no constraints are applied.
        /// </summary>
        [Test]
        public void SatisfiesConstraints_NoConstraints_ReturnsTrue()
        {
            // Arrange
            var genericParameter = typeof(SimpleGenericClass<>).GetGenericArguments()[0];
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Act
            var result = analyzer.SatisfiesConstraints(typeof(string));

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that SatisfiesConstraints returns false when targetType is null.
        /// </summary>
        [Test]
        public void SatisfiesConstraints_NullTargetType_ReturnsFalse()
        {
            // Arrange
            var genericParameter = typeof(SimpleGenericClass<>).GetGenericArguments()[0];
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Act
            var result = analyzer.SatisfiesConstraints(null);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verifies that SatisfiesConstraints checks class constraint correctly.
        /// </summary>
        [Test]
        public void SatisfiesConstraints_ClassConstraint_ReferenceTypeSatisfies()
        {
            // Arrange
            var genericParameter = typeof(ClassConstrainedClass<>).GetGenericArguments()[0];
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Act
            var stringResult = analyzer.SatisfiesConstraints(typeof(string));
            var intResult = analyzer.SatisfiesConstraints(typeof(int));

            // Assert
            Assert.IsTrue(stringResult, "Reference type should satisfy class constraint");
            Assert.IsFalse(intResult, "Value type should not satisfy class constraint");
        }

        /// <summary>
        /// Verifies that SatisfiesConstraints checks struct constraint correctly.
        /// </summary>
        [Test]
        public void SatisfiesConstraints_StructConstraint_ValueTypeSatisfies()
        {
            // Arrange
            var genericParameter = typeof(StructConstrainedClass<>).GetGenericArguments()[0];
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Act
            var intResult = analyzer.SatisfiesConstraints(typeof(int));
            var stringResult = analyzer.SatisfiesConstraints(typeof(string));

            // Assert
            Assert.IsTrue(intResult, "Value type should satisfy struct constraint");
            Assert.IsFalse(stringResult, "Reference type should not satisfy struct constraint");
        }

        /// <summary>
        /// Verifies that SatisfiesConstraints checks new() constraint correctly.
        /// </summary>
        [Test]
        public void SatisfiesConstraints_NewConstraint_TypeWithDefaultConstructorSatisfies()
        {
            // Arrange
            var genericParameter = typeof(NewConstrainedClass<>).GetGenericArguments()[0];
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Act
            var listResult = analyzer.SatisfiesConstraints(typeof(List<string>));
            var stringResult = analyzer.SatisfiesConstraints(typeof(string));

            // Assert
            Assert.IsTrue(listResult, "Type with default constructor should satisfy new() constraint");
            Assert.IsFalse(stringResult, "Type without default constructor should not satisfy new() constraint");
        }

        /// <summary>
        /// Verifies that SatisfiesConstraints checks base class constraint correctly.
        /// </summary>
        [Test]
        public void SatisfiesConstraints_BaseClassConstraint_DerivedTypeSatisfies()
        {
            // Arrange
            var genericParameter = typeof(BaseClassConstrainedClass<>).GetGenericArguments()[0];
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Act
            var derivedResult = analyzer.SatisfiesConstraints(typeof(TestDerivedClass));
            var unrelatedResult = analyzer.SatisfiesConstraints(typeof(string));

            // Assert
            Assert.IsTrue(derivedResult, "Derived type should satisfy base class constraint");
            Assert.IsFalse(unrelatedResult, "Unrelated type should not satisfy base class constraint");
        }

        /// <summary>
        /// Verifies that SatisfiesConstraints checks interface constraint correctly.
        /// </summary>
        [Test]
        public void SatisfiesConstraints_InterfaceConstraint_ImplementingTypeSatisfies()
        {
            // Arrange
            var genericParameter = typeof(InterfaceConstrainedClass<>).GetGenericArguments()[0];
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Act
            var implResult = analyzer.SatisfiesConstraints(typeof(TestInterfaceImplementation));
            var nonImplResult = analyzer.SatisfiesConstraints(typeof(string));

            // Assert
            Assert.IsTrue(implResult, "Type implementing interface should satisfy interface constraint");
            Assert.IsFalse(nonImplResult, "Type not implementing interface should not satisfy interface constraint");
        }

        /// <summary>
        /// Verifies that SatisfiesConstraints checks multiple interface constraints correctly.
        /// </summary>
        [Test]
        public void SatisfiesConstraints_MultipleInterfaceConstraints_TypeImplementingAllSatisfies()
        {
            // Arrange
            var genericParameter = typeof(MultipleInterfaceConstrainedClass<>).GetGenericArguments()[0];
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Act
            var bothResult = analyzer.SatisfiesConstraints(typeof(TestMultiInterfaceImplementation));
            var singleResult = analyzer.SatisfiesConstraints(typeof(TestInterfaceImplementation));

            // Assert
            Assert.IsTrue(bothResult, "Type implementing all interfaces should satisfy all constraints");
            Assert.IsFalse(singleResult, "Type implementing only some interfaces should not satisfy all constraints");
        }

        #endregion

        #region TryInferTypeFrom

        /// <summary>
        /// Verifies that TryInferTypeFrom returns false when dependentParameter is null.
        /// </summary>
        [Test]
        public void TryInferTypeFrom_NullDependentParameter_ReturnsFalse()
        {
            // Arrange
            var genericParameter = typeof(SimpleGenericClass<>).GetGenericArguments()[0];
            var analyzer = new GenericParameterAnalyzer(genericParameter);

            // Act
            var result = analyzer.TryInferTypeFrom(null, typeof(List<int>), out var inferredType);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(inferredType);
        }

        /// <summary>
        /// Verifies that TryInferTypeFrom returns false when dependentParameterType is null.
        /// </summary>
        [Test]
        public void TryInferTypeFrom_NullDependentParameterType_ReturnsFalse()
        {
            // Arrange
            var genericParameters = typeof(DependentGenericClass<,>).GetGenericArguments();
            var uParameter = genericParameters[1];
            var tParameter = genericParameters[0];
            var analyzer = new GenericParameterAnalyzer(uParameter);

            // Act
            var result = analyzer.TryInferTypeFrom(tParameter, null, out var inferredType);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(inferredType);
        }

        /// <summary>
        /// Verifies that TryInferTypeFrom returns false when dependentParameter is not in DependedOnBy.
        /// </summary>
        [Test]
        public void TryInferTypeFrom_DependentParameterNotInDependedOnBy_ReturnsFalse()
        {
            // Arrange
            var genericParameters = typeof(DoubleGenericClass<,>).GetGenericArguments();
            var tParameter = genericParameters[0];
            var uParameter = genericParameters[1];
            var analyzer = new GenericParameterAnalyzer(tParameter);

            // Act
            // T doesn't depend on U, so inference should fail
            var result = analyzer.TryInferTypeFrom(uParameter, typeof(int), out var inferredType);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(inferredType);
        }

        /// <summary>
        /// Verifies that TryInferTypeFrom successfully infers type from simple generic constraint.
        /// </summary>
        [Test]
        public void TryInferTypeFrom_SimpleGenericConstraint_ReturnsCorrectType()
        {
            // Arrange
            var genericParameters = typeof(DependentGenericClass<,>).GetGenericArguments();
            var tParameter = genericParameters[0];
            var uParameter = genericParameters[1];
            var analyzer = new GenericParameterAnalyzer(uParameter);

            // Act
            // T : List<U>, if T is List<int>, then U should be int
            var result = analyzer.TryInferTypeFrom(tParameter, typeof(List<int>), out var inferredType);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(typeof(int), inferredType);
        }

        /// <summary>
        /// Verifies that TryInferTypeFrom successfully infers type from dictionary constraint with value parameter.
        /// </summary>
        [Test]
        public void TryInferTypeFrom_DictionaryConstraintValueParameter_ReturnsCorrectType()
        {
            // Arrange
            var genericParameters = typeof(MultiDependentGenericClass<,,>).GetGenericArguments();
            var tParameter = genericParameters[0];
            var vParameter = genericParameters[2];
            var analyzer = new GenericParameterAnalyzer(vParameter);

            // Act
            // T : Dictionary<U, V>, if T is Dictionary<string, int>, then V should be int
            var result = analyzer.TryInferTypeFrom(tParameter, typeof(Dictionary<string, int>), out var inferredType);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(typeof(int), inferredType);
        }

        /// <summary>
        /// Verifies that TryInferTypeFrom successfully infers type from dictionary constraint with key parameter.
        /// </summary>
        [Test]
        public void TryInferTypeFrom_DictionaryConstraintKeyParameter_ReturnsCorrectType()
        {
            // Arrange
            var genericParameters = typeof(MultiDependentGenericClass<,,>).GetGenericArguments();
            var tParameter = genericParameters[0];
            var uParameter = genericParameters[1];
            var analyzer = new GenericParameterAnalyzer(uParameter);

            // Act
            // T : Dictionary<U, V>, if T is Dictionary<string, int>, then U should be string
            var result = analyzer.TryInferTypeFrom(tParameter, typeof(Dictionary<string, int>), out var inferredType);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(typeof(string), inferredType);
        }

        /// <summary>
        /// Verifies that TryInferTypeFrom successfully infers type from nested generic constraint.
        /// </summary>
        [Test]
        public void TryInferTypeFrom_NestedGenericConstraint_ReturnsCorrectType()
        {
            // Arrange
            var genericParameters = typeof(NestedDependentGenericClass<,>).GetGenericArguments();
            var tParameter = genericParameters[0];
            var uParameter = genericParameters[1];
            var analyzer = new GenericParameterAnalyzer(uParameter);

            // Act
            // T : List<List<U>>, if T is List<List<int>>, then U should be int
            var result = analyzer.TryInferTypeFrom(tParameter, typeof(List<List<int>>), out var inferredType);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(typeof(int), inferredType);
        }

        /// <summary>
        /// Verifies that TryInferTypeFrom returns false when concrete type doesn't match constraint definition.
        /// </summary>
        [Test]
        public void TryInferTypeFrom_TypeMismatch_ReturnsFalse()
        {
            // Arrange
            var genericParameters = typeof(DependentGenericClass<,>).GetGenericArguments();
            var tParameter = genericParameters[0];
            var uParameter = genericParameters[1];
            var analyzer = new GenericParameterAnalyzer(uParameter);

            // Act
            // T : List<U>, but providing Dictionary<int, string> which doesn't match List<>
            var result = analyzer.TryInferTypeFrom(tParameter, typeof(Dictionary<int, string>), out var inferredType);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(inferredType);
        }

        #endregion
    }

    #region Test Helper Types

    /// <summary>
    /// Simple generic class with no constraints.
    /// </summary>
    public class SimpleGenericClass<T>
    {
    }

    /// <summary>
    /// Generic class with two type parameters.
    /// </summary>
    public class DoubleGenericClass<T, U>
    {
    }

    /// <summary>
    /// Generic class with three type parameters.
    /// </summary>
    public class TripleGenericClass<T, U, V>
    {
    }

    /// <summary>
    /// Generic class with class constraint.
    /// </summary>
    public class ClassConstrainedClass<T> where T : class
    {
    }

    /// <summary>
    /// Generic class with struct constraint.
    /// </summary>
    public class StructConstrainedClass<T> where T : struct
    {
    }

    /// <summary>
    /// Generic class with new() constraint.
    /// </summary>
    public class NewConstrainedClass<T> where T : new()
    {
    }

    /// <summary>
    /// Generic class with class and new() constraints.
    /// </summary>
    public class ClassAndNewConstrainedClass<T> where T : class, new()
    {
    }

    /// <summary>
    /// Generic class with base class constraint.
    /// </summary>
    public class BaseClassConstrainedClass<T> where T : TestBaseClass
    {
    }

    /// <summary>
    /// Generic class with interface constraint.
    /// </summary>
    public class InterfaceConstrainedClass<T> where T : ITestInterface
    {
    }

    /// <summary>
    /// Generic class with multiple interface constraints.
    /// </summary>
    public class MultipleInterfaceConstrainedClass<T> where T : ITestInterface, IAnotherInterface
    {
    }

    /// <summary>
    /// Generic class where T depends on U (T : List&lt;U&gt;).
    /// </summary>
    public class DependentGenericClass<T, U> where T : List<U>
    {
    }

    /// <summary>
    /// Generic class where T depends on U and V.
    /// </summary>
    public class MultiDependentGenericClass<T, U, V> where T : Dictionary<U, V>
    {
    }

    /// <summary>
    /// Generic class with nested generic constraint.
    /// </summary>
    public class NestedDependentGenericClass<T, U> where T : List<List<U>>
    {
    }

    /// <summary>
    /// Class with method generic parameter for testing method vs type generic parameters.
    /// </summary>
    public class TestClassWithMethodGeneric
    {
        public void GenericMethod<T>(T param)
        {
        }
    }

    /// <summary>
    /// Base class for constraint testing.
    /// </summary>
    public class TestBaseClass
    {
    }

    /// <summary>
    /// Derived class for constraint testing.
    /// </summary>
    public class TestDerivedClass : TestBaseClass
    {
    }

    /// <summary>
    /// Test interface.
    /// </summary>
    public interface ITestInterface
    {
    }

    /// <summary>
    /// Another test interface.
    /// </summary>
    public interface IAnotherInterface
    {
    }

    /// <summary>
    /// Class implementing single test interface.
    /// </summary>
    public class TestInterfaceImplementation : ITestInterface
    {
    }

    /// <summary>
    /// Class implementing multiple test interfaces.
    /// </summary>
    public class TestMultiInterfaceImplementation : ITestInterface, IAnotherInterface
    {
    }

    #endregion
}
