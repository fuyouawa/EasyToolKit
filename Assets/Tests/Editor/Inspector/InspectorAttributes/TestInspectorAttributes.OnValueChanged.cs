using System;
using System.Linq;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Inspector.Attributes;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.Inspector.Editor.Implementations;
using NUnit.Framework;

namespace Tests.Editor.Inspector.InspectorAttributes
{
    public class TestInspectorAttributes_OnValueChanged
    {
        [Serializable]
        public class TestClass
        {
            [OnValueChanged(nameof(OnIntValueChanged))]
            public int OnValueChangedInt;

            private void OnIntValueChanged()
            {
            }
        }

        /// <summary>
        /// Verifies that GetDrawerTypes returns the correct OnValueChanged drawer type
        /// when custom TypeMatchCandidates are registered.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithCustomTypeMatchCandidates_ReturnsOnValueChangedDrawer()
        {
            var onValueChangedElement = GetOnValueChangedElement();
            var originalTypeMatchCandidates = HandlerUtility.TypeMatcher.TypeMatchCandidates.ToArray();
            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(new []
            {
                new TypeMatchCandidate(typeof(OnValueChangedAttributeDrawer<>), 0,
                    HandlerUtility.GetConstraints(typeof(OnValueChangedAttributeDrawer<>)))
            });
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(onValueChangedElement).ToArray();
            Assert.AreEqual(drawerTypes.Length, 1);
            Assert.AreEqual(drawerTypes[0], typeof(OnValueChangedAttributeDrawer<int>));

            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(originalTypeMatchCandidates);
        }

        /// <summary>
        /// Verifies that the default TypeMatcher candidates include the OnValueChanged drawer type.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithDefaultCandidates_ContainsOnValueChangedDrawer()
        {
            var onValueChangedElement = GetOnValueChangedElement();
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(onValueChangedElement).ToArray();
            Assert.IsTrue(drawerTypes.Contains(typeof(OnValueChangedAttributeDrawer<int>)));
        }

        /// <summary>
        /// Verifies that GetDrawerChain returns a drawer chain containing the OnValueChanged drawer
        /// when the field has OnValueChanged attribute.
        /// </summary>
        [Test]
        public void GetDrawerChain_WithOnValueChangedInt_ContainsOnValueChangedDrawer()
        {
            var onValueChangedElement = GetOnValueChangedElement();
            var drawerChainResolver = new DefaultDrawerChainResolverFactory().CreateResolver(onValueChangedElement);
            Assert.IsNotNull(drawerChainResolver);
            var drawerChain = drawerChainResolver.GetDrawerChain();
            Assert.IsTrue(drawerChain.Drawers.Any(drawer => drawer.GetType() == typeof(OnValueChangedAttributeDrawer<int>)));
        }

        private IFieldElement GetOnValueChangedElement()
        {
            var testInstance = new TestClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var root = tree.Root;
            var onValueChangedElement = root.LogicalChildren[0];
            Assert.IsNotNull(onValueChangedElement);
            Assert.IsInstanceOf<IFieldElement>(onValueChangedElement);
            return (IFieldElement)onValueChangedElement;
        }
    }
}
