using System;
using System.Linq;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Inspector.Attributes;
using EasyToolkit.Inspector.Editor;
using EasyToolkit.Inspector.Editor.Implementations;
using NUnit.Framework;

namespace Tests.Editor.Inspector.InspectorAttributes
{
    public class TestInspectorAttributes_Required
    {
        [Serializable]
        public class TestClass
        {
            [Required]
            public string RequiredString;

            [Required]
            public int RequiredInt;

            [Required]
            public UnityEngine.Object RequiredUnityObject;
        }

        /// <summary>
        /// Verifies that GetDrawerTypes returns the correct Required drawer type
        /// when custom TypeMatchCandidates are registered.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithCustomTypeMatchCandidates_ReturnsRequiredDrawer()
        {
            var requiredStringElement = GetRequiredStringElement();
            var originalTypeMatchCandidates = HandlerUtility.TypeMatcher.TypeMatchCandidates.ToArray();
            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(new []
            {
                new TypeMatchCandidate(typeof(RequiredAttributeDrawer<>), 0,
                    HandlerUtility.GetConstraints(typeof(RequiredAttributeDrawer<>)))
            });
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(requiredStringElement).ToArray();
            Assert.AreEqual(drawerTypes.Length, 1);
            Assert.AreEqual(drawerTypes[0], typeof(RequiredAttributeDrawer<string>));

            HandlerUtility.TypeMatcher.SetTypeMatchCandidates(originalTypeMatchCandidates);
        }

        /// <summary>
        /// Verifies that the default TypeMatcher candidates include the Required drawer type.
        /// </summary>
        [Test]
        public void GetDrawerTypes_WithDefaultCandidates_ContainsRequiredDrawer()
        {
            var requiredStringElement = GetRequiredStringElement();
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(requiredStringElement).ToArray();
            Assert.IsTrue(drawerTypes.Contains(typeof(RequiredAttributeDrawer<string>)));
        }

        /// <summary>
        /// Verifies that GetDrawerChain returns a drawer chain containing the Required drawer
        /// when the field has Required attribute.
        /// </summary>
        [Test]
        public void GetDrawerChain_WithRequiredString_ContainsRequiredDrawer()
        {
            var requiredStringElement = GetRequiredStringElement();
            var drawerChainResolver = new DefaultDrawerChainResolverFactory().CreateResolver(requiredStringElement);
            Assert.IsNotNull(drawerChainResolver);
            var drawerChain = drawerChainResolver.GetDrawerChain();
            Assert.IsTrue(drawerChain.Drawers.Any(drawer => drawer.GetType() == typeof(RequiredAttributeDrawer<string>)));
        }

        /// <summary>
        /// Verifies that GetDrawerChain returns a drawer chain containing the Required drawer
        /// for int fields with Required attribute.
        /// </summary>
        [Test]
        public void GetDrawerChain_WithRequiredInt_ContainsRequiredDrawer()
        {
            var requiredIntElement = GetRequiredIntElement();
            var drawerChainResolver = new DefaultDrawerChainResolverFactory().CreateResolver(requiredIntElement);
            Assert.IsNotNull(drawerChainResolver);
            var drawerChain = drawerChainResolver.GetDrawerChain();
            Assert.IsTrue(drawerChain.Drawers.Any(drawer => drawer.GetType() == typeof(RequiredAttributeDrawer<int>)));
        }

        /// <summary>
        /// Verifies that GetDrawerChain returns a drawer chain containing the Required drawer
        /// for UnityEngine.Object fields with Required attribute.
        /// </summary>
        [Test]
        public void GetDrawerChain_WithRequiredUnityObject_ContainsRequiredDrawer()
        {
            var requiredUnityObjectElement = GetRequiredUnityObjectElement();
            var drawerChainResolver = new DefaultDrawerChainResolverFactory().CreateResolver(requiredUnityObjectElement);
            Assert.IsNotNull(drawerChainResolver);
            var drawerChain = drawerChainResolver.GetDrawerChain();
            Assert.IsTrue(drawerChain.Drawers.Any(drawer => drawer.GetType() == typeof(RequiredAttributeDrawer<UnityEngine.Object>)));
        }

        private IFieldElement GetRequiredStringElement()
        {
            var testInstance = new TestClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var root = tree.Root;
            var requiredStringElement = root.LogicalChildren[0];
            Assert.IsNotNull(requiredStringElement);
            Assert.IsInstanceOf<IFieldElement>(requiredStringElement);
            return (IFieldElement)requiredStringElement;
        }

        private IFieldElement GetRequiredIntElement()
        {
            var testInstance = new TestClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var root = tree.Root;
            var requiredIntElement = root.LogicalChildren[1];
            Assert.IsNotNull(requiredIntElement);
            Assert.IsInstanceOf<IFieldElement>(requiredIntElement);
            return (IFieldElement)requiredIntElement;
        }

        private IFieldElement GetRequiredUnityObjectElement()
        {
            var testInstance = new TestClass();
            var tree = InspectorElements.TreeFactory.CreateTree(new object[] { testInstance }, null);
            tree.BeginDraw();
            tree.EndDraw();
            var root = tree.Root;
            var requiredUnityObjectElement = root.LogicalChildren[2];
            Assert.IsNotNull(requiredUnityObjectElement);
            Assert.IsInstanceOf<IFieldElement>(requiredUnityObjectElement);
            return (IFieldElement)requiredUnityObjectElement;
        }
    }
}
