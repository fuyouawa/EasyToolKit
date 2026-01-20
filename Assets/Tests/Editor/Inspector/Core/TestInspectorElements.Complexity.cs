using System.Collections.Generic;
using EasyToolKit.Inspector.Editor;
using NUnit.Framework;

namespace Tests.Editor.Inspector.Core
{
    /// <summary>
    /// Integration tests for Inspector element tree complexity scenarios.
    /// Tests verify that complex element trees with multiple fields are correctly created,
    /// with proper field types, names, and ordering.
    /// </summary>
    [TestFixture]
    public class TestInspectorElements_Complexity
    {
        #region Multiple Fields - Field Type Verification

        /// <summary>
        /// Tests that the IntField element has the correct field type.
        /// </summary>
        [Test]
        public void MultipleFields_IntField_HasCorrectFieldType()
        {
            // Arrange
            var testInstance = new MultipleFieldsClass { IntField = 1 };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement = root.LogicalChildren[0];
            var fieldDef = ((IFieldElement)fieldElement).Definition;

            // Act
            var fieldType = fieldDef.ValueType;

            // Assert
            Assert.AreEqual(typeof(int), fieldType, "IntField should have int type");
        }

        /// <summary>
        /// Tests that the FloatField element has the correct field type.
        /// </summary>
        [Test]
        public void MultipleFields_FloatField_HasCorrectFieldType()
        {
            // Arrange
            var testInstance = new MultipleFieldsClass { FloatField = 2.0f };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement = root.LogicalChildren[1];
            var fieldDef = ((IFieldElement)fieldElement).Definition;

            // Act
            var fieldType = fieldDef.ValueType;

            // Assert
            Assert.AreEqual(typeof(float), fieldType, "FloatField should have float type");
        }

        /// <summary>
        /// Tests that the StringField element has the correct field type.
        /// </summary>
        [Test]
        public void MultipleFields_StringField_HasCorrectFieldType()
        {
            // Arrange
            var testInstance = new MultipleFieldsClass { StringField = "test" };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement = root.LogicalChildren[2];
            var fieldDef = ((IFieldElement)fieldElement).Definition;

            // Act
            var fieldType = fieldDef.ValueType;

            // Assert
            Assert.AreEqual(typeof(string), fieldType, "StringField should have string type");
        }

        /// <summary>
        /// Tests that the BoolField element has the correct field type.
        /// </summary>
        [Test]
        public void MultipleFields_BoolField_HasCorrectFieldType()
        {
            // Arrange
            var testInstance = new MultipleFieldsClass { BoolField = true };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement = root.LogicalChildren[3];
            var fieldDef = ((IFieldElement)fieldElement).Definition;

            // Act
            var fieldType = fieldDef.ValueType;

            // Assert
            Assert.AreEqual(typeof(bool), fieldType, "BoolField should have bool type");
        }

        #endregion

        #region Multiple Fields - Field Name Verification

        /// <summary>
        /// Tests that the IntField element has the correct field name.
        /// </summary>
        [Test]
        public void MultipleFields_IntField_HasCorrectFieldName()
        {
            // Arrange
            var testInstance = new MultipleFieldsClass { IntField = 1 };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement = root.LogicalChildren[0];

            // Act
            var fieldName = fieldElement.Definition.Name;

            // Assert
            Assert.AreEqual(nameof(MultipleFieldsClass.IntField), fieldName,
                "IntField element should have correct field name");
        }

        /// <summary>
        /// Tests that the FloatField element has the correct field name.
        /// </summary>
        [Test]
        public void MultipleFields_FloatField_HasCorrectFieldName()
        {
            // Arrange
            var testInstance = new MultipleFieldsClass { FloatField = 2.0f };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement = root.LogicalChildren[1];

            // Act
            var fieldName = fieldElement.Definition.Name;

            // Assert
            Assert.AreEqual(nameof(MultipleFieldsClass.FloatField), fieldName,
                "FloatField element should have correct field name");
        }

        /// <summary>
        /// Tests that the StringField element has the correct field name.
        /// </summary>
        [Test]
        public void MultipleFields_StringField_HasCorrectFieldName()
        {
            // Arrange
            var testInstance = new MultipleFieldsClass { StringField = "test" };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement = root.LogicalChildren[2];

            // Act
            var fieldName = fieldElement.Definition.Name;

            // Assert
            Assert.AreEqual(nameof(MultipleFieldsClass.StringField), fieldName,
                "StringField element should have correct field name");
        }

