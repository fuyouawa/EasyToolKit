using System.Collections;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    public static class ValueEntryExtensions
    {
        public static IEnumerable EnumerateWeakValues(this IValueAccessor valueAccessor)
        {
            for (int i = 0; i < valueAccessor.TargetCount; i++)
            {
                yield return valueAccessor.GetWeakValue(i);
            }
        }

        public static IEnumerable<TValue> EnumerateValues<TValue>(this IValueAccessor<TValue> valueAccessor)
        {
            for (int i = 0; i < valueAccessor.TargetCount; i++)
            {
                yield return valueAccessor.GetValue(i);
            }
        }
    }
}
