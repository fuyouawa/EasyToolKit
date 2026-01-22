using System;
using System.Linq;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Inspector.Attributes;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.Inspector.Editor.Implementations;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Editor.Inspector.InspectorAttributes
{
    public class TestInspectorAttributes_InlineEditor
    {
        [Serializable]
        public class TestClass
        {
            [InlineEditor]
            public Rigidbody InlineEditorObject;
        }

        /// <summary>
        /// Verifies that GetDrawerTypes returns the correct InlineEditor drawer type
        /// when custom TypeMatchCandidates are registered.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithCustomTypeMatchCandidates_ReturnsInlineEditorDrawer()
        {
            var inlineEditorElement = GetInlineEditorElement();
            var originalTypeMatchCandidates = HandlerUtility.TypeMatcher.TypeMatchCandidates.ToArray();
            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(new []
            {
                new TypeMatchCandidate(typeof(InlineEditorAttributeDrawer<>), 0,
                    HandlerUtility.GetConstraints(typeof(InlineEditorAttributeDrawer<>)))
            });
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(inlineEditorElement).ToArray();
            Assert.AreEqual(drawerTypes.Length, 1);
            Assert.AreEqual(drawerTypes[0], typeof(InlineEditorAttributeDrawer<Rigidbody>));

            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(originalTypeMatchCandidates);
        }

        /// <summary>
        /// Verifies that the default TypeMatcher candidates include the InlineEditor drawer type.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithDefaultCandidates_ContainsInlineEditorDrawer()
        {
            var inlineEditorElement = GetInlineEditorElement();
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(inlineEditorElement).ToArray();
            Assert.IsTrue(drawerTypes.Contains(typeof(InlineEditorAttributeDrawer<Rigidbody>)));
        }

        /// <summary>
        /// Verifies that GetDrawerChain returns a drawer chain containing the InlineEditor drawer
        /// when the field has InlineEditor attribute.
        /// </summary>
        [Test]
        public void GetDrawerChain_WithInlineEditorObject_ContainsInlineEditorDrawer()
        {
            var inlineEditorElement = GetInlineEditorElement();
            var drawerChainResolver = new DefaultDrawerChainResolverFactory().CreateResolver(inlineEditorElement);
            Assert.IsNotNull(drawerChainResolver);
            var drawerChain = drawerChainResolver.GetDrawerChain();
            Assert.IsTrue(drawerChain.Drawers.Any(drawer => drawer.GetType() == typeof(InlineEditorAttributeDrawer<Rigidbody>)));
        }

        private IFieldElement GetInlineEditorElement()
        {
            var testInstance = new TestClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var root = tree.Root;
            var inlineEditorElement = root.LogicalChildren.FirstOrDefault(c =>
                ((IFieldElement)c).Definition.Name == nameof(TestClass.InlineEditorObject));
            Assert.IsNotNull(inlineEditorElement);
            Assert.IsInstanceOf<IFieldElement>(inlineEditorElement);
            return (IFieldElement)inlineEditorElement;
        }
    }
}
