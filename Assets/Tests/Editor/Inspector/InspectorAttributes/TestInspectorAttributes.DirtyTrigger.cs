using System;
using System.Linq;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Inspector.Attributes;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.Inspector.Editor.Implementations;
using NUnit.Framework;

namespace Tests.Editor.Inspector.InspectorAttributes
{
    public class TestInspectorAttributes_DirtyTrigger
    {
        [Serializable]
        public class TestClass
        {
            [DirtyTrigger]
            public Action<string> DirtyTriggerInt;
        }

        /// <summary>
        /// Verifies that GetDrawerTypes returns the correct DirtyTrigger drawer type
        /// when custom TypeMatchCandidates are registered.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithCustomTypeMatchCandidates_ReturnsDirtyTriggerDrawer()
        {
            var dirtyTriggerElement = GetDirtyTriggerElement();
            var originalTypeMatchCandidates = HandlerUtility.TypeMatcher.TypeMatchCandidates.ToArray();
            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(new []
            {
                new TypeMatchCandidate(typeof(DirtyTriggerAttributeDrawer<>), 0,
                    HandlerUtility.GetConstraints(typeof(DirtyTriggerAttributeDrawer<>)))
            });
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(dirtyTriggerElement).ToArray();
            Assert.AreEqual(drawerTypes.Length, 1);
            Assert.AreEqual(drawerTypes[0], typeof(DirtyTriggerAttributeDrawer<Action<string>>));

            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(originalTypeMatchCandidates);
        }

        /// <summary>
        /// Verifies that the default TypeMatcher candidates include the DirtyTrigger drawer type.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithDefaultCandidates_ContainsDirtyTriggerDrawer()
        {
            var dirtyTriggerElement = GetDirtyTriggerElement();
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(dirtyTriggerElement).ToArray();
            Assert.IsTrue(drawerTypes.Contains(typeof(DirtyTriggerAttributeDrawer<Action<string>>)));
        }

        /// <summary>
        /// Verifies that GetDrawerChain returns a drawer chain containing the DirtyTrigger drawer
        /// when the field has DirtyTrigger attribute.
        /// </summary>
        [Test]
        public void GetDrawerChain_WithDirtyTriggerInt_ContainsDirtyTriggerDrawer()
        {
            var dirtyTriggerElement = GetDirtyTriggerElement();
            var drawerChainResolver = new DefaultDrawerChainResolverFactory().CreateResolver(dirtyTriggerElement);
            Assert.IsNotNull(drawerChainResolver);
            var drawerChain = drawerChainResolver.GetDrawerChain();
            Assert.IsTrue(drawerChain.Drawers.Any(drawer => drawer.GetType() == typeof(DirtyTriggerAttributeDrawer<Action<string>>)));
        }

        private IFieldElement GetDirtyTriggerElement()
        {
            var testInstance = new TestClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var root = tree.Root;
            var dirtyTriggerElement = root.LogicalChildren[0];
            Assert.IsNotNull(dirtyTriggerElement);
            Assert.IsInstanceOf<IFieldElement>(dirtyTriggerElement);
            return (IFieldElement)dirtyTriggerElement;
        }
    }
}