        /// <summary>
        /// Tests that the BoolField element has the correct field name.
        /// </summary>
        [Test]
        public void MultipleFields_BoolField_HasCorrectFieldName()
        {
            // Arrange
            var testInstance = new MultipleFieldsClass { BoolField = true };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement = root.LogicalChildren[3];

            // Act
            var fieldName = fieldElement.Definition.Name;

            // Assert
            Assert.AreEqual(nameof(MultipleFieldsClass.BoolField), fieldName,
                "BoolField element should have correct field name");
        }

        #endregion

        #region Multiple Fields - Field Order Verification

        /// <summary>
        /// Tests that fields appear in the correct order as defined in the class.
        /// </summary>
        [Test]
        public void MultipleFields_FieldOrder_MatchesClassDefinition()
        {
            // Arrange
            var testInstance = new MultipleFieldsClass
            {
                IntField = 1,
                FloatField = 2.0f,
                StringField = "test",
                BoolField = true
            };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();

            // Act
            var field0Name = root.LogicalChildren[0].Definition.Name;
            var field1Name = root.LogicalChildren[1].Definition.Name;
            var field2Name = root.LogicalChildren[2].Definition.Name;
            var field3Name = root.LogicalChildren[3].Definition.Name;

            // Assert
            Assert.AreEqual(nameof(MultipleFieldsClass.IntField), field0Name,
                "First field should be IntField");
            Assert.AreEqual(nameof(MultipleFieldsClass.FloatField), field1Name,
                "Second field should be FloatField");
            Assert.AreEqual(nameof(MultipleFieldsClass.StringField), field2Name,
                "Third field should be StringField");
            Assert.AreEqual(nameof(MultipleFieldsClass.BoolField), field3Name,
                "Fourth field should be BoolField");
        }

        /// <summary>
        /// Tests that NestedClass fields appear in the correct order as defined in the class.
        /// </summary>
        [Test]
        public void NestedClass_FieldOrder_MatchesClassDefinition()
        {
            // Arrange
            var testInstance = new NestedClass
            {
                Id = 100,
                NestedObject = new SingleFieldClass { TestInt = 42 }
            };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();

            // Act
            var field0Name = root.LogicalChildren[0].Definition.Name;
            var field1Name = root.LogicalChildren[1].Definition.Name;

            // Assert
            Assert.AreEqual(nameof(NestedClass.Id), field0Name,
                "First field should be Id");
            Assert.AreEqual(nameof(NestedClass.NestedObject), field1Name,
                "Second field should be NestedObject");
        }

        #endregion

        #region Multiple Fields - Comprehensive Integration Test

        /// <summary>
        /// Comprehensive test verifying all field properties (type, name, order) for MultipleFieldsClass.
        /// </summary>
        [Test]
        public void MultipleFields_AllFields_HaveCorrectTypeAndNameAndOrder()
        {
            // Arrange
            var testInstance = new MultipleFieldsClass
            {
                IntField = 1,
                FloatField = 2.0f,
                StringField = "test",
                BoolField = true
            };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();

            // Act & Assert - Field 0: IntField
            {
                var fieldElement = root.LogicalChildren[0];
                var fieldDef = ((IFieldElement)fieldElement).Definition;

                Assert.AreEqual(nameof(MultipleFieldsClass.IntField), fieldDef.Name,
                    "Field 0 should have name IntField");
                Assert.AreEqual(typeof(int), fieldDef.ValueType,
                    "Field 0 should have type int");
                Assert.IsInstanceOf<IFieldElement>(fieldElement,
                    "Field 0 should implement IFieldElement");
            }

            // Act & Assert - Field 1: FloatField
            {
                var fieldElement = root.LogicalChildren[1];
                var fieldDef = ((IFieldElement)fieldElement).Definition;

                Assert.AreEqual(nameof(MultipleFieldsClass.FloatField), fieldDef.Name,
                    "Field 1 should have name FloatField");
                Assert.AreEqual(typeof(float), fieldDef.ValueType,
                    "Field 1 should have type float");
                Assert.IsInstanceOf<IFieldElement>(fieldElement,
                    "Field 1 should implement IFieldElement");
            }

            // Act & Assert - Field 2: StringField
            {
                var fieldElement = root.LogicalChildren[2];
                var fieldDef = ((IFieldElement)fieldElement).Definition;

                Assert.AreEqual(nameof(MultipleFieldsClass.StringField), fieldDef.Name,
                    "Field 2 should have name StringField");
                Assert.AreEqual(typeof(string), fieldDef.ValueType,
                    "Field 2 should have type string");
                Assert.IsInstanceOf<IFieldElement>(fieldElement,
                    "Field 2 should implement IFieldElement");
            }

            // Act & Assert - Field 3: BoolField
            {
                var fieldElement = root.LogicalChildren[3];
                var fieldDef = ((IFieldElement)fieldElement).Definition;

                Assert.AreEqual(nameof(MultipleFieldsClass.BoolField), fieldDef.Name,
                    "Field 3 should have name BoolField");
                Assert.AreEqual(typeof(bool), fieldDef.ValueType,
                    "Field 3 should have type bool");
                Assert.IsInstanceOf<IFieldElement>(fieldElement,
                    "Field 3 should implement IFieldElement");
            }
        }

