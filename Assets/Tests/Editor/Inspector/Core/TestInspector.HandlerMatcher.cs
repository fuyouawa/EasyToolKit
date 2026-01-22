using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Inspector.Editor;
using NUnit.Framework;

namespace Tests.Editor.Inspector.Core
{
    [TestFixture]
    public class TestInspector_StructureResolverMatcher
    {
        private static Type[] s_resolverTypes;

        private static Type[] ResolverTypes
        {
            get
            {
                if (s_resolverTypes == null)
                {
                    s_resolverTypes = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly => assembly.GetTypes())
                        .Where(type => type.IsDerivedFrom<IStructureResolver>() && !type.IsAbstract && !type.IsInterface)
                        .ToArray();
                }

                return s_resolverTypes;
            }
        }

        private static ITypeMatcher CreateTypeMatcher()
        {
            var typeMatcher = TypeMatcherFactory.CreateDefault();
            typeMatcher.SetTypeMatchCandidates(ResolverTypes
                .Select((type, i) =>
                {
                    Type[] constraints = null;
                    if (type.BaseType != null && type.BaseType.IsGenericType)
                    {
                        constraints = type.GetGenericArgumentsRelativeTo(type.BaseType.GetGenericTypeDefinition());
                    }

                    return new TypeMatchCandidate(type, ResolverTypes.Length - i, constraints);
                }));
            return typeMatcher;
        }

        [Test]
        public void Test()
        {
            var typeMatcher = CreateTypeMatcher();

            var matches = typeMatcher.GetMatches(typeof(List<int>));
            Assert.AreEqual(typeof(CollectionStructureResolver<List<int>, int>), matches[0].MatchedType);
        }
    }
}
