using System;
using System.Linq;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Inspector.Attributes;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.Inspector.Editor.Implementations;
using NUnit.Framework;

namespace Tests.Editor.Inspector.InspectorAttributes
{
    public class TestInspectorAttributes_Button
    {
        [Serializable]
        public class TestClass
        {
            [Button]
            public void DefaultButton()
            {
            }

            [Button("Custom Label")]
            public void CustomButton()
            {
            }
        }

        /// <summary>
        /// Verifies that GetDrawerTypes returns the correct Button drawer type
        /// when custom TypeMatchCandidates are registered.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithCustomTypeMatchCandidates_ReturnsButtonDrawer()
        {
            var buttonElement = GetDefaultButtonElement();
            var originalTypeMatchCandidates = HandlerUtility.TypeMatcher.TypeMatchCandidates.ToArray();
            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(new []
            {
                new TypeMatchCandidate(typeof(ButtonAttributeDrawer), 0,
                    HandlerUtility.GetConstraints(typeof(ButtonAttributeDrawer)))
            });
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(buttonElement).ToArray();
            Assert.AreEqual(drawerTypes.Length, 1);
            Assert.AreEqual(drawerTypes[0], typeof(ButtonAttributeDrawer));

            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(originalTypeMatchCandidates);
        }

        /// <summary>
        /// Verifies that the default TypeMatcher candidates include the Button drawer type.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithDefaultCandidates_ContainsButtonDrawer()
        {
            var buttonElement = GetDefaultButtonElement();
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(buttonElement).ToArray();
            Assert.IsTrue(drawerTypes.Contains(typeof(ButtonAttributeDrawer)));
        }

        /// <summary>
        /// Verifies that GetDrawerChain returns a drawer chain containing the Button drawer
        /// when the method has Button attribute.
        /// </summary>
        [Test]
        public void GetDrawerChain_WithDefaultButton_ContainsButtonDrawer()
        {
            var buttonElement = GetDefaultButtonElement();
            var drawerChainResolver = new DefaultDrawerChainResolverFactory().CreateResolver(buttonElement);
            Assert.IsNotNull(drawerChainResolver);
            var drawerChain = drawerChainResolver.GetDrawerChain();
            Assert.IsTrue(drawerChain.Drawers.Any(drawer => drawer.GetType() == typeof(ButtonAttributeDrawer)));
        }

        /// <summary>
        /// Verifies that GetDrawerChain returns a drawer chain containing the Button drawer
        /// when the method has Button attribute with custom label.
        /// </summary>
        [Test]
        public void GetDrawerChain_WithCustomButton_ContainsButtonDrawer()
        {
            var buttonElement = GetCustomButtonElement();
            var drawerChainResolver = new DefaultDrawerChainResolverFactory().CreateResolver(buttonElement);
            Assert.IsNotNull(drawerChainResolver);
            var drawerChain = drawerChainResolver.GetDrawerChain();
            Assert.IsTrue(drawerChain.Drawers.Any(drawer => drawer.GetType() == typeof(ButtonAttributeDrawer)));
        }

        private IMethodElement GetDefaultButtonElement()
        {
            var testInstance = new TestClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var root = tree.Root;
            var buttonElement = root.LogicalChildren[0];
            Assert.IsNotNull(buttonElement);
            Assert.IsInstanceOf<IMethodElement>(buttonElement);
            return (IMethodElement)buttonElement;
        }

        private IMethodElement GetCustomButtonElement()
        {
            var testInstance = new TestClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var root = tree.Root;
            var buttonElement = root.LogicalChildren[1];
            Assert.IsNotNull(buttonElement);
            Assert.IsInstanceOf<IMethodElement>(buttonElement);
            return (IMethodElement)buttonElement;
        }
    }
}
