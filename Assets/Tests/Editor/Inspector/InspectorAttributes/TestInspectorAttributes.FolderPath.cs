using System;
using System.Linq;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Inspector.Attributes;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.Inspector.Editor.Implementations;
using NUnit.Framework;

namespace Tests.Editor.Inspector.InspectorAttributes
{
    public class TestInspectorAttributes_FolderPath
    {
        [Serializable]
        public class TestClass
        {
            [FolderPath]
            public string FolderPathString;

            [FolderPath("Assets/Scripts")]
            public string ParentFolderPathString;
        }

        /// <summary>
        /// Verifies that GetDrawerTypes returns the correct FolderPath drawer type
        /// when custom TypeMatchCandidates are registered.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithCustomTypeMatchCandidates_ReturnsFolderPathDrawer()
        {
            var folderPathElement = GetFolderPathElement();
            var originalTypeMatchCandidates = HandlerUtility.TypeMatcher.TypeMatchCandidates.ToArray();
            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(new []
            {
                new TypeMatchCandidate(typeof(FolderPathAttributeDrawer), 0,
                    HandlerUtility.GetConstraints(typeof(FolderPathAttributeDrawer)))
            });
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(folderPathElement).ToArray();
            Assert.AreEqual(drawerTypes.Length, 1);
            Assert.AreEqual(drawerTypes[0], typeof(FolderPathAttributeDrawer));

            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(originalTypeMatchCandidates);
        }

        /// <summary>
        /// Verifies that the default TypeMatcher candidates include the FolderPath drawer type.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithDefaultCandidates_ContainsFolderPathDrawer()
        {
            var folderPathElement = GetFolderPathElement();
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(folderPathElement).ToArray();
            Assert.IsTrue(drawerTypes.Contains(typeof(FolderPathAttributeDrawer)));
        }

        /// <summary>
        /// Verifies that GetDrawerChain returns a drawer chain containing the FolderPath drawer
        /// when the field has FolderPath attribute.
        /// </summary>
        [Test]
        public void GetDrawerChain_WithFolderPathString_ContainsFolderPathDrawer()
        {
            var folderPathElement = GetFolderPathElement();
            var drawerChainResolver = new DefaultDrawerChainResolverFactory().CreateResolver(folderPathElement);
            Assert.IsNotNull(drawerChainResolver);
            var drawerChain = drawerChainResolver.GetDrawerChain();
            Assert.IsTrue(drawerChain.Drawers.Any(drawer => drawer.GetType() == typeof(FolderPathAttributeDrawer)));
        }

        private IFieldElement GetFolderPathElement()
        {
            var testInstance = new TestClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var root = tree.Root;
            var folderPathElement = root.LogicalChildren[0];
            Assert.IsNotNull(folderPathElement);
            Assert.IsInstanceOf<IFieldElement>(folderPathElement);
            return (IFieldElement)folderPathElement;
        }
    }
}
