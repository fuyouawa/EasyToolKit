using System;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    public class ReadOnlyCollectionStructureResolver<TCollection, TElement> : CollectionStructureResolverBase<TCollection>
        where TCollection : IReadOnlyCollection<TElement>
    {
        public override Type ItemType => typeof(TElement);

        protected override int CalculateChildCount()
        {
            var minLength = int.MaxValue;
            for (int i = 0; i < ValueEntry.TargetCount; i++)
            {
                var value = ValueEntry.GetValue(i);
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
