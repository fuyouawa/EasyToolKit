using System;
using System.Linq;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Inspector.Attributes;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.Inspector.Editor.Implementations;
using NUnit.Framework;

namespace Tests.Editor.Inspector.InspectorAttributes
{
    public class TestInspectorAttributes_LabelText
    {
        [Serializable]
        public class TestClass
        {
            [LabelText("Custom Label")]
            public int CustomLabelInt;
        }

        /// <summary>
        /// Verifies that GetDrawerTypes returns the correct LabelText drawer type
        /// when custom TypeMatchCandidates are registered.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithCustomTypeMatchCandidates_ReturnsLabelTextDrawer()
        {
            var labelTextElement = GetLabelTextElement();
            var originalTypeMatchCandidates = HandlerUtility.TypeMatcher.TypeMatchCandidates.ToArray();
            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(new []
            {
                new TypeMatchCandidate(typeof(LabelTextAttributeDrawer), 0,
                    HandlerUtility.GetConstraints(typeof(LabelTextAttributeDrawer)))
            });
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(labelTextElement).ToArray();
            Assert.AreEqual(drawerTypes.Length, 1);
            Assert.AreEqual(drawerTypes[0], typeof(LabelTextAttributeDrawer));

            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(originalTypeMatchCandidates);
        }

        /// <summary>
        /// Verifies that the default TypeMatcher candidates include the LabelText drawer type.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithDefaultCandidates_ContainsLabelTextDrawer()
        {
            var labelTextElement = GetLabelTextElement();
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(labelTextElement).ToArray();
            Assert.IsTrue(drawerTypes.Contains(typeof(LabelTextAttributeDrawer)));
        }

        /// <summary>
        /// Verifies that GetDrawerChain returns a drawer chain containing the LabelText drawer
        /// when the field has LabelText attribute.
        /// </summary>
        [Test]
        public void GetDrawerChain_WithLabelTextInt_ContainsLabelTextDrawer()
        {
            var labelTextElement = GetLabelTextElement();
            var drawerChainResolver = new DefaultDrawerChainResolverFactory().CreateResolver(labelTextElement);
            Assert.IsNotNull(drawerChainResolver);
            var drawerChain = drawerChainResolver.GetDrawerChain();
            Assert.IsTrue(drawerChain.Drawers.Any(drawer => drawer.GetType() == typeof(LabelTextAttributeDrawer)));
        }

        private IFieldElement GetLabelTextElement()
        {
            var testInstance = new TestClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var root = tree.Root;
            var labelTextElement = root.LogicalChildren[0];
            Assert.IsNotNull(labelTextElement);
            Assert.IsInstanceOf<IFieldElement>(labelTextElement);
            return (IFieldElement)labelTextElement;
        }
    }
}
