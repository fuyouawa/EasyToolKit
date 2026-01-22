using System;
using System.Linq;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Inspector.Attributes;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.Inspector.Editor.Implementations;
using NUnit.Framework;

namespace Tests.Editor.Inspector.InspectorAttributes
{
    public class TestInspectorAttributes_MessageBox
    {
        [Serializable]
        public class TestClass
        {
            [MessageBox("This is an info message")]
            public int MessageBoxInfo;

            [MessageBox("This is a warning message", MessageBoxType.Warning)]
            public int MessageBoxWarning;

            [MessageBox("This is an error message", MessageBoxType.Error)]
            public int MessageBoxError;
        }

        /// <summary>
        /// Verifies that GetDrawerTypes returns the correct MessageBox drawer type
        /// when custom TypeMatchCandidates are registered.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithCustomTypeMatchCandidates_ReturnsMessageBoxDrawer()
        {
            var messageBoxElement = GetMessageBoxInfoElement();
            var originalTypeMatchCandidates = HandlerUtility.TypeMatcher.TypeMatchCandidates.ToArray();
            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(new []
            {
                new TypeMatchCandidate(typeof(MessageBoxAttributeDrawer), 0,
                    HandlerUtility.GetConstraints(typeof(MessageBoxAttributeDrawer)))
            });
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(messageBoxElement).ToArray();
            Assert.AreEqual(drawerTypes.Length, 1);
            Assert.AreEqual(drawerTypes[0], typeof(MessageBoxAttributeDrawer));

            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(originalTypeMatchCandidates);
        }

        /// <summary>
        /// Verifies that the default TypeMatcher candidates include the MessageBox drawer type.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithDefaultCandidates_ContainsMessageBoxDrawer()
        {
            var messageBoxElement = GetMessageBoxInfoElement();
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(messageBoxElement).ToArray();
            Assert.IsTrue(drawerTypes.Contains(typeof(MessageBoxAttributeDrawer)));
        }

        /// <summary>
        /// Verifies that GetDrawerChain returns a drawer chain containing the MessageBox drawer
        /// when the field has MessageBox attribute.
        /// </summary>
        [Test]
        public void GetDrawerChain_WithMessageBoxInfo_ContainsMessageBoxDrawer()
        {
            var messageBoxElement = GetMessageBoxInfoElement();
            var drawerChainResolver = new DefaultDrawerChainResolverFactory().CreateResolver(messageBoxElement);
            Assert.IsNotNull(drawerChainResolver);
            var drawerChain = drawerChainResolver.GetDrawerChain();
            Assert.IsTrue(drawerChain.Drawers.Any(drawer => drawer.GetType() == typeof(MessageBoxAttributeDrawer)));
        }

        private IFieldElement GetMessageBoxInfoElement()
        {
            var testInstance = new TestClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var root = tree.Root;
            var messageBoxElement = root.LogicalChildren[0];
            Assert.IsNotNull(messageBoxElement);
            Assert.IsInstanceOf<IFieldElement>(messageBoxElement);
            return (IFieldElement)messageBoxElement;
        }
    }
}
