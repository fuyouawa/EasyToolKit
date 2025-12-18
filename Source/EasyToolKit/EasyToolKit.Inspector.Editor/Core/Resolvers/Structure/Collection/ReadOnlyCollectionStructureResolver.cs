using System;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    public class ReadOnlyCollectionStructureResolver<TCollection, TElement> : CollectionStructureResolverBase<TCollection, TElement>
        where TCollection : IReadOnlyCollection<TElement>
    {
        protected override int CalculateChildCount()
        {
            var minLength = int.MaxValue;
            foreach (var value in ValueEntry.Values)
            {
                if (value == null)
                {
                    return 0;
                }

                var count = value.Count;
                minLength = Math.Min(minLength, count);
            }
            return minLength == int.MaxValue ? 0 : minLength;
        }
    }
}
