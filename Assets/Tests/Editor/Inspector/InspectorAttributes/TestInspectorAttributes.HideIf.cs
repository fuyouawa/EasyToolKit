using System;
using System.Linq;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Inspector.Attributes;
using EasyToolkit.Inspector.Editor;
using EasyToolkit.Inspector.Editor.Implementations;
using NUnit.Framework;

namespace Tests.Editor.Inspector.InspectorAttributes
{
    public class TestInspectorAttributes_HideIf
    {
        [Serializable]
        public class TestClass
        {
            public bool HideField = false;

            [HideIf(nameof(HideField))]
            public int HideIfInt;

            [HideIf(nameof(HideField), true)]
            public int HideIfTrueInt;
        }

        /// <summary>
        /// Verifies that GetDrawerTypes returns the correct HideIf drawer type
        /// when custom TypeMatchCandidates are registered.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithCustomTypeMatchCandidates_ReturnsHideIfDrawer()
        {
            var hideIfIntElement = GetHideIfIntElement();
            var originalTypeMatchCandidates = HandlerUtility.TypeMatcher.TypeMatchCandidates.ToArray();
            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(new []
            {
                new TypeMatchCandidate(typeof(HideIfAttributeDrawer), 0,
                    HandlerUtility.GetConstraints(typeof(HideIfAttributeDrawer)))
            });
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(hideIfIntElement).ToArray();
            Assert.AreEqual(drawerTypes.Length, 1);
            Assert.AreEqual(drawerTypes[0], typeof(HideIfAttributeDrawer));

            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(originalTypeMatchCandidates);
        }

        /// <summary>
        /// Verifies that the default TypeMatcher candidates include the HideIf drawer type.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithDefaultCandidates_ContainsHideIfDrawer()
        {
            var hideIfIntElement = GetHideIfIntElement();
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(hideIfIntElement).ToArray();
            Assert.IsTrue(drawerTypes.Contains(typeof(HideIfAttributeDrawer)));
        }

        /// <summary>
        /// Verifies that GetDrawerChain returns a drawer chain containing the HideIf drawer
        /// when the field has HideIf attribute.
        /// </summary>
        [Test]
        public void GetDrawerChain_WithHideIfInt_ContainsHideIfDrawer()
        {
            var hideIfIntElement = GetHideIfIntElement();
            var drawerChainResolver = new DefaultDrawerChainResolverFactory().CreateResolver(hideIfIntElement);
            Assert.IsNotNull(drawerChainResolver);
            var drawerChain = drawerChainResolver.GetDrawerChain();
            Assert.IsTrue(drawerChain.Drawers.Any(drawer => drawer.GetType() == typeof(HideIfAttributeDrawer)));
        }

        private IFieldElement GetHideIfIntElement()
        {
            var testInstance = new TestClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var root = tree.Root;
            var hideIfIntElement = root.LogicalChildren[1];
            Assert.IsNotNull(hideIfIntElement);
            Assert.IsInstanceOf<IFieldElement>(hideIfIntElement);
            return (IFieldElement)hideIfIntElement;
        }
    }
}
