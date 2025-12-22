using System.Collections;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    public static class ValueEntryExtensions
    {
        public static IEnumerable EnumerateWeakValues(this IValueEntry valueEntry)
        {
            for (int i = 0; i < valueEntry.TargetCount; i++)
            {
                yield return valueEntry.GetWeakValue(i);
            }
        }

        public static IEnumerable<TValue> EnumerateValues<TValue>(this IValueEntry<TValue> valueEntry)
        {
            for (int i = 0; i < valueEntry.TargetCount; i++)
            {
                yield return valueEntry.GetValue(i);
            }
        }
    }
}
