using System;
using System.Linq;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Inspector.Attributes;
using EasyToolkit.Inspector.Editor;
using EasyToolkit.Inspector.Editor.Implementations;
using NUnit.Framework;

namespace Tests.Editor.Inspector.InspectorAttributes
{
    public class TestInspectorAttributes_HideLabel
    {
        [Serializable]
        public class TestClass
        {
            [HideLabel]
            public int HideLabelInt;
        }

        /// <summary>
        /// Verifies that GetDrawerTypes returns the correct HideLabel drawer type
        /// when custom TypeMatchCandidates are registered.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithCustomTypeMatchCandidates_ReturnsHideLabelDrawer()
        {
            var hideLabelElement = GetHideLabelElement();
            var originalTypeMatchCandidates = HandlerUtility.TypeMatcher.TypeMatchCandidates.ToArray();
            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(new []
            {
                new TypeMatchCandidate(typeof(HideLabelAttributeDrawer), 0,
                    HandlerUtility.GetConstraints(typeof(HideLabelAttributeDrawer)))
            });
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(hideLabelElement).ToArray();
            Assert.AreEqual(drawerTypes.Length, 1);
            Assert.AreEqual(drawerTypes[0], typeof(HideLabelAttributeDrawer));

            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(originalTypeMatchCandidates);
        }

        /// <summary>
        /// Verifies that the default TypeMatcher candidates include the HideLabel drawer type.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithDefaultCandidates_ContainsHideLabelDrawer()
        {
            var hideLabelElement = GetHideLabelElement();
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(hideLabelElement).ToArray();
            Assert.IsTrue(drawerTypes.Contains(typeof(HideLabelAttributeDrawer)));
        }

        /// <summary>
        /// Verifies that GetDrawerChain returns a drawer chain containing the HideLabel drawer
        /// when the field has HideLabel attribute.
        /// </summary>
        [Test]
        public void GetDrawerChain_WithHideLabelInt_ContainsHideLabelDrawer()
        {
            var hideLabelElement = GetHideLabelElement();
            var drawerChainResolver = new DefaultDrawerChainResolverFactory().CreateResolver(hideLabelElement);
            Assert.IsNotNull(drawerChainResolver);
            var drawerChain = drawerChainResolver.GetDrawerChain();
            Assert.IsTrue(drawerChain.Drawers.Any(drawer => drawer.GetType() == typeof(HideLabelAttributeDrawer)));
        }

        private IFieldElement GetHideLabelElement()
        {
            var testInstance = new TestClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var root = tree.Root;
            var hideLabelElement = root.LogicalChildren[0];
            Assert.IsNotNull(hideLabelElement);
            Assert.IsInstanceOf<IFieldElement>(hideLabelElement);
            return (IFieldElement)hideLabelElement;
        }
    }
}
