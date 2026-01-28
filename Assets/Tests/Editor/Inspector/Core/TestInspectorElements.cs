using EasyToolkit.Inspector.Editor;
using NUnit.Framework;

namespace Tests.Editor.Inspector.Core
{
    /// <summary>
    /// Unit tests for Inspector element tree creation and structure.
    /// Tests verify that element trees are correctly created and structured for various target objects.
    /// </summary>
    [TestFixture]
    public class TestInspectorElements
    {
        #region Tree Creation

        /// <summary>
        /// Tests that creating a tree with a single target object succeeds.
        /// </summary>
        [Test]
        public void CreateTree_SingleTarget_ReturnsValidTree()
        {
            // Arrange
            var testInstance = new SingleFieldClass { TestInt = 42 };

            // Act
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);

            // Assert
            Assert.IsNotNull(tree, "Tree should not be null");
            Assert.IsNotNull(tree.Root, "Root element should not be null");
        }

        /// <summary>
        /// Tests that creating a tree with multiple target objects of the same type succeeds.
        /// </summary>
        [Test]
        public void CreateTree_MultipleTargetsSameType_ReturnsValidTree()
        {
            // Arrange
            var target1 = new SingleFieldClass { TestInt = 1 };
            var target2 = new SingleFieldClass { TestInt = 2 };

            // Act
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { target1, target2 }, null);

