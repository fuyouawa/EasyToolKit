using System;
using System.Linq;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Inspector.Attributes;
using EasyToolkit.Inspector.Editor;
using EasyToolkit.Inspector.Editor.Implementations;
using NUnit.Framework;

namespace Tests.Editor.Inspector.InspectorAttributes
{
    public class TestInspectorAttributes_ShowIf
    {
        [Serializable]
        public class TestClass
        {
            public bool ShowField = true;

            [ShowIf(nameof(ShowField))]
            public int ShowIfInt;

            [ShowIf(nameof(ShowField), false)]
            public int ShowIfFalseInt;
        }

        /// <summary>
        /// Verifies that GetDrawerTypes returns the correct ShowIf drawer type
        /// when custom TypeMatchCandidates are registered.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithCustomTypeMatchCandidates_ReturnsShowIfDrawer()
        {
            var showIfIntElement = GetShowIfIntElement();
            var originalTypeMatchCandidates = HandlerUtility.TypeMatcher.TypeMatchCandidates.ToArray();
            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(new []
            {
                new TypeMatchCandidate(typeof(ShowIfAttributeDrawer), 0,
                    HandlerUtility.GetConstraints(typeof(ShowIfAttributeDrawer)))
            });
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(showIfIntElement).ToArray();
            Assert.AreEqual(drawerTypes.Length, 1);
            Assert.AreEqual(drawerTypes[0], typeof(ShowIfAttributeDrawer));

            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(originalTypeMatchCandidates);
        }

        /// <summary>
        /// Verifies that the default TypeMatcher candidates include the ShowIf drawer type.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithDefaultCandidates_ContainsShowIfDrawer()
        {
            var showIfIntElement = GetShowIfIntElement();
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(showIfIntElement).ToArray();
            Assert.IsTrue(drawerTypes.Contains(typeof(ShowIfAttributeDrawer)));
        }

        /// <summary>
        /// Verifies that GetDrawerChain returns a drawer chain containing the ShowIf drawer
        /// when the field has ShowIf attribute.
        /// </summary>
        [Test]
        public void GetDrawerChain_WithShowIfInt_ContainsShowIfDrawer()
        {
            var showIfIntElement = GetShowIfIntElement();
            var drawerChainResolver = new DefaultDrawerChainResolverFactory().CreateResolver(showIfIntElement);
            Assert.IsNotNull(drawerChainResolver);
            var drawerChain = drawerChainResolver.GetDrawerChain();
            Assert.IsTrue(drawerChain.Drawers.Any(drawer => drawer.GetType() == typeof(ShowIfAttributeDrawer)));
        }

        private IFieldElement GetShowIfIntElement()
        {
            var testInstance = new TestClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var root = tree.Root;
            var showIfIntElement = root.LogicalChildren.FirstOrDefault(c =>
                ((IFieldElement)c).Definition.Name == nameof(TestClass.ShowIfInt));
            Assert.IsNotNull(showIfIntElement);
            Assert.IsInstanceOf<IFieldElement>(showIfIntElement);
            return (IFieldElement)showIfIntElement;
        }
    }
}
