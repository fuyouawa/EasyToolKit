using System;
using System.Linq;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Inspector.Attributes;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.Inspector.Editor.Implementations;
using NUnit.Framework;

namespace Tests.Editor.Inspector.InspectorAttributes
{
    public class TestInspectorAttributes_OnInspectorInit
    {
        [Serializable]
        public class TestClass
        {
            [OnInspectorInit]
            public void OnInitMethod()
            {
            }
        }

        /// <summary>
        /// Verifies that GetDrawerTypes returns the correct OnInspectorInit drawer type
        /// when custom TypeMatchCandidates are registered.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithCustomTypeMatchCandidates_ReturnsOnInspectorInitDrawer()
        {
            var onInspectorInitElement = GetOnInspectorInitElement();
            var originalTypeMatchCandidates = HandlerUtility.TypeMatcher.TypeMatchCandidates.ToArray();
            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(new []
            {
                new TypeMatchCandidate(typeof(OnInspectorInitAttributeDrawer), 0,
                    HandlerUtility.GetConstraints(typeof(OnInspectorInitAttributeDrawer)))
            });
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(onInspectorInitElement).ToArray();
            Assert.AreEqual(drawerTypes.Length, 1);
            Assert.AreEqual(drawerTypes[0], typeof(OnInspectorInitAttributeDrawer));

            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(originalTypeMatchCandidates);
        }

        /// <summary>
        /// Verifies that the default TypeMatcher candidates include the OnInspectorInit drawer type.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithDefaultCandidates_ContainsOnInspectorInitDrawer()
        {
            var onInspectorInitElement = GetOnInspectorInitElement();
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(onInspectorInitElement).ToArray();
            Assert.IsTrue(drawerTypes.Contains(typeof(OnInspectorInitAttributeDrawer)));
        }

        /// <summary>
        /// Verifies that GetDrawerChain returns a drawer chain containing the OnInspectorInit drawer
        /// when the method has OnInspectorInit attribute.
        /// </summary>
        [Test]
        public void GetDrawerChain_WithOnInspectorInitMethod_ContainsOnInspectorInitDrawer()
        {
            var onInspectorInitElement = GetOnInspectorInitElement();
            var drawerChainResolver = new DefaultDrawerChainResolverFactory().CreateResolver(onInspectorInitElement);
            Assert.IsNotNull(drawerChainResolver);
            var drawerChain = drawerChainResolver.GetDrawerChain();
            Assert.IsTrue(drawerChain.Drawers.Any(drawer => drawer.GetType() == typeof(OnInspectorInitAttributeDrawer)));
        }

        private IMethodElement GetOnInspectorInitElement()
        {
            var testInstance = new TestClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var root = tree.Root;
            var onInspectorInitElement = root.LogicalChildren[0];
            Assert.IsNotNull(onInspectorInitElement);
            Assert.IsInstanceOf<IMethodElement>(onInspectorInitElement);
            return (IMethodElement)onInspectorInitElement;
        }
    }
}
