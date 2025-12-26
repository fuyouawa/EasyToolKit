namespace EasyToolKit.Inspector.Editor
{
    public static class CollectionEntryExtensions
    {
        public static int GetMinimumItemCount(this ICollectionAccessor accessor)
        {
            var min = int.MaxValue;
            for (int i = 0; i < accessor.TargetCount; i++)
            {
                var count = accessor.GetItemCount(i);
                if (count < min)
                {
                    min = count;
                }
            }
            return min;
        }
    }
}
