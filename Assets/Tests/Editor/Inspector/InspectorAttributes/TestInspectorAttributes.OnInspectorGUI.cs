using System;
using System.Linq;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Inspector.Attributes;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.Inspector.Editor.Implementations;
using NUnit.Framework;

namespace Tests.Editor.Inspector.InspectorAttributes
{
    public class TestInspectorAttributes_OnInspectorGUI
    {
        [Serializable]
        public class TestClass
        {
            [OnInspectorGUI]
            public void OnGUIMethod()
            {
            }
        }

        /// <summary>
        /// Verifies that GetDrawerTypes returns the correct OnInspectorGUI drawer type
        /// when custom TypeMatchCandidates are registered.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithCustomTypeMatchCandidates_ReturnsOnInspectorGUIDrawer()
        {
            var onInspectorGUIElement = GetOnInspectorGUIElement();
            var originalTypeMatchCandidates = HandlerUtility.TypeMatcher.TypeMatchCandidates.ToArray();
            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(new []
            {
                new TypeMatchCandidate(typeof(OnInspectorGUIAttributeDrawer), 0,
                    HandlerUtility.GetConstraints(typeof(OnInspectorGUIAttributeDrawer)))
            });
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(onInspectorGUIElement).ToArray();
            Assert.AreEqual(drawerTypes.Length, 1);
            Assert.AreEqual(drawerTypes[0], typeof(OnInspectorGUIAttributeDrawer));

            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(originalTypeMatchCandidates);
        }

        /// <summary>
        /// Verifies that the default TypeMatcher candidates include the OnInspectorGUI drawer type.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithDefaultCandidates_ContainsOnInspectorGUIDrawer()
        {
            var onInspectorGUIElement = GetOnInspectorGUIElement();
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(onInspectorGUIElement).ToArray();
            Assert.IsTrue(drawerTypes.Contains(typeof(OnInspectorGUIAttributeDrawer)));
        }

        /// <summary>
        /// Verifies that GetDrawerChain returns a drawer chain containing the OnInspectorGUI drawer
        /// when the method has OnInspectorGUI attribute.
        /// </summary>
        [Test]
        public void GetDrawerChain_WithOnInspectorGUIMethod_ContainsOnInspectorGUIDrawer()
        {
            var onInspectorGUIElement = GetOnInspectorGUIElement();
            var drawerChainResolver = new DefaultDrawerChainResolverFactory().CreateResolver(onInspectorGUIElement);
            Assert.IsNotNull(drawerChainResolver);
            var drawerChain = drawerChainResolver.GetDrawerChain();
            Assert.IsTrue(drawerChain.Drawers.Any(drawer => drawer.GetType() == typeof(OnInspectorGUIAttributeDrawer)));
        }

        private IMethodElement GetOnInspectorGUIElement()
        {
            var testInstance = new TestClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var root = tree.Root;
            var onInspectorGUIElement = root.LogicalChildren[0];
            Assert.IsNotNull(onInspectorGUIElement);
            Assert.IsInstanceOf<IMethodElement>(onInspectorGUIElement);
            return (IMethodElement)onInspectorGUIElement;
        }
    }
}
