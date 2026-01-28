using System.Collections.Generic;
using EasyToolkit.Inspector.Editor;
using EasyToolkit.Inspector.Editor.Implementations;
using NUnit.Framework;

namespace Tests.Editor.Inspector.Core
{
    [TestFixture]
    public class TestInspector_StructureResolver
    {
        [Test]
        public void Test()
        {
            var testInstance = new ListFieldClass
            {
                Integers = new List<int>()
                {
                    1, 2, 3
                }
            };
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var root = tree.Root;

            Assert.AreEqual(1, root.LogicalChildren.Count);

            var collectionElement = root.LogicalChildren[0] as ICollectionElement;
            Assert.IsNotNull(collectionElement);

            var resolver = new DefaultStructureResolverFactory().CreateResolver(collectionElement);
            Assert.IsInstanceOf<CollectionStructureResolver<List<int>, int>>(resolver);
            var definitions = resolver.GetChildrenDefinitions();
            Assert.AreEqual(3, definitions.Length);
        }
    }
}