        /// <summary>
        /// Comprehensive test verifying all field properties (type, name, order) for NestedClass.
        /// </summary>
        [Test]
        public void NestedClass_AllFields_HaveCorrectTypeAndNameAndOrder()
        {
            // Arrange
            var testInstance = new NestedClass
            {
                Id = 100,
                NestedObject = new SingleFieldClass { TestInt = 42 }
            };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();

            // Act & Assert - Field 0: Id
            {
                var fieldElement = root.LogicalChildren[0];
                var fieldDef = ((IFieldElement)fieldElement).Definition;

                Assert.AreEqual(nameof(NestedClass.Id), fieldDef.Name,
                    "Field 0 should have name Id");
                Assert.AreEqual(typeof(int), fieldDef.ValueType,
                    "Field 0 should have type int");
                Assert.IsInstanceOf<IFieldElement>(fieldElement,
                    "Field 0 should implement IFieldElement");
            }

            // Act & Assert - Field 1: NestedObject
            {
                var fieldElement = root.LogicalChildren[1];
                var fieldDef = ((IFieldElement)fieldElement).Definition;

                Assert.AreEqual(nameof(NestedClass.NestedObject), fieldDef.Name,
                    "Field 1 should have name NestedObject");
                Assert.AreEqual(typeof(SingleFieldClass), fieldDef.ValueType,
                    "Field 1 should have type SingleFieldClass");
                Assert.IsInstanceOf<IFieldElement>(fieldElement,
                    "Field 1 should implement IFieldElement");
            }
        }

        #endregion

        #region List Field Tests

        /// <summary>
        /// Tests that a List field implements IFieldCollectionElement.
        /// </summary>
        [Test]
        public void ListField_ImplementsIFieldCollectionElement()
        {
            // Arrange
            var testInstance = new ListFieldClass
            {
                Integers = new List<int> { 1, 2, 3 }
            };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement = root.LogicalChildren[0];

            // Act & Assert
            Assert.IsInstanceOf<IFieldCollectionElement>(fieldElement,
                "List field should implement IFieldCollectionElement");
        }

        /// <summary>
        /// Tests that the LogicalChildren count matches the number of elements in the List.
        /// </summary>
        [Test]
        public void ListField_LogicalChildrenCount_MatchesElementCount()
        {
            // Arrange
            var expectedCount = 5;
            var testInstance = new ListFieldClass
            {
                Integers = new List<int> { 10, 20, 30, 40, 50 }
            };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement = root.LogicalChildren[0];
            var collectionElement = (ICollectionElement)fieldElement;

            // Act
            var actualCount = collectionElement.LogicalChildren.Count;

            // Assert
            Assert.AreEqual(expectedCount, actualCount,
                $"LogicalChildren count should be {expectedCount} to match the number of List elements");
        }

        /// <summary>
        /// Tests that an empty List has zero LogicalChildren.
        /// </summary>
        [Test]
        public void ListField_EmptyList_HasZeroLogicalChildren()
        {
            // Arrange
            var testInstance = new ListFieldClass
            {
                Integers = new List<int>()
            };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement = root.LogicalChildren[0];
            var collectionElement = (ICollectionElement)fieldElement;

            // Act
            var actualCount = collectionElement.LogicalChildren.Count;

            // Assert
            Assert.AreEqual(0, actualCount,
                "Empty List should have zero LogicalChildren");
        }

        /// <summary>
        /// Tests that the LogicalChildren count updates when the List is modified.
        /// </summary>
        [Test]
        public void ListField_AfterAddingElement_LogicalChildrenCountUpdates()
        {
            // Arrange
            var testInstance = new ListFieldClass
            {
                Integers = new List<int> { 1, 2 }
            };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement = root.LogicalChildren[0];
            var collectionElement = (ICollectionElement)fieldElement;

            // Act - add element and update
            testInstance.Integers.Add(3);
            tree.BeginDraw();
            tree.EndDraw();

            var actualCount = collectionElement.LogicalChildren.Count;

            // Assert
            Assert.AreEqual(3, actualCount,
                "LogicalChildren count should be 3 after adding one element to List of 2");
        }

        #endregion
    }
}
