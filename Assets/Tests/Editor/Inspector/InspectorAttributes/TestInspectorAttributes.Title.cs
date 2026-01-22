using System;
using System.Linq;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Inspector.Attributes;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.Inspector.Editor.Implementations;
using NUnit.Framework;

namespace Tests.Editor.Inspector.InspectorAttributes
{
    public class TestInspectorAttributes_Title
    {
        [Serializable]
        public class TestClass
        {
            [Title("Section Title")]
            public int TitleInt;

            [Title("Section Title", Subtitle = "Subtitle", BoldTitle = false, HorizontalLine = false)]
            public int CustomTitleInt;
        }

        /// <summary>
        /// Verifies that GetDrawerTypes returns the correct Title drawer type
        /// when custom TypeMatchCandidates are registered.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithCustomTypeMatchCandidates_ReturnsTitleDrawer()
        {
            var titleElement = GetTitleElement();
            var originalTypeMatchCandidates = HandlerUtility.TypeMatcher.TypeMatchCandidates.ToArray();
            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(new []
            {
                new TypeMatchCandidate(typeof(TitleAttributeDrawer), 0,
                    HandlerUtility.GetConstraints(typeof(TitleAttributeDrawer)))
            });
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(titleElement).ToArray();
            Assert.AreEqual(drawerTypes.Length, 1);
            Assert.AreEqual(drawerTypes[0], typeof(TitleAttributeDrawer));

            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(originalTypeMatchCandidates);
        }

        /// <summary>
        /// Verifies that the default TypeMatcher candidates include the Title drawer type.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithDefaultCandidates_ContainsTitleDrawer()
        {
            var titleElement = GetTitleElement();
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(titleElement).ToArray();
            Assert.IsTrue(drawerTypes.Contains(typeof(TitleAttributeDrawer)));
        }

        /// <summary>
        /// Verifies that GetDrawerChain returns a drawer chain containing the Title drawer
        /// when the field has Title attribute.
        /// </summary>
        [Test]
        public void GetDrawerChain_WithTitleInt_ContainsTitleDrawer()
        {
            var titleElement = GetTitleElement();
            var drawerChainResolver = new DefaultDrawerChainResolverFactory().CreateResolver(titleElement);
            Assert.IsNotNull(drawerChainResolver);
            var drawerChain = drawerChainResolver.GetDrawerChain();
            Assert.IsTrue(drawerChain.Drawers.Any(drawer => drawer.GetType() == typeof(TitleAttributeDrawer)));
        }

        private IFieldElement GetTitleElement()
        {
            var testInstance = new TestClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var root = tree.Root;
            var titleElement = root.LogicalChildren[0];
            Assert.IsInstanceOf<IFieldElement>(titleElement);
            return (IFieldElement)titleElement;
        }
    }
}
