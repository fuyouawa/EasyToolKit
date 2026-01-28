using System;
using System.Linq;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Inspector.Attributes;
using EasyToolkit.Inspector.Editor;
using EasyToolkit.Inspector.Editor.Implementations;
using NUnit.Framework;

namespace Tests.Editor.Inspector.InspectorAttributes
{
    public class TestInspectorAttributes_ValueDropdown
    {
        [Serializable]
        public class TestClass
        {
            [ValueDropdown(nameof(IntValueDropdown))]
            public int IntValue;

            private static int[] IntValueDropdown => new int[] { 1, 2, 3 };
        }

        /// <summary>
        /// Verifies that GetDrawerTypes returns the correct ValueDropdown drawer type
        /// when custom TypeMatchCandidates are registered.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithCustomTypeMatchCandidates_ReturnsValueDropdownDrawer()
        {
            var intValueElement = GetIntValueElement();
            var originalTypeMatchCandidates = HandlerUtility.TypeMatcher.TypeMatchCandidates.ToArray();
            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(new []
            {
                new TypeMatchCandidate(typeof(ValueDropdownAttributeDrawer<>), 0,
                    HandlerUtility.GetConstraints(typeof(ValueDropdownAttributeDrawer<>)))
            });
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(intValueElement).ToArray();
            Assert.AreEqual(drawerTypes.Length, 1);
            Assert.AreEqual(drawerTypes[0], typeof(ValueDropdownAttributeDrawer<int>));

            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(originalTypeMatchCandidates);
        }

        /// <summary>
        /// Verifies that the default TypeMatcher candidates include the ValueDropdown drawer type.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithDefaultCandidates_ContainsValueDropdownDrawer()
        {
            var intValueElement = GetIntValueElement();
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(intValueElement).ToArray();
            Assert.IsTrue(drawerTypes.Contains(typeof(ValueDropdownAttributeDrawer<int>)));
        }

        /// <summary>
        /// Verifies that GetDrawerChain returns a drawer chain containing the ValueDropdown drawer
        /// when the field has ValueDropdown attribute.
        /// </summary>
        [Test]
        public void GetDrawerChain_WithValueDropdownInt_ContainsValueDropdownDrawer()
        {
            var intValueElement = GetIntValueElement();
            var drawerChainResolver = new DefaultDrawerChainResolverFactory().CreateResolver(intValueElement);
            Assert.IsNotNull(drawerChainResolver);
            var drawerChain = drawerChainResolver.GetDrawerChain();
            Assert.IsTrue(drawerChain.Drawers.Any(drawer => drawer.GetType() == typeof(ValueDropdownAttributeDrawer<int>)));
        }


        private IFieldElement GetIntValueElement()
        {
            var testInstance = new TestClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var root = tree.Root;
            Assert.AreEqual(root.LogicalChildren.Count, 1);
            var intValueElement = root.LogicalChildren[0];
            Assert.IsInstanceOf<IFieldElement>(intValueElement);
            return (IFieldElement)intValueElement;
        }
    }
}