            // Assert
            Assert.IsNotNull(tree, "Tree should not be null");
            Assert.AreEqual(2, tree.Targets.Count, "Targets count should match input");
        }

        /// <summary>
        /// Tests that the tree has the correct target type.
        /// </summary>
        [Test]
        public void CreateTree_TargetObject_HasCorrectTargetType()
        {
            // Arrange
            var testInstance = new SingleFieldClass();

            // Act
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);

            // Assert
            Assert.AreEqual(typeof(SingleFieldClass), tree.TargetType, "Target type should match the object type");
        }

        #endregion

        #region Root Element

        /// <summary>
        /// Tests that the root element is created with the correct definition type.
        /// </summary>
        [Test]
        public void RootElement_AfterCreation_HasCorrectDefinitionType()
        {
            // Arrange
            var testInstance = new SingleFieldClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);

            // Act
            var root = tree.Root;

            // Assert
            Assert.IsNotNull(root.Definition, "Root definition should not be null");
        }

        /// <summary>
        /// Tests that the root element has the correct path.
        /// </summary>
        [Test]
        public void RootElement_AfterCreation_HasCorrectPath()
        {
            // Arrange
            var testInstance = new SingleFieldClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);

            // Act
            var root = tree.Root;

            // Assert
            Assert.AreEqual("$ROOT$", root.Path, "Root path should be $ROOT$");
        }

        /// <summary>
        /// Tests that the root element has no parent.
        /// </summary>
        [Test]
        public void RootElement_AfterCreation_HasNoParent()
        {
            // Arrange
            var testInstance = new SingleFieldClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);

            // Act
            var root = tree.Root;

            // Assert
            Assert.IsNull(root.Parent, "Root element should have no parent");
        }

        #endregion

        #region Field Elements

        /// <summary>
        /// Tests that a single field is correctly created as a child element.
        /// </summary>
        [Test]
        public void FieldElement_SinglePublicField_CreatesOneChild()
        {
            // Arrange
            var testInstance = new SingleFieldClass { TestInt = 42 };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;

            // Act
            tree.BeginDraw();
            tree.EndDraw();

            // Assert
            Assert.AreEqual(1, root.LogicalChildren.Count, "Should have exactly one logical child");
        }

        /// <summary>
        /// Tests that the field element implements the correct interface.
        /// </summary>
        [Test]
        public void FieldElement_SinglePublicField_ImplementsIFieldElement()
        {
            // Arrange
            var testInstance = new SingleFieldClass { TestInt = 42 };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;

            // Act
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement0 = root.LogicalChildren[0];

            // Assert
            Assert.IsInstanceOf<IFieldElement>(fieldElement0, "Field element should implement IFieldElement");
            Assert.AreEqual(nameof(SingleFieldClass.TestInt), fieldElement0.Definition.Name,
                "Field element name should match the field name");
        }

        /// <summary>
        /// Tests that the field element has the correct name.
        /// </summary>
        [Test]
        public void FieldElement_SinglePublicField_HasCorrectName()
        {
            // Arrange
            var testInstance = new SingleFieldClass { TestInt = 42 };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;

            // Act
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement = root.LogicalChildren[0];

            // Assert
            Assert.AreEqual(nameof(SingleFieldClass.TestInt), fieldElement.Definition.Name,
                "Field element name should match the field name");
        }

        /// <summary>
        /// Tests that multiple public fields are correctly created as child elements.
        /// </summary>
        [Test]
        public void FieldElement_MultiplePublicFields_CreatesAllFields()
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

            // Act
            tree.BeginDraw();
            tree.EndDraw();

            // Assert
            Assert.AreEqual(4, root.LogicalChildren.Count, "Should have four logical children for four public fields");
        }

        /// <summary>
        /// Tests that the IntField element implements the correct interface and has the correct name.
        /// </summary>
        [Test]
        public void FieldElement_MultiplePublicFields_IntFieldImplementsIFieldElement()
        {
            // Arrange
            var testInstance = new MultipleFieldsClass { IntField = 1 };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;

            // Act
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement0 = root.LogicalChildren[0];

            // Assert
            Assert.IsInstanceOf<IFieldElement>(fieldElement0, "IntField element should implement IFieldElement");
            Assert.AreEqual(nameof(MultipleFieldsClass.IntField), fieldElement0.Definition.Name,
                "IntField element name should match the field name");
        }

        /// <summary>
        /// Tests that the FloatField element implements the correct interface and has the correct name.
        /// </summary>
        [Test]
        public void FieldElement_MultiplePublicFields_FloatFieldImplementsIFieldElement()
        {
            // Arrange
            var testInstance = new MultipleFieldsClass { FloatField = 2.0f };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;

            // Act
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement1 = root.LogicalChildren[1];

            // Assert
            Assert.IsInstanceOf<IFieldElement>(fieldElement1, "FloatField element should implement IFieldElement");
            Assert.AreEqual(nameof(MultipleFieldsClass.FloatField), fieldElement1.Definition.Name,
                "FloatField element name should match the field name");
        }

        /// <summary>
        /// Tests that the StringField element implements the correct interface and has the correct name.
        /// </summary>
        [Test]
        public void FieldElement_MultiplePublicFields_StringFieldImplementsIFieldElement()
        {
            // Arrange
            var testInstance = new MultipleFieldsClass { StringField = "test" };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;

            // Act
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement2 = root.LogicalChildren[2];

            // Assert
            Assert.IsInstanceOf<IFieldElement>(fieldElement2, "StringField element should implement IFieldElement");
            Assert.AreEqual(nameof(MultipleFieldsClass.StringField), fieldElement2.Definition.Name,
                "StringField element name should match the field name");
        }

        /// <summary>
        /// Tests that the BoolField element implements the correct interface and has the correct name.
        /// </summary>
        [Test]
        public void FieldElement_MultiplePublicFields_BoolFieldImplementsIFieldElement()
        {
            // Arrange
            var testInstance = new MultipleFieldsClass { BoolField = true };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;

            // Act
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement3 = root.LogicalChildren[3];

            // Assert
            Assert.IsInstanceOf<IFieldElement>(fieldElement3, "BoolField element should implement IFieldElement");
            Assert.AreEqual(nameof(MultipleFieldsClass.BoolField), fieldElement3.Definition.Name,
                "BoolField element name should match the field name");
        }

        /// <summary>
        /// Tests that private fields are not created as child elements.
        /// </summary>
        [Test]
        public void FieldElement_MixedVisibility_CreatesOnlyPublicFields()
        {
            // Arrange
            var testInstance = new MixedMembersClass
            {
                PublicField = 1
            };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;

            // Act
            tree.BeginDraw();
            tree.EndDraw();

            // Assert
            Assert.AreEqual(1, root.LogicalChildren.Count, "Should only create elements for public fields");
            Assert.AreEqual(nameof(MixedMembersClass.PublicField), root.LogicalChildren[0].Definition.Name,
                "Should create element for the public field");
        }

        /// <summary>
        /// Tests that the MixedMembersClass PublicField element implements the correct interface and has the correct name.
        /// </summary>
        [Test]
        public void FieldElement_MixedVisibility_PublicFieldImplementsIFieldElement()
        {
            // Arrange
            var testInstance = new MixedMembersClass { PublicField = 1 };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;

            // Act
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement = root.LogicalChildren[0];

            // Assert
            Assert.IsInstanceOf<IFieldElement>(fieldElement, "PublicField element should implement IFieldElement");
            Assert.AreEqual(nameof(MixedMembersClass.PublicField), fieldElement.Definition.Name,
                "PublicField element name should match the field name");
        }

        /// <summary>
        /// Tests that nested object fields are correctly created as child elements.
        /// </summary>
        [Test]
        public void FieldElement_NestedClass_CreatesAllFields()
        {
            // Arrange
            var testInstance = new NestedClass
            {
                Id = 100,
                NestedObject = new SingleFieldClass { TestInt = 42 }
            };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;

            // Act
            tree.BeginDraw();
            tree.EndDraw();

            // Assert
            Assert.AreEqual(2, root.LogicalChildren.Count, "Should have two logical children for Id and NestedObject");
        }

        /// <summary>
        /// Tests that the Id field element implements the correct interface and has the correct name.
        /// </summary>
        [Test]
        public void FieldElement_NestedClass_IdFieldImplementsIFieldElement()
        {
            // Arrange
            var testInstance = new NestedClass { Id = 100 };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;

            // Act
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement0 = root.LogicalChildren[0];

            // Assert
            Assert.IsInstanceOf<IFieldElement>(fieldElement0, "Id field element should implement IFieldElement");
            Assert.AreEqual(nameof(NestedClass.Id), fieldElement0.Definition.Name,
                "Id field element name should match the field name");
        }

        /// <summary>
        /// Tests that the NestedObject field element implements the correct interface and has the correct name.
        /// </summary>
        [Test]
        public void FieldElement_NestedClass_NestedObjectFieldImplementsIFieldElement()
        {
            // Arrange
            var testInstance = new NestedClass { NestedObject = new SingleFieldClass { TestInt = 42 } };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;

            // Act
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement1 = root.LogicalChildren[1];

            // Assert
            Assert.IsInstanceOf<IFieldElement>(fieldElement1, "NestedObject field element should implement IFieldElement");
            Assert.AreEqual(nameof(NestedClass.NestedObject), fieldElement1.Definition.Name,
                "NestedObject field element name should match the field name");
        }

        #endregion

        #region Element Hierarchy

        /// <summary>
        /// Tests that the field element has the correct parent.
        /// </summary>
        [Test]
        public void ElementHierarchy_FieldElement_HasRootAsParent()
        {
            // Arrange
            var testInstance = new SingleFieldClass { TestInt = 42 };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();

            // Act
            var fieldElement = root.LogicalChildren[0];

            // Assert
            Assert.AreSame(root, fieldElement.Parent, "Field element parent should be the root element");
        }

        /// <summary>
        /// Tests that the field element has the correct path.
        /// </summary>
        [Test]
        public void ElementHierarchy_FieldElement_HasCorrectPath()
        {
            // Arrange
            var testInstance = new SingleFieldClass { TestInt = 42 };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();

            // Act
            var fieldElement = root.LogicalChildren[0];

            // Assert
            Assert.AreEqual("$ROOT$." + nameof(SingleFieldClass.TestInt), fieldElement.Path,
                "Field element path should be prefixed with root path");
        }

        #endregion

        #region Element State

        /// <summary>
        /// Tests that the root element is in the correct initial state.
        /// </summary>
        [Test]
        public void ElementState_RootElement_AfterCreationHasInitialState()
        {
            // Arrange
            var testInstance = new SingleFieldClass { TestInt = 42 };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);

            // Act
            var root = tree.Root;

            // Assert
            Assert.IsNotNull(root.State, "Root element state should not be null");
        }

        /// <summary>
        /// Tests that the field element is in the correct initial state.
        /// </summary>
        [Test]
        public void ElementState_FieldElement_AfterCreationHasInitialState()
        {
            // Arrange
            var testInstance = new SingleFieldClass { TestInt = 42 };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();

            // Act
            var fieldElement = root.LogicalChildren[0];

            // Assert
            Assert.IsNotNull(fieldElement.State, "Field element state should not be null");
        }

        #endregion

        #region Update Mechanism

        /// <summary>
        /// Tests that the root element can be updated.
        /// </summary>
        [Test]
        public void Update_RootElement_CanBeUpdated()
        {
            // Arrange
            var testInstance = new SingleFieldClass { TestInt = 42 };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;

            // Act
            Assert.DoesNotThrow(() => root.Update(), "Root element should be updatable without throwing");
        }

        /// <summary>
        /// Tests that the field element can be updated.
        /// </summary>
        [Test]
        public void Update_FieldElement_CanBeUpdated()
        {
            // Arrange
            var testInstance = new SingleFieldClass { TestInt = 42 };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement = root.LogicalChildren[0];

            // Act & Assert
            Assert.DoesNotThrow(() => fieldElement.Update(), "Field element should be updatable without throwing");
        }

        /// <summary>
        /// Tests that multiple updates in the same frame are handled correctly.
        /// </summary>
        [Test]
        public void Update_MultipleCallsInSameFrame_HandlesCorrectly()
        {
            // Arrange
            var testInstance = new SingleFieldClass { TestInt = 42 };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;

            // Act
            tree.BeginDraw();
            tree.EndDraw();
            tree.BeginDraw();
            tree.EndDraw();
            tree.BeginDraw();
            tree.EndDraw();

            // Assert
            Assert.AreEqual(1, root.LogicalChildren.Count, "Multiple updates should not create duplicate elements");
        }

        #endregion

        #region Shared Context

        /// <summary>
        /// Tests that all elements share the same context.
        /// </summary>
        [Test]
        public void SharedContext_AllElements_UseSameContext()
        {
            // Arrange
            var testInstance = new SingleFieldClass { TestInt = 42 };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            var root = tree.Root;
            tree.BeginDraw();
            tree.EndDraw();
            var fieldElement = root.LogicalChildren[0];

            // Act & Assert
            Assert.AreSame(root.SharedContext, fieldElement.SharedContext,
                "All elements in the tree should share the same context");
        }

        /// <summary>
        /// Tests that the shared context provides access to the tree factory.
        /// </summary>
        [Test]
        public void SharedContext_Tree_HasElementFactory()
        {
            // Arrange
            var testInstance = new SingleFieldClass { TestInt = 42 };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);

            // Act
            var factory = tree.ElementFactory;

            // Assert
            Assert.IsNotNull(factory, "Tree should have an element factory");
        }

        #endregion

        #region Element Factory

        /// <summary>
        /// Tests that the element factory is accessible from the tree.
        /// </summary>
        [Test]
        public void ElementFactory_Tree_HasValidFactory()
        {
            // Arrange
            var testInstance = new SingleFieldClass { TestInt = 42 };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);

            // Act
            var factory = tree.ElementFactory;

            // Assert
            Assert.IsNotNull(factory, "Element factory should not be null");
        }

        #endregion
    }
}
