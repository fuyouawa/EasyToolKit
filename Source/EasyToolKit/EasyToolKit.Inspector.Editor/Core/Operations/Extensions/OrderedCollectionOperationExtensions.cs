using System;

namespace EasyToolKit.Inspector.Editor
{
    public static class OrderedCollectionOperationExtensions
    {
        public static void InsertWeakElementAt(this IOrderedCollectionOperation collectionOperation, InspectorProperty property, int targetIndex, int elementIndex, object value)
        {
            var collection = property.ValueEntry.WeakValues[targetIndex];
            collectionOperation.InsertWeakItem(ref collection, elementIndex, value);
            property.ValueEntry.WeakValues[targetIndex] = collection;
        }

        public static void RemoveWeakElementAt(this IOrderedCollectionOperation collectionOperation, InspectorProperty property, int targetIndex, int elementIndex)
        {
            var collection = property.ValueEntry.WeakValues[targetIndex];
            collectionOperation.RemoveWeakItem(ref collection, elementIndex);
            property.ValueEntry.WeakValues[targetIndex] = collection;
        }
    }
}
