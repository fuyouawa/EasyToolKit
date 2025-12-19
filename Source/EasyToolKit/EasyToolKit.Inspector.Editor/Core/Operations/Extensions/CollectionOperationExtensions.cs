using System;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public static class CollectionOperationExtensions
    {
        public static void AddWeakElement(this ICollectionOperation collectionOperation, InspectorProperty property, int targetIndex, object element)
        {
            var collection = property.ValueEntry.WeakValues[targetIndex];
            collectionOperation.AddWeakElement(ref collection, element);
            property.ValueEntry.WeakValues[targetIndex] = collection;
        }

        public static void RemoveWeakElement(this ICollectionOperation collectionOperation, InspectorProperty property, int targetIndex, object element)
        {
            var collection = property.ValueEntry.WeakValues[targetIndex];
            collectionOperation.RemoveWeakElement(ref collection, element);
            property.ValueEntry.WeakValues[targetIndex] = collection;
        }
    }
}
